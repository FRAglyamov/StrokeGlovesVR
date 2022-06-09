using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu of recent saved results of current exercise.
/// </summary>
public class ProgressMenu : MonoBehaviour
{
    public enum PageChange
    {
        Recent,
        Left,
        Right,
    }

    private int _curFromResult = 0;
    private int _resultAmount; // Amount of progress results to show on the menu.
    private int _filesAmount; // Amount of files in the directory (based on user, exercise).
    [SerializeField]
    private GameObject resultsPanel;
    private Text[] _elementsText;
    private ProgressSystem _progressSystem;

    private void Start()
    {
        _elementsText = resultsPanel.GetComponentsInChildren<Text>();
        _resultAmount = _elementsText.Length;
        ChangeResultsPage(0);
        _progressSystem = AssistantSystem.Instance.progressSystem;
    }

    public void ChangeResultsPage(int pageChange)
    {
        PageChange pageChangeState = (PageChange)pageChange;
        _curFromResult = pageChangeState switch
        {
            PageChange.Recent => 0,
            PageChange.Left => _curFromResult - _resultAmount,
            PageChange.Right => _curFromResult + _resultAmount,
            _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(pageChangeState)),
        };

        ShowResultsInMenu(_curFromResult, _curFromResult + _resultAmount);
    }

    private void ShowResultsInMenu(int fromResult, int toResult)
    {
        _progressSystem.FilesInfoUpdate();
        _filesAmount = _progressSystem.Files.Length;

        CheckResultsOnLeft(ref fromResult, ref toResult);
        CheckResultsOnRight(ref fromResult, ref toResult);

        for (int i = fromResult; i < toResult; i++)
        {
            ExerciseResult tmpResult = _progressSystem.LoadResultFromJSON(_progressSystem.Files[i].FullName);
            _elementsText[i % _resultAmount].text = tmpResult.date + " - " + tmpResult.time;
        }
    }

    private void CheckResultsOnLeft(ref int fromResult, ref int toResult)
    {
        if (fromResult < 0)
        {
            _curFromResult = fromResult = (int)(_filesAmount / _resultAmount) * _resultAmount;
            toResult = fromResult + _resultAmount;

            CheckFilesEnd(ref toResult);
        }
    }

    private void CheckResultsOnRight(ref int fromResult, ref int toResult)
    {
        if (fromResult > _filesAmount)
        {
            _curFromResult = fromResult = 0;
            toResult = _resultAmount;
        }

        CheckFilesEnd(ref toResult);
    }

    private void CheckFilesEnd(ref int toResult)
    {
        if (toResult > _filesAmount)
        {
            for (int i = _filesAmount; i < toResult; i++)
            {
                _elementsText[i % _resultAmount].text = "";
            }
            toResult = _filesAmount;
        }
    }
}
