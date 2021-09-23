using System.Collections.Generic;
using UnityEngine;

// Remark: May be better to start on UI button click? Or physical button?
[RequireComponent(typeof(ProgressSystem))]
public class ExerciseStart : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> exerciseObjects;
    private ProgressSystem _progressSystem;

    private void Start()
    {
        _progressSystem = GetComponent<ProgressSystem>();
    }

    public void StartExercise()
    {
        foreach (GameObject go in exerciseObjects)
        {
            go.SetActive(true);
        }
        _progressSystem.StartTimer();
    }
}
