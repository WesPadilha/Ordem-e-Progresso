using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageZone : MonoBehaviour
{
    [Header("Configurações de Dano")]
    [SerializeField] private int damagePerSecond = 10;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private bool destroyOnContact = false;

    private float nextDamageTime;
    private PlayerLife playerLife;

    private void Start()
    {
        // Garante que o collider está em modo Trigger
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // Garante que há um Rigidbody para que os triggers funcionem
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLife = other.GetComponent<PlayerLife>();

            if (playerLife != null && destroyOnContact)
            {
                playerLife.TakeDamage(damagePerSecond);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerLife != null)
        {
            if (Time.time >= nextDamageTime)
            {
                playerLife.TakeDamage(damagePerSecond);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLife = null;
        }
    }
}
