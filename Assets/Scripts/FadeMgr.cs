using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeMgr : MonoBehaviour
{
    [SerializeField] private GameObject m_fade_cube;
    [SerializeField] private GameObject m_fade_panel;

    private Material m_cube_material;
    private Material m_panel_material;

    // Start is called before the first frame update
    void Start()
    {
        m_cube_material = m_fade_cube.GetComponent<Renderer>().material;
        m_panel_material = m_fade_panel.GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        m_cube_material.SetVector("_Amplitude", new Vector4(0.3f, 0.0f, 0.0f, 0.0f));
        //m_panel_material.SetColor("_Color", Color.white);
    }

    // ‰B‚·
    public void FadeInStart()
    {

    }

    // ‚Í‚¯‚é
    public void FadeOutStart()
    {

    }
}
