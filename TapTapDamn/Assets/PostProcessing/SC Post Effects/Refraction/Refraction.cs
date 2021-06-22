using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RefractionRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Refraction", true)]
public sealed class Refraction : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("Amount")]
    public FloatParameter amount = new FloatParameter { value = 1f };

    public TextureParameter refractionTex = new TextureParameter { value = null };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (amount == 0 || refractionTex.value == null) { return false; }
            return true;
        }

        return false;
    }
}

public sealed class RefractionRenderer : PostProcessEffectRenderer<Refraction>
{
    Shader RefractionShader;

    public override void Init()
    {
        RefractionShader = Shader.Find("Hidden/SC Post Effects/Refraction");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(RefractionShader);

        sheet.properties.SetFloat("_Amount", settings.amount);
        if (settings.refractionTex.value) sheet.properties.SetTexture("_RefractionTex", settings.refractionTex);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif