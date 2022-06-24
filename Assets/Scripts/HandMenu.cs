using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandMenu : MonoBehaviour
{
    [SerializeField]
    private Button recenterBtn;
    [SerializeField]
    private Button restartBtn;
    [SerializeField]
    private Button menuBtn;
    [SerializeField]
    private Button exercisesBtn;
    [SerializeField]
    private Transform hmdCameraTransform;
    [SerializeField]
    private GameObject handMenuPanel;
    [SerializeField]
    private GameObject progressMenuPanel;
    private float _dotProduct; // Dot product between hand menu canvas and camera
    private ExerciseSettingsUI _exerciseSettings;

    private void Start()
    {
        _exerciseSettings = AssistantSystem.Instance.exerciseSettingUI;
        recenterBtn.onClick.AddListener(_exerciseSettings.Recenter);
        restartBtn.onClick.AddListener(_exerciseSettings.RestartExercise);
        // TODO: Add pop-up menu with latest results of the current exercise.
        menuBtn.onClick.AddListener(delegate { progressMenuPanel.SetActive(true); });
        //menuBtn.onClick.AddListener(delegate { progressMenuPanel.SetActive(!progressMenuPanel.activeInHierarchy); });
        exercisesBtn.onClick.AddListener(() => SceneManager.LoadScene("Selection Menu"));
    }

    private void Update()
    {
        _dotProduct = Vector3.Dot(transform.forward, (hmdCameraTransform.position - transform.position).normalized);
        if (_dotProduct < -0.9f)
        { 
            handMenuPanel.SetActive(true);
        }
        else
        {
            handMenuPanel.SetActive(false);
            progressMenuPanel.SetActive(false);
        }

    }
}
