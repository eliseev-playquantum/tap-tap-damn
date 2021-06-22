using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class SCPE_GUI : Object {

    private static GUIStyle _HelpButtonStyle;
    public static GUIStyle HelpButtonStyle
    {
        get
        {
            if (_HelpButtonStyle == null)
            {
                _HelpButtonStyle = new GUIStyle(EditorStyles.miniButton)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Normal,
                    fixedWidth = 30,
                    fixedHeight = 20f,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            return _HelpButtonStyle;
        }
    }
    private static Texture _HelpIcon;
    public static Texture HelpIcon
    {
        get
        {
            if (_HelpIcon == null)
            {
                _HelpIcon = EditorGUIUtility.FindTexture("d_UnityEditor.InspectorWindow");
            }

            return _HelpIcon;
        }
    }

}
#endif