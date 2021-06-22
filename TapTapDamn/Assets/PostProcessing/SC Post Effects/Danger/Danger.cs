using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(DangerRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Danger", true)]
public sealed class Danger : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("Opacity")]
    public FloatParameter amount = new FloatParameter { value = 1f };

    [Range(0f, 1f), Tooltip("Size")]
    public FloatParameter size = new FloatParameter { value = 1f };

    [Range(0f, 1f), Tooltip("Refraction")]
    public FloatParameter refraction = new FloatParameter { value = 0.1f };

    public ColorParameter color = new ColorParameter { value = new Color(0.66f, 0f, 0f) };

    public TextureParameter overlayTex = new TextureParameter { value = null };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (size == 0) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class DangerRenderer : PostProcessEffectRenderer<Danger>
{
    Shader painShader;

    public override void Init()
    {
        painShader = Shader.Find("Hidden/SC Post Effects/Danger");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(painShader);

        sheet.properties.SetFloat("_Size", settings.size);
        sheet.properties.SetFloat("_Refraction", settings.refraction/10f);
        sheet.properties.SetColor("_Color", new Color(settings.color.value.r, settings.color.value.g, settings.color.value.b, settings.amount));
        if(settings.overlayTex.value) sheet.properties.SetTexture("_Overlay", settings.overlayTex);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif