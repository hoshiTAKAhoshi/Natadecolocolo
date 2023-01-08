using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ClearText : MonoBehaviour
{
    public TextMeshProTilt m_text;

    private float m_char_space = -84;

    // Start is called before the first frame update
    void Start()
    {
        m_text.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_text)
            m_text.SetCharSpace(m_char_space);
    }

    public void Play()
    {
        m_char_space = -84;
        DOTween.To(() => m_char_space, (y) => m_char_space = y, -5, 1.3f).SetEase(Ease.OutExpo);

        m_text.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        m_text.transform.DOScaleX(1.0f,1.0f).SetEase(Ease.OutExpo);
    }
}
