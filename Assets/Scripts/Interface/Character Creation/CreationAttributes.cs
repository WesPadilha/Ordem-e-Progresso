using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreationAttributes : MonoBehaviour
{
    public CreationSkills skills;
    public CharacterData characterData;
    
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

    public int strength;
    public int intellection;
    public int luck;
    public int intelligence;
    public int charisma;
    public int agility;

    void Start()
    {
        InitializeDefaultAttributes();

        // Button listeners
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

        UpdateUI();
    }

    private void InitializeDefaultAttributes()
    {
        strength = 5;
        intellection = 5;
        luck = 5;
        intelligence = 5;
        charisma = 5;
        agility = 5;

        UpdateUI();
    }

    void IncreaseAttribute(string attribute)
    {
        if (availablePoints <= 0) return;

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

    public void ResetAttributePoints()
    {
        availablePoints = 5;
        InitializeDefaultAttributes();
    }

    public int GetAvailablePoints()
    {
        return availablePoints;
    }

    public void UpdateUI()
    {
        // Update attributes display
        strengthText.text = strength.ToString();
        intellectionText.text = intellection.ToString();
        luckText.text = luck.ToString();
        intelligenceText.text = intelligence.ToString();
        charismaText.text = charisma.ToString();
        agilityText.text = agility.ToString();

        // Update derived stats display
        lifeText.text = $"Vida: {CalculateMaxLife()}";
        weightText.text = $"Peso: {CalculateMaxWeight()}";
        actionPointsText.text = $"PA: {CalculateActionPoints()}";

        availablePointsText.text = "Pontos: " + availablePoints;

        if(skills != null)
        {
            skills.UpdateSkillsBasedOnAttributes();
        }
    }

    private int CalculateMaxLife()
    {
        // 20% da Força x 20
        return Mathf.RoundToInt(0.2f * strength * 20);
    }

    private int CalculateMaxWeight()
    {
        // (40 + Força) x 2
        return (40 + strength) * 2;
    }

    private int CalculateActionPoints()
    {
        // Base 5 PA + 1 for each point in Agility above 5
        return 5 + Mathf.Max(0, agility - 5);
    }

    public void SaveToCharacterData()
    {
        if (characterData != null)
        {
            characterData.SaveAttributes(this);
            
            // Atualiza a UI para refletir os valores salvos
            UpdateUI();
        }
    }
}