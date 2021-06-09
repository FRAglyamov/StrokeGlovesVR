using UnityEngine;
using UnityEngine.UI;

public class BallStepManager : MonoBehaviour
{
    private static BallStepManager _instance;
    public static BallStepManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public bool isBallInGlass = false;
    public bool isBallDrinked = false;

    [SerializeField]
    private Text hintText;

    private void Update()
    {
        if (!isBallInGlass)
        {
            hintText.text = "Положите шарик в стакан";
        }
        else
        {
            if (!isBallDrinked)
            {
                hintText.text = "Выпейте шарик из стакана";
            }
            else
            {
                hintText.text = "Упражнение завершено";
            }
        }
    }
}
