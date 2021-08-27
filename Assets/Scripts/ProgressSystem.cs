using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    [SerializeField]
    private string exerciseName = "none";
    private float _startTime = -1f;
    private float _timer = -1f;
    private string savePath = "";

    private void Awake()
    {
        // Application.persistentDataPath = C:\Users\дмл\AppData\LocalLow\DML\StrokeVR
        savePath = Path.Combine(Application.persistentDataPath, "progress_saves", exerciseName);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    private void Start()
    {
        StartCoroutine(ProgressSystemTest(2f));
    }

    IEnumerator ProgressSystemTest(float time)
    {
        StartTimer();

        yield return new WaitForSeconds(time);

        EndTimer();
        SaveResultIntoJSON();

        var results = GetPreviousResults(5);
        foreach (var r in results)
        {
            Debug.Log($"Result date = {r.date}, time = {r.time}");
        }
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
        File.WriteAllText(Path.Combine(savePath, result.date + ".json"), json);
    }

    public ExerciseResult LoadResultFromJSON(string filePath)
    {
        if (File.Exists(filePath))
        {
            string fileContents = File.ReadAllText(filePath);
            return JsonUtility.FromJson<ExerciseResult>(fileContents);
        }
        Debug.LogError("Can't load. File don't exist! File path: " + filePath);
        return null;
    }

    public List<ExerciseResult> GetPreviousResults(int resultAmount)
    {
        List<ExerciseResult> results = new List<ExerciseResult>();

        DirectoryInfo dir = new DirectoryInfo(savePath);
        FileInfo[] info = dir.GetFiles("*.json").OrderByDescending(f => f.LastWriteTime).ToArray();

        if (info.Length < resultAmount)
        {
            resultAmount = info.Length;
        }
        for (int i = 0; i < resultAmount; i++)
        {
            results.Add(LoadResultFromJSON(info[i].FullName));
        }

        return results;
    }

}

