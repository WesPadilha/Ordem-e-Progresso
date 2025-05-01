using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public event Action OnDataChanged;
    public event Action OnLifeChanged;

    [Header("Atributos")]
    public int strength;
    public int intellection;
    public int luck;
    public int intelligence;
    public int charisma;
    public int agility;

    [Header("Habilidades")]
    public int arrombamento;
    public int atletismo;
    public int ciencias;
    public int diplomacia;
    public int eletrica;
    public int furtividade;
    public int geografia;
    public int idiomas;
    public int intuicao;
    public int medicina;
    public int mecanica;
    public int negociacao;
    public int religiao;
    public int roubo;

    [Header("Status Jogador")]
    public int maxLife;
    public int actionPoints;
    public int defense;
    public int damage;
    public int currentExperience;
    public int maxExperience = 100;
    public int level = 0;
    public int maxWeight;

    [Header("Progressão")]
    public int availableSkillPoints = 0;

    public void NotifyChanges()
    {
        OnDataChanged?.Invoke();
    }

    public void NotifyLifeChanged()
    {
        OnLifeChanged?.Invoke();
        OnDataChanged?.Invoke();
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
        if (currentExperience >= maxExperience && level < 10)
        {
            LevelUp();
        }
        NotifyChanges();
    }

    private void LevelUp()
    {
        if (level >= 10) return;

        level++;
        currentExperience -= maxExperience;
        maxExperience += 100;

        int bonusLife = Mathf.RoundToInt(0.2f * strength * 10);
        maxLife += bonusLife;

        maxWeight = CalculateMaxWeight(strength);
        actionPoints = CalculateActionPoints(agility);
        
        if (level > 0)
        {
            availableSkillPoints += 20;
        }

        NotifyLifeChanged();
    }

    public void SaveAttributes(CreationAttributes attributes)
    {
        strength = attributes.strength;
        intellection = attributes.intellection;
        luck = attributes.luck;
        intelligence = attributes.intelligence;
        charisma = attributes.charisma;
        agility = attributes.agility;

        maxLife = CalculateMaxLife(strength);
        actionPoints = CalculateActionPoints(agility);
        maxWeight = CalculateMaxWeight(strength);

        NotifyLifeChanged();
    }

    public void SaveSkills(CreationSkills skills)
    {
        arrombamento = Mathf.Min(100, skills.GetFinalArrombamento());
        atletismo = Mathf.Min(100, skills.GetFinalAtletismo());
        ciencias = Mathf.Min(100, skills.GetFinalCiencias());
        diplomacia = Mathf.Min(100, skills.GetFinalDiplomacia());
        eletrica = Mathf.Min(100, skills.GetFinalEletrica());
        furtividade = Mathf.Min(100, skills.GetFinalFurtividade());
        geografia = Mathf.Min(100, skills.GetFinalGeografia());
        idiomas = Mathf.Min(100, skills.GetFinalIdiomas());
        intuicao = Mathf.Min(100, skills.GetFinalIntuicao());
        medicina = Mathf.Min(100, skills.GetFinalMedicina());
        mecanica = Mathf.Min(100, skills.GetFinalMecanica());
        negociacao = Mathf.Min(100, skills.GetFinalNegociacao());
        religiao = Mathf.Min(100, skills.GetFinalReligiao());
        roubo = Mathf.Min(100, skills.GetFinalRoubo());

        NotifyChanges();
    }

    private int CalculateMaxLife(int strength)
    {
        return Mathf.RoundToInt(0.2f * strength * 20);
    }

    private int CalculateMaxWeight(int strength)
    {
        return (40 + strength) * 2;
    }

    private int CalculateActionPoints(int agility)
    {
        return 5 + Mathf.Max(0, agility - 5);
    }
}