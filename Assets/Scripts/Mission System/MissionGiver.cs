using UnityEngine;

public class MissionGiver : MonoBehaviour
{
    public string missionID;
    public string missionName;
    [TextArea] public string missionDescription;
    [TextArea] public string missionObjectives;

    private MissionManager missionManager;

    private void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();
    }

    // Este método pode ser chamado por um botão na UI de diálogo
    public void StartMission()
    {
        if (missionManager != null)
        {
            missionManager.StartMission(missionID, missionName, missionDescription, missionObjectives);
        }
    }
}
