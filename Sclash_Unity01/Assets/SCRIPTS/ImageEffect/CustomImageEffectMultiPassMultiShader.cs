using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CustomImageEffectMultiPassMultiShader : MonoBehaviour
{
    [Header("PARAMETERS")]
    [Tooltip("If disabled, shaders that sue the depth texture will give weird and probably unwanted results, if not a black screen")]
    [SerializeField] bool enableDepthTexture = true;
    [SerializeField] bool useSrcTexture = true;
    [SerializeField] string srcTextureName = "_SrcTex";
    [SerializeField] int materialIndexToUseSrcTexFor = 1;
    [SerializeField] int downSampleAmount = 2;
    [SerializeField] int upSampleAmount = 1;

    [Header("RANDOM VALUE")]
    [SerializeField] bool useRandomValue = true;
    [SerializeField] int materialToUseRandomValueFor = 0;
    [SerializeField] Vector2 randomValueRange = new Vector2(-2, 5);
    [SerializeField] float randomValueOscillationPeriod = 0.5f;
    float randomValue = 0;
    float lastRandomValue = 0;
    float randomValueStartTime = 0;




    [Header("MATERIALS")]
    [Tooltip("Materials will be applied to the image in the order they are referenced here, be careful")]
    [SerializeField] List<Material> materials = new List<Material>();










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

        src.wrapMode = TextureWrapMode.Repeat;



        if (useSrcTexture)
        {
            if (materials.Count > materialIndexToUseSrcTexFor)
            {
                try
                {
                    materials[materialIndexToUseSrcTexFor].SetTexture(srcTextureName, src);
                }
                catch
                {
                    Debug.Log("Image effect material does not have a property named " + srcTextureName + ", ignoring");
                }
            }
            else
                Debug.Log("The index for the material that will use the src texture is invalid");
        }



        // Random value
        if (useRandomValue && materials.Count > materialToUseRandomValueFor)
        {
            if (Time.time - randomValueStartTime > randomValueOscillationPeriod)
            {
                randomValue = Random.Range(randomValueRange.x, randomValueRange.y);
                randomValueStartTime = Time.time;
            }


            materials[materialToUseRandomValueFor].SetFloat("_RandomValue", randomValue);
        }




        // Get temporary work texture
        RenderTexture tmp = RenderTexture.GetTemporary((src.width / downSampleAmount) * upSampleAmount, (src.height / downSampleAmount) * upSampleAmount, 0, src.format);
        RenderTexture tmp2 = RenderTexture.GetTemporary((src.width / downSampleAmount) * upSampleAmount, (src.height / downSampleAmount) * upSampleAmount, 0, src.format);
        if (downSampleAmount > 1)
        {
            tmp.filterMode = FilterMode.Point;
            tmp2.filterMode = FilterMode.Point;
        }
        if (upSampleAmount > 1)
        {
            tmp.filterMode = FilterMode.Bilinear;
            tmp2.filterMode = FilterMode.Bilinear;
        }
        Graphics.Blit(src, tmp);




        
        // Blit with all materials (Applying post process)
        if (materials != null)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                for (int o = 0; o < materials[i].passCount; o++)
                {
                    Graphics.Blit(tmp, tmp2, materials[i], o);
                    Graphics.Blit(tmp2, tmp);
                }
            }
        }
        else
            Debug.Log("No image effect material referenced, ignoring");
        

        // Apply result to destination texture
        Graphics.Blit(tmp, dst);


        // Release memory
        RenderTexture.ReleaseTemporary(tmp);
        RenderTexture.ReleaseTemporary(tmp2);
    }
}
