using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ColorWashingRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Color Washing", true)]
public sealed class ColorWashing : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Intensity")]
    public FloatParameter intensity = new FloatParameter { value = 0.33f };

    [Range(0f, 1f), Tooltip("Speed")]
    public FloatParameter speed = new FloatParameter { value = 0.3f };

    [Range(0f, 3f), Tooltip("Size")]
    public FloatParameter size = new FloatParameter { value = 1f };

    [Range(0f, 10f), Tooltip("Geometry influence")]
    public FloatParameter geoInfluence = new FloatParameter { value = 5f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (intensity == 0) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class ColorWashingRenderer : PostProcessEffectRenderer<ColorWashing>
{
    Shader shader;

    public override void Init()
    {
        shader = Shader.Find("Hidden/SC Post Effects/Color Washing");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(shader);

        sheet.properties.SetVector("_Params", new Vector4(settings.speed, settings.size, settings.geoInfluence, settings.intensity));

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.DepthNormals;
    }

}
#endif