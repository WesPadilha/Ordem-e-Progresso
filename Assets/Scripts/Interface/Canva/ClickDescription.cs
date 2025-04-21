using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClickDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 10)]
    public string description;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        DescriptionManager.Instance.ShowDescription(description, gameObject);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Não esconde mais a descrição ao sair do botão
        // A descrição só será atualizada quando entrar em outro botão
    }
    
    // Opcional: Se quiser limpar a descrição quando clicar no botão
    public void OnPointerClick(PointerEventData eventData)
    {
        DescriptionManager.Instance.ClearCurrentHover();
    }
}