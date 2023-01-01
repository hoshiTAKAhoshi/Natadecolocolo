using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NoseAttach : StageObjectBase
{
    [SerializeField] private GameObject m_nose;
    [SerializeField] private GameObject m_light;

    private Material m_material;

    private float m_light_alpha_ofs = 0.0f;
    private float m_light_flash_ofs = -1.0f;

    [SerializeField] AnimationCurve m_light_flash_curve;

    // Start is called before the first frame update
    void Start()
    {
        m_material = m_light.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        m_material.SetFloat("_AlphaOfs", m_light_alpha_ofs);
        m_material.SetFloat("_FlashOfs", m_light_flash_ofs);
    }

    public void AttachToNtdcc()
    {
        Destroy(m_nose.gameObject);
        DOTween.To(() => m_light_alpha_ofs, (y) => m_light_alpha_ofs = y, -1.5f, 0.3f).SetEase(Ease.InCubic);
        m_light_flash_ofs = 0.0f;
        DOTween.To(() => m_light_flash_ofs, (y) => m_light_flash_ofs = y, 0.79f, 0.5f).SetEase(Ease.OutExpo);
    }
}
