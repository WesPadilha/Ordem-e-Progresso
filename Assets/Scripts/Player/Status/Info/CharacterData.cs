using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
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
    public int level;
    public int maxWeight;

    public void SaveAttributes(CreationAttributes attributes)
    {
        strength = attributes.strength;
        intellection = attributes.intellection;
        luck = attributes.luck;
        intelligence = attributes.intelligence;
        charisma = attributes.charisma;
        agility = attributes.agility;

        // Calcular e salvar os valores derivados
        maxLife = CalculateMaxLife(strength);
        actionPoints = CalculateActionPoints(agility);
        maxWeight = CalculateMaxWeight(strength);
    }

    public void SaveSkills(CreationSkills skills)
    {
        arrombamento = skills.arrombamento;
        atletismo = skills.atletismo;
        ciencias = skills.ciencias;
        diplomacia = skills.diplomacia;
        eletrica = skills.eletrica;
        furtividade = skills.furtividade;
        geografia = skills.geografia;
        idiomas = skills.idiomas;
        intuicao = skills.intuicao;
        medicina = skills.medicina;
        mecanica = skills.mecanica;
        negociacao = skills.negociacao;
        religiao = skills.religiao;
        roubo = skills.roubo;
    }

    private int CalculateMaxLife(int strength)
    {
        // 20% da Força x 20
        return Mathf.RoundToInt(0.2f * strength * 20);
    }

    private int CalculateMaxWeight(int strength)
    {
        // (40 + Força) x 2
        return (40 + strength) * 2;
    }

    private int CalculateActionPoints(int agility)
    {
        // Base 5 PA + 1 para cada ponto em Agilidade acima de 5
        return 5 + Mathf.Max(0, agility - 5);
    }
}