using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ExercisesToDropdown : MonoBehaviour
{

    Dropdown dropdown = null;
    [SerializeField]
    private WindowGraph windowGraph = null;
    Dictionary<string, string> nameFullPath = new Dictionary<string, string>();
    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        var SavePath = Path.Combine(Application.persistentDataPath, "progress_saves");

        DirectoryInfo dir = new DirectoryInfo(SavePath);
        DirectoryInfo[] info = dir.GetDirectories();

        dropdown.ClearOptions();
        foreach (DirectoryInfo f in info)
        {
            //Debug.Log(f.ToString());
            //Debug.Log(f.Name);
            dropdown.options.Add(new Dropdown.OptionData() { text = f.Name });
            nameFullPath.Add(f.Name, f.FullName);
        }
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(); });

    }
    void DropdownItemSelected()
    {
        int value = dropdown.value;
        Debug.Log(dropdown.options[value].text);
        windowGraph.ChangeExercise(nameFullPath[dropdown.options[value].text]);
    }
}
