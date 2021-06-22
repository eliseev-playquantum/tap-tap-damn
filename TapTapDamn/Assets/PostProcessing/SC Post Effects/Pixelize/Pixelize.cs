using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PixelizeRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Pixelize", true)]
public sealed class Pixelize : PostProcessEffectSettings
{

    [Range(1, 2048), Tooltip("Resolution")]
    public IntParameter resolution = new IntParameter { value = 270 };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (resolution == 2048) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class PixelizeRenderer : PostProcessEffectRenderer<Pixelize>
{
    Shader pixelizeShader;

    public override void Init()
    {
        pixelizeShader = Shader.Find("Hidden/SC Post Effects/Pixelize");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(pixelizeShader);

        sheet.properties.SetFloat("_Resolution", 1f / settings.resolution);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif