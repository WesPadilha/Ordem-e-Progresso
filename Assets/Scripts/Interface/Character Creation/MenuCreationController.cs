using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuCreationController : MonoBehaviour
{
    public CreationAttributes attributes;
    public CreationSkills skills;
    public CreationPerk perkController;
    public Button nextSceneButton;
    public Button resetAndReturnButton;
    public TMP_Text errorMessageText;
    public GameObject errorMessageObject;
    public CharacterData characterData;

    void Start()
    {
        nextSceneButton.onClick.AddListener(TryLoadNextScene);
        resetAndReturnButton.onClick.AddListener(ResetAndReturnToMenu);
        errorMessageObject.SetActive(false);
    }

    private void TryLoadNextScene()
    {
        // Check if all attribute points are used
        if (attributes.GetAvailablePoints() > 0)
        {
            ShowErrorMessage("Você precisa usar todos os pontos de atributo antes de continuar!");
            return;
        }

        // Check if all skill points are used
        if (skills.GetAvailableSkillPoints() > 0)
        {
            ShowErrorMessage("Você precisa usar todos os pontos de habilidade antes de continuar!");
            return;
        }

        // Check if all bonus points are used
        if (skills.GetAvailableBonusPoints() > 0)
        {
            ShowErrorMessage("Você precisa usar todos os pontos bônus antes de continuar!");
            return;
        }

        // Keep the perk check but don't save it
        if (!perkController.IsAnyPerkChosen())
        {
            ShowErrorMessage("Você precisa escolher um perk para continuar!");
            return;
        }

        SaveCharacterData();
        LoadNextScene();
    }

    private void SaveCharacterData()
    {
        // Save attributes and calculate derived stats
        characterData.SaveAttributes(attributes);
        
        // Save skills
        characterData.SaveSkills(skills);
        
        // Explicitly save to character data
        attributes.SaveToCharacterData();
    }

    private void ShowErrorMessage(string message)
    {
        errorMessageText.text = message;
        errorMessageObject.SetActive(true);
        Invoke("HideErrorMessage", 3f);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void ResetAndReturnToMenu()
    {
        attributes.ResetAttributePoints();
        skills.ResetSkillPoints();
        SceneManager.LoadScene("MainMenu");
    }

    private void HideErrorMessage()
    {
        errorMessageObject.SetActive(false);
    }
}