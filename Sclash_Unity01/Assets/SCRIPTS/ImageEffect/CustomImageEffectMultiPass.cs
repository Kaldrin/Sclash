using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CustomImageEffectMultiPass : MonoBehaviour
{
    [Tooltip("If disabled, shaders that sue the depth texture will give weird and probably unwanted results, if not a black screen")]
    [SerializeField] bool enableDepthTexture = true;
    [Tooltip("Materials will be applied to the image in the order they are referenced here, be careful")]
    [SerializeField] Material material = null;










    private void Start()
    {
        // Enable depth texture on the attached camera
        if (enableDepthTexture)
        {
            Camera camera = GetComponent<Camera>();
            camera.depthTextureMode = DepthTextureMode.Depth;
        }
    }


    protected void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        // Set wrap mode for blur shaders
        src.wrapMode = TextureWrapMode.Clamp;


        // Get temporary work texture
        RenderTexture tmp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        RenderTexture tmp2 = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        tmp = src;
        tmp2 = src;




        
        if (material.passCount > 1)
        {
            for (int i = 0; i < material.passCount; i++)
            {
                if (i == material.passCount - 1)
                    Graphics.Blit(tmp, dst, material, i);
                else
                {
                    Graphics.Blit(tmp, tmp2, material, i);
                    Graphics.Blit(tmp2, tmp);
                }
            }
        }
        else
            Graphics.Blit(src, dst, material);




        // Release memory
        RenderTexture.ReleaseTemporary(tmp);
        RenderTexture.ReleaseTemporary(tmp2);
    }
}
