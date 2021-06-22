using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(SharpenRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Sharpen", true)]
public sealed class Sharpen : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("Amount")]
    public FloatParameter amount = new FloatParameter { value = 0.5f };

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

public sealed class SharpenRenderer : PostProcessEffectRenderer<Sharpen>
{
    Shader sharpenShader;

    public override void Init()
    {
        sharpenShader = Shader.Find("Hidden/SC Post Effects/Sharpen");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(sharpenShader);

        sheet.properties.SetFloat("_Amount", settings.amount);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif