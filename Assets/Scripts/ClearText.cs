using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ClearText : MonoBehaviour
{
    public TextMeshProTilt m_text;
    public GameObject m_panel;

    private float m_char_space = -84;
    private float m_panel_scale_x;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_text)
            m_text.SetCharSpace(m_char_space);
    }

    public void Init()
    {
        m_text.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        m_panel.transform.localScale = new Vector3(0.0f, 1.0f, 0.13f);
    }

    public void Play()
    {
        m_char_space = -84;
        DOTween.To(() => m_char_space, (y) => m_char_space = y, -5, 1.3f).SetEase(Ease.OutExpo);

        m_text.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        m_text.transform.DOScaleX(1.0f,1.0f).SetEase(Ease.OutQuint);

        //m_panel_scale_x = 0.0f;
        //DOTween.To(() => m_panel_scale_x, (y) => m_panel_scale_x = y, 1.3f, 1.3f).SetEase(Ease.OutExpo);
        m_panel.transform.localScale = new Vector3(0.0f, 1.0f, 0.13f);
        m_panel.transform.DOScaleX(1.3f, 1.3f).SetEase(Ease.OutQuint);
    }
}
