using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeMgr : MonoBehaviour
{
    [SerializeField] private GameObject m_cube;
    private Material m_cube_material;
    private float m_cube_scale = 2.8f;
    [SerializeField] private AnimationCurve m_cube_in_curve;
    private float m_cube_pru_time = 0.0f;
    private float m_cube_pru_amplitude = 0.0f;
    [SerializeField] private AnimationCurve m_cube_out_curve;

    [SerializeField] private AnimationCurve m_cube_rot_out_curve;
    private float m_cube_rot_y = 0.0f;
    private float m_cube_pru_out;

    [SerializeField] private GameObject m_panel;
    private Material m_panel_material;
    private float m_panel_alpha;


    // Start is called before the first frame update
    void Start()
    {
        m_cube_material = m_cube.GetComponent<Renderer>().material;
        m_panel_material = m_panel.GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //m_cube_material.SetVector("_Amplitude", new Vector4(0.3f, 0.0f, 0.0f, 0.0f));
        //m_panel_material.SetColor("_Color", Color.white);
        if(Input.GetKeyDown(KeyCode.P))
        {
            FadeInStart();
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            FadeOutStart();
        }
        m_cube.transform.localScale = new Vector3(m_cube_scale, m_cube_scale, m_cube_scale);
        m_cube.transform.localEulerAngles = new Vector3(0.0f,m_cube_rot_y,0.0f);

        m_cube_material.SetVector("_Amplitude", new Vector3(m_cube_pru_amplitude*Mathf.Sin(m_cube_pru_time),0,0));
        //m_panel_material.SetFloat("_Alpha", 1-m_cube_scale/5.0f);
        m_panel_material.SetFloat("_Alpha", m_panel_alpha);


    }

    // ‰B‚·
    public void FadeInStart(float time = 1.0f)
    {
        Debug.Log("FadeInStart()");
        DOTween.To(() => m_cube_scale, (x) => m_cube_scale = x, 0, time).SetEase(m_cube_in_curve)
            .OnUpdate(() =>
            {
                m_cube.transform.localScale = new Vector3(m_cube_scale, m_cube_scale, m_cube_scale);
                m_cube.transform.localEulerAngles = new Vector3(0.0f, m_cube_rot_y, 0.0f);

                m_cube_material.SetVector("_Amplitude", new Vector3(m_cube_pru_amplitude * Mathf.Sin(m_cube_pru_time), 0, 0));
                //m_panel_material.SetFloat("_Alpha", 1-m_cube_scale/5.0f);
                m_panel_material.SetFloat("_Alpha", m_panel_alpha);
            });

        float delay = time*0.38f;
        m_cube_pru_time = 0.0f;
        DOTween.To(() => m_cube_pru_time, (x) => m_cube_pru_time = x, 4.0f * Mathf.PI, (time - delay)*1.0f).SetEase(Ease.OutSine).SetDelay(delay);
        m_cube_pru_amplitude = 0.17f;
        DOTween.To(() => m_cube_pru_amplitude, (x) => m_cube_pru_amplitude = x, 0.05f, (time - delay) * 1.0f).SetEase(Ease.Linear).SetDelay(delay);
        m_panel_alpha = 0.0f;
        DOTween.To(() => m_panel_alpha, (x) => m_panel_alpha = x, 1.0f, delay).SetEase(Ease.Linear);

        m_cube_rot_y = 0.0f;
    }

    // ‚Í‚¯‚é
    public void FadeOutStart(float time = 0.7f)
    {
        Debug.Log("FadeOutStart()");
        float delay = time * 0.3f;

        DOTween.To(() => m_cube_scale, (x) => m_cube_scale = x, 3.0f, time).SetEase(m_cube_out_curve)
            .OnUpdate(() =>
            {
                m_cube.transform.localScale = new Vector3(m_cube_scale, m_cube_scale, m_cube_scale);
                m_cube.transform.localEulerAngles = new Vector3(0.0f, m_cube_rot_y, 0.0f);

                //m_cube_material.SetVector("_Amplitude", new Vector3(m_cube_pru_out, 0, 0));
                m_cube_material.SetVector("_Amplitude", new Vector3(m_cube_pru_amplitude * Mathf.Sin(m_cube_pru_time), 0, 0));

                //m_panel_material.SetFloat("_Alpha", 1-m_cube_scale/5.0f);
                m_panel_material.SetFloat("_Alpha", m_panel_alpha);

            });
        DOTween.To(() => m_cube_rot_y, (x) => m_cube_rot_y = x, 120*0, time).SetEase(m_cube_rot_out_curve);
        m_panel_alpha = 1.0f;
        DOTween.To(() => m_panel_alpha, (x) => m_panel_alpha = x, 0.0f, (time - delay)).SetEase(Ease.InCubic).SetDelay(delay);
        //m_cube_pru_out = 0.5f;
        //DOTween.To(() => m_cube_pru_out, (x) => m_cube_pru_out = x, 0.0f, delay).SetEase(Ease.OutQuart);

        m_cube_pru_time = 0.0f;
        DOTween.To(() => m_cube_pru_time, (x) => m_cube_pru_time = x, 4.0f * Mathf.PI, time-delay).SetEase(Ease.InSine).SetDelay(delay);
        m_cube_pru_amplitude = 0.04f;
        DOTween.To(() => m_cube_pru_amplitude, (x) => m_cube_pru_amplitude = x, 0.08f, time).SetEase(Ease.Linear);



    }
}
