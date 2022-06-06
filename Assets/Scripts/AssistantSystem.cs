using UnityEngine;

public class AssistantSystem : MonoBehaviour
{
    public static AssistantSystem Instance { get; private set; }
    public ProgressSystem progressSystem;
    public ExerciseSettingsUI exerciseSettingUI;
    public SerialController serialController;

    // Singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
