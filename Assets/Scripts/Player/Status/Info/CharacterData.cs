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
    public int currentLife;
    public int maxLife;
    public int actionPoints;
    public int defense;
    public int damage;
    public int currentExperience;
    public int maxExperience = 300;
    public int level = 0;
    public float currentWeight;
    public float maxWeight;

    [Header("Progressão")]
    public int availableSkillPoints = 0;

    public void Initialize()
    {
        // Apenas define currentLife = maxLife se ainda não tiver sido definido (ou estiver inválido)
        if (currentLife <= 0 || currentLife > maxLife)
        {
            currentLife = maxLife;
        }

        maxWeight = CalculateMaxWeight(strength);
        currentWeight = 0; // Inicia com 0 de peso
        actionPoints = CalculateActionPoints(agility);
        NotifyLifeChanged();
    }

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

    public void SetCurrentLife(int newLife)
    {
        currentLife = Mathf.Clamp(newLife, 0, maxLife);
        NotifyLifeChanged();
    }

    private void LevelUp()
    {
        if (level >= 10) return;

        // Verifica se a vida está cheia antes do level up
        bool wasFullLife = currentLife == maxLife;

        level++;
        currentExperience -= maxExperience;
        maxExperience = 300 * (level + 1);

        int bonusLife = Mathf.RoundToInt(0.2f * strength * 10);
        int previousMaxLife = maxLife;
        maxLife += bonusLife;

        // Se a vida estava cheia antes do level up, atualiza currentLife para o novo maxLife
        if (wasFullLife)
        {
            currentLife = maxLife;
        }
        // Caso contrário, mantém a mesma porcentagem de vida
        else
        {
            float lifePercentage = (float)currentLife / previousMaxLife;
            currentLife = Mathf.RoundToInt(maxLife * lifePercentage);
        }

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

    public void UpdateCurrentWeight()
    {
        currentWeight = 0;
        // Você precisará acessar o inventário do jogador aqui
        // Isso será implementado no próximo passo
        NotifyChanges();
    }

    public float GetNegotiationBuyDiscount()
    {
        // 5% de desconto a cada 10 pontos, máximo de 50% (100 pontos)
        int negotiationBonus = Mathf.Min(negociacao / 10, 10); // Máximo de 10 níveis (100 pontos)
        return 1f - (negotiationBonus * 0.05f); // Retorna um multiplicador (0.95, 0.90, etc.)
    }

    public float GetNegotiationSellBonus()
    {
        // 5% de aumento a cada 10 pontos, máximo de 50% (100 pontos)
        int negotiationBonus = Mathf.Min(negociacao / 10, 10); // Máximo de 10 níveis (100 pontos)
        return 1f + (negotiationBonus * 0.05f); // Retorna um multiplicador (1.05, 1.10, etc.)
    }

    public void AddNegotiationPoints(int points)
    {
        int oldNegotiation = negociacao;
        negociacao = Mathf.Min(negociacao + points, 100); // Máximo de 100 pontos

        if (negociacao != oldNegotiation)
        {
            NotifyChanges(); // Dispara o evento de mudança
        }
    }
    public void ResetData()
    {
        strength = 0;
        intellection = 0;
        luck = 0;
        intelligence = 0;
        charisma = 0;
        agility = 0;

        arrombamento = 0;
        atletismo = 0;
        ciencias = 0;
        diplomacia = 0;
        eletrica = 0;
        furtividade = 0;
        geografia = 0;
        idiomas = 0;
        intuicao = 0;
        medicina = 0;
        mecanica = 0;
        negociacao = 0;
        religiao = 0;
        roubo = 0;

        currentLife = 0;
        currentWeight = 0;
        maxWeight = 0;
        actionPoints = 0;
        currentExperience = 0;
        maxExperience = 300;
        level = 0;
        availableSkillPoints = 0;
        
        NotifyLifeChanged();
    }
}