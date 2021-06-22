using System;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(LensflaresRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Lens Flares (Expiremental)", true)]
public sealed class Lensflares : PostProcessEffectSettings
{
    public BoolParameter debug = new BoolParameter { value = false };

    [Header("Anamorphic Lensfares")]
    [Range(0f, 1f), Tooltip("Intensity")]
    public FloatParameter intensity = new FloatParameter { value = 0.5f };

    [Range(0.01f, 1f), Tooltip("Luminance threshold")]
    public FloatParameter luminanceThreshold = new FloatParameter { value = 0.05f };

    [Header("Blur")]
    [Range(0f, 10f), Tooltip("Blur")]
    public FloatParameter blur = new FloatParameter { value = 1f };

    [Range(1, 8), Tooltip("Itterations")]
    public IntParameter itterations = new IntParameter { value = 2 };

    [Range(1, 8), Tooltip("Downsampling")]
    public IntParameter downsamples = new IntParameter { value = 1 };
}

public sealed class LensflaresRenderer : PostProcessEffectRenderer<Lensflares>
{
    Shader shader;
    private int emissionTex;
    RenderTexture aoRT;

    public override void Init()
    {
        shader = Shader.Find("Hidden/SC Post Effects/Lens Flares");
        emissionTex = Shader.PropertyToID("_BloomTex");
    }

    public override void Release()
    {
        base.Release();
    }

    enum Pass
    {
        LuminanceDiff,
        Blur,
        Blend,
        Debug
    }


    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(shader);
        CommandBuffer cmd = context.command;

        //float luminanceThreshold = (context.isSceneView) ? settings.luminanceThreshold / 20f : settings.luminanceThreshold;
        float luminanceThreshold = Mathf.GammaToLinearSpace(settings.luminanceThreshold.value);

        sheet.properties.SetFloat("_Threshold", luminanceThreshold);
        sheet.properties.SetFloat("_Blur", settings.blur);
        sheet.properties.SetFloat("_Intensity", settings.intensity);

        // Create RT for storing edge detection in
        context.command.GetTemporaryRT(emissionTex, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat);

        //Luminance difference check on RT
        context.command.BlitFullscreenTriangle(context.source, emissionTex, sheet, (int)Pass.LuminanceDiff);  

        // get two smaller RTs
        int blurredID = Shader.PropertyToID("_Temp1");
        int blurredID2 = Shader.PropertyToID("_Temp2");
        cmd.GetTemporaryRT(blurredID, context.screenWidth / settings.downsamples, context.screenHeight / settings.downsamples, 0, FilterMode.Bilinear);
        cmd.GetTemporaryRT(blurredID2, context.screenWidth / settings.downsamples, context.screenHeight / settings.downsamples, 0, FilterMode.Bilinear);

        //Pass AO into blur target texture
        cmd.Blit(emissionTex, blurredID);

        for (int i = 0; i < settings.itterations; i++)
        {
            // horizontal blur
            cmd.SetGlobalVector("_BlurOffsets", new Vector4((settings.blur ) / context.screenWidth, 0, 0, 0));
            context.command.BlitFullscreenTriangle(blurredID, blurredID2, sheet, (int)Pass.Blur); 

            // vertical blur
            cmd.SetGlobalVector("_BlurOffsets", new Vector4((settings.blur * 2f) / context.screenWidth, 0, 0, 0));
            context.command.BlitFullscreenTriangle(blurredID2, blurredID, sheet, (int)Pass.Blur);
        }

        context.command.SetGlobalTexture("_BloomTex", blurredID);

        //Blend AO tex with image
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (settings.debug) ? (int)Pass.Debug : (int)Pass.Blend);

        // release
        context.command.ReleaseTemporaryRT(blurredID);
        context.command.ReleaseTemporaryRT(blurredID2);
        context.command.ReleaseTemporaryRT(emissionTex);
    }
}
#endif