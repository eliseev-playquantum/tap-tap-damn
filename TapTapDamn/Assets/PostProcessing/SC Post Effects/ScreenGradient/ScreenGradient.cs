using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ScreenGradientRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Gradient", true)]
public sealed class ScreenGradient : PostProcessEffectSettings
{

    //[DisplayName("Gradient"), Tooltip("")]
    //public GradientParameter gradient = new GradientParameter { value = null };

    [DisplayName("Gradient Texture"), Tooltip("")]
    public TextureParameter gradientTex = new TextureParameter { value = null };

    [Range(0f,1f)]
    public FloatParameter intensity = new FloatParameter { value = 1f };

    private const int RESOLUTION = 64;

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (enabled.value)
        {
            if (gradientTex.value == null) return false;
            return true;
        }

        return false;
    }

    //Converting a gradient to a texture currently breaks volume blending

    /*
    public Texture2D m_gradientTex;

    public Texture2D GenerateGradient(Gradient gradient)
    {
        if (this.gradient.overrideState == false) return null;
        Debug.Log("Converting gradient to texture");

        //Create texture first time
        if (!m_gradientTex)
        {
            m_gradientTex = new Texture2D(RESOLUTION, 1, TextureFormat.ARGB32, false)
            {
                //Smooth interpolation
                filterMode = FilterMode.Bilinear
            };
        }

        Color gradientPixel;

        for (int x = 0; x < RESOLUTION; x++)
        {
            gradientPixel = gradient.Evaluate(x / (float)RESOLUTION);
            m_gradientTex.SetPixel(x, 1, gradientPixel);
        }

        m_gradientTex.Apply();

        return m_gradientTex;
    }
    */
}

public sealed class ScreenGradientRenderer : PostProcessEffectRenderer<ScreenGradient>
{
    Shader gradientShader;

    public override void Init()
    {
        gradientShader = Shader.Find("Hidden/SC Post Effects/ScreenGradient");
        //settings.gradient.value = new Gradient();
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(gradientShader);

        //This should be editor inspector only, but that's not possible currently
        //Texture2D gradientTexture = settings.GenerateGradient(settings.gradient.value);
        //if(settings.gradient.value.colorKeys.Length > 0) settings.gradientTex.value = settings.GenerateGradient(settings.gradient);

        if (settings.gradientTex.value) sheet.properties.SetTexture("_Gradient", settings.gradientTex);
        sheet.properties.SetFloat("_Intensity", settings.intensity);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

}
#endif