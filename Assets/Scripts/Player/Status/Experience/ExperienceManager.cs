using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    public delegate void ExperienceChangeHandler(int amount);
    public event ExperienceChangeHandler OnExperienceChange;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantém entre cenas
        }
    }

    public void AddExperience(int amount)
    {   
        OnExperienceChange?.Invoke(amount);
    }
}