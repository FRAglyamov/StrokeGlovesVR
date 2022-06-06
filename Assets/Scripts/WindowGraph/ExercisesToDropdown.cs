using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Get names of exercises based on folder names from saves' directory.
/// Change dropdown options to these names and add a call to change the graph's exercise to selected.
/// </summary>
public class ExercisesToDropdown : MonoBehaviour
{

    private Dropdown _dropdown = null;
    private Dictionary<string, string> _nameFullPath = new Dictionary<string, string>();
    [SerializeField]
    private WindowGraph windowGraph;
    private ProgressSystem _progressSystem;

    private void Start()
    {
        _dropdown = GetComponent<Dropdown>();
        // TODO: Integration with Assistant System, VR. Or make as standalone scene?
        UpdateDropdown();
        _progressSystem = AssistantSystem.Instance.progressSystem;
    }

    public void UpdateDropdown()
    {
        var SavePath = Path.Combine(Application.persistentDataPath, "progress_saves", _progressSystem.userID);
        DirectoryInfo dir = new DirectoryInfo(SavePath);
        DirectoryInfo[] info = dir.GetDirectories();

        _dropdown.ClearOptions();
        _nameFullPath.Clear();
        foreach (DirectoryInfo f in info)
        {
            if (f.EnumerateFiles().Any())
            {
                _dropdown.options.Add(new Dropdown.OptionData() { text = f.Name });
                _nameFullPath.Add(f.Name, f.FullName);
            }
        }
        _dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(); });
        
        _dropdown.captionText.text = _dropdown.options[0].text;
        string firstOptionPath = _nameFullPath[_dropdown.options[0].text];
        windowGraph.ChangeExercise(firstOptionPath);
    }

    void DropdownItemSelected()
    {
        int value = _dropdown.value;
        windowGraph.ChangeExercise(_nameFullPath[_dropdown.options[value].text]);
    }
}
