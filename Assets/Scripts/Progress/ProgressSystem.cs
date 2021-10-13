using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    [SerializeField]
    private string exerciseName = "none";
    private float _startTime = -1f;
    private float _timer = -1f;
    public string SavePath { get; private set; } = "";
    private DirectoryInfo _dir;
    public FileInfo[] Files { get; private set; }

    private void Awake()
    {
        // Application.persistentDataPath = C:\Users\дмл\AppData\LocalLow\DML\StrokeVR
        SavePath = Path.Combine(Application.persistentDataPath, "progress_saves", exerciseName);
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
    }

    public void FilesInfoUpdate()
    {
        _dir = new DirectoryInfo(SavePath);
        Files = _dir.GetFiles("*.json").OrderByDescending(f => f.LastWriteTime).ToArray();
    }

    public void StartTimer()
    {
        _startTime = Time.time;
    }

    public void EndTimer()
    {
        if (_startTime < 0f)
        {
            Debug.LogError("Timer didn't start!");
            return;
        }

        _timer = Time.time - _startTime;
    }

    public void SaveResultIntoJSON()
    {
        if (_timer < 0f)
        {
            Debug.LogError("Timer didn't start or end!");
            return;
        }

        ExerciseResult result = new ExerciseResult { date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), time = _timer.ToString("f4") };
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
    //    EndTimer();
    //    SaveResultIntoJSON();
    //}

}

