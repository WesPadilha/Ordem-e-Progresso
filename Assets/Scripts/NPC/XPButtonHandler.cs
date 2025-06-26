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

    public bool IsXPGranted()
    {
        return xpGranted;
    }

    public void SetXPGranted(bool granted)
    {
        xpGranted = granted;
        // Se quiser, pode atualizar a UI do botão aqui para refletir o estado (ex: desabilitar o botão)
    }
}
