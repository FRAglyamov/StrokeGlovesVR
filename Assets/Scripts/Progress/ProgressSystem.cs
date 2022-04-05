using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton class. Responsible for saving current results or loading others from JSON.
/// </summary>
public class ProgressSystem : MonoBehaviour
{
    [SerializeField]
    private string exerciseName = "none";
    private float _startTime = -1f;
    private DirectoryInfo _dir;
    public string SavePath { get; private set; } = "";
    public FileInfo[] Files { get; private set; }
    public float Timer { get; private set; } = 0f;
    public string userID = "";

    // Singleton
    public static ProgressSystem Instance { get; private set; }

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
        //exerciseName = SceneManager.GetActiveScene().name;
        //UpdateSavePath();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (_startTime > 0f)
        {
            Timer = Time.time - _startTime;
        }
        else
        {
            StartTimer(); // Remark: For test, remove later
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        exerciseName = scene.name;
        _startTime = -1f;
        Timer = 0f;
        UpdateSavePath();
    }

    /// <summary>
    /// Update path of progress saves (if userID or exercise name was changed).
    /// </summary>
    public void UpdateSavePath()
    {
        // Application.persistentDataPath = C:\Users\дмл\AppData\LocalLow\DML\StrokeVR
        //SavePath = Path.Combine(Application.persistentDataPath, "progress_saves", exerciseName);
        SavePath = Path.Combine(Application.persistentDataPath, "progress_saves", userID, exerciseName);
        Debug.Log("SavePath: " + SavePath);
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
    }

    /// <summary>
    /// Get and update list FileInfo in save directory.
    /// </summary>
    /// <param name="savePath"></param>
    public void FilesInfoUpdate(string savePath = "none")
    {
        if(savePath == "none")
        {
            savePath = SavePath;
        }
        _dir = new DirectoryInfo(savePath);
        Files = _dir.GetFiles("*.json").OrderByDescending(f => f.Name).ToArray();
    }

    public void StartTimer()
    {
        _startTime = Time.time;
    }

    public void SaveResultIntoJSON()
    {
        if (Timer <= 0f)
        {
            Debug.LogError("Timer didn't start or end!");
            return;
        }

        ExerciseResult result = new ExerciseResult { date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), time = Timer.ToString("f4") };
        string json = JsonUtility.ToJson(result);
        Debug.Log($"JSON: {json}");
        File.WriteAllText(Path.Combine(SavePath, result.date + ".json"), json);
    }

    public ExerciseResult LoadResultFromJSON(string filePath)
    {
        if (File.Exists(filePath))
        {
            string fileContents = File.ReadAllText(filePath);
            return JsonUtility.FromJson<ExerciseResult>(fileContents);
        }
        else
        {
            Debug.LogError("Can't load. File don't exist! File path: " + filePath);
            return null;
        }
    }

    //private void Start()
    //{
    //    StartCoroutine(ProgressSystemTest(2f));
    //}

    //IEnumerator ProgressSystemTest(float time)
    //{
    //    StartTimer();
    //    yield return new WaitForSeconds(time);
    //    SaveResultIntoJSON();
    //}

}

