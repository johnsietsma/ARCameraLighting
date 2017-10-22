using UnityEngine;
using UnityEngine.Rendering;

// Mostly copied from the Unity Cinematic Effects blur
// Will optionally blend into the existing RenderTexture target rather then replacing the contents
//   allowing a smoother blur over time.
public class FastBlur : MonoBehaviour
{
    [Range(0, 3)]
    public int downsample = 1;

    public enum BlurType
    {
        StandardGauss = 0,
        SgxGauss = 1,
    }

    [Range(0.0f, 10.0f)]
    public float blurSize = 3.0f;

    [Range(1, 4)]
    public int blurIterations = 2;

    public BlurType blurType = BlurType.StandardGauss;

    [HideInInspector]
    public RenderTexture blurredTexture;

    private Material blurMaterial = null;
    private Material blendMaterial = null;

    private int renderTextureWidth;
    private int renderTextureHeight;

    private void Start()
    {
        renderTextureWidth = Screen.width >> downsample;
        renderTextureHeight = Screen.height >> downsample;

        blurMaterial = Resources.Load<Material>("Materials/Fast Blur");
        blendMaterial = Resources.Load<Material>("Materials/Blend");

        blurredTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 0);
    }


    public void BlurCommandBuffer(CommandBuffer commandBuffer, RenderTexture source)
    {
        float widthMod = 1.0f / (1.0f * (1 << downsample));

        commandBuffer.SetGlobalVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
        source.filterMode = FilterMode.Bilinear;

        int renderTextureWidth = source.width >> downsample;
        int renderTextureHeight = source.height >> downsample;

        // downsample
        int rt = Shader.PropertyToID("BlurDownsample");
        commandBuffer.GetTemporaryRT(rt, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear, source.format);
        commandBuffer.Blit(source, rt, blurMaterial, 0);

        var passOffs = blurType == BlurType.StandardGauss ? 0 : 2;

        int rt1 = Shader.PropertyToID("BlurBuffer1");
        int rt2 = Shader.PropertyToID("BlurBuffer2");


        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            commandBuffer.SetGlobalVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

            // vertical blur
            commandBuffer.GetTemporaryRT(rt1, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear, source.format);
            commandBuffer.Blit(rt, rt1, blurMaterial, 1 + passOffs);
            commandBuffer.ReleaseTemporaryRT(rt);
            rt = rt1;

            // horizontal blur
            commandBuffer.GetTemporaryRT(rt2, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear, source.format);
            commandBuffer.Blit(rt, rt2, blurMaterial, 2 + passOffs);
            commandBuffer.ReleaseTemporaryRT(rt);
            rt = rt2;
        }

        commandBuffer.Blit(rt, blurredTexture, blendMaterial);
        commandBuffer.ReleaseTemporaryRT(rt);
    }
}
