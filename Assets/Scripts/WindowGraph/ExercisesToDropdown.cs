using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ExercisesToDropdown : MonoBehaviour
{
    [SerializeField]
    private WindowGraph windowGraph = null;

    private Dropdown _dropdown = null;
    private Dictionary<string, string> _nameFullPath = new Dictionary<string, string>();

    private void Start()
    {
        _dropdown = GetComponent<Dropdown>();
        var SavePath = Path.Combine(Application.persistentDataPath, "progress_saves");

        DirectoryInfo dir = new DirectoryInfo(SavePath);
        DirectoryInfo[] info = dir.GetDirectories();

        _dropdown.ClearOptions();
        foreach (DirectoryInfo f in info)
        {
            _dropdown.options.Add(new Dropdown.OptionData() { text = f.Name });
            _nameFullPath.Add(f.Name, f.FullName);
        }
        _dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(); });
    }

    void DropdownItemSelected()
    {
        int value = _dropdown.value;
        windowGraph.ChangeExercise(_nameFullPath[_dropdown.options[value].text]);
    }
}
