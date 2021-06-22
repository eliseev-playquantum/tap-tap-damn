using UnityEngine;
using UnityEditor;

public class LocalizationEditor
{

    [MenuItem("Tools/Open Localization Folder")]
    private static void Open()
    {
        Application.OpenURL(Application.dataPath + "/Project/_Scripts/System/Localization/Resources");
    }

}
