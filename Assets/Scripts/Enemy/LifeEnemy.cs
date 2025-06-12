using UnityEngine;
using UnityEngine.UI;

public class LifeEnemy : MonoBehaviour
{
    public Slider slider; // Slider para exibir a vida
    public int maxLife = 10; // Vida máxima do inimigo
    private int currentLife; // Vida atual do inimigo
    public int expAmount = 25; // Quantidade de XP que o inimigo dá ao morrer
    
    private EnemyChase enemyChase;
    private EnemyGroupController groupController;

    void Start()
    {
        currentLife = maxLife;
        slider.maxValue = maxLife;
        slider.value = currentLife;
        
        // Obtém as referências no Start
        enemyChase = GetComponent<EnemyChase>();
        groupController = GetComponentInParent<EnemyGroupController>();
    }

    public void TakeDamage(int damage, bool isCritical)
    {
        currentLife -= damage;
        slider.value = currentLife;

        // Cria o popup de dano
        DamagePopupPro.Create(transform.position, damage, isCritical);

        if (currentLife <= 0)
        {
            Die();
        }
        
        if (enemyChase != null && !enemyChase.IsChasing() && groupController != null)
        {
            groupController.StartGroupChase();
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo derrotado! XP concedido: " + expAmount);
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(expAmount);
        }

        // Notifica o groupController antes de destruir
        EnemyGroupController groupController = GetComponentInParent<EnemyGroupController>();
        EnemyChase enemyChase = GetComponent<EnemyChase>();
        
        if (groupController != null && enemyChase != null)
        {
            groupController.RemoveEnemy(enemyChase);
        }

        Destroy(gameObject);
    }
}