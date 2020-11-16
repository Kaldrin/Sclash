using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(KuwaharaRenderer), PostProcessEvent.AfterStack, "Custom/Kuwahara")]
public sealed class Kuwahara : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Kuwahara effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
    public IntParameter kernelSize = new IntParameter { value = 7 };
    public IntParameter powX = new IntParameter { value = 2 };
    public IntParameter powY = new IntParameter { value = 2 };
    public FloatParameter spread = new FloatParameter { value = 5.0f };


    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        return enabled.value
            && blend.value > 0f;
    }
}

public sealed class KuwaharaRenderer : PostProcessEffectRenderer<Kuwahara>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Kuwahara"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        sheet.properties.SetFloat("_KernelSize", settings.kernelSize);
        sheet.properties.SetFloat("_PowX", settings.powX);
        sheet.properties.SetFloat("_PowY", settings.powY);
        sheet.properties.SetFloat("_Spread", settings.spread);


        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}