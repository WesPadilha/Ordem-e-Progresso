using UnityEngine;
using TMPro;

public class DescriptionManager : MonoBehaviour
{
    public static DescriptionManager Instance;
    
    public TMP_Text descriptionText;
    private GameObject currentHoveredObject;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        HideDescription();
    }
    
    public void ShowDescription(string description, GameObject source)
    {
        // Só atualiza se for um objeto diferente do atual
        if (source != currentHoveredObject)
        {
            descriptionText.text = description;
            currentHoveredObject = source;
        }
    }
    
    public void HideDescription()
    {
        descriptionText.text = "";
        currentHoveredObject = null;
    }

    public void ClearCurrentHover()
    {
        currentHoveredObject = null;
    }

    
}