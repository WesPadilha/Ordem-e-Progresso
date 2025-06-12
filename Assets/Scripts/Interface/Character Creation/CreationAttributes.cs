using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreationAttributes : MonoBehaviour
{
    [System.Serializable]
    public class AttributeDescriptions
    {
        [TextArea(3, 10)]
        public string strength;
        [TextArea(3, 10)]
        public string intellection;
        [TextArea(3, 10)]
        public string luck;
        [TextArea(3, 10)]
        public string intelligence;
        [TextArea(3, 10)]
        public string charisma;
        [TextArea(3, 10)]
        public string agility;
    }

    public CreationSkills skills;
    public CharacterData characterData;
    public AttributeDescriptions descriptions;
    
    [Header("UI Elements")]
    public TMP_Text strengthText;
    public TMP_Text intellectionText;
    public TMP_Text luckText;
    public TMP_Text intelligenceText;
    public TMP_Text charismaText;
    public TMP_Text agilityText;
    public TMP_Text lifeText;
    public TMP_Text weightText;
    public TMP_Text actionPointsText;
    public TMP_Text criticalChanceText;

    [Header("Buttons")]
    public Button increaseStrengthBtn;
    public Button decreaseStrengthBtn;
    public Button increaseIntellectionBtn;
    public Button decreaseIntellectionBtn;
    public Button increaseLuckBtn;
    public Button decreaseLuckBtn;
    public Button increaseIntelligenceBtn;
    public Button decreaseIntelligenceBtn;
    public Button increaseCharismaBtn;
    public Button decreaseCharismaBtn;
    public Button increaseAgilityBtn;
    public Button decreaseAgilityBtn;

    public TMP_Text availablePointsText;

    private int availablePoints = 5;
    private const int MIN_ATTRIBUTE_VALUE = 1;
    private const int MAX_ATTRIBUTE_VALUE = 10;

    // Armazena os valores anteriores para cálculo das habilidades
    public int previousStrength;
    public int previousIntellection;
    public int previousLuck;
    public int previousIntelligence;
    public int previousCharisma;
    public int previousAgility;

    public int strength;
    public int intellection;
    public int luck;
    public int intelligence;
    public int charisma;
    public int agility;

    void Start()
    {
        InitializeDefaultAttributes();
        // Armazena os valores iniciais como anteriores
        previousStrength = strength;
        previousIntellection = intellection;
        previousLuck = luck;
        previousIntelligence = intelligence;
        previousCharisma = charisma;
        previousAgility = agility;
        
        SetupButtonListeners();
        SetupAttributeButtons();
        UpdateUI();
    }

    private void SetupButtonListeners()
    {
        increaseStrengthBtn.onClick.AddListener(() => IncreaseAttribute("Strength"));
        decreaseStrengthBtn.onClick.AddListener(() => DecreaseAttribute("Strength"));
        increaseIntellectionBtn.onClick.AddListener(() => IncreaseAttribute("Intellection"));
        decreaseIntellectionBtn.onClick.AddListener(() => DecreaseAttribute("Intellection"));
        increaseLuckBtn.onClick.AddListener(() => IncreaseAttribute("Luck"));
        decreaseLuckBtn.onClick.AddListener(() => DecreaseAttribute("Luck"));
        increaseIntelligenceBtn.onClick.AddListener(() => IncreaseAttribute("Intelligence"));
        decreaseIntelligenceBtn.onClick.AddListener(() => DecreaseAttribute("Intelligence"));
        increaseCharismaBtn.onClick.AddListener(() => IncreaseAttribute("Charisma"));
        decreaseCharismaBtn.onClick.AddListener(() => DecreaseAttribute("Charisma"));
        increaseAgilityBtn.onClick.AddListener(() => IncreaseAttribute("Agility"));
        decreaseAgilityBtn.onClick.AddListener(() => DecreaseAttribute("Agility"));
    }

    private void SetupAttributeButtons()
    {
        SetupButtonDescription(increaseStrengthBtn, descriptions.strength);
        SetupButtonDescription(increaseIntellectionBtn, descriptions.intellection);
        SetupButtonDescription(increaseLuckBtn, descriptions.luck);
        SetupButtonDescription(increaseIntelligenceBtn, descriptions.intelligence);
        SetupButtonDescription(increaseCharismaBtn, descriptions.charisma);
        SetupButtonDescription(increaseAgilityBtn, descriptions.agility);
    }

    private void SetupButtonDescription(Button button, string description)
    {
        var desc = button.gameObject.AddComponent<ClickDescription>();
        desc.description = description;
    }

    private void InitializeDefaultAttributes()
    {
        strength = 5;
        intellection = 5;
        luck = 5;
        intelligence = 5;
        charisma = 5;
        agility = 5;
    }

    void IncreaseAttribute(string attribute)
    {
        if (availablePoints <= 0) return;

        // Armazena os valores atuais antes de modificar
        StorePreviousAttributes();

        switch (attribute)
        {
            case "Strength":
                if (strength < MAX_ATTRIBUTE_VALUE)
                {
                    strength++;
                    availablePoints--;
                }
                break;
            case "Intellection":
                if (intellection < MAX_ATTRIBUTE_VALUE)
                {
                    intellection++;
                    availablePoints--;
                }
                break;
            case "Luck":
                if (luck < MAX_ATTRIBUTE_VALUE)
                {
                    luck++;
                    availablePoints--;
                }
                break;
            case "Intelligence":
                if (intelligence < MAX_ATTRIBUTE_VALUE)
                {
                    intelligence++;
                    availablePoints--;
                }
                break;
            case "Charisma":
                if (charisma < MAX_ATTRIBUTE_VALUE)
                {
                    charisma++;
                    availablePoints--;
                }
                break;
            case "Agility":
                if (agility < MAX_ATTRIBUTE_VALUE)
                {
                    agility++;
                    availablePoints--;
                }
                break;
        }

        UpdateUI();
    }

    void DecreaseAttribute(string attribute)
    {
        // Armazena os valores atuais antes de modificar
        StorePreviousAttributes();

        switch (attribute)
        {
            case "Strength":
                if (strength > MIN_ATTRIBUTE_VALUE)
                {
                    strength--;
                    availablePoints++;
                }
                break;
            case "Intellection":
                if (intellection > MIN_ATTRIBUTE_VALUE)
                {
                    intellection--;
                    availablePoints++;
                }
                break;
            case "Luck":
                if (luck > MIN_ATTRIBUTE_VALUE)
                {
                    luck--;
                    availablePoints++;
                }
                break;
            case "Intelligence":
                if (intelligence > MIN_ATTRIBUTE_VALUE)
                {
                    intelligence--;
                    availablePoints++;
                }
                break;
            case "Charisma":
                if (charisma > MIN_ATTRIBUTE_VALUE)
                {
                    charisma--;
                    availablePoints++;
                }
                break;
            case "Agility":
                if (agility > MIN_ATTRIBUTE_VALUE)
                {
                    agility--;
                    availablePoints++;
                }
                break;
        }

        UpdateUI();
    }

    private void StorePreviousAttributes()
    {
        previousStrength = strength;
        previousIntellection = intellection;
        previousLuck = luck;
        previousIntelligence = intelligence;
        previousCharisma = charisma;
        previousAgility = agility;
    }

    public void ResetAttributePoints()
    {
        availablePoints = 5;
        InitializeDefaultAttributes();
        StorePreviousAttributes();
        UpdateUI();
    }

    public int GetAvailablePoints()
    {
        return availablePoints;
    }

    public void UpdateUI()
    {
        strengthText.text = strength.ToString();
        intellectionText.text = intellection.ToString();
        luckText.text = luck.ToString();
        intelligenceText.text = intelligence.ToString();
        charismaText.text = charisma.ToString();
        agilityText.text = agility.ToString();

        lifeText.text = $"Vida: {CalculateMaxLife()}";
        weightText.text = $"Peso: {CalculateMaxWeight()}";
        actionPointsText.text = $"PA: {CalculateActionPoints()}";
        criticalChanceText.text = $"Crítico: {luck}%";

        availablePointsText.text = "Pontos: " + availablePoints;

        if(skills != null)
        {
            skills.UpdateSkillsBasedOnAttributes();
        }
    }

    private int CalculateMaxLife()
    {
        return Mathf.RoundToInt(0.2f * strength * 20);
    }

    private int CalculateMaxWeight()
    {
        return (40 + strength) * 2;
    }

    private int CalculateActionPoints()
    {
        return 5 + Mathf.Max(0, agility - 5);
    }

    public void SaveToCharacterData()
    {
        if (characterData != null)
        {
            characterData.SaveAttributes(this);
            if (skills != null)
            {
                characterData.SaveSkills(skills);
            }
            UpdateUI();
        }
    }
}