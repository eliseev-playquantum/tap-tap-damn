using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(InvertRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Invert", true)]
public sealed class Invert : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("Amount")]
    public FloatParameter amount = new FloatParameter { value = 1f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (amount == 0) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class InvertRenderer : PostProcessEffectRenderer<Invert>
{
    Shader InvertShader;

    public override void Init()
    {
        InvertShader = Shader.Find("Hidden/SC Post Effects/Invert");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(InvertShader);

        sheet.properties.SetFloat("_Amount", settings.amount);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif