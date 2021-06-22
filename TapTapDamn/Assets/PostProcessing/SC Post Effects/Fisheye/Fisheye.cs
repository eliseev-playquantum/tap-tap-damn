using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(FisheyeRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Fisheye", true)]
public sealed class Fisheye : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Strength")]
    public FloatParameter strength = new FloatParameter { value = 1f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (strength == 0) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class FisheyeRenderer : PostProcessEffectRenderer<Fisheye>
{
    Shader fisheyeShader;

    public override void Init()
    {
        fisheyeShader = Shader.Find("Hidden/Custom/Fisheye");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(fisheyeShader);

        sheet.properties.SetFloat("_Strength", 1-settings.strength-1);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif
