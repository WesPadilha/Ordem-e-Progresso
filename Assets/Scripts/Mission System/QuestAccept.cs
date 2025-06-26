using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAccept : MonoBehaviour
{
    public MissionManager missionManager;

    public void AcceptMission(CreateMissions mission)
    {
        missionManager.AddMission(mission);
    }
}
