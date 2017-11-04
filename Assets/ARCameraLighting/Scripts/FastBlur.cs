using UnityEngine;
using UnityEngine.Rendering;

// Mostly copied from the Unity Cinematic Effects blur
// Will optionally blend into the existing RenderTexture target rather then replacing the contents
//   allowing a smoother blur over time.
public class FastBlur : MonoBehaviour
{
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

    private Material blurMaterial = null;
    private Material blendMaterial = null;

    private void Start()
    {
        blurMaterial = Resources.Load<Material>("Materials/FastBlur");
        Debug.Assert(blurMaterial);
        blendMaterial = Resources.Load<Material>("Materials/Blend");
        Debug.Assert(blendMaterial);
    }


    public void CreateBlurCommandBuffer(CommandBuffer commandBuffer, RenderTargetIdentifier sourceID, int renderTextureWidth, int renderTextureHeight)
    {
		int rt1 = Shader.PropertyToID("BlurBuffer1");
		int rt2 = Shader.PropertyToID("BlurBuffer2");
		int rt = rt2;

        commandBuffer.SetGlobalVector("_Parameter", new Vector4(blurSize, -blurSize, 0.0f, 0.0f));
        commandBuffer.GetTemporaryRT(rt, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear);
        commandBuffer.Blit(sourceID, rt, blurMaterial, 0);

        var passOffs = blurType == BlurType.StandardGauss ? 0 : 2;

        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            commandBuffer.SetGlobalVector("_Parameter", new Vector4(blurSize + iterationOffs, -blurSize - iterationOffs, 0.0f, 0.0f));

            // vertical blur
            commandBuffer.GetTemporaryRT(rt1, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear);
            commandBuffer.Blit(rt, rt1, blurMaterial, 1 + passOffs);
            commandBuffer.ReleaseTemporaryRT(rt);
            rt = rt1;

            // horizontal blur
            commandBuffer.GetTemporaryRT(rt2, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear);
            commandBuffer.Blit(rt, rt2, blurMaterial, 2 + passOffs);
            commandBuffer.ReleaseTemporaryRT(rt);
            rt = rt2;
        }

        commandBuffer.Blit(rt, sourceID, blurMaterial);
        commandBuffer.ReleaseTemporaryRT(rt);
    }
}
