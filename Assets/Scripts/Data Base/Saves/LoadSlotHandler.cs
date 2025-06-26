using UnityEngine;

public class LoadSlotHandler : MonoBehaviour
{
    public static LoadSlotHandler Instance;

    public int slotIndexToLoad = -1;
    public bool hasSlotToLoad = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
