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
    private float _dotProduct; // Dot product between hand menu canvas and camera

    private void Awake()
    {
        recenterBtn.onClick.AddListener(ExerciseSettingsUI.Instance.Recenter);
        restartBtn.onClick.AddListener(ExerciseSettingsUI.Instance.RestartExercise);
        //menuBtn.onClick.AddListener();
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
        }

    }
}
