using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(OverlayRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Overlay", true)]
public sealed class Overlay : PostProcessEffectSettings
{
    public TextureParameter overlayTex = new TextureParameter { value = null };

    [Range(0f, 1f)]
    public FloatParameter intensity = new FloatParameter { value = 1f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (overlayTex.value == null) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class OverlayRenderer : PostProcessEffectRenderer<Overlay>
{
    Shader shader;

    public override void Init()
    {
        shader = Shader.Find("Hidden/SC Post Effects/Overlay");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(shader);

        if (settings.overlayTex.value) sheet.properties.SetTexture("_OverlayTex", settings.overlayTex);
        sheet.properties.SetFloat("_Intensity", settings.intensity);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif