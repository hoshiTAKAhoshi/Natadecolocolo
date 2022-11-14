using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distortion : MonoBehaviour
{

    private Material m_material;
    private float m_diameter = -0.5f;
    private float m_alpha = 1.0f;
    private float m_ratio = 0.4f;// * (Random.Range(0,0)*2-1);
    private float m_thickness = 1.5f;

    [SerializeField] private float m_anim_time = 0.4f;

    [SerializeField] private float m_diameter_init = -0.3f;
    [SerializeField] private float m_alpha_init = 1.0f;
    [SerializeField] private float m_ratio_init = 0.4f;// * (Random.Range(0,0)*2-1);
    [SerializeField] private float m_thickness_init = 1.5f;

    [SerializeField] private float m_diameter_fin = 0.5f;
    [SerializeField] private float m_alpha_fin = 0.0f;
    [SerializeField] private float m_ratio_fin = 0.2f;// * (Random.Range(0,0)*2-1);
    [SerializeField] private float m_thickness_fin = 0.0f;

    [SerializeField] private AnimationCurve m_diameter_curve = null;
    [SerializeField] private AnimationCurve m_alpha_curve = null;
    [SerializeField] private AnimationCurve m_ratio_curve = null;
    [SerializeField] private AnimationCurve m_thickness_curve = null;

    // Start is called before the first frame update
    void Awake()
    {
        m_material = GetComponent<Renderer>().material;
        
        //float time = 0.45f;
        m_ratio *= Random.Range(0, 2) * 2 - 1;
        //DOTween.To(() => m_diameter, (x) => m_diameter = x, Random.Range(0.4f, 0.5f), time).SetEase(Ease.OutExpo).OnComplete(() => { EndUpdate(); });
        //DOTween.To(() => m_alpha, (x) => m_alpha = x, 0.0f, time).SetEase(Ease.InSine);
        //DOTween.To(() => m_ratio, (x) => m_ratio = x, m_ratio * 0.2f, time).SetEase(Ease.OutCubic);
        //DOTween.To(() => m_thickness, (x) => m_thickness = x, 0.2f, time * 0.65f).SetEase(Ease.InSine);
        DOTween.To(() => m_diameter_init, (x) => m_diameter = x, Random.Range(m_diameter_fin-0.1f, m_diameter_fin), m_anim_time).SetEase(m_diameter_curve).OnComplete(() => { EndUpdate(); });
        DOTween.To(() => m_alpha_init, (x) => m_alpha = x, m_alpha_fin, m_anim_time).SetEase(m_alpha_curve);
        DOTween.To(() => m_ratio_init, (x) => m_ratio = x, m_ratio_fin, m_anim_time).SetEase(m_ratio_curve);
        DOTween.To(() => m_thickness_init, (x) => m_thickness = x, m_thickness_fin, m_anim_time).SetEase(m_thickness_curve);

        m_material.SetFloat("_Diameter", m_diameter_init);
        m_material.SetFloat("_Alpha", m_alpha_init);
        m_material.SetFloat("_Ratio", m_ratio_init);
        m_material.SetFloat("_ThicknessRatio", m_thickness_init);

    }

    // Update is called once per frame
    void Update()
    {
        //m_diameter += Time.deltaTime / 5;

        m_material.SetFloat("_Diameter", m_diameter);
        m_material.SetFloat("_Alpha", m_alpha);
        m_material.SetFloat("_Ratio", m_ratio);
        m_material.SetFloat("_ThicknessRatio", m_thickness);
    }

    void EndUpdate()
    {
        Destroy(gameObject);
    }
}
