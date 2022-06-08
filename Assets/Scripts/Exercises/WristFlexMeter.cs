using UnityEngine;
using UnityEngine.UI;

public class WristFlexMeter : MonoBehaviour
{
    enum MeasurementWristDirection // Ладонь направлена вниз, тыльная сторона ладони - вверх
    {
        Down,
        Up,
        Left,
        Right
    }

    [SerializeField]
    private Transform hand;

    //[SerializeField]
    //private Text currentMeasurmentDirectionText;
    //[SerializeField]
    //private Text currentMeasureValueText;
    //[SerializeField]
    //private Text flexCountText;
    [SerializeField]
    private Text hintText;

    [SerializeField]
    private float requiredDownFlex = 60f, requiredUpFlex = 45f, requiredSideFlex = 15f;
    [SerializeField]
    private int requiredFlexesAmount = 2;
    private int _currentFlexesAmount = 0;
    
    private Vector3 _defaultRotation;
    private Vector3 _currentRotation;
    private MeasurementWristDirection _measureDirection = 0;
    private bool _isFlexed = false;

    private void Start()
    {
        _defaultRotation = GetInspectorLikeRotation();
    }

    private void Update()
    {
        _currentRotation = GetInspectorLikeRotation();

        switch (_measureDirection)
        {
            case MeasurementWristDirection.Down:
                //Debug.Log("Local rotation: " + _currentRotation.x + ", Default: " + _defaultRotation.x);
                if (!_isFlexed && _currentRotation.x >= _defaultRotation.x + requiredDownFlex)
                {
                    _isFlexed = true;
                    _currentFlexesAmount++;
                    //UpdateFlexCountText();
                }
                else if (_isFlexed && _currentRotation.x <= _defaultRotation.x + requiredDownFlex / 2)
                {
                    _isFlexed = false;
                }
                UpdateHintText("Вниз", _currentRotation.x - _defaultRotation.x);
                //ChangeMeasurmentDirectionText("Вниз");
                //ChangeMeasureValueText(_currentRotation.x - _defaultRotation.x);
                break;
            case MeasurementWristDirection.Up:
                if (!_isFlexed && _currentRotation.x <= _defaultRotation.x - requiredUpFlex)
                {
                    _isFlexed = true;
                    _currentFlexesAmount++;
                    //UpdateFlexCountText();
                }
                else if (_isFlexed && _currentRotation.x >= _defaultRotation.x - requiredDownFlex / 2)
                {
                    _isFlexed = false;
                }
                UpdateHintText("Вверх", _currentRotation.x - _defaultRotation.x);
                //ChangeMeasurmentDirectionText("Вверх");
                //ChangeMeasureValueText(_currentRotation.x - _defaultRotation.x);
                break;
            case MeasurementWristDirection.Left:
                if (!_isFlexed && _currentRotation.y <= _defaultRotation.y - requiredSideFlex)
                {
                    _isFlexed = true;
                    _currentFlexesAmount++;
                    //UpdateFlexCountText();
                }
                else if (_isFlexed && _currentRotation.y >= _defaultRotation.y - requiredSideFlex / 2)
                {
                    _isFlexed = false;
                }
                UpdateHintText("Влево", _currentRotation.y - _defaultRotation.y);
                //ChangeMeasurmentDirectionText("Влево");
                //ChangeMeasureValueText(_currentRotation.y - _defaultRotation.y);
                break;
            case MeasurementWristDirection.Right:
                if (!_isFlexed && _currentRotation.y >= _defaultRotation.y + requiredSideFlex)
                {
                    _isFlexed = true;
                    _currentFlexesAmount++;
                    //UpdateFlexCountText();
                }
                else if (_isFlexed && _currentRotation.y <= _defaultRotation.y + requiredSideFlex / 2)
                {
                    _isFlexed = false;
                }
                UpdateHintText("Вправо", _currentRotation.y - _defaultRotation.y);
                //ChangeMeasurmentDirectionText("Вправо");
                //ChangeMeasureValueText(_currentRotation.y - _defaultRotation.y);
                break;
            default:
                hintText.text = 
                    "Текущее измеряемое направление: None \n" +
                    "Сгибы: \n" + 
                    (_currentRotation.x - _defaultRotation.x) + "\n" + 
                    (_currentRotation.y - _defaultRotation.y) + "\n" +
                    "Количество сгибаний: None";
                break;
        }
        if (_currentFlexesAmount >= requiredFlexesAmount)
        {
            _measureDirection++;
            _currentFlexesAmount = 0;
            //UpdateFlexCountText();
            // TODO: Сделать "Конец упражнения"
        }
    }

    private void UpdateHintText(string directionName, float value)
    {
        hintText.text = "Текущее измеряемое направление: " + directionName + "\n";
        hintText.text += "Текущее значение сгиба: " + value.ToString() + "\n";
        hintText.text += "Количество сгибаний: " + _currentFlexesAmount + " из " + requiredFlexesAmount;
    }

    //private void ChangeMeasurmentDirectionText(string directionName)
    //{
    //    currentMeasurmentDirectionText.text = "Текущее измеряемое направление: " + directionName;
    //}

    //private void ChangeMeasureValueText(float value)
    //{
    //    currentMeasureValueText.text = "Текущее значение сгиба: " + value.ToString();
    //}

    //private void UpdateFlexCountText()
    //{
    //    flexCountText.text = "Количество сгибаний: " + _currentFlexesAmount + " из " + requiredFlexesAmount;
    //}

    private Vector3 GetInspectorLikeRotation()
    {
        Vector3 angle = hand.eulerAngles;
        float x = angle.x;
        float y = angle.y;
        float z = angle.z;

        if (Vector3.Dot(transform.up, Vector3.up) >= 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = angle.x - 360f;
            }
        }
        if (Vector3.Dot(transform.up, Vector3.up) < 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = 180 - angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = 180 - angle.x;
            }
        }

        if (angle.y > 180)
        {
            y = angle.y - 360f;
        }

        if (angle.z > 180)
        {
            z = angle.z - 360f;
        }

        return new Vector3(x, y, z);
    }

    public void MemoriseRotations()
    {
        _defaultRotation = _currentRotation;
        _currentFlexesAmount = 0;
        _measureDirection = 0;
        _isFlexed = false;
    }

}
