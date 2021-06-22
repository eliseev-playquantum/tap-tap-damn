using UnityEditor;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;

[PostProcessEditor(typeof(FogGradient))]
public sealed class FogGradientEditor : PostProcessEffectEditor<FogGradient>
{
    //SerializedParameterOverride m_ExcludeSkybox;
    SerializedParameterOverride m_Blend;
    SerializedParameterOverride m_MaxDistance;
    SerializedParameterOverride m_GradientTex;
    SerializedParameterOverride m_Gradient;

    public override void OnEnable()
    {
        //m_ExcludeSkybox = FindParameterOverride(x => x.excludeSkybox);
        m_Blend = FindParameterOverride(x => x.blend);
        m_MaxDistance = FindParameterOverride(x => x.maxDistance);
        m_GradientTex = FindParameterOverride(x => x.gradientTex);
        //m_Gradient = FindParameterOverride(x => x.gradient);
    }

    public override void OnInspectorGUI()
    {
        //PropertyField(m_Gradient);
        PropertyField(m_GradientTex);

        if (m_GradientTex.value.objectReferenceValue)
        {
            EditorGUILayout.Space();

            Rect rect = GUILayoutUtility.GetRect(6, 18, "TextField");
            EditorGUI.DrawPreviewTexture(rect, m_GradientTex.value.objectReferenceValue as Texture2D);
        }
        GUILayout.Space(5f);

       // PropertyField(m_ExcludeSkybox);
        
        PropertyField(m_MaxDistance);



        PropertyField(m_Blend);

    }

}
#endif