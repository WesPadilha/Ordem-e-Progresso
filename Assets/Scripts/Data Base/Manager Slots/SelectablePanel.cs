using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectablePanel : MonoBehaviour
{
    public Image panelImage;                 
    public TMP_InputField inputField;       
    public TextMeshProUGUI slotText;        
    public int slotIndex;                  
    public bool isSaveSlot;                

    private Color defaultColor = Color.white;
    private Color selectedColor = Color.yellow;

    private PanelSelector controller;

    public void Initialize(PanelSelector selector)
    {
        controller = selector;
        if (inputField != null)
            inputField.characterLimit = 15; // Limita a 20 caracteres
        Deselect();
    }

    public void OnPanelClick()
    {
        controller.SelectPanel(this);
    }

    public void Select()
    {
        if (panelImage != null)
            panelImage.color = selectedColor;

        controller.SetSelectedSlot(this);

        if (isSaveSlot)
        {
            if (inputField != null) inputField.gameObject.SetActive(true);
            if (slotText != null) slotText.gameObject.SetActive(false);
        }
    }

    public void Deselect()
    {
        if (panelImage != null)
            panelImage.color = defaultColor;

        if (isSaveSlot)
        {
            if (inputField != null) inputField.gameObject.SetActive(false);
            if (slotText != null) slotText.gameObject.SetActive(true);
        }
    }

    public void SetSlotName(string newName)
    {
        if (slotText != null)
            slotText.text = newName;

        if (inputField != null)
            inputField.text = newName;
    }

    public string GetSlotName()
    {
        return slotText != null ? slotText.text : "[Vazio]";
    }
}
