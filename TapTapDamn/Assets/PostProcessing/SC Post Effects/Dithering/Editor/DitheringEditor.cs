using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

[PostProcessEditor(typeof(Dithering))]
public sealed class DitheringEditor : PostProcessEffectEditor<Dithering>
{
    SerializedParameterOverride size;
    SerializedParameterOverride luminanceThreshold;
    SerializedParameterOverride intensity;

    public override void OnEnable()
    {
        size = FindParameterOverride(x => x.size);
        luminanceThreshold = FindParameterOverride(x => x.luminanceThreshold);
        intensity = FindParameterOverride(x => x.intensity);
    }

    public override void OnInspectorGUI()
    {
        PropertyField(size);
        PropertyField(luminanceThreshold);
        PropertyField(intensity);
    }
}
#endif