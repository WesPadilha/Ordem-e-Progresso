using UnityEngine;
using UnityEngine.UI;

public class ResetScrollbar : MonoBehaviour
{
    // Referência à Scrollbar
    public Scrollbar scrollbar;

    // Este método é chamado quando o script é ativado
    void OnEnable()
    {
        // Garante que a barra de rolagem esteja visível antes de modificar seu valor
        if (scrollbar != null)
        {
            scrollbar.value = 1f; // Define o valor da barra de rolagem como 0
        }
    }

    // Start é chamado antes do primeiro frame de atualização
    void Start()
    {
        // Aqui podemos também garantir que a Scrollbar seja visível
        if (scrollbar != null)
        {
            scrollbar.gameObject.SetActive(true); // Garante que o GameObject da Scrollbar esteja ativo
        }
    }
}
