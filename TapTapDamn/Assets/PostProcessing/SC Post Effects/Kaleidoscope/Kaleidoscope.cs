using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(KaleidoscopeRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Kaleidoscope", true)]
public sealed class Kaleidoscope : PostProcessEffectSettings
{

    [Range(0f, 10f), Tooltip("Offset")]
    public IntParameter splits = new IntParameter { value = 5 };


    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (splits == 0) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class KaleidoscopeRenderer : PostProcessEffectRenderer<Kaleidoscope>
{
    Shader shader;

    public override void Init()
    {
        shader = Shader.Find("Hidden/SC Post Effects/Kaleidoscope");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(shader);

        float splits = Mathf.PI * 2 / Mathf.Max(1, settings.splits);
        sheet.properties.SetFloat("_Splits", splits);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif