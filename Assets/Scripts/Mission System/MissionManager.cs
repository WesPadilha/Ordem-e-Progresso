using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentPanel; // Conteúdo do Scroll View para missões ativas
    public Transform contentPanelComplete; // Conteúdo do Scroll View para missões completas
    public GameObject missionButtonPrefab; // Prefab do botão para cada missão

    public TextMeshProUGUI missionNameText; // Texto para o nome da missão
    public TextMeshProUGUI missionDescriptionText; // Texto para descrição
    public TextMeshProUGUI missionObjectivesText; // Texto para listar objetivos

    [Header("Lista de missões")]
    public List<CreateMissions> allMissions;
    public List<CreateMissions> acceptedMissions = new List<CreateMissions>();
    public List<CreateMissions> completedMissions = new List<CreateMissions>();

    private CreateMissions currentSelectedMission;

    void Start()
    {
        RefreshMissionLists();
    }

    void Update()
    {
        // Verifica se alguma missão foi completada e move para a lista de completas
        CheckForCompletedMissions();

        if (currentSelectedMission != null)
        {
            ShowMissionDetails(currentSelectedMission);
        }
    }

    // Verifica e move missões completadas para a lista de completas
    private void CheckForCompletedMissions()
    {
        for (int i = acceptedMissions.Count - 1; i >= 0; i--)
        {
            var mission = acceptedMissions[i];
            if (mission.isCompleted && !completedMissions.Contains(mission))
            {
                acceptedMissions.Remove(mission);
                completedMissions.Add(mission);
                RefreshMissionLists();
                Debug.Log($"Missão {mission.missionName} movida para completas.");
            }
        }
    }

    // Atualiza ambas as listas de missões (ativas e completas)
    public void RefreshMissionLists()
    {
        // Limpa ambas as listas
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in contentPanelComplete)
        {
            Destroy(child.gameObject);
        }

        // Cria botões para missões ativas
        foreach (var mission in acceptedMissions)
        {
            CreateMissionButton(mission, contentPanel);
        }

        // Cria botões para missões completas
        foreach (var mission in completedMissions)
        {
            CreateMissionButton(mission, contentPanelComplete);
        }
    }

    private void CreateMissionButton(CreateMissions mission, Transform parentPanel)
    {
        GameObject newButton = Instantiate(missionButtonPrefab, parentPanel);
        TextMeshProUGUI btnText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = mission.missionName;

        // Muda a cor do botão se a missão estiver completa
        if (mission.isCompleted)
        {
            var colors = newButton.GetComponent<Button>().colors;
            colors.normalColor = Color.green;
            newButton.GetComponent<Button>().colors = colors;
        }

        Button btnComponent = newButton.GetComponent<Button>();
        CreateMissions capturedMission = mission;
        btnComponent.onClick.AddListener(() => ShowMissionDetails(capturedMission));
    }

    public void ShowMissionDetails(CreateMissions mission)
    {
        currentSelectedMission = mission;

        missionNameText.text = mission.missionName;

        // Monta a descrição principal + complementos (apenas se concluídos)
        string fullDescription = mission.description + "\n\n";

        foreach (var obj in mission.objectives)
        {
            if (obj.isCompleted && !string.IsNullOrWhiteSpace(obj.moreDescription))
            {
                fullDescription += "" + obj.moreDescription + "\n";
            }
        }

        missionDescriptionText.text = fullDescription.Trim();

        // Atualiza os objetivos com estado
        string objectivesInfo = "";
        bool showNext = true;

        foreach (var obj in mission.objectives)
        {
            if (obj.isCompleted)
            {
                objectivesInfo += $"<s>- {obj.description}: {obj.requiredAmount}/{obj.requiredAmount}</s>\n";
            }
            else if (showNext)
            {
                objectivesInfo += $"- {obj.description}: Progresso: {obj.currentAmount}/{obj.requiredAmount}\n";
                showNext = false;
            }
        }

        missionObjectivesText.text = objectivesInfo;
    }

    public void AddMission(CreateMissions mission)
    {
        if (mission != null && !acceptedMissions.Contains(mission) && !completedMissions.Contains(mission))
        {
            mission.isActive = true;
            mission.isCompleted = false;
            acceptedMissions.Add(mission);

            RefreshMissionLists();
            Debug.Log("Missão aceita: " + mission.missionName);
        }
        else
        {
            Debug.LogWarning("Missão já foi aceita/completada ou está nula.");
        }
    }
    public MissionManagerData GetSaveData()
    {
        MissionManagerData data = new MissionManagerData();
        
        // Salva IDs das missões aceitas e completas
        foreach (var mission in acceptedMissions)
        {
            data.acceptedMissionIDs.Add(mission.missionID);
        }
        
        foreach (var mission in completedMissions)
        {
            data.completedMissionIDs.Add(mission.missionID);
        }
        
        // Salva o estado de todas as missões conhecidas
        foreach (var mission in allMissions)
        {
            MissionData missionData = new MissionData
            {
                missionID = mission.missionID,
                isActive = mission.isActive,
                isCompleted = mission.isCompleted
            };
            
            foreach (var objective in mission.objectives)
            {
                missionData.objectives.Add(new ObjectiveData
                {
                    isCompleted = objective.isCompleted,
                    currentAmount = objective.currentAmount
                });
            }
            
            data.missionStates.Add(missionData);
        }
        
        return data;
    }

    // Método para carregar o estado das missões
    public void LoadFromData(MissionManagerData data)
    {
        acceptedMissions.Clear();
        completedMissions.Clear();
        
        // Restaura o estado de cada missão
        foreach (var missionData in data.missionStates)
        {
            CreateMissions mission = allMissions.Find(m => m.missionID == missionData.missionID);
            if (mission != null)
            {
                mission.isActive = missionData.isActive;
                mission.isCompleted = missionData.isCompleted;
                
                for (int i = 0; i < mission.objectives.Count && i < missionData.objectives.Count; i++)
                {
                    mission.objectives[i].isCompleted = missionData.objectives[i].isCompleted;
                    mission.objectives[i].currentAmount = missionData.objectives[i].currentAmount;
                }
                
                // Adiciona às listas apropriadas
                if (data.acceptedMissionIDs.Contains(mission.missionID))
                {
                    acceptedMissions.Add(mission);
                }
                else if (data.completedMissionIDs.Contains(mission.missionID))
                {
                    completedMissions.Add(mission);
                }
            }
        }
        
        RefreshMissionLists();
    }
}