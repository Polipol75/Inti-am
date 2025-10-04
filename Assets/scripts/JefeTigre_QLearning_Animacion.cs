using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class JefeTigre_QLearning_Reborn : MonoBehaviour
{
    public Animator animator;
    public float speed_walk = 3f;
    public GameObject target;
    public float detectionRange = 10f;
    public float attackRange = 1f;
    public float longRangeAttackThreshold = 5f;

    public float learningRate = 0.1f;
    public float discountFactor = 0.9f;
    public float explorationRate = 0.2f;
    public float explorationDecay = 0.999f;
    public float minExplorationRate = 0.01f;

    public float reward_MovementTowardsPlayer = 0.01f;
    public float reward_AttackAttempt = -0.05f;
    public float reward_Idle = -0.01f;
    public float reward_BossCollidesWithPlayer = 0.1f;
    public float reward_AttackHitPlayer_ShortRange = 1.0f;
    public float reward_AttackHitPlayer_LongRange = 1.0f;
    public float reward_AttackMissPlayer = -0.5f;

    private Dictionary<string, Dictionary<int, float>> qTable = new Dictionary<string, Dictionary<int, float>>();

    private const int ACTION_IDLE = 0;
    private const int ACTION_MOVE = 1;
    private const int ACTION_ATTACK_1 = 2;
    private const int ACTION_ATTACK_2 = 3;
    private List<int> allActions;

    private string lastStateKey = "";
    private int lastAction = -1;
    private string lastDebugState = "";

    private string qTableFilePath;

    public float attackCooldownTime = 1.5f;
    private float currentAttackCooldown = 0f;

    private bool isInAttack1Loop = false;
    private int attack1Count = 0;
    private const int MAX_ATTACK1_COUNT = 3;
    public float timeBetweenAttack1Repeats = 0.5f;
    private float currentRepeatTimer = 0f;

    void Awake()
    {
        allActions = new List<int> { ACTION_IDLE, ACTION_MOVE, ACTION_ATTACK_1, ACTION_ATTACK_2 };
        qTableFilePath = @"C:\Users\paulj\Desktop\tigrejefe\BossQTable.json";
        LoadQTable();
    }

    void Start()
    {
        if (animator == null) { animator = GetComponent<Animator>(); }
        if (target == null) { Debug.LogError("El Boss no tiene un objetivo (Target) asignado!", this); }
    }

    void Update()
    {
        if (currentAttackCooldown > 0) currentAttackCooldown -= Time.deltaTime;
        if (target == null) { animator.SetFloat("speed", 0f); return; }

        Vector3 diff = target.transform.position - transform.position;
        float distance = diff.magnitude;

        if (isInAttack1Loop)
        {
            currentRepeatTimer -= Time.deltaTime;
            if (currentRepeatTimer <= 0)
            {
                if (currentAttackCooldown <= 0 && attack1Count < MAX_ATTACK1_COUNT)
                {
                    animator.ResetTrigger("DoAttack1");
                    animator.SetTrigger("DoAttack1");
                    currentAttackCooldown = attackCooldownTime;
                    currentRepeatTimer = timeBetweenAttack1Repeats;
                    attack1Count++;
                    string currentState = GetCurrentStateKey(diff, distance, GetPlayerRelativePositionState(diff));
                    float rewardForRepeat = reward_AttackAttempt;
                    UpdateQTable(lastStateKey, ACTION_ATTACK_1, rewardForRepeat, currentState);
                    lastStateKey = currentState;
                    lastAction = ACTION_ATTACK_1;
                }
                else if (attack1Count >= MAX_ATTACK1_COUNT)
                {
                    isInAttack1Loop = false;
                    attack1Count = 0;
                    animator.SetFloat("speed", 0f);
                }
            }
            return;
        }

        string playerRelativePosState = GetPlayerRelativePositionState(diff);
        if (playerRelativePosState != lastDebugState)
        {
            Debug.Log("Jugador está en: " + playerRelativePosState + ", Distancia: " + distance.ToString("F2"));
            lastDebugState = playerRelativePosState;
        }

        string currentStateKey = GetCurrentStateKey(diff, distance, playerRelativePosState);
        int chosenAction = ChooseAction(currentStateKey);
        ApplyAction(chosenAction, diff, distance);

        if (lastStateKey != "" && lastAction != -1 && distance <= detectionRange)
        {
            float rewardForLastAction = GetImmediateReward(lastAction, distance);
            UpdateQTable(lastStateKey, lastAction, rewardForLastAction, currentStateKey);
        }

        lastStateKey = currentStateKey;
        lastAction = chosenAction;
        explorationRate = Mathf.Max(minExplorationRate, explorationRate * explorationDecay);
    }

    private string GetPlayerRelativePositionState(Vector3 diff)
    {
        if (diff.y > 1f) return "Arriba";
        else if (diff.x > 0.5f) return diff.y > 0.5f ? "Derecha_saltando" : "Derecha";
        else if (diff.x < -0.5f) return diff.y > 0.5f ? "Izquierda_saltando" : "Izquierda";
        else return "Centro";
    }

    private string GetCurrentStateKey(Vector3 diff, float distance, string playerRelativePosState)
    {
        string state = "";
        if (distance <= attackRange) state += "VERY_NEAR_";
        else if (distance <= longRangeAttackThreshold) state += "NEAR_";
        else if (distance <= detectionRange) state += "MEDIUM_";
        else state += "FAR_";
        state += playerRelativePosState.Replace(" ", "_").ToUpper();
        return state.TrimEnd('_');
    }

    private int ChooseAction(string stateKey)
    {
        if (!qTable.ContainsKey(stateKey))
        {
            qTable[stateKey] = new Dictionary<int, float>();
            foreach (int action in allActions) qTable[stateKey][action] = 0f;
        }

        if (Random.value < explorationRate) return allActions[Random.Range(0, allActions.Count)];

        float maxQ = qTable[stateKey].Values.Any() ? qTable[stateKey].Values.Max() : 0f;
        List<int> bestActions = new List<int>();
        foreach (var entry in qTable[stateKey]) if (entry.Value == maxQ) bestActions.Add(entry.Key);
        return bestActions[Random.Range(0, bestActions.Count)];
    }

    private void ApplyAction(int action, Vector3 diff, float distance)
    {
        if (currentAttackCooldown > 0 && (action == ACTION_ATTACK_1 || action == ACTION_ATTACK_2))
            action = (distance > longRangeAttackThreshold) ? ACTION_MOVE : ACTION_IDLE;

        animator.ResetTrigger("DoAttack1");
        animator.ResetTrigger("DoAttack2");

        switch (action)
        {
            case ACTION_IDLE: animator.SetFloat("speed", 0f); break;
            case ACTION_MOVE: PerformMovement(diff, distance); break;
            case ACTION_ATTACK_1:
                if (distance <= attackRange)
                {
                    animator.SetFloat("speed", 0f);
                    animator.SetTrigger("DoAttack1");
                    currentAttackCooldown = attackCooldownTime;
                    isInAttack1Loop = true;
                    attack1Count = 1;
                    currentRepeatTimer = timeBetweenAttack1Repeats;
                }
                else animator.SetFloat("speed", 0f);
                break;
            case ACTION_ATTACK_2:
                if (distance > attackRange && distance <= detectionRange)
                {
                    animator.SetFloat("speed", 0f);
                    animator.SetTrigger("DoAttack2");
                    currentAttackCooldown = attackCooldownTime;
                }
                else animator.SetFloat("speed", 0f);
                break;
        }
    }

    private void PerformMovement(Vector3 diff, float distance)
    {
        Vector3 dir = diff.normalized;
        if (distance <= attackRange * 0.5f) { animator.SetFloat("speed", 0f); return; }

        transform.position += dir * speed_walk * Time.deltaTime;
        animator.SetFloat("speed", speed_walk);
        if (dir.x != 0) { Vector3 scale = transform.localScale; scale.x = -Mathf.Sign(dir.x) * Mathf.Abs(scale.x); transform.localScale = scale; }
    }

    private float GetImmediateReward(int action, float distance)
    {
        switch (action)
        {
            case ACTION_IDLE: return reward_Idle;
            case ACTION_MOVE: return (distance > attackRange) ? reward_MovementTowardsPlayer : -reward_MovementTowardsPlayer * 0.5f;
            case ACTION_ATTACK_1:
            case ACTION_ATTACK_2: return reward_AttackAttempt;
            default: return 0f;
        }
    }

    private void UpdateQTable(string state, int action, float reward, string newState)
    {
        if (!qTable.ContainsKey(state)) { qTable[state] = new Dictionary<int, float>(); foreach (int a in allActions) qTable[state][a] = 0f; }
        if (!qTable.ContainsKey(newState)) { qTable[newState] = new Dictionary<int, float>(); foreach (int a in allActions) qTable[newState][a] = 0f; }

        float currentQ = qTable[state][action];
        float maxQ_newState = qTable[newState].Values.Any() ? qTable[newState].Values.Max() : 0f;
        qTable[state][action] = currentQ + learningRate * (reward + discountFactor * maxQ_newState - currentQ);
    }

    public void ReportAttackHit(int attackType)
    {
        if (lastStateKey != "" && lastAction != -1 && lastAction == attackType)
        {
            Vector3 currentDiff = target.transform.position - transform.position;
            float currentDistance = currentDiff.magnitude;
            string currentPlayerRelativePosState = GetPlayerRelativePositionState(currentDiff);
            float reward = (attackType == ACTION_ATTACK_1) ? reward_AttackHitPlayer_ShortRange : reward_AttackHitPlayer_LongRange;
            UpdateQTable(lastStateKey, lastAction, reward, GetCurrentStateKey(currentDiff, currentDistance, currentPlayerRelativePosState));
        }
    }

    public void ReportAttackMiss(int attackType)
    {
        if (lastStateKey != "" && lastAction != -1 && lastAction == attackType)
        {
            Vector3 currentDiff = target.transform.position - transform.position;
            float currentDistance = currentDiff.magnitude;
            string currentPlayerRelativePosState = GetPlayerRelativePositionState(currentDiff);
            UpdateQTable(lastStateKey, lastAction, reward_AttackMissPlayer, GetCurrentStateKey(currentDiff, currentDistance, currentPlayerRelativePosState));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (target != null && collision.gameObject == target && (lastAction == ACTION_MOVE || lastAction == ACTION_IDLE))
        {
            Vector3 currentDiff = target.transform.position - transform.position;
            float currentDistance = currentDiff.magnitude;
            string currentPlayerRelativePosState = GetPlayerRelativePositionState(currentDiff);
            UpdateQTable(lastStateKey, lastAction, reward_BossCollidesWithPlayer, GetCurrentStateKey(currentDiff, currentDistance, currentPlayerRelativePosState));
        }
    }

    [System.Serializable]
    public class QTableData
    {
        public List<string> states = new List<string>();
        public List<QStateData> qValues = new List<QStateData>();
    }

    [System.Serializable]
    public class QStateData
    {
        public List<int> actions = new List<int>();
        public List<float> values = new List<float>();
    }

    public void SaveQTable()
    {
        QTableData data = new QTableData();
        foreach (var stateEntry in qTable)
        {
            data.states.Add(stateEntry.Key);
            QStateData qState = new QStateData();
            foreach (var actionEntry in stateEntry.Value)
            {
                qState.actions.Add(actionEntry.Key);
                qState.values.Add(actionEntry.Value);
            }
            data.qValues.Add(qState);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(qTableFilePath, json);

        Debug.Log("==== Q-TABLE COMPLETA ====");
        foreach (var stateEntry in qTable)
        {
            string line = stateEntry.Key + " : ";
            foreach (var actionEntry in stateEntry.Value) line += $"A{actionEntry.Key}={actionEntry.Value:F2}  ";
            Debug.Log(line);
        }
        Debug.Log("==========================");
    }

    public void LoadQTable()
    {
        if (File.Exists(qTableFilePath))
        {
            string json = File.ReadAllText(qTableFilePath);
            QTableData data = JsonUtility.FromJson<QTableData>(json);
            qTable.Clear();
            for (int i = 0; i < data.states.Count; i++)
            {
                string state = data.states[i];
                qTable[state] = new Dictionary<int, float>();
                for (int j = 0; j < data.qValues[i].actions.Count; j++)
                    qTable[state][data.qValues[i].actions[j]] = data.qValues[i].values[j];
            }
            Debug.Log("Q-Table cargada desde: " + qTableFilePath + " con " + qTable.Count + " estados.");
        }
        else
        {
            Debug.Log("No se encontró Q-Table para cargar en: " + qTableFilePath + ". Se iniciará una nueva.");
        }
    }

    void OnApplicationQuit() { SaveQTable(); }
    void OnDisable() { SaveQTable(); }
}
