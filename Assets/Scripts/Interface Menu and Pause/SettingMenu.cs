using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Filtra resoluções menores que 800 x 600
            if (resolutions[i].width >= 800 && resolutions[i].height >= 600)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                // Marca a resolução atual
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1; // Atualiza com base na lista filtrada
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Carregar configurações persistentes
        LoadSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        List<Resolution> filteredResolutions = new List<Resolution>();
        foreach (var res in resolutions)
        {
            if (res.width >= 800 && res.height >= 600)
            {
                filteredResolutions.Add(res);
            }
        }

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        // Salvar qualidade no PlayerPrefs
        PlayerPrefs.SetInt("QualitySetting", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // Salvar estado do fullscreen no PlayerPrefs
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // Carregar qualidade gráfica
        int savedQuality = PlayerPrefs.GetInt("QualitySetting", QualitySettings.GetQualityLevel());
        qualityDropdown.value = savedQuality;
        qualityDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(savedQuality);

        // Carregar estado do fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }
}
