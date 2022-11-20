using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMgr : MonoBehaviour
{
    [SerializeField] private StageMgr m_stage_mgr;
    [SerializeField] private BgCube m_pref_bg_cube;

    private int m_cube_num_max = 25;
    private List<BgCube> m_cube_list = new List<BgCube>();

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = Camera.main.transform.position;
        for (int group = 0; group < 2; group++)
        {
            for (int i = 0; i < m_cube_num_max; i++)
            {
                float x = ((float)i / m_cube_num_max) * 10 - 5;//Random.Range(-5.0f, 5.0f);
                float y = Random.Range(-1.0f, 4.0f) + group * 5; ;
                BgCube cube = Instantiate(m_pref_bg_cube, new Vector3(-7.0f + x + y, -12.0f, 7.0f + x - y), Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f)));
                cube.transform.parent = transform;
                cube.SetScale((float)(i % 2) * 0.2f + 0.35f);
                cube.SetStageMgr(m_stage_mgr);
                m_cube_list.Add(cube);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position;
        transform.localScale = Vector3.one * Camera.main.orthographicSize / 2.76f;
    }
}
