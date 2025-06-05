using UnityEngine;

public enum WeaponRangeCategory
{
    ShortRange,
    MediumRange,
    LongRange
}

public class EnemyWeapon : MonoBehaviour
{
    [Header("Ponta do disparo e projétil")]
    [SerializeField] private Transform ponta;
    [SerializeField] private GameObject prefabEnemyProjectile;

    [Header("Dano (min e max)")]
    [SerializeField] private int danoMin = 25;
    [SerializeField] private int danoMax = 35;

    [Header("Categoria da arma")]
    [SerializeField] private WeaponRangeCategory categoria = WeaponRangeCategory.LongRange;

    public int GetActionPointsCost()
    {
        switch (categoria)
        {
            case WeaponRangeCategory.LongRange:
                return 7;
            case WeaponRangeCategory.MediumRange:
                return 5;
            case WeaponRangeCategory.ShortRange:
                return 2;
            default:
                return 7;
        }
    }

    public float GetAttackRange()
    {
        switch (categoria)
        {
            case WeaponRangeCategory.LongRange:
                return 15f;
            case WeaponRangeCategory.MediumRange:
                return 10f;
            case WeaponRangeCategory.ShortRange:
                return 5f;
            default:
                return 10f;
        }
    }

    public void Atirar()
    {
        Vector3 direcao = transform.forward;

        GameObject projetil = Instantiate(prefabEnemyProjectile, ponta.position, Quaternion.LookRotation(direcao));
        EnemyProjectile flecha = projetil.GetComponent<EnemyProjectile>();

        // Gera um dano aleatório entre danoMin e danoMax
        flecha.dano = Random.Range(danoMin, danoMax + 1);

        RaycastHit hit;
        if (Physics.Raycast(ponta.position, direcao, out hit, Mathf.Infinity))
        {
            flecha.posicaoAlvo = hit.point;
        }
        else
        {
            flecha.posicaoAlvo = ponta.position + direcao * 25f;
        }
    }
}
