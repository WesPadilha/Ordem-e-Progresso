// CharacterStatusData.cs
using System;
using UnityEngine;

[Serializable]
public class CharacterStatusData
{
    // Atributos
    public int strength;
    public int intellection;
    public int luck;
    public int intelligence;
    public int charisma;
    public int agility;

    // Habilidades
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

    // Status
    public int currentLife;
    public int maxLife;
    public int actionPoints;
    public int defense;
    public int damage;
    public int currentExperience;
    public int maxExperience;
    public int level;
    public float currentWeight;
    public float maxWeight;

    public int availableSkillPoints;

    public CharacterStatusData(CharacterData data)
    {
        strength = data.strength;
        intellection = data.intellection;
        luck = data.luck;
        intelligence = data.intelligence;
        charisma = data.charisma;
        agility = data.agility;

        arrombamento = data.arrombamento;
        atletismo = data.atletismo;
        ciencias = data.ciencias;
        diplomacia = data.diplomacia;
        eletrica = data.eletrica;
        furtividade = data.furtividade;
        geografia = data.geografia;
        idiomas = data.idiomas;
        intuicao = data.intuicao;
        medicina = data.medicina;
        mecanica = data.mecanica;
        negociacao = data.negociacao;
        religiao = data.religiao;
        roubo = data.roubo;

        currentLife = data.currentLife;
        maxLife = data.maxLife;
        actionPoints = data.actionPoints;
        defense = data.defense;
        damage = data.damage;
        currentExperience = data.currentExperience;
        maxExperience = data.maxExperience;
        level = data.level;
        currentWeight = data.currentWeight;
        maxWeight = data.maxWeight;

        availableSkillPoints = data.availableSkillPoints;
    }

    public void LoadTo(CharacterData data)
    {
        data.strength = strength;
        data.intellection = intellection;
        data.luck = luck;
        data.intelligence = intelligence;
        data.charisma = charisma;
        data.agility = agility;

        data.arrombamento = arrombamento;
        data.atletismo = atletismo;
        data.ciencias = ciencias;
        data.diplomacia = diplomacia;
        data.eletrica = eletrica;
        data.furtividade = furtividade;
        data.geografia = geografia;
        data.idiomas = idiomas;
        data.intuicao = intuicao;
        data.medicina = medicina;
        data.mecanica = mecanica;
        data.negociacao = negociacao;
        data.religiao = religiao;
        data.roubo = roubo;

        data.currentLife = currentLife;
        data.maxLife = maxLife;
        data.actionPoints = actionPoints;
        data.defense = defense;
        data.damage = damage;
        data.currentExperience = currentExperience;
        data.maxExperience = maxExperience;
        data.level = level;
        data.currentWeight = currentWeight;
        data.maxWeight = maxWeight;

        data.availableSkillPoints = availableSkillPoints;

        data.NotifyLifeChanged();
    }
}
