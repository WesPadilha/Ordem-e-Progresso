using UnityEngine;
using TMPro;

public class MovementUI : MonoBehaviour
{
    public TextMeshProUGUI actionPointsText;

    public void UpdateActionPoints(int currentPoints)
    {
        actionPointsText.text = "PA: " + currentPoints.ToString();
    }

    public void ShowNoCombat()
    {
        actionPointsText.text = "PA: --";
    }
}
