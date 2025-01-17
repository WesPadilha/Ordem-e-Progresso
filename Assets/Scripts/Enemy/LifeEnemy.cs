using UnityEngine;
using UnityEngine.UI;

public class LifeEnemy : MonoBehaviour
{
    public Slider slider; // Slider para exibir a vida
    public int maxLife = 10; // Vida máxima do inimigo
    private int currentLife; // Vida atual do inimigo

    void Start()
    {
        currentLife = maxLife; // Inicializa a vida com o valor máximo
        slider.maxValue = maxLife; // Define o valor máximo do Slider
        slider.value = currentLife; // Define o valor inicial do Slider
    }

    void Update()
    {
        slider.value = currentLife; // Atualiza o Slider com a vida atual
    }

    void OnMouseDown()
    {
        // Quando o inimigo for clicado, tira 1 ponto de vida
        if (currentLife > 0)
        {
            currentLife -= 1; // Reduz 1 ponto de vida
        }
        
        // Verifica se a vida do inimigo chegou a 0 (opcional)
        if (currentLife <= 0)
        {
            Destroy(gameObject); // Destrói o inimigo quando a vida chegar a 0
        }
    }
}
