using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PosterizeRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Posterize", true)]
public sealed class Posterize : PostProcessEffectSettings
{

    [Range(0.1f, 8f), Tooltip("Color depth")]
    public FloatParameter depth = new FloatParameter { value = 4f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (depth == 8) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class PosterizeRenderer : PostProcessEffectRenderer<Posterize>
{
    Shader PosterizeShader;

    public override void Init()
    {
        PosterizeShader = Shader.Find("Hidden/SC Post Effects/Posterize");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(PosterizeShader);

        sheet.properties.SetFloat("_Depth", settings.depth);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif