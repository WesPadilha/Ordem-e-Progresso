using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUpSkills : MonoBehaviour
{
    public CharacterData characterData;
    
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

    [Header("Skill Buttons")]
    public Button increaseArrombamentoBtn;
    public Button decreaseArrombamentoBtn;
    public Button increaseAtletismoBtn;
    public Button decreaseAtletismoBtn;
    public Button increaseCienciasBtn;
    public Button decreaseCienciasBtn;
    public Button increaseDiplomaciaBtn;
    public Button decreaseDiplomaciaBtn;
    public Button increaseEletricaBtn;
    public Button decreaseEletricaBtn;
    public Button increaseFurtividadeBtn;
    public Button decreaseFurtividadeBtn;
    public Button increaseGeografiaBtn;
    public Button decreaseGeografiaBtn;
    public Button increaseIdiomasBtn;
    public Button decreaseIdiomasBtn;
    public Button increaseIntuicaoBtn;
    public Button decreaseIntuicaoBtn;
    public Button increaseMedicinaBtn;
    public Button decreaseMedicinaBtn;
    public Button increaseMecanicaBtn;
    public Button decreaseMecanicaBtn;
    public Button increaseNegociacaoBtn;
    public Button decreaseNegociacaoBtn;
    public Button increaseReligiaoBtn;
    public Button decreaseReligiaoBtn;
    public Button increaseRouboBtn;
    public Button decreaseRouboBtn;
    
    [Header("UI Elements")]
    public TMP_Text availablePointsText;
    public Button confirmChangesButton;
    
    private Dictionary<string, int> temporarySkillChanges = new Dictionary<string, int>();
    private int pointsUsed = 0;
    private const int MAX_SKILL_VALUE = 100;

    private void Start()
    {
        if (characterData != null)
        {
            characterData.OnDataChanged += UpdateUI;
            UpdateUI();
        }
        
        // Configura todos os botões de aumento e diminuição
        increaseArrombamentoBtn.onClick.AddListener(() => ModifySkill("arrombamento", 1));
        decreaseArrombamentoBtn.onClick.AddListener(() => ModifySkill("arrombamento", -1));
        increaseAtletismoBtn.onClick.AddListener(() => ModifySkill("atletismo", 1));
        decreaseAtletismoBtn.onClick.AddListener(() => ModifySkill("atletismo", -1));
        increaseCienciasBtn.onClick.AddListener(() => ModifySkill("ciencias", 1));
        decreaseCienciasBtn.onClick.AddListener(() => ModifySkill("ciencias", -1));
        increaseDiplomaciaBtn.onClick.AddListener(() => ModifySkill("diplomacia", 1));
        decreaseDiplomaciaBtn.onClick.AddListener(() => ModifySkill("diplomacia", -1));
        increaseEletricaBtn.onClick.AddListener(() => ModifySkill("eletrica", 1));
        decreaseEletricaBtn.onClick.AddListener(() => ModifySkill("eletrica", -1));
        increaseFurtividadeBtn.onClick.AddListener(() => ModifySkill("furtividade", 1));
        decreaseFurtividadeBtn.onClick.AddListener(() => ModifySkill("furtividade", -1));
        increaseGeografiaBtn.onClick.AddListener(() => ModifySkill("geografia", 1));
        decreaseGeografiaBtn.onClick.AddListener(() => ModifySkill("geografia", -1));
        increaseIdiomasBtn.onClick.AddListener(() => ModifySkill("idiomas", 1));
        decreaseIdiomasBtn.onClick.AddListener(() => ModifySkill("idiomas", -1));
        increaseIntuicaoBtn.onClick.AddListener(() => ModifySkill("intuicao", 1));
        decreaseIntuicaoBtn.onClick.AddListener(() => ModifySkill("intuicao", -1));
        increaseMedicinaBtn.onClick.AddListener(() => ModifySkill("medicina", 1));
        decreaseMedicinaBtn.onClick.AddListener(() => ModifySkill("medicina", -1));
        increaseMecanicaBtn.onClick.AddListener(() => ModifySkill("mecanica", 1));
        decreaseMecanicaBtn.onClick.AddListener(() => ModifySkill("mecanica", -1));
        increaseNegociacaoBtn.onClick.AddListener(() => ModifySkill("negociacao", 1));
        decreaseNegociacaoBtn.onClick.AddListener(() => ModifySkill("negociacao", -1));
        increaseReligiaoBtn.onClick.AddListener(() => ModifySkill("religiao", 1));
        decreaseReligiaoBtn.onClick.AddListener(() => ModifySkill("religiao", -1));
        increaseRouboBtn.onClick.AddListener(() => ModifySkill("roubo", 1));
        decreaseRouboBtn.onClick.AddListener(() => ModifySkill("roubo", -1));
        
        confirmChangesButton.onClick.AddListener(ConfirmChanges);
        confirmChangesButton.interactable = false;
        
        // Desativa todos os botões inicialmente se não houver pontos
        if (characterData != null && characterData.availableSkillPoints <= 0)
        {
            SetAllButtonsInteractable(false);
        }
    }

    private void OnDestroy()
    {
        if (characterData != null)
        {
            characterData.OnDataChanged -= UpdateUI;
        }
    }

    private void ModifySkill(string skillName, int change)
    {
        if (change > 0)
        {
            if (pointsUsed >= characterData.availableSkillPoints)
            {
                Debug.Log("Não há pontos suficientes disponíveis");
                return;
            }
            
            // Verifica se a habilidade já atingiu o máximo
            if (GetSkillValue(skillName) >= MAX_SKILL_VALUE)
            {
                Debug.Log($"Habilidade {skillName} já atingiu o máximo de {MAX_SKILL_VALUE} pontos");
                return;
            }
        }
        
        int currentChange = GetTemporaryChange(skillName);
        
        if (change < 0 && currentChange <= 0)
        {
            Debug.Log("Não pode diminuir mais esta habilidade");
            return;
        }
        
        if (!temporarySkillChanges.ContainsKey(skillName))
        {
            temporarySkillChanges[skillName] = 0;
        }
        
        temporarySkillChanges[skillName] += change;
        pointsUsed += change;
        
        UpdateUI();
    }

    private int GetSkillValue(string skillName)
    {
        int baseValue = 0;
        
        switch (skillName)
        {
            case "arrombamento": baseValue = characterData.arrombamento; break;
            case "atletismo": baseValue = characterData.atletismo; break;
            case "ciencias": baseValue = characterData.ciencias; break;
            case "diplomacia": baseValue = characterData.diplomacia; break;
            case "eletrica": baseValue = characterData.eletrica; break;
            case "furtividade": baseValue = characterData.furtividade; break;
            case "geografia": baseValue = characterData.geografia; break;
            case "idiomas": baseValue = characterData.idiomas; break;
            case "intuicao": baseValue = characterData.intuicao; break;
            case "medicina": baseValue = characterData.medicina; break;
            case "mecanica": baseValue = characterData.mecanica; break;
            case "negociacao": baseValue = characterData.negociacao; break;
            case "religiao": baseValue = characterData.religiao; break;
            case "roubo": baseValue = characterData.roubo; break;
        }
        
        if (temporarySkillChanges.ContainsKey(skillName))
        {
            baseValue += temporarySkillChanges[skillName];
        }
        
        return baseValue;
    }

    private int GetTemporaryChange(string skillName)
    {
        if (temporarySkillChanges.ContainsKey(skillName))
        {
            return temporarySkillChanges[skillName];
        }
        return 0;
    }

    private void UpdateUI()
    {
        if (characterData == null) return;
        
        // Mostra apenas os valores temporários com sinal de +
        arrombamentoText.text = FormatChangeText("arrombamento");
        atletismoText.text = FormatChangeText("atletismo");
        cienciasText.text = FormatChangeText("ciencias");
        diplomaciaText.text = FormatChangeText("diplomacia");
        eletricaText.text = FormatChangeText("eletrica");
        furtividadeText.text = FormatChangeText("furtividade");
        geografiaText.text = FormatChangeText("geografia");
        idiomasText.text = FormatChangeText("idiomas");
        intuicaoText.text = FormatChangeText("intuicao");
        medicinaText.text = FormatChangeText("medicina");
        mecanicaText.text = FormatChangeText("mecanica");
        negociacaoText.text = FormatChangeText("negociacao");
        religiaoText.text = FormatChangeText("religiao");
        rouboText.text = FormatChangeText("roubo");
        
        availablePointsText.text = $"Pontos: {characterData.availableSkillPoints - pointsUsed}";
        
        bool canIncrease = (characterData.availableSkillPoints - pointsUsed) > 0;
        bool hasChanges = pointsUsed > 0;
        
        // Configura botões de aumento - só ativa se a habilidade ainda não atingiu o máximo
        increaseArrombamentoBtn.interactable = canIncrease && GetSkillValue("arrombamento") < MAX_SKILL_VALUE;
        increaseAtletismoBtn.interactable = canIncrease && GetSkillValue("atletismo") < MAX_SKILL_VALUE;
        increaseCienciasBtn.interactable = canIncrease && GetSkillValue("ciencias") < MAX_SKILL_VALUE;
        increaseDiplomaciaBtn.interactable = canIncrease && GetSkillValue("diplomacia") < MAX_SKILL_VALUE;
        increaseEletricaBtn.interactable = canIncrease && GetSkillValue("eletrica") < MAX_SKILL_VALUE;
        increaseFurtividadeBtn.interactable = canIncrease && GetSkillValue("furtividade") < MAX_SKILL_VALUE;
        increaseGeografiaBtn.interactable = canIncrease && GetSkillValue("geografia") < MAX_SKILL_VALUE;
        increaseIdiomasBtn.interactable = canIncrease && GetSkillValue("idiomas") < MAX_SKILL_VALUE;
        increaseIntuicaoBtn.interactable = canIncrease && GetSkillValue("intuicao") < MAX_SKILL_VALUE;
        increaseMedicinaBtn.interactable = canIncrease && GetSkillValue("medicina") < MAX_SKILL_VALUE;
        increaseMecanicaBtn.interactable = canIncrease && GetSkillValue("mecanica") < MAX_SKILL_VALUE;
        increaseNegociacaoBtn.interactable = canIncrease && GetSkillValue("negociacao") < MAX_SKILL_VALUE;
        increaseReligiaoBtn.interactable = canIncrease && GetSkillValue("religiao") < MAX_SKILL_VALUE;
        increaseRouboBtn.interactable = canIncrease && GetSkillValue("roubo") < MAX_SKILL_VALUE;
        
        // Configura botões de diminuição
        decreaseArrombamentoBtn.interactable = hasChanges && GetTemporaryChange("arrombamento") > 0;
        decreaseAtletismoBtn.interactable = hasChanges && GetTemporaryChange("atletismo") > 0;
        decreaseCienciasBtn.interactable = hasChanges && GetTemporaryChange("ciencias") > 0;
        decreaseDiplomaciaBtn.interactable = hasChanges && GetTemporaryChange("diplomacia") > 0;
        decreaseEletricaBtn.interactable = hasChanges && GetTemporaryChange("eletrica") > 0;
        decreaseFurtividadeBtn.interactable = hasChanges && GetTemporaryChange("furtividade") > 0;
        decreaseGeografiaBtn.interactable = hasChanges && GetTemporaryChange("geografia") > 0;
        decreaseIdiomasBtn.interactable = hasChanges && GetTemporaryChange("idiomas") > 0;
        decreaseIntuicaoBtn.interactable = hasChanges && GetTemporaryChange("intuicao") > 0;
        decreaseMedicinaBtn.interactable = hasChanges && GetTemporaryChange("medicina") > 0;
        decreaseMecanicaBtn.interactable = hasChanges && GetTemporaryChange("mecanica") > 0;
        decreaseNegociacaoBtn.interactable = hasChanges && GetTemporaryChange("negociacao") > 0;
        decreaseReligiaoBtn.interactable = hasChanges && GetTemporaryChange("religiao") > 0;
        decreaseRouboBtn.interactable = hasChanges && GetTemporaryChange("roubo") > 0;
        
        confirmChangesButton.interactable = hasChanges;
    }

    private string FormatChangeText(string skillName)
    {
        int change = GetTemporaryChange(skillName);
        return change > 0 ? $"+{change}" : "";
    }

    private void ConfirmChanges()
    {
        if (characterData == null) return;
        
        foreach (var change in temporarySkillChanges)
        {
            switch (change.Key)
            {
                case "arrombamento": 
                    characterData.arrombamento = Mathf.Min(MAX_SKILL_VALUE, characterData.arrombamento + change.Value); 
                    break;
                case "atletismo": 
                    characterData.atletismo = Mathf.Min(MAX_SKILL_VALUE, characterData.atletismo + change.Value); 
                    break;
                case "ciencias": 
                    characterData.ciencias = Mathf.Min(MAX_SKILL_VALUE, characterData.ciencias + change.Value); 
                    break;
                case "diplomacia": 
                    characterData.diplomacia = Mathf.Min(MAX_SKILL_VALUE, characterData.diplomacia + change.Value); 
                    break;
                case "eletrica": 
                    characterData.eletrica = Mathf.Min(MAX_SKILL_VALUE, characterData.eletrica + change.Value); 
                    break;
                case "furtividade": 
                    characterData.furtividade = Mathf.Min(MAX_SKILL_VALUE, characterData.furtividade + change.Value); 
                    break;
                case "geografia": 
                    characterData.geografia = Mathf.Min(MAX_SKILL_VALUE, characterData.geografia + change.Value); 
                    break;
                case "idiomas": 
                    characterData.idiomas = Mathf.Min(MAX_SKILL_VALUE, characterData.idiomas + change.Value); 
                    break;
                case "intuicao": 
                    characterData.intuicao = Mathf.Min(MAX_SKILL_VALUE, characterData.intuicao + change.Value); 
                    break;
                case "medicina": 
                    characterData.medicina = Mathf.Min(MAX_SKILL_VALUE, characterData.medicina + change.Value); 
                    break;
                case "mecanica": 
                    characterData.mecanica = Mathf.Min(MAX_SKILL_VALUE, characterData.mecanica + change.Value); 
                    break;
                case "negociacao": 
                    characterData.negociacao = Mathf.Min(MAX_SKILL_VALUE, characterData.negociacao + change.Value); 
                    break;
                case "religiao": 
                    characterData.religiao = Mathf.Min(MAX_SKILL_VALUE, characterData.religiao + change.Value); 
                    break;
                case "roubo": 
                    characterData.roubo = Mathf.Min(MAX_SKILL_VALUE, characterData.roubo + change.Value); 
                    break;
            }
        }
        
        characterData.availableSkillPoints -= pointsUsed;
        temporarySkillChanges.Clear();
        pointsUsed = 0;
        
        // Desativa todos os botões após confirmar
        SetAllButtonsInteractable(false);
        
        characterData.NotifyChanges();
    }

    private void SetAllButtonsInteractable(bool state)
    {
        increaseArrombamentoBtn.interactable = state && GetSkillValue("arrombamento") < MAX_SKILL_VALUE;
        decreaseArrombamentoBtn.interactable = state;
        increaseAtletismoBtn.interactable = state && GetSkillValue("atletismo") < MAX_SKILL_VALUE;
        decreaseAtletismoBtn.interactable = state;
        increaseCienciasBtn.interactable = state && GetSkillValue("ciencias") < MAX_SKILL_VALUE;
        decreaseCienciasBtn.interactable = state;
        increaseDiplomaciaBtn.interactable = state && GetSkillValue("diplomacia") < MAX_SKILL_VALUE;
        decreaseDiplomaciaBtn.interactable = state;
        increaseEletricaBtn.interactable = state && GetSkillValue("eletrica") < MAX_SKILL_VALUE;
        decreaseEletricaBtn.interactable = state;
        increaseFurtividadeBtn.interactable = state && GetSkillValue("furtividade") < MAX_SKILL_VALUE;
        decreaseFurtividadeBtn.interactable = state;
        increaseGeografiaBtn.interactable = state && GetSkillValue("geografia") < MAX_SKILL_VALUE;
        decreaseGeografiaBtn.interactable = state;
        increaseIdiomasBtn.interactable = state && GetSkillValue("idiomas") < MAX_SKILL_VALUE;
        decreaseIdiomasBtn.interactable = state;
        increaseIntuicaoBtn.interactable = state && GetSkillValue("intuicao") < MAX_SKILL_VALUE;
        decreaseIntuicaoBtn.interactable = state;
        increaseMedicinaBtn.interactable = state && GetSkillValue("medicina") < MAX_SKILL_VALUE;
        decreaseMedicinaBtn.interactable = state;
        increaseMecanicaBtn.interactable = state && GetSkillValue("mecanica") < MAX_SKILL_VALUE;
        decreaseMecanicaBtn.interactable = state;
        increaseNegociacaoBtn.interactable = state && GetSkillValue("negociacao") < MAX_SKILL_VALUE;
        decreaseNegociacaoBtn.interactable = state;
        increaseReligiaoBtn.interactable = state && GetSkillValue("religiao") < MAX_SKILL_VALUE;
        decreaseReligiaoBtn.interactable = state;
        increaseRouboBtn.interactable = state && GetSkillValue("roubo") < MAX_SKILL_VALUE;
        decreaseRouboBtn.interactable = state;
        
        confirmChangesButton.interactable = state;
    }

    public void ResetUI()
    {
        temporarySkillChanges.Clear();
        pointsUsed = 0;
        UpdateUI();
    }
}