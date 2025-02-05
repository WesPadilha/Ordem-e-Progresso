using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCombat : MonoBehaviour
{
    public MovimentCombat playerMovement;
    public Button resetButton;
    public int maxActionPoints = 6; // Defina o máximo de pontos de ação

    void Start()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetActionPoints);
        }
    }

    public void ResetActionPoints()
    {
        if (playerMovement != null)
        {
            playerMovement.RestoreActionPoints(maxActionPoints - playerMovement.actionPoints);
        }
    }
}
