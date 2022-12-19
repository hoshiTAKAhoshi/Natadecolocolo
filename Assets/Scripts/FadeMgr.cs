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
    [SerializeField] private AnimationCurve m_cuve_in_curve;
    private float m_cube_pru_time = 0.0f;
    private float m_cube_pru_amplitude = 0.0f;
    
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
        m_cube_material.SetVector("_Amplitude", new Vector3(m_cube_pru_amplitude*Mathf.Sin(m_cube_pru_time),0,0));
        m_panel_material.SetFloat("_Alpha", 1-m_cube_scale/5.0f);

    }

    // ‰B‚·
    public void FadeInStart(float time = 1.0f)
    {
        Debug.Log("FadeInStart()");
        DOTween.To(() => m_cube_scale, (x) => m_cube_scale = x, 0, time).SetEase(m_cuve_in_curve);

        float delay = time*0.38f;
        m_cube_pru_time = 0.0f;
        DOTween.To(() => m_cube_pru_time, (x) => m_cube_pru_time = x, 4.0f * Mathf.PI, (time - delay)*1.0f).SetEase(Ease.OutSine).SetDelay(delay);
        m_cube_pru_amplitude = 0.20f;
        DOTween.To(() => m_cube_pru_amplitude, (x) => m_cube_pru_amplitude = x, 0.05f, (time - delay) * 1.0f).SetEase(Ease.Linear).SetDelay(delay);

    }

    // ‚Í‚¯‚é
    public void FadeOutStart(float time = 0.4f)
    {
        Debug.Log("FadeOutStart()");
        DOTween.To(() => m_cube_scale, (x) => m_cube_scale = x, 2.8f, time).SetEase(Ease.InQuad);

    }
}
