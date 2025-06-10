using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public enum WeaponType { LongRange, MediumRange }
    public WeaponType weaponType = WeaponType.MediumRange;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;

    [Header("Dano")]
    public float minDamage = 5f;
    public float maxDamage = 15f;

    private float nextFireTime = 0f;
    private float range;

    void Start()
    {
        range = weaponType == WeaponType.LongRange ? 15f : 7f;
    }

    // Em WeaponController.cs
    public int GetActionPointCost()
    {
        return weaponType == WeaponType.LongRange ? 7 : 4;
    }

    public bool CanShoot(Vector3 targetPosition)
    {
        if (firePoint == null) return false;
        float distance = Vector3.Distance(firePoint.position, targetPosition);
        return distance <= range;
    }

    public void Shoot(Vector3 targetPosition)
    {
        if (Time.time >= nextFireTime && firePoint != null)
        {
            if (CanShoot(targetPosition))
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Vector3 direction = (targetPosition - firePoint.position).normalized;

                BulletController bulletController = bullet.GetComponent<BulletController>();
                bulletController.SetDirection(direction);

                float randomDamage = Random.Range(minDamage, maxDamage); // Dano aleatório
                bulletController.SetDamage(randomDamage);

                nextFireTime = Time.time + fireRate;
            }
        }
    }
}
