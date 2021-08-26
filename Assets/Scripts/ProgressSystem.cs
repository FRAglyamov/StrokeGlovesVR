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
        savePath = Application.persistentDataPath + "/progress_saves/" + exerciseName;
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

        ExerciseResult result = new ExerciseResult { date = System.DateTime.Now, time = _timer };
        string json = JsonUtility.ToJson(result);
        Debug.Log($"JSON: {json}");
        File.WriteAllText($"{savePath}/{result.date}.json", json);
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

