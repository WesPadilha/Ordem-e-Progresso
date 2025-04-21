using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreationPerk : MonoBehaviour
{
    [System.Serializable]
    public class PerkDescriptions
    {
        [TextArea(10, 10)]
        public string perk1;
        [TextArea(10, 10)]
        public string perk2;
        [TextArea(10, 10)]
        public string perk3;
        [TextArea(10, 10)]
        public string perk4;
        [TextArea(10, 10)]
        public string perk5;
    }

    public PerkDescriptions descriptions;

    [Header("Perk Buttons")]
    public Button perk1;
    public Button perk2;
    public Button perk3;
    public Button perk4;
    public Button perk5;

    public TMP_Text pointText;

    private bool perk1Chosen = false;
    private bool perk2Chosen = false;
    private bool perk3Chosen = false;
    private bool perk4Chosen = false;
    private bool perk5Chosen = false;

    private int points = 1;

    void Start()
    {
        perk1.onClick.AddListener(() => TogglePerk(1));
        perk2.onClick.AddListener(() => TogglePerk(2));
        perk3.onClick.AddListener(() => TogglePerk(3));
        perk4.onClick.AddListener(() => TogglePerk(4));
        perk5.onClick.AddListener(() => TogglePerk(5));

        SetupPerkDescriptions();
        UpdatePointsText();
        UpdatePerkButtonColors();
    }

    private void SetupPerkDescriptions()
    {
        SetupPerkDescription(perk1, descriptions.perk1);
        SetupPerkDescription(perk2, descriptions.perk2);
        SetupPerkDescription(perk3, descriptions.perk3);
        SetupPerkDescription(perk4, descriptions.perk4);
        SetupPerkDescription(perk5, descriptions.perk5);
    }

    private void SetupPerkDescription(Button button, string description)
    {
        var desc = button.gameObject.AddComponent<ClickDescription>();
        desc.description = description;
    }

    void TogglePerk(int perkNumber)
    {
        if (points == 0 && !IsPerkChosen(perkNumber))
            return;

        switch (perkNumber)
        {
            case 1:
                perk1Chosen = !perk1Chosen;
                points = perk1Chosen ? 0 : 1;
                break;
            case 2:
                perk2Chosen = !perk2Chosen;
                points = perk2Chosen ? 0 : 1;
                break;
            case 3:
                perk3Chosen = !perk3Chosen;
                points = perk3Chosen ? 0 : 1;
                break;
            case 4:
                perk4Chosen = !perk4Chosen;
                points = perk4Chosen ? 0 : 1;
                break;
            case 5:
                perk5Chosen = !perk5Chosen;
                points = perk5Chosen ? 0 : 1;
                break;
        }

        UpdatePointsText();
        UpdatePerkButtonColors();
    }

    void UpdatePointsText()
    {
        pointText.text = "Pontos: " + points.ToString();
    }

    private void UpdatePerkButtonColors()
    {
        UpdatePerkButtonColor(perk1, perk1Chosen);
        UpdatePerkButtonColor(perk2, perk2Chosen);
        UpdatePerkButtonColor(perk3, perk3Chosen);
        UpdatePerkButtonColor(perk4, perk4Chosen);
        UpdatePerkButtonColor(perk5, perk5Chosen);
    }

    private void UpdatePerkButtonColor(Button button, bool isActive)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        colors.normalColor = isActive ? Color.green : Color.white;
        colors.highlightedColor = isActive ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.8f, 0.8f, 0.8f);
        colors.pressedColor = isActive ? new Color(0.1f, 0.5f, 0.1f) : new Color(0.7f, 0.7f, 0.7f);
        colors.selectedColor = isActive ? Color.green : Color.white;
        button.colors = colors;
        
        button.image.color = isActive ? Color.green : Color.white;
    }

    public bool IsAnyPerkChosen()
    {
        return perk1Chosen || perk2Chosen || perk3Chosen || perk4Chosen || perk5Chosen;
    }

    private bool IsPerkChosen(int perkNumber)
    {
        switch (perkNumber)
        {
            case 1: return perk1Chosen;
            case 2: return perk2Chosen;
            case 3: return perk3Chosen;
            case 4: return perk4Chosen;
            case 5: return perk5Chosen;
            default: return false;
        }
    }
}