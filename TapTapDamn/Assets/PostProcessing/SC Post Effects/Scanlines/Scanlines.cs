using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ScanlinesRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Scanlines", true)]
public sealed class Scanlines : PostProcessEffectSettings
{

    [Range(0f, 2048f), Tooltip("Amount X")]
    public FloatParameter amountHorizontal = new FloatParameter { value = 700 };


    [Range(0f, 1f), Tooltip("Intensity")]
    public FloatParameter intensity = new FloatParameter { value = 0.5f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (intensity.value == 0) return false;
            return true;
        }

        return false;
    }
}

public sealed class ScanlinesRenderer : PostProcessEffectRenderer<Scanlines>
{
    Shader ScanlinesShader;

    public override void Init()
    {
        ScanlinesShader = Shader.Find("Hidden/SC Post Effects/Scanlines");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(ScanlinesShader);

        sheet.properties.SetVector("_Params", new Vector4(settings.amountHorizontal, settings.intensity / 1000, 0, 0));

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif