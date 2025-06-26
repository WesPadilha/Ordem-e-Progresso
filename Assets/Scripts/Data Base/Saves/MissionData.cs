// MissionData.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string missionID;
    public bool isActive;
    public bool isCompleted;
    public List<ObjectiveData> objectives = new List<ObjectiveData>();
}

[System.Serializable]
public class ObjectiveData
{
    public bool isCompleted;
    public int currentAmount;
}

[System.Serializable]
public class MissionManagerData
{
    public List<string> acceptedMissionIDs = new List<string>();
    public List<string> completedMissionIDs = new List<string>();
    public List<MissionData> missionStates = new List<MissionData>();
}

[System.Serializable]
public class ActiveAreasData
{
    public List<bool> areaStates;

    public ActiveAreasData(List<ActiveArea> activeAreas)
    {
        areaStates = new List<bool>();
        foreach (var area in activeAreas)
        {
            areaStates.Add(area.objetoParaAtivar != null && area.objetoParaAtivar.activeSelf);
        }
    }
}