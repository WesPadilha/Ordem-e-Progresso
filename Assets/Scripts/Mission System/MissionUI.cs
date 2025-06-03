using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MissionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform missionListContainer;
    public GameObject missionButtonPrefab;
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDescriptionText;
    public TextMeshProUGUI missionObjectivesText;

    private MissionManager missionManager;
    private Dictionary<string, GameObject> missionButtons = new Dictionary<string, GameObject>();

    private void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();
        RefreshMissionList();
    }

    public void RefreshMissionList()
    {
        foreach (Transform child in missionListContainer)
        {
            Destroy(child.gameObject);
        }
        missionButtons.Clear();

        List<MissionData> activeMissions = missionManager.GetActiveMissions();

        foreach (MissionData mission in activeMissions)
        {
            GameObject buttonObj = Instantiate(missionButtonPrefab, missionListContainer);
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonText.text = mission.missionName;

            Button btn = buttonObj.GetComponent<Button>();
            string missionID = mission.missionID;

            btn.onClick.AddListener(() => ShowMissionDetails(missionID));

            missionButtons.Add(missionID, buttonObj);
        }
    }

    public void ShowMissionDetails(string missionID)
    {
        MissionData mission = missionManager.GetMissionData(missionID);

        if (mission != null)
        {
            missionNameText.text = mission.missionName;
            missionDescriptionText.text = mission.missionDescription;
            missionObjectivesText.text = mission.missionObjectives;
        }
    }
}
