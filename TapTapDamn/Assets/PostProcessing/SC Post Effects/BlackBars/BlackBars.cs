using System;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(BlackBarsRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Black Bars", true)]
public sealed class BlackBars : PostProcessEffectSettings
{
    public enum Direction
    {
        Horizontal = 0,
        Vertical = 1,
    }

    [Serializable]
    public sealed class DirectionParam : ParameterOverride<Direction> { }

    [DisplayName("Direction"), Tooltip("")]
    public DirectionParam mode = new DirectionParam { value = Direction.Horizontal };

    [Range(0f, 1f), Tooltip("Size")]
    public FloatParameter size = new FloatParameter { value = 1f };

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

public sealed class BlackBarsRenderer : PostProcessEffectRenderer<BlackBars>
{
    Shader shader;

    public override void Init()
    {
        shader = Shader.Find("Hidden/SC Post Effects/Black Bars");
    }

    public override void Release()
    {
        base.Release();
    }

    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(shader);

        sheet.properties.SetFloat("_Size", settings.size/10f);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
    }

}
#endif