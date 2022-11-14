using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class StencilOutline : MonoBehaviour
{

    [SerializeField]
    private Shader _shader;
    [SerializeField]
    private Color _outlineColor;

    private void Awake()
    {
        Initialize();
        Debug.Log("CAMERA");
    }

    private void Initialize()
    {
        var camera = GetComponent<Camera>();
        if (camera.allowMSAA || camera.allowHDR)
        {
            return;
        }
        var material = new Material(_shader);

        // アウトラインの色を適用
        material.SetColor("_OutlineColor", _outlineColor);

        // CommandBufferを登録
        var commandBuffer = new CommandBuffer();
        int tempTextureIdentifier = Shader.PropertyToID("_WorldPostEffectTempTexture");
        commandBuffer.GetTemporaryRT(tempTextureIdentifier, -1, -1);
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tempTextureIdentifier);
        commandBuffer.Blit(tempTextureIdentifier, BuiltinRenderTextureType.CurrentActive, material);
        commandBuffer.ReleaseTemporaryRT(tempTextureIdentifier);
        camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
    }
}