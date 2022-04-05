using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Singleton class, which contains methods and references to assistant menu ui.
/// </summary>
public class ExerciseSettingsUI : MonoBehaviour
{
    [SerializeField]
    private Text currentExerciseText;
    [SerializeField]
    private Dropdown COMPortDropdown;
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Dropdown mirrorDropdown;
    [SerializeField]
    private Dropdown exerciseSelectionDropdown;
    [SerializeField]
    private InputField userIDInputField;
    [SerializeField]
    private SerialController serialController;
    private TeleportationProvider _teleportationProvider;

    // Singleton
    public static ExerciseSettingsUI Instance { get; private set; }
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
        Initialize();
    }

    private void Initialize()
    {
        //currentExerciseText.text = "Текущее упражнение: " + SceneManager.GetActiveScene().name;
        LoadCOMPorts();
        LoadExerciseList();
        SceneManager.sceneLoaded += OnSceneLoaded;
        userIDInputField.onEndEdit.AddListener(delegate { OnUserChange(userIDInputField); });
    }

    private void Update()
    {
        timerText.text = "Таймер: " + ProgressSystem.Instance.Timer.ToString("f2");
    }

    private void OnUserChange(InputField input)
    {
        ProgressSystem.Instance.userID = input.text;
        ProgressSystem.Instance.UpdateSavePath();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentExerciseText.text = "Текущее упражнение: " + scene.name;
        _teleportationProvider = FindObjectOfType<TeleportationProvider>();

        // TODO: Rewrite.
        serialController = FindObjectOfType<SerialController>();
        serialController.ChangeCOM(COMPortDropdown.options[COMPortDropdown.value].text);
    }

    /// <summary>
    /// Reset VR centering via teleport system.
    /// </summary>
    public void Recenter()
    {
        var teleportRequest = new TeleportRequest();
        teleportRequest.destinationPosition = Vector3.zero;
        teleportRequest.destinationRotation = Quaternion.identity;
        _teleportationProvider.QueueTeleportRequest(teleportRequest);
    }

    /// <summary>
    /// Reload current scene.
    /// </summary>
    public void RestartExercise()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Load scene based on selected in dropdown.
    /// </summary>
    public void ChangeExercise()
    {
        SceneManager.LoadScene(exerciseSelectionDropdown.options[exerciseSelectionDropdown.value].text);
    }

    /// <summary>
    /// Load connected COM ports and refresh dropdown.
    /// </summary>
    public void LoadCOMPorts()
    {
        string[] ports = SerialPort.GetPortNames();
        COMPortDropdown.ClearOptions();
        foreach (string p in ports)
        {
            COMPortDropdown.options.Add(new Dropdown.OptionData() { text = p });
        }
        COMPortDropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(); });
    }

    /// <summary>
    /// Change SerialController port, when changing it in dropdown.
    /// </summary>
    private void DropdownItemSelected()
    {
        serialController.portName = COMPortDropdown.options[COMPortDropdown.value].text;
    }

    /// <summary>
    /// Load all builded scenes and display it in dropdown.
    /// </summary>
    private void LoadExerciseList()
    {
        var optionDataList = new List<Dropdown.OptionData>();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i) 
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            optionDataList.Add(new Dropdown.OptionData(name));
        }

        exerciseSelectionDropdown.ClearOptions();
        exerciseSelectionDropdown.AddOptions(optionDataList);
    }
}
