using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public sealed class GradientParameter : ParameterOverride<Gradient> { }

[Serializable]
[PostProcess(typeof(FogGradientRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Fog Gradient", true)]
public sealed class FogGradient : PostProcessEffectSettings
{
    //Not yet implemented
    //public BoolParameter excludeSkybox = new BoolParameter { value = false };

    //[DisplayName("Gradient"), Tooltip("")]
    //public GradientParameter gradient = new GradientParameter { value = null };

    [DisplayName("Gradient Texture"), Tooltip("")]
    public TextureParameter gradientTex = new TextureParameter { value = null };

    [Range(1f, 10f), Tooltip("Max distance")]
    public FloatParameter maxDistance = new FloatParameter { value = 1f };

    [Range(0f, 1f), Tooltip("Effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 1f };

    private Texture2D m_gradientTex;

    private const int resolution = 64;

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (gradientTex.value != null || RenderSettings.fog == false)
                return true;
        }

        return false;
    }

}

public sealed class FogGradientRenderer : PostProcessEffectRenderer<FogGradient>
{

    Shader shader;

    public override void Init()
    {
        shader = Shader.Find("Hidden/SC Post Effects/Fog Gradient");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(shader);

        if(settings.gradientTex.value) sheet.properties.SetTexture("_Gradient", settings.gradientTex);
        sheet.properties.SetFloat("_Blend", settings.blend);

        float maxDistance = (context.isSceneView) ? settings.maxDistance / 10f : settings.maxDistance;
        sheet.properties.SetFloat("_MaxDistance", maxDistance+0.1f);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.Depth;
    }


}
#endif