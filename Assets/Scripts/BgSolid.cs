using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgSolid : MonoBehaviour
{
    [SerializeField] protected StageMgr m_stage_mgr;
    protected Material m_material;

    // Start is called before the first frame update
    protected void Start()
    {
        m_material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    protected void Update()
    {
        //Debug.Log(m_stage_mgr.GetNtdccScreenPos());
        m_material.SetVector("_NtdccScreenPos", m_stage_mgr.GetNtdccScreenPos());
        m_material.SetFloat("_Radius0", m_stage_mgr.GetBgRadius0());
        m_material.SetFloat("_Radius1", m_stage_mgr.GetBgRadius1());
        m_material.SetFloat("_WhiteRatio", m_stage_mgr.GetWhiteRatio());
        //m_material.SetFloat()
        //Debug.Log("SetVector");
    }

    public void SetStageMgr(StageMgr stage_mgr)
    {
        m_stage_mgr = stage_mgr;
    }
}
