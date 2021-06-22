using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(EdgeDetectionRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Edge Detection", true)]
public sealed class EdgeDetection : PostProcessEffectSettings
{
    public enum EdgeDetectMode
    {
        TriangleDepthNormals = 0,
        RobertsCrossDepthNormals = 1,
        SobelDepth = 2,
        SobelDepthThin = 3,
        TriangleLuminance = 4,
    }

    [Serializable]
    public sealed class EdgeDetectionMode : ParameterOverride<EdgeDetectMode> { }

    [DisplayName("Method"), Tooltip("")]
    public EdgeDetectionMode mode = new EdgeDetectionMode { value = EdgeDetectMode.TriangleDepthNormals };

    [DisplayName("Depth"), Range(0f, 10f), Tooltip("Sensitivity Depth")]
    public FloatParameter sensitivityDepth = new FloatParameter { value = 0f };

    [DisplayName("Normals"), Range(0f, 1f), Tooltip("Sensitivity Normals")]
    public FloatParameter sensitivityNormals = new FloatParameter { value = 1f };

    [Range(0.01f, 1f), Tooltip("Luminance Threshold")]
    public FloatParameter lumThreshold = new FloatParameter { value = 0.01f };

    [Range(1f, 50f), Tooltip("Edge Exponent")]
    public FloatParameter edgeExp = new FloatParameter { value = 1f };

    [DisplayName("Size"), Range(0, 3), Tooltip("Edge Distance")]
    public IntParameter edgeSize = new IntParameter { value = 1 };

    //bool
    [Range(0f, 1f), Tooltip("Edges Only")]
    public FloatParameter edgesOnly = new FloatParameter { value = 0f };

    [DisplayName("Color"), Tooltip("")]
    public ColorParameter edgeColor = new ColorParameter { value = Color.black };

    [DisplayName("Opacity"), Range(0f, 1f), Tooltip("Opacity")]
    public FloatParameter edgeOpacity = new FloatParameter { value = 1f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
           // if (gradient.value != null)
                return true;
        }

        return false;
    }
}

public sealed class EdgeDetectionRenderer : PostProcessEffectRenderer<EdgeDetection>
{

    Shader edgeDetectShader;

    public override void Init()
    {

        edgeDetectShader = Shader.Find("Hidden/SC Post Effects/Edge Detect");
        
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {
        //context.camera.depthTextureMode = DepthTextureMode.DepthNormals;
        var sheet = context.propertySheets.Get(edgeDetectShader);

        Vector2 sensitivity = new Vector2(settings.sensitivityDepth, settings.sensitivityNormals);
        sheet.properties.SetVector("_Sensitivity", sensitivity);
        sheet.properties.SetFloat("_BgFade", settings.edgesOnly);
        sheet.properties.SetFloat("_SampleDistance", settings.edgeSize);
        sheet.properties.SetFloat("_Exponent", settings.edgeExp);
        sheet.properties.SetFloat("_Threshold", settings.lumThreshold);
        sheet.properties.SetColor("_EdgeColor", settings.edgeColor);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
    }

    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.DepthNormals;
    }

}
#endif