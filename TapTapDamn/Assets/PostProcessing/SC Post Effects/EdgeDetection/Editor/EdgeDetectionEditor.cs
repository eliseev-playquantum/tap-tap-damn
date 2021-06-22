using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

[PostProcessEditor(typeof(EdgeDetection))]
public sealed class EdgeDetectionEditor : PostProcessEffectEditor<EdgeDetection>
{
    SerializedParameterOverride mode;
    SerializedParameterOverride sensitivityDepth;
    SerializedParameterOverride sensitivityNormals;
    SerializedParameterOverride lumThreshold;
    SerializedParameterOverride edgeExp;
    SerializedParameterOverride sampleDist;
    SerializedParameterOverride edgesOnly;
    SerializedParameterOverride edgeColor;
    SerializedParameterOverride edgeOpacity;

    private static bool showHelp;

    public override void OnEnable()
    {
        mode = FindParameterOverride(x => x.mode);
        sensitivityDepth = FindParameterOverride(x => x.sensitivityDepth);
        sensitivityNormals = FindParameterOverride(x => x.sensitivityNormals);
        lumThreshold = FindParameterOverride(x => x.lumThreshold);
        edgeExp = FindParameterOverride(x => x.edgeExp);
        sampleDist = FindParameterOverride(x => x.edgeSize);
        edgesOnly = FindParameterOverride(x => x.edgesOnly);
        edgeColor = FindParameterOverride(x => x.edgeColor);
        edgeOpacity = FindParameterOverride(x => x.edgeOpacity);
    }

    public override void OnInspectorGUI()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            showHelp = GUILayout.Toggle((showHelp) ? showHelp : false, SCPE_GUI.HelpIcon, SCPE_GUI.HelpButtonStyle);
        }

        //If the color is overriden, als override the edge opacity
        edgeOpacity.overrideState.boolValue = (edgeColor.overrideState.boolValue == true) ? true : false;

        EditorUtilities.DrawHeaderLabel("Solver");
        PropertyField(mode);
        if (showHelp)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            switch (mode.value.intValue)
            {
                case 0:
                    EditorGUILayout.HelpBox("Checks the differences between geometry normals and their distance from the camera", MessageType.None);
                    break;
                case 1:
                    EditorGUILayout.HelpBox("Same as Triangle Depth Normals but uses an additional sample for accuracy", MessageType.None);
                    break;
                case 2:
                    EditorGUILayout.HelpBox("Draws edges only where neighboring pixels greatly differ in their depth value.\n\nSame method as used in Borderlands", MessageType.None);
                    break;
                case 3:
                    EditorGUILayout.HelpBox("Draws edges only where neighboring pixels greatly differ in their depth value.\n\nSame method as used in Borderlands", MessageType.None);
                    break;
                case 4:
                    EditorGUILayout.HelpBox("Creates a edge where the luminance value of a pixel differs from it's neighbors past the threshold", MessageType.Info);
                    break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (mode.value.intValue < 2)
        {
            EditorUtilities.DrawHeaderLabel("Sensitivity");
            PropertyField(sensitivityDepth);
            PropertyField(sensitivityNormals);
        }
        else if (mode.value.intValue < 4)
        {
            PropertyField(edgeExp);
        }
        else
        {
            // lum based mode
            PropertyField(lumThreshold);
        }

        EditorUtilities.DrawHeaderLabel("Edges");
        PropertyField(edgeColor);
        PropertyField(edgeOpacity);
        PropertyField(sampleDist);

        EditorUtilities.DrawHeaderLabel("Debug");
        PropertyField(edgesOnly);

        //Store edge opacity value in the color's alpha channel
        edgeColor.value.colorValue = new Color(edgeColor.value.colorValue.r, edgeColor.value.colorValue.g, edgeColor.value.colorValue.b, edgeOpacity.value.floatValue);
    }
}
#endif