using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float maxDistance = 10f;
    public bool isCritical = false;
    
    private Vector3 direction;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    public void SetDamage(float value, bool critical)
    {
        damage = value;
        isCritical = critical;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            LifeEnemy lifeEnemy = other.GetComponent<LifeEnemy>();
            if (lifeEnemy != null)
            {
                lifeEnemy.TakeDamage((int)damage, isCritical);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player"))// && !other.CompareTag("Weapon"))
        {
            Destroy(gameObject);
        }
    }
}