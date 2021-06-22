using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(DoubleVisionRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Double Vision", true)]
public sealed class DoubleVision : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("Amount")]
    public FloatParameter amount = new FloatParameter { value = 0.1f };

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

public sealed class DoubleVisionRenderer : PostProcessEffectRenderer<DoubleVision>
{
    Shader DoubleVisionShader;

    public override void Init()
    {
        DoubleVisionShader = Shader.Find("Hidden/SC Post Effects/Double Vision");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(DoubleVisionShader);

        sheet.properties.SetFloat("_Amount", settings.amount /10);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif