using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    private int bounceCount = 0;
    public int maxBounces = 3;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceCount++;

        if (bounceCount > maxBounces)
        {
            Destroy(gameObject);
        }
    }
}
