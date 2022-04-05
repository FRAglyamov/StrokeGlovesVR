using System;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(ProgressSystem))]
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

    private bool _isDirInit = false;
    private int _curFromResult = 0;
    private int _resultAmount;
    private ProgressSystem _system;
    [SerializeField]
    private GameObject resultsPanel;
    private Text[] _elementsText;

    private void Start()
    {
        //_system = GetComponent<ProgressSystem>();
        _system = ProgressSystem.Instance;
        _elementsText = resultsPanel.GetComponentsInChildren<Text>();
        _resultAmount = _elementsText.Length;
        ChangeResultsPage(0);
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
        if (!_isDirInit)
        {
            _system.FilesInfoUpdate();
            _isDirInit = true;
        }

        CheckResultsOnLeft(ref fromResult, ref toResult);
        CheckResultsOnRight(ref fromResult, ref toResult);

        for (int i = fromResult; i < toResult; i++)
        {
            ExerciseResult tmpResult = _system.LoadResultFromJSON(_system.Files[i].FullName);
            _elementsText[i % _resultAmount].text = tmpResult.date + " - " + tmpResult.time;
        }
    }

    private void CheckResultsOnLeft(ref int fromResult, ref int toResult)
    {
        if (fromResult < 0)
        {
            _curFromResult = fromResult = (int)(_system.Files.Length / _resultAmount) * _resultAmount;
            toResult = fromResult + _resultAmount;

            CheckFilesEnd(ref toResult);
        }
    }

    private void CheckResultsOnRight(ref int fromResult, ref int toResult)
    {
        if (fromResult > _system.Files.Length)
        {
            _curFromResult = fromResult = 0;
            toResult = _resultAmount;
        }

        CheckFilesEnd(ref toResult);
    }

    private void CheckFilesEnd(ref int toResult)
    {
        if (toResult > _system.Files.Length)
        {
            for (int i = _system.Files.Length; i < toResult; i++)
            {
                _elementsText[i % _resultAmount].text = "";
            }
            toResult = _system.Files.Length;
        }
    }
}
