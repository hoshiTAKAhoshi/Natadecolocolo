using UnityEngine;
using TMPro;
using System.Globalization;
using UnityEditor;

[ExecuteInEditMode]//ExecuteInEditModeを付ける事でOnEnableが再生していなくても実行されるようになる
public class TextMeshProTilt : MonoBehaviour
{
    [SerializeField] private float m_tilt;
    [SerializeField] private float m_add_height;
    [SerializeField] private float m_add_width;

    [SerializeField] private float m_chara_space;
    private TMP_Text m_textComponent;
    //private TMP_TextInfo textInfo;

    private void OnEnable()
    {
        if (this.m_textComponent == null)
            this.m_textComponent = GetComponent<TMP_Text>();
        UpdateAnimation();
    }

    private void Start()
    {
        if (this.m_textComponent == null)
            this.m_textComponent = GetComponent<TMP_Text>();

        UpdateAnimation();

    }

    private void Update()
    {
        if (this.m_textComponent == null)
            this.m_textComponent = GetComponent<TMP_Text>();

        //m_textComponent.characterSpacing = m_chara_space;

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // ① メッシュを再生成する（リセット）
        this.m_textComponent.ForceMeshUpdate(true);
        var textInfo = m_textComponent.textInfo;

        // ②頂点データを編集した配列の作成
        var count = Mathf.Min(textInfo.characterCount, textInfo.characterInfo.Length);
        for (int i = 0; i < count; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            //// Gradient
            //Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            //float timeOffset = -0.5f * i;
            //float time1 = Mathf.PingPong(timeOffset + Time.realtimeSinceStartup, 1.0f);
            //float time2 = Mathf.PingPong(timeOffset + Time.realtimeSinceStartup - 0.1f, 1.0f);
            //colors[vertexIndex + 0] = gradientColor.Evaluate(time1); // 左下
            //colors[vertexIndex + 1] = gradientColor.Evaluate(time1); // 左上
            //colors[vertexIndex + 2] = gradientColor.Evaluate(time2); // 右上
            //colors[vertexIndex + 3] = gradientColor.Evaluate(time2); // 右下

            // Wave
            Vector3[] verts = textInfo.meshInfo[materialIndex].vertices;

            //float sinWaveOffset = 0.5f * i;
            //float d = i*m_tilt;
            //verts[vertexIndex + 0].y += d;
            //verts[vertexIndex + 1].y += d;
            //verts[vertexIndex + 2].y += d + (verts[vertexIndex + 2].x - verts[vertexIndex + 0].x);
            //verts[vertexIndex + 3].y += d + (verts[vertexIndex + 3].x- verts[vertexIndex + 0].x);
            //Debug.Log(verts[vertexIndex + 0].x);

            verts[vertexIndex + 0].x += (verts[vertexIndex + 0].x - verts[0].x) * m_add_width;
            verts[vertexIndex + 1].x += (verts[vertexIndex + 1].x - verts[0].x) * m_add_width;
            verts[vertexIndex + 2].x += (verts[vertexIndex + 2].x - verts[0].x) * m_add_width;
            verts[vertexIndex + 3].x += (verts[vertexIndex + 3].x - verts[0].x) * m_add_width;

            verts[vertexIndex + 0].y += (verts[vertexIndex + 0].x - verts[0].x) * m_tilt;
            verts[vertexIndex + 1].y += (verts[vertexIndex + 1].x - verts[0].x) * m_tilt+ m_add_height;
            verts[vertexIndex + 2].y += (verts[vertexIndex + 2].x - verts[0].x) * m_tilt+ m_add_height;
            verts[vertexIndex + 3].y += (verts[vertexIndex + 3].x - verts[0].x) * m_tilt;

        }

        m_textComponent.characterSpacing = m_chara_space;
        
        // ③ メッシュを更新
        for (int i = 0; i < textInfo.materialCount; i++)
        {
            if (textInfo.meshInfo[i].mesh == null) { continue; }

            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;  // 変更
            m_textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    public void SetCharSpace(float space)
    {
        m_chara_space = space;
    }
}