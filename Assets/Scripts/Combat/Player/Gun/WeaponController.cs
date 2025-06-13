using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public enum WeaponType { LongRange, MediumRange }
    public WeaponType weaponType = WeaponType.MediumRange;

    public enum WeaponCategory { Pistol, Rifle, MachineGun }
    public WeaponCategory weaponCategory = WeaponCategory.Pistol;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;

    [Header("Dados de Munição")]
    public WeaponLoader weaponLoader;

    [Header("Tipo de Munição Aceita")]
    public AmmoType requiredAmmoType = AmmoType.None; // <- Exemplo padrão

    [Header("Dano")]
    public float minDamage = 5f;
    public float maxDamage = 15f;

    [Header("Dados do Personagem")]
    public CharacterData characterData;

    private float nextFireTime = 0f;
    private float range;

    void Start()
    {
        range = weaponType == WeaponType.LongRange ? 15f : 10f;
    }

    public int GetActionPointCost()
    {
        return weaponType == WeaponType.LongRange ? 6 : 4;
    }

    public bool CanShootAtEnemy()
    {
        return bulletPrefab != null && firePoint != null;
    }

    public bool CanShoot(Vector3 targetPosition)
    {
        if (firePoint == null) return false;
        float distance = Vector3.Distance(firePoint.position, targetPosition);
        return distance <= range;
    }

    private (int damage, bool isCritical) CalculateDamage()
    {
        float critChance = characterData != null ? Mathf.Clamp01(characterData.luck / 100f) : 0f;
        bool isCritical = Random.value < critChance;

        float damage = isCritical ? maxDamage * 2f : Random.Range(minDamage, maxDamage);
        return (Mathf.CeilToInt(damage), isCritical);
    }

    public void Shoot(Vector3 targetPosition)
    {
        if (Time.time < nextFireTime || firePoint == null || !CanShoot(targetPosition) || weaponLoader == null)
            return;

        // Aqui adicionamos a verificação de tipo de munição
        if (weaponLoader is WeaponLoaderWithType loaderWithType)
        {
            if (loaderWithType.ammoType != requiredAmmoType)
                return; // Tipo de munição errado
        }

        if (weaponCategory == WeaponCategory.MachineGun)
        {
            int bulletsCount = 5;
            if (weaponLoader.ammoCurrent < bulletsCount) return;
            weaponLoader.ammoCurrent -= bulletsCount;
            StartCoroutine(ShootMachineGun(targetPosition, bulletsCount));
        }
        else
        {
            if (weaponLoader.ammoCurrent < 1) return;
            weaponLoader.ammoCurrent--;
            ShootSingleBullet(targetPosition);
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootSingleBullet(Vector3 targetPosition)
    {
        var (finalDamage, isCritical) = CalculateDamage();

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector3 direction = (targetPosition - firePoint.position).normalized;

        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetDirection(direction);
        bulletController.SetDamage(finalDamage, isCritical);
    }

    private IEnumerator ShootMachineGun(Vector3 targetPosition, int bulletsCount)
    {
        float critChance = characterData != null ? Mathf.Clamp01(characterData.luck / 100f) : 0f;
        bool isCritical = Random.value < critChance;

        int totalDamage = isCritical
            ? Mathf.CeilToInt(maxDamage * 2f)
            : Mathf.CeilToInt(Random.Range(minDamage, maxDamage));

        int basePerBullet = totalDamage / bulletsCount;
        int remainder = totalDamage % bulletsCount;

        int[] bulletDamages = new int[bulletsCount];
        for (int i = 0; i < bulletsCount; i++)
        {
            bulletDamages[i] = basePerBullet + (i < remainder ? 1 : 0);
        }

        float interval = 0.01f;

        for (int i = 0; i < bulletsCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.SetDirection(direction);
            bulletController.SetDamage(bulletDamages[i], isCritical);

            yield return new WaitForSeconds(interval);
        }

        nextFireTime = Time.time + fireRate;
    }
}
