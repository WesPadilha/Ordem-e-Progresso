using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreationSkills : MonoBehaviour
{
    [System.Serializable]
    public class SkillDescriptions
    {
        [TextArea(3, 10)]
        public string arrombamento;
        [TextArea(3, 10)]
        public string atletismo;
        [TextArea(3, 10)]
        public string ciencias;
        [TextArea(3, 10)]
        public string diplomacia;
        [TextArea(3, 10)]
        public string eletrica;
        [TextArea(3, 10)]
        public string furtividade;
        [TextArea(3, 10)]
        public string geografia;
        [TextArea(3, 10)]
        public string idiomas;
        [TextArea(3, 10)]
        public string intuicao;
        [TextArea(3, 10)]
        public string medicina;
        [TextArea(3, 10)]
        public string mecanica;
        [TextArea(3, 10)]
        public string negociacao;
        [TextArea(3, 10)]
        public string religiao;
        [TextArea(3, 10)]
        public string roubo;
    }

    public CreationAttributes attributes; 
    public SkillDescriptions descriptions;
    
    [Header("Skill Texts")]
    public TMP_Text ArrombamentoText;
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

    [Header("Bonus Buttons")]
    public Button bonusArrombamentoBtn;
    public Button bonusAtletismoBtn;
    public Button bonusCienciasBtn;
    public Button bonusDiplomaciaBtn;
    public Button bonusEletricaBtn;
    public Button bonusFurtividadeBtn;
    public Button bonusGeografiaBtn;
    public Button bonusIdiomasBtn;
    public Button bonusIntuicaoBtn;
    public Button bonusMedicinaBtn;
    public Button bonusMecanicaBtn;
    public Button bonusNegociacaoBtn;
    public Button bonusReligiaoBtn;
    public Button bonusRouboBtn;

    public TMP_Text availableSkillPointsText;
    public TMP_Text availableBonusPointsText;

    [SerializeField] private int availableSkillPoints = 20;
    [SerializeField] private int availableBonusPoints = 3;

    // Base skill values
    private int baseArrombamento;
    private int baseAtletismo;
    private int baseCiencias;
    private int baseDiplomacia;
    private int baseEletrica;
    private int baseFurtividade;
    private int baseGeografia;
    private int baseIdiomas;
    private int baseIntuicao;
    private int baseMedicina;
    private int baseMecanica;
    private int baseNegociacao;
    private int baseReligiao;
    private int baseRoubo;

    // Player added points
    private int playerArrombamento;
    private int playerAtletismo;
    private int playerCiencias;
    private int playerDiplomacia;
    private int playerEletrica;
    private int playerFurtividade;
    private int playerGeografia;
    private int playerIdiomas;
    private int playerIntuicao;
    private int playerMedicina;
    private int playerMecanica;
    private int playerNegociacao;
    private int playerReligiao;
    private int playerRoubo;

    // Bonus skill values (5 points each)
    private bool bonusArrombamento = false;
    private bool bonusAtletismo = false;
    private bool bonusCiencias = false;
    private bool bonusDiplomacia = false;
    private bool bonusEletrica = false;
    private bool bonusFurtividade = false;
    private bool bonusGeografia = false;
    private bool bonusIdiomas = false;
    private bool bonusIntuicao = false;
    private bool bonusMedicina = false;
    private bool bonusMecanica = false;
    private bool bonusNegociacao = false;
    private bool bonusReligiao = false;
    private bool bonusRoubo = false;

    void Start()
    {
        SetupButtonListeners();
        SetupBonusButtonListeners();
        SetupSkillButtons();
        UpdateSkillsBasedOnAttributes(false);
        UpdateUI();
    }

    private void SetupButtonListeners()
    {
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
    }

    private void SetupBonusButtonListeners()
    {
        bonusArrombamentoBtn.onClick.AddListener(() => ToggleBonus("Arrombamento"));
        bonusAtletismoBtn.onClick.AddListener(() => ToggleBonus("Atletismo"));
        bonusCienciasBtn.onClick.AddListener(() => ToggleBonus("Ciencias"));
        bonusDiplomaciaBtn.onClick.AddListener(() => ToggleBonus("Diplomacia"));
        bonusEletricaBtn.onClick.AddListener(() => ToggleBonus("Eletrica"));
        bonusFurtividadeBtn.onClick.AddListener(() => ToggleBonus("Furtividade"));
        bonusGeografiaBtn.onClick.AddListener(() => ToggleBonus("Geografia"));
        bonusIdiomasBtn.onClick.AddListener(() => ToggleBonus("Idiomas"));
        bonusIntuicaoBtn.onClick.AddListener(() => ToggleBonus("Intuicao"));
        bonusMedicinaBtn.onClick.AddListener(() => ToggleBonus("Medicina"));
        bonusMecanicaBtn.onClick.AddListener(() => ToggleBonus("Mecanica"));
        bonusNegociacaoBtn.onClick.AddListener(() => ToggleBonus("Negociacao"));
        bonusReligiaoBtn.onClick.AddListener(() => ToggleBonus("Religiao"));
        bonusRouboBtn.onClick.AddListener(() => ToggleBonus("Roubo"));
    }

    private void SetupSkillButtons()
    {
        SetupButtonDescription(increaseArrombamentoBtn, descriptions.arrombamento);
        SetupButtonDescription(increaseAtletismoBtn, descriptions.atletismo);
        SetupButtonDescription(increaseCienciasBtn, descriptions.ciencias);
        SetupButtonDescription(increaseDiplomaciaBtn, descriptions.diplomacia);
        SetupButtonDescription(increaseEletricaBtn, descriptions.eletrica);
        SetupButtonDescription(increaseFurtividadeBtn, descriptions.furtividade);
        SetupButtonDescription(increaseGeografiaBtn, descriptions.geografia);
        SetupButtonDescription(increaseIdiomasBtn, descriptions.idiomas);
        SetupButtonDescription(increaseIntuicaoBtn, descriptions.intuicao);
        SetupButtonDescription(increaseMedicinaBtn, descriptions.medicina);
        SetupButtonDescription(increaseMecanicaBtn, descriptions.mecanica);
        SetupButtonDescription(increaseNegociacaoBtn, descriptions.negociacao);
        SetupButtonDescription(increaseReligiaoBtn, descriptions.religiao);
        SetupButtonDescription(increaseRouboBtn, descriptions.roubo);
    }

    private void SetupButtonDescription(Button button, string description)
    {
        var desc = button.gameObject.AddComponent<ClickDescription>();
        desc.description = description;
    }

    public void UpdateSkillsBasedOnAttributes(bool preservePlayerPoints = true)
    {
        // Calculate base values from attributes
        baseArrombamento = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intellection)));
        baseAtletismo = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.agility)));
        baseCiencias = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intelligence)));
        baseDiplomacia = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.charisma)));
        baseEletrica = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intellection)));
        baseFurtividade = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.agility)));
        baseGeografia = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intellection)));
        baseIdiomas = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.charisma)));
        baseIntuicao = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intellection)));
        baseMedicina = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intelligence)));
        baseMecanica = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.intelligence)));
        baseNegociacao = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.charisma)));
        baseReligiao = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.charisma)));
        baseRoubo = Mathf.Max(1, Mathf.CeilToInt(2 + (0.4f * attributes.agility)));

        if (!preservePlayerPoints)
        {
            // Reset player points if not preserving
            playerArrombamento = 0;
            playerAtletismo = 0;
            playerCiencias = 0;
            playerDiplomacia = 0;
            playerEletrica = 0;
            playerFurtividade = 0;
            playerGeografia = 0;
            playerIdiomas = 0;
            playerIntuicao = 0;
            playerMedicina = 0;
            playerMecanica = 0;
            playerNegociacao = 0;
            playerReligiao = 0;
            playerRoubo = 0;
        }

        UpdateUI();
    }

    void IncreaseSkill(string skill)
    {
        if (availableSkillPoints > 0)
        {
            switch (skill)
            {
                case "Arrombamento": playerArrombamento++; break;
                case "Atletismo": playerAtletismo++; break;
                case "Ciencias": playerCiencias++; break;
                case "Diplomacia": playerDiplomacia++; break;
                case "Eletrica": playerEletrica++; break;
                case "Furtividade": playerFurtividade++; break;
                case "Geografia": playerGeografia++; break;
                case "Idiomas": playerIdiomas++; break;
                case "Intuicao": playerIntuicao++; break;
                case "Medicina": playerMedicina++; break;
                case "Mecanica": playerMecanica++; break;
                case "Negociacao": playerNegociacao++; break;
                case "Religiao": playerReligiao++; break;
                case "Roubo": playerRoubo++; break;
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
                if (playerArrombamento > 0) { playerArrombamento--; availableSkillPoints++; } 
                break;
            case "Atletismo": 
                if (playerAtletismo > 0) { playerAtletismo--; availableSkillPoints++; } 
                break;
            case "Ciencias": 
                if (playerCiencias > 0) { playerCiencias--; availableSkillPoints++; } 
                break;
            case "Diplomacia": 
                if (playerDiplomacia > 0) { playerDiplomacia--; availableSkillPoints++; } 
                break;
            case "Eletrica": 
                if (playerEletrica > 0) { playerEletrica--; availableSkillPoints++; } 
                break;
            case "Furtividade": 
                if (playerFurtividade > 0) { playerFurtividade--; availableSkillPoints++; } 
                break;
            case "Geografia": 
                if (playerGeografia > 0) { playerGeografia--; availableSkillPoints++; } 
                break;
            case "Idiomas": 
                if (playerIdiomas > 0) { playerIdiomas--; availableSkillPoints++; } 
                break;
            case "Intuicao": 
                if (playerIntuicao > 0) { playerIntuicao--; availableSkillPoints++; } 
                break;
            case "Medicina": 
                if (playerMedicina > 0) { playerMedicina--; availableSkillPoints++; } 
                break;
            case "Mecanica": 
                if (playerMecanica > 0) { playerMecanica--; availableSkillPoints++; } 
                break;
            case "Negociacao": 
                if (playerNegociacao > 0) { playerNegociacao--; availableSkillPoints++; } 
                break;
            case "Religiao": 
                if (playerReligiao > 0) { playerReligiao--; availableSkillPoints++; } 
                break;
            case "Roubo": 
                if (playerRoubo > 0) { playerRoubo--; availableSkillPoints++; } 
                break;
        }
        UpdateUI();
    }

    void ToggleBonus(string skill)
    {
        bool hasBonus = false;
        
        switch (skill)
        {
            case "Arrombamento": hasBonus = bonusArrombamento; break;
            case "Atletismo": hasBonus = bonusAtletismo; break;
            case "Ciencias": hasBonus = bonusCiencias; break;
            case "Diplomacia": hasBonus = bonusDiplomacia; break;
            case "Eletrica": hasBonus = bonusEletrica; break;
            case "Furtividade": hasBonus = bonusFurtividade; break;
            case "Geografia": hasBonus = bonusGeografia; break;
            case "Idiomas": hasBonus = bonusIdiomas; break;
            case "Intuicao": hasBonus = bonusIntuicao; break;
            case "Medicina": hasBonus = bonusMedicina; break;
            case "Mecanica": hasBonus = bonusMecanica; break;
            case "Negociacao": hasBonus = bonusNegociacao; break;
            case "Religiao": hasBonus = bonusReligiao; break;
            case "Roubo": hasBonus = bonusRoubo; break;
        }

        if (hasBonus)
        {
            availableBonusPoints++;
            
            switch (skill)
            {
                case "Arrombamento": bonusArrombamento = false; break;
                case "Atletismo": bonusAtletismo = false; break;
                case "Ciencias": bonusCiencias = false; break;
                case "Diplomacia": bonusDiplomacia = false; break;
                case "Eletrica": bonusEletrica = false; break;
                case "Furtividade": bonusFurtividade = false; break;
                case "Geografia": bonusGeografia = false; break;
                case "Idiomas": bonusIdiomas = false; break;
                case "Intuicao": bonusIntuicao = false; break;
                case "Medicina": bonusMedicina = false; break;
                case "Mecanica": bonusMecanica = false; break;
                case "Negociacao": bonusNegociacao = false; break;
                case "Religiao": bonusReligiao = false; break;
                case "Roubo": bonusRoubo = false; break;
            }
        }
        else if (availableBonusPoints > 0)
        {
            availableBonusPoints--;
            
            switch (skill)
            {
                case "Arrombamento": bonusArrombamento = true; break;
                case "Atletismo": bonusAtletismo = true; break;
                case "Ciencias": bonusCiencias = true; break;
                case "Diplomacia": bonusDiplomacia = true; break;
                case "Eletrica": bonusEletrica = true; break;
                case "Furtividade": bonusFurtividade = true; break;
                case "Geografia": bonusGeografia = true; break;
                case "Idiomas": bonusIdiomas = true; break;
                case "Intuicao": bonusIntuicao = true; break;
                case "Medicina": bonusMedicina = true; break;
                case "Mecanica": bonusMecanica = true; break;
                case "Negociacao": bonusNegociacao = true; break;
                case "Religiao": bonusReligiao = true; break;
                case "Roubo": bonusRoubo = true; break;
            }
        }

        UpdateUI();
    }

    public void ResetSkillPoints()
    {
        availableSkillPoints = 20;
        availableBonusPoints = 3;
        
        // Reset bonus skills
        bonusArrombamento = false;
        bonusAtletismo = false;
        bonusCiencias = false;
        bonusDiplomacia = false;
        bonusEletrica = false;
        bonusFurtividade = false;
        bonusGeografia = false;
        bonusIdiomas = false;
        bonusIntuicao = false;
        bonusMedicina = false;
        bonusMecanica = false;
        bonusNegociacao = false;
        bonusReligiao = false;
        bonusRoubo = false;

        // Reset player points and update base from attributes
        UpdateSkillsBasedOnAttributes(false);
        UpdateUI();
    }

    public int GetAvailableSkillPoints()
    {
        return availableSkillPoints;
    }

    public int GetAvailableBonusPoints()
    {
        return availableBonusPoints;
    }

    void UpdateUI()
    {
        // Update skill texts with base + player points + bonus values
        if (ArrombamentoText != null) 
            ArrombamentoText.text = (baseArrombamento + playerArrombamento + (bonusArrombamento ? 5 : 0)).ToString();
        if (AtletismoText != null) 
            AtletismoText.text = (baseAtletismo + playerAtletismo + (bonusAtletismo ? 5 : 0)).ToString();
        if (CienciasText != null) 
            CienciasText.text = (baseCiencias + playerCiencias + (bonusCiencias ? 5 : 0)).ToString();
        if (DiplomaciaText != null) 
            DiplomaciaText.text = (baseDiplomacia + playerDiplomacia + (bonusDiplomacia ? 5 : 0)).ToString();
        if (EletricaText != null) 
            EletricaText.text = (baseEletrica + playerEletrica + (bonusEletrica ? 5 : 0)).ToString();
        if (FurtividadeText != null) 
            FurtividadeText.text = (baseFurtividade + playerFurtividade + (bonusFurtividade ? 5 : 0)).ToString();
        if (GeografiaText != null) 
            GeografiaText.text = (baseGeografia + playerGeografia + (bonusGeografia ? 5 : 0)).ToString();
        if (IdiomasText != null) 
            IdiomasText.text = (baseIdiomas + playerIdiomas + (bonusIdiomas ? 5 : 0)).ToString();
        if (IntuicaoText != null) 
            IntuicaoText.text = (baseIntuicao + playerIntuicao + (bonusIntuicao ? 5 : 0)).ToString();
        if (MedicinaText != null) 
            MedicinaText.text = (baseMedicina + playerMedicina + (bonusMedicina ? 5 : 0)).ToString();
        if (MecanicaText != null) 
            MecanicaText.text = (baseMecanica + playerMecanica + (bonusMecanica ? 5 : 0)).ToString();
        if (NegociacaoText != null) 
            NegociacaoText.text = (baseNegociacao + playerNegociacao + (bonusNegociacao ? 5 : 0)).ToString();
        if (ReligiaoText != null) 
            ReligiaoText.text = (baseReligiao + playerReligiao + (bonusReligiao ? 5 : 0)).ToString();
        if (RouboText != null) 
            RouboText.text = (baseRoubo + playerRoubo + (bonusRoubo ? 5 : 0)).ToString();

        // Update bonus button colors
        UpdateBonusButtonColor(bonusArrombamentoBtn, bonusArrombamento);
        UpdateBonusButtonColor(bonusAtletismoBtn, bonusAtletismo);
        UpdateBonusButtonColor(bonusCienciasBtn, bonusCiencias);
        UpdateBonusButtonColor(bonusDiplomaciaBtn, bonusDiplomacia);
        UpdateBonusButtonColor(bonusEletricaBtn, bonusEletrica);
        UpdateBonusButtonColor(bonusFurtividadeBtn, bonusFurtividade);
        UpdateBonusButtonColor(bonusGeografiaBtn, bonusGeografia);
        UpdateBonusButtonColor(bonusIdiomasBtn, bonusIdiomas);
        UpdateBonusButtonColor(bonusIntuicaoBtn, bonusIntuicao);
        UpdateBonusButtonColor(bonusMedicinaBtn, bonusMedicina);
        UpdateBonusButtonColor(bonusMecanicaBtn, bonusMecanica);
        UpdateBonusButtonColor(bonusNegociacaoBtn, bonusNegociacao);
        UpdateBonusButtonColor(bonusReligiaoBtn, bonusReligiao);
        UpdateBonusButtonColor(bonusRouboBtn, bonusRoubo);

        if (availableSkillPointsText != null) 
            availableSkillPointsText.text = "Pontos: " + availableSkillPoints;
        if (availableBonusPointsText != null)
            availableBonusPointsText.text = "Bônus: " + availableBonusPoints;
    }

    private void UpdateBonusButtonColor(Button button, bool isActive)
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

    // Public getters for CharacterData to save final values
    public int GetFinalArrombamento() => baseArrombamento + playerArrombamento + (bonusArrombamento ? 5 : 0);
    public int GetFinalAtletismo() => baseAtletismo + playerAtletismo + (bonusAtletismo ? 5 : 0);
    public int GetFinalCiencias() => baseCiencias + playerCiencias + (bonusCiencias ? 5 : 0);
    public int GetFinalDiplomacia() => baseDiplomacia + playerDiplomacia + (bonusDiplomacia ? 5 : 0);
    public int GetFinalEletrica() => baseEletrica + playerEletrica + (bonusEletrica ? 5 : 0);
    public int GetFinalFurtividade() => baseFurtividade + playerFurtividade + (bonusFurtividade ? 5 : 0);
    public int GetFinalGeografia() => baseGeografia + playerGeografia + (bonusGeografia ? 5 : 0);
    public int GetFinalIdiomas() => baseIdiomas + playerIdiomas + (bonusIdiomas ? 5 : 0);
    public int GetFinalIntuicao() => baseIntuicao + playerIntuicao + (bonusIntuicao ? 5 : 0);
    public int GetFinalMedicina() => baseMedicina + playerMedicina + (bonusMedicina ? 5 : 0);
    public int GetFinalMecanica() => baseMecanica + playerMecanica + (bonusMecanica ? 5 : 0);
    public int GetFinalNegociacao() => baseNegociacao + playerNegociacao + (bonusNegociacao ? 5 : 0);
    public int GetFinalReligiao() => baseReligiao + playerReligiao + (bonusReligiao ? 5 : 0);
    public int GetFinalRoubo() => baseRoubo + playerRoubo + (bonusRoubo ? 5 : 0);
}