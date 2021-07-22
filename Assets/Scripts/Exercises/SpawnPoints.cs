using UnityEngine;
using UnityEngine.UI;

public class SpawnPoints : MonoBehaviour
{
    private enum ExerciseMode
    {
        Amount,
        Time
    }

    [SerializeField]
    private Text hintText;

    [SerializeField]
    private int requiredAmount = 5;
    private int _currentAmount = 0;

    [SerializeField]
    private float exerciseTimeAmount = 60f;
    private float _exerciseStartTime = -1f;
    private short count = 0;

    private Collider _areaCollider;

    [SerializeField]
    private GameObject pointPrefab;
    private GameObject _spawnedPoint = null;

    private ExerciseMode _exerciseMode = ExerciseMode.Amount;

    private void Start()
    {
        _areaCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_spawnedPoint == null)
        {
            SpawnPointInArea();

            if(_exerciseMode == ExerciseMode.Time)
            {
                if(count < 2)
                {
                    count++;
                }
                else if (count == 2) // Start timer after destroying first point
                {
                    _exerciseStartTime = Time.time;
                }
            }
        }

        switch (_exerciseMode)
        {
            case ExerciseMode.Amount:
                if (_currentAmount < requiredAmount)
                {
                    hintText.text = $"Количество задетых точек: {_currentAmount} из {requiredAmount}";
                }
                else
                {
                    hintText.text = "Упражнение завершено \n";
                }
                break;
            case ExerciseMode.Time:
                if (_exerciseStartTime < 0f)
                {
                    return;
                }
                if (_exerciseStartTime < exerciseTimeAmount)
                {
                    hintText.text = $"Оставшееся время: {exerciseTimeAmount - (Time.time - _exerciseStartTime)} \n";
                    hintText.text += $"Количество задетых точек: {_currentAmount}";
                }
                else
                {
                    hintText.text = "Упражнение завершено \n";
                    hintText.text += $"Количество задетых точек: {_currentAmount}";
                }
                break;
            default:
                break;
        }

    }

    private void SpawnPointInArea()
    {
        Vector3 min = _areaCollider.bounds.min;
        Vector3 max = _areaCollider.bounds.max;
        var randomX = Random.Range(min.x, max.x);
        var randomY = Random.Range(min.y, max.y);
        var randomZ = Random.Range(min.z, max.z);
        _spawnedPoint = Instantiate(pointPrefab, new Vector3(randomX, randomY, randomZ), Quaternion.identity);
    }
}
