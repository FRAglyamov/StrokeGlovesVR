using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseSettingsUI : MonoBehaviour
{
    [SerializeField]
    private Text _currentExerciseText;
    [SerializeField]
    private Dropdown _COMPortDropdown;
    [SerializeField]
    private Text _timerText;
    [SerializeField]
    private Dropdown _mirrorDropdown;
    [SerializeField]
    private Dropdown _exerciseSelectionDropdown;
    [SerializeField]
    private InputField _userIDInputField;
    [SerializeField]
    private SerialController _serialController;

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
        _currentExerciseText.text = "Текущее упражнение: " + SceneManager.GetActiveScene().name;
        LoadCOMPorts();
        LoadExerciseList();
        SceneManager.sceneLoaded += OnSceneLoaded;
        _userIDInputField.onEndEdit.AddListener(delegate { OnUserChange(_userIDInputField); });
    }

    private void Update()
    {
        _timerText.text = "Таймер: " + ProgressSystem.Instance.Timer.ToString("f2");
    }

    private void OnUserChange(InputField input)
    {
        ProgressSystem.Instance.userID = input.text;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentExerciseText.text = "Текущее упражнение: " + scene.name;
        
        // TODO: Rewrite
        _serialController = FindObjectOfType<SerialController>();
        _serialController.ChangeCOM(_COMPortDropdown.options[_COMPortDropdown.value].text);
    }

    public void RestartExercise()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeExercise()
    {
        SceneManager.LoadScene(_exerciseSelectionDropdown.options[_exerciseSelectionDropdown.value].text);
    }

    public void LoadCOMPorts()
    {
        string[] ports = SerialPort.GetPortNames();
        _COMPortDropdown.ClearOptions();
        foreach (string p in ports)
        {
            _COMPortDropdown.options.Add(new Dropdown.OptionData() { text = p });
        }
        _COMPortDropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(); });
    }

    private void DropdownItemSelected()
    {
        _serialController.portName = _COMPortDropdown.options[_COMPortDropdown.value].text;
    }

    private void LoadExerciseList()
    {
        var optionDataList = new List<Dropdown.OptionData>();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i) 
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            optionDataList.Add(new Dropdown.OptionData(name));
        }

        _exerciseSelectionDropdown.ClearOptions();
        _exerciseSelectionDropdown.AddOptions(optionDataList);
    }
}
