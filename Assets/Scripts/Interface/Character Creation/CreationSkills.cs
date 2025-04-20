using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreationSkills : MonoBehaviour
{
    public CreationAttributes attributes; 
    public TMP_Text ArrombamentoText; // Changed from Text to TMP_Text
    public TMP_Text AtletismoText;
    public TMP_Text CienciasText;
    public TMP_Text DiplomaciaText;
    public TMP_Text EletricaText;
    public TMP_Text FurtividadeText;
    public TMP_Text GeografiaText;
    public TMP_Text IdiomasText;
    public TMP_Text IntuicaoText;
    public TMP_Text MedicinaText;
    public TMP_Text MecanicaText;
    public TMP_Text NegociacaoText;
    public TMP_Text ReligiaoText;
    public TMP_Text RouboText;

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

    public TMP_Text availableSkillPointsText;

    private int availableSkillPoints = 20;

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

    void Start()
    {
        // Inicialização dos botões e texto
        increaseArrombamentoBtn.onClick.AddListener(() => IncreaseSkill("Arrombamento"));
        decreaseArrombamentoBtn.onClick.AddListener(() => DecreaseSkill("Arrombamento"));
        increaseAtletismoBtn.onClick.AddListener(() => IncreaseSkill("Atletismo"));
        decreaseAtletismoBtn.onClick.AddListener(() => DecreaseSkill("Atletismo"));
        increaseCienciasBtn.onClick.AddListener(() => IncreaseSkill("Ciencias"));
        decreaseCienciasBtn.onClick.AddListener(() => DecreaseSkill("Ciencias"));
        increaseDiplomaciaBtn.onClick.AddListener(() => IncreaseSkill("Diplomacia"));
        decreaseDiplomaciaBtn.onClick.AddListener(() => DecreaseSkill("Diplomacia"));
        increaseEletricaBtn.onClick.AddListener(() => IncreaseSkill("Eletrica"));
        decreaseEletricaBtn.onClick.AddListener(() => DecreaseSkill("Eletrica"));
        increaseFurtividadeBtn.onClick.AddListener(() => IncreaseSkill("Furtividade"));
        decreaseFurtividadeBtn.onClick.AddListener(() => DecreaseSkill("Furtividade"));
        increaseGeografiaBtn.onClick.AddListener(() => IncreaseSkill("Geografia"));
        decreaseGeografiaBtn.onClick.AddListener(() => DecreaseSkill("Geografia"));
        increaseIdiomasBtn.onClick.AddListener(() => IncreaseSkill("Idiomas"));
        decreaseIdiomasBtn.onClick.AddListener(() => DecreaseSkill("Idiomas"));
        increaseIntuicaoBtn.onClick.AddListener(() => IncreaseSkill("Intuicao"));
        decreaseIntuicaoBtn.onClick.AddListener(() => DecreaseSkill("Intuicao"));
        increaseMedicinaBtn.onClick.AddListener(() => IncreaseSkill("Medicina"));
        decreaseMedicinaBtn.onClick.AddListener(() => DecreaseSkill("Medicina"));
        increaseMecanicaBtn.onClick.AddListener(() => IncreaseSkill("Mecanica"));
        decreaseMecanicaBtn.onClick.AddListener(() => DecreaseSkill("Mecanica"));
        increaseNegociacaoBtn.onClick.AddListener(() => IncreaseSkill("Negociacao"));
        decreaseNegociacaoBtn.onClick.AddListener(() => DecreaseSkill("Negociacao"));
        increaseReligiaoBtn.onClick.AddListener(() => IncreaseSkill("Religiao"));
        decreaseReligiaoBtn.onClick.AddListener(() => DecreaseSkill("Religiao"));
        increaseRouboBtn.onClick.AddListener(() => IncreaseSkill("Roubo"));
        decreaseRouboBtn.onClick.AddListener(() => DecreaseSkill("Roubo"));

        UpdateSkillsBasedOnAttributes(); // Para calcular valores iniciais
        UpdateUI();
    }

    public void UpdateSkillsBasedOnAttributes()
    {
        // Calculate rounded values first
        int intellectionValue = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intellection)));
        int intelligenceValue = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intelligence)));
        int charismaValue = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.charisma)));
        int agilityValue = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.agility)));

        // Apply values to skills
        arrombamento = intellectionValue;
        atletismo = agilityValue;
        ciencias = intelligenceValue;
        diplomacia = charismaValue;
        eletrica = intellectionValue;
        furtividade = agilityValue;
        geografia = intellectionValue;
        idiomas = charismaValue;
        intuicao = intellectionValue;
        medicina = intelligenceValue;
        mecanica = intelligenceValue;
        negociacao = charismaValue;
        religiao = charismaValue;
        roubo = agilityValue;

        UpdateUI();
    }

    void IncreaseSkill(string skill)
    {
        if (availableSkillPoints > 0)
        {
            switch (skill)
            {
                case "Arrombamento":
                    arrombamento++;
                    break;
                case "Atletismo":
                    atletismo++;
                    break;
                case "Ciencias":
                    ciencias++;
                    break;
                case "Diplomacia":
                    diplomacia++;
                    break;
                case "Eletrica":
                    eletrica++;
                    break;
                case "Furtividade":
                    furtividade++;
                    break;
                case "Geografia":
                    geografia++;
                    break;
                case "Idiomas":
                    idiomas++;
                    break;
                case "Intuicao":
                    intuicao++;
                    break;
                case "Medicina":
                    medicina++;
                    break;
                case "Mecanica":
                    mecanica++;
                    break;
                case "Negociacao":
                    negociacao++;
                    break;
                case "Religiao":
                    religiao++;
                    break;
                case "Roubo":
                    roubo++;
                    break;
            }

            availableSkillPoints--;
            UpdateUI();
        }
    }

    void DecreaseSkill(string skill)
    {
        switch (skill)
        {
            case "Arrombamento":
                if (arrombamento > 0)
                {
                    arrombamento--;
                    availableSkillPoints++;
                }
                break;
            case "Atletismo":
                if (atletismo > 0)
                {
                    atletismo--;
                    availableSkillPoints++;
                }
                break;
            case "Ciencias":
                if (ciencias > 0)
                {
                    ciencias--;
                    availableSkillPoints++;
                }
                break;
            case "Diplomacia":
                if (diplomacia > 0)
                {
                    diplomacia--;
                    availableSkillPoints++;
                }
                break;
            case "Eletrica":
                if (eletrica > 0)
                {
                    eletrica--;
                    availableSkillPoints++;
                }
                break;
            case "Furtividade":
                if (furtividade > 0)
                {
                    furtividade--;
                    availableSkillPoints++;
                }
                break;
            case "Geografia":
                if (geografia > 0)
                {
                    geografia--;
                    availableSkillPoints++;
                }
                break;
            case "Idiomas":
                if (idiomas > 0)
                {
                    idiomas--;
                    availableSkillPoints++;
                }
                break;
            case "Intuicao":
                if (intuicao > 0)
                {
                    intuicao--;
                    availableSkillPoints++;
                }
                break;
            case "Medicina":
                if (medicina > 0)
                {
                    medicina--;
                    availableSkillPoints++;
                }
                break;
            case "Mecanica":
                if (mecanica > 0)
                {
                    mecanica--;
                    availableSkillPoints++;
                }
                break;
            case "Negociacao":
                if (negociacao > 0)
                {
                    negociacao--;
                    availableSkillPoints++;
                }
                break;
            case "Religiao":
                if (religiao > 0)
                {
                    religiao--;
                    availableSkillPoints++;
                }
                break;
            case "Roubo":
                if (roubo > 0)
                {
                    roubo--;
                    availableSkillPoints++;
                }
                break;
        }
        UpdateUI();
    }

    public void ResetSkillPoints()
    {
        availableSkillPoints = 20;
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
        UpdateUI();
    }

    public int GetAvailableSkillPoints()
    {
        return availableSkillPoints;
    }

    void UpdateUI()
    {
        ArrombamentoText.text = arrombamento.ToString();
        AtletismoText.text = atletismo.ToString();
        CienciasText.text = ciencias.ToString();
        DiplomaciaText.text = diplomacia.ToString();
        EletricaText.text = eletrica.ToString();
        FurtividadeText.text = furtividade.ToString();
        GeografiaText.text = geografia.ToString();
        IdiomasText.text = idiomas.ToString();
        IntuicaoText.text = intuicao.ToString();
        MedicinaText.text = medicina.ToString();
        MecanicaText.text = mecanica.ToString();
        NegociacaoText.text = negociacao.ToString();
        ReligiaoText.text = religiao.ToString();
        RouboText.text = roubo.ToString();

        availableSkillPointsText.text = "Pontos: " + availableSkillPoints;
    }
}