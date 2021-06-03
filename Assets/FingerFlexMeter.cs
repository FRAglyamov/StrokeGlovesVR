using UnityEngine;
using UnityEngine.UI;

public class FingerFlexMeter : MonoBehaviour
{
    enum MeasurmentObject
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky,
        All
    }

    [SerializeField] 
    private Text currentMeasurmentObjectText;
    [SerializeField] 
    private Text currentMeasureValueText;
    [SerializeField] 
    private Text flexCountText;

    [SerializeField, Range(0f,1f)]
    private float requiredFlexPercentage = 0.8f;
    [SerializeField] 
    private int requiredFlexesAmount = 2;

    private int _currentFlexesAmount = 0;
    [SerializeField] // Remove SerializeField after tests
    private MeasurmentObject _measurmentObject = 0;
    private bool _isFlexed = false;

    private void Update()
    {
        var glove = GloveDevice.current;
        switch (_measurmentObject)
        {
            case MeasurmentObject.Thumb:
                UpdateMeasureInfo("Большой", glove.thumb.ReadValue());
                break;
            case MeasurmentObject.Index:
                UpdateMeasureInfo("Указательный", glove.index.ReadValue());
                break;
            case MeasurmentObject.Middle:
                UpdateMeasureInfo("Средний", glove.middle.ReadValue());
                break;
            case MeasurmentObject.Ring:
                UpdateMeasureInfo("Безымянный", glove.ring.ReadValue());
                break;
            case MeasurmentObject.Pinky:
                UpdateMeasureInfo("Мизинец", glove.pinky.ReadValue());
                break;
            case MeasurmentObject.All:
                UpdateMeasureInfoAllFingers(glove);
                break;
            default:
                break;
        }
    }

    private void UpdateMeasureInfo(string fingerName, float flexValue)
    {
        if (!_isFlexed && flexValue >= requiredFlexPercentage)
        {
            _isFlexed = true;
            _currentFlexesAmount++;
        }
        else if(_isFlexed && flexValue <= requiredFlexPercentage / 2)
        {
            _isFlexed = false;
        }
        if (_currentFlexesAmount >= requiredFlexesAmount)
        {
            _measurmentObject++;
            _currentFlexesAmount = 0;
        }

        ChangeMeasurmentObjectText(fingerName);
        ChangeMeasureValueText(flexValue);
        UpdateFlexCountText();
    }

    private void ChangeMeasurmentObjectText(string fingerName)
    {
        currentMeasurmentObjectText.text = "Текущий измеряемый палец: " + fingerName;
    }

    private void ChangeMeasureValueText(float value)
    {
        currentMeasureValueText.text = "Текущее значение сгиба: " + value.ToString();
    }

    private void UpdateFlexCountText()
    {
        flexCountText.text = "Количество сгибаний: " + _currentFlexesAmount + " из " + requiredFlexesAmount;
    }

    private void UpdateMeasureInfoAllFingers(GloveDevice glove)
    {
        if (!_isFlexed && IsAllFingersFlexed(glove))
        {
            _isFlexed = true;
            _currentFlexesAmount++;
        }
        else if (_isFlexed && IsAllFingersUnflexed(glove))
        {
            _isFlexed = false;
        }
        if (_currentFlexesAmount >= requiredFlexesAmount)
        {
            _measurmentObject = 0;
            _currentFlexesAmount = 0;
            // TODO: Переделать в "Конец упражнения"
        }

        currentMeasurmentObjectText.text = "Текущий измеряемый палец: Все";
        currentMeasureValueText.text = "Текущее значение сгибов: \n"
            + "Большой - " + glove.thumb.ReadValue() + "\n"
            + "Указательный - " + glove.index.ReadValue() + "\n"
            + "Средний - " + glove.middle.ReadValue() + "\n"
            + "Безымянный - " + glove.ring.ReadValue() + "\n"
            + "Мизинец - " + glove.pinky.ReadValue();
        UpdateFlexCountText();
    }

    private bool IsAllFingersFlexed(GloveDevice glove)
    {
        return glove.thumb.ReadValue() >= requiredFlexPercentage
                    && glove.index.ReadValue() >= requiredFlexPercentage
                    && glove.middle.ReadValue() >= requiredFlexPercentage
                    && glove.ring.ReadValue() >= requiredFlexPercentage
                    && glove.pinky.ReadValue() >= requiredFlexPercentage;
    }

    private bool IsAllFingersUnflexed(GloveDevice glove)
    {
        return glove.thumb.ReadValue() <= requiredFlexPercentage / 2
                    && glove.index.ReadValue() <= requiredFlexPercentage / 2
                    && glove.middle.ReadValue() <= requiredFlexPercentage / 2
                    && glove.ring.ReadValue() <= requiredFlexPercentage / 2
                    && glove.pinky.ReadValue() <= requiredFlexPercentage / 2;
    }

}
