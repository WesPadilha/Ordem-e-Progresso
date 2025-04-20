using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAttributesSkills : MonoBehaviour
{
    public CharacterData characterData;
    
    [Header("Attributes UI References")]
    public TMP_Text strengthText;
    public TMP_Text intellectionText;
    public TMP_Text luckText;
    public TMP_Text intelligenceText;
    public TMP_Text charismaText;
    public TMP_Text agilityText;
    
    [Header("Skills UI References")]
    public TMP_Text arrombamentoText;
    public TMP_Text atletismoText;
    public TMP_Text cienciasText;
    public TMP_Text diplomaciaText;
    public TMP_Text eletricaText;
    public TMP_Text furtividadeText;
    public TMP_Text geografiaText;
    public TMP_Text idiomasText;
    public TMP_Text intuicaoText;
    public TMP_Text medicinaText;
    public TMP_Text mecanicaText;
    public TMP_Text negociacaoText;
    public TMP_Text religiaoText;
    public TMP_Text rouboText;
    
    [Header("Status UI References")]
    public TMP_Text maxLifeText;
    public TMP_Text actionPointsText;
    public TMP_Text defenseText;
    public TMP_Text levelText;
    public TMP_Text maxWeightText;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (characterData == null)
        {
            Debug.LogWarning("CharacterData not assigned!");
            return;
        }

        // Update Attributes with labels
        strengthText.text = $"Força: {characterData.strength}";
        intellectionText.text = $"Intelecto: {characterData.intellection}";
        luckText.text = $"Sorte: {characterData.luck}";
        intelligenceText.text = $"Inteligência: {characterData.intelligence}";
        charismaText.text = $"Carisma: {characterData.charisma}";
        agilityText.text = $"Agilidade: {characterData.agility}";

        // Update Skills with labels
        arrombamentoText.text = $"Arrombamento: {characterData.arrombamento}";
        atletismoText.text = $"Atletismo: {characterData.atletismo}";
        cienciasText.text = $"Ciências: {characterData.ciencias}";
        diplomaciaText.text = $"Diplomacia: {characterData.diplomacia}";
        eletricaText.text = $"Elétrica: {characterData.eletrica}";
        furtividadeText.text = $"Furtividade: {characterData.furtividade}";
        geografiaText.text = $"Geografia: {characterData.geografia}";
        idiomasText.text = $"Idiomas: {characterData.idiomas}";
        intuicaoText.text = $"Intuição: {characterData.intuicao}";
        medicinaText.text = $"Medicina: {characterData.medicina}";
        mecanicaText.text = $"Mecânica: {characterData.mecanica}";
        negociacaoText.text = $"Negociação: {characterData.negociacao}";
        religiaoText.text = $"Religião: {characterData.religiao}";
        rouboText.text = $"Roubo: {characterData.roubo}";

        // Update Status with labels
        maxLifeText.text = $"Vida: {characterData.maxLife}";
        actionPointsText.text = $"PA: {characterData.actionPoints}";
        defenseText.text = $"Defesa: {characterData.defense}";
        levelText.text = $"Nível: {characterData.level}";
        maxWeightText.text = $"Peso Máximo: {characterData.maxWeight}kg";
    }

    public void RefreshUI()
    {
        UpdateUI();
    }
}