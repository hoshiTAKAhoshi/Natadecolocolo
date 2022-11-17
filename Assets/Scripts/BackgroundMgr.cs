using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMgr : MonoBehaviour
{
    [SerializeField] private StageMgr m_stage_mgr;
    [SerializeField] private BgSolid m_pref_bg_cube;

    private List<BgSolid> m_cube_list = new List<BgSolid>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
