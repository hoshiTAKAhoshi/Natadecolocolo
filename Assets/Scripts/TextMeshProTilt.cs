using UnityEngine;
using TMPro;
using System.Globalization;

public class TextMeshProTilt : MonoBehaviour
{
    [SerializeField] private float m_tilt;
    [SerializeField] private float m_add_height;
    private TMP_Text textComponent;
    //private TMP_TextInfo textInfo;

    private void Start()
    {
        this.textComponent = GetComponent<TMP_Text>();

        UpdateAnimation();

    }

    private void Update()
    {
        if (this.textComponent == null)
            this.textComponent = GetComponent<TMP_Text>();

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // �@ ���b�V�����Đ�������i���Z�b�g�j
        this.textComponent.ForceMeshUpdate(true);
        var textInfo = textComponent.textInfo;

        // �A���_�f�[�^��ҏW�����z��̍쐬
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
            //colors[vertexIndex + 0] = gradientColor.Evaluate(time1); // ����
            //colors[vertexIndex + 1] = gradientColor.Evaluate(time1); // ����
            //colors[vertexIndex + 2] = gradientColor.Evaluate(time2); // �E��
            //colors[vertexIndex + 3] = gradientColor.Evaluate(time2); // �E��

            // Wave
            Vector3[] verts = textInfo.meshInfo[materialIndex].vertices;

            //float sinWaveOffset = 0.5f * i;
            //float d = i*m_tilt;
            //verts[vertexIndex + 0].y += d;
            //verts[vertexIndex + 1].y += d;
            //verts[vertexIndex + 2].y += d + (verts[vertexIndex + 2].x - verts[vertexIndex + 0].x);
            //verts[vertexIndex + 3].y += d + (verts[vertexIndex + 3].x- verts[vertexIndex + 0].x);
            //Debug.Log(verts[vertexIndex + 0].x);

            verts[vertexIndex + 0].y += (verts[vertexIndex + 0].x - verts[0].x) * m_tilt;
            verts[vertexIndex + 1].y += (verts[vertexIndex + 1].x - verts[0].x) * m_tilt+ m_add_height;
            verts[vertexIndex + 2].y += (verts[vertexIndex + 2].x - verts[0].x) * m_tilt+ m_add_height;
            verts[vertexIndex + 3].y += (verts[vertexIndex + 3].x - verts[0].x) * m_tilt;

        }

        // �B ���b�V�����X�V
        // �B ���b�V�����X�V
        for (int i = 0; i < textInfo.materialCount; i++)
        {
            if (textInfo.meshInfo[i].mesh == null) { continue; }

            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;  // �ύX
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}