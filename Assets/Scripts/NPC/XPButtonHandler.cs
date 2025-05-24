using UnityEngine;

public class XPButtonHandler : MonoBehaviour
{
    private bool xpGranted = false;

    public void ToggleXP()
    {
        if (!xpGranted && ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(25);
            xpGranted = true;
        }
    }
}
