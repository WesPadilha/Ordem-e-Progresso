using UnityEngine;
using System;
using System.Collections.Generic;

public enum MissionState
{
    Inactive,
    Active,
    Completed
}

public class MissionCompletedEventArgs : EventArgs
{
    public string MissionID { get; private set; }

    public MissionCompletedEventArgs(string missionID)
    {
        MissionID = missionID;
    }
}

[Serializable]
public class MissionData
{
    public string missionID;
    public string missionName;
    public string missionDescription;
    public string missionObjectives;
}

public class MissionManager : MonoBehaviour
{
    public static event EventHandler<MissionCompletedEventArgs> MissionCompleted;

    private Dictionary<string, MissionState> missionStates = new Dictionary<string, MissionState>();
    private Dictionary<string, int> missionCheckpoints = new Dictionary<string, int>();
    private Dictionary<string, MissionData> missionData = new Dictionary<string, MissionData>();

    // Iniciar uma missão
    public void StartMission(string missionID, string name, string description, string objectives)
    {
        if (!missionStates.ContainsKey(missionID))
        {
            missionStates[missionID] = MissionState.Active;
            missionCheckpoints[missionID] = 0;

            MissionData data = new MissionData()
            {
                missionID = missionID,
                missionName = name,
                missionDescription = description,
                missionObjectives = objectives
            };
            missionData[missionID] = data;

            Debug.Log("Mission started: " + missionID);
        }
    }

    // Completar missão
    public void CompleteMission(string missionID)
    {
        if (missionStates.ContainsKey(missionID))
        {
            missionStates[missionID] = MissionState.Completed;
            Debug.Log("Mission completed: " + missionID);
            OnMissionCompleted(missionID);
        }
    }

    public bool IsMissionActive(string missionID)
    {
        return missionStates.ContainsKey(missionID) && missionStates[missionID] == MissionState.Active;
    }

    public bool IsMissionCompleted(string missionID)
    {
        return missionStates.ContainsKey(missionID) && missionStates[missionID] == MissionState.Completed;
    }

    protected virtual void OnMissionCompleted(string missionID)
    {
        MissionCompleted?.Invoke(this, new MissionCompletedEventArgs(missionID));
    }

    public List<MissionData> GetActiveMissions()
    {
        List<MissionData> activeMissions = new List<MissionData>();

        foreach (var kvp in missionStates)
        {
            if (kvp.Value == MissionState.Active && missionData.ContainsKey(kvp.Key))
            {
                activeMissions.Add(missionData[kvp.Key]);
            }
        }

        return activeMissions;
    }

    public MissionData GetMissionData(string missionID)
    {
        return missionData.ContainsKey(missionID) ? missionData[missionID] : null;
    }

    // Checkpoint functions (opcional)
    public void SaveMissionCheckpoint(string missionID, int checkpointIndex)
    {
        if (missionCheckpoints.ContainsKey(missionID))
        {
            missionCheckpoints[missionID] = checkpointIndex;
        }
    }

    public int LoadMissionCheckpoint(string missionID)
    {
        return missionCheckpoints.ContainsKey(missionID) ? missionCheckpoints[missionID] : 0;
    }
}
