using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgSolid : MonoBehaviour
{
    [SerializeField] private StageMgr m_stage_mgr;
    private Material m_material;
    // Start is called before the first frame update
    void Start()
    {
        m_material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(m_stage_mgr.GetNtdccScreenPos());
        m_material.SetVector("_NtdccPos", m_stage_mgr.GetNtdccScreenPos());
        //m_material.SetFloat()
    }
}
