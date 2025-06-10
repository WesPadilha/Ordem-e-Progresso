using UnityEngine;
using UnityEngine.UI;

public class LifeEnemy : MonoBehaviour
{
    public Slider slider; // Slider para exibir a vida
    public int maxLife = 10; // Vida máxima do inimigo
    private int currentLife; // Vida atual do inimigo
    public int expAmount = 25; // Quantidade de XP que o inimigo dá ao morrer

    void Start()
    {
        currentLife = maxLife;
        slider.maxValue = maxLife;
        slider.value = currentLife;
    }

    public void TakeDamage(int damage)
    {
        currentLife -= damage;
        slider.value = currentLife; // Atualiza o slider quando tomar dano
        
        if (currentLife <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo derrotado! XP concedido: " + expAmount);
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(expAmount);
        }
        Destroy(gameObject);
    }
}