using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : StageObjectBase
{
    [SerializeField] private BlockFragment m_pref_block_fragment;
    [SerializeField] private Distortion m_pref_distortion;

    private Vector3 m_pru_dire = Vector3.zero;
    private float m_amplitude = 0.0f;
    private float m_white_ratio = 0.0f;
    private float m_expansion = 0.0f;
    private Material m_material;
    private bool m_is_hit_tama = false;

    private Tama m_tama;
    // Start is called before the first frame update
    void Start()
    {
        SetObjectType(ObjectType.BLOCK);
        m_material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_is_hit_tama)
        {
            m_material.SetVector("_Amplitude", m_pru_dire * m_amplitude);
            m_material.SetFloat("_WhiteRatio", m_white_ratio);
            m_material.SetFloat("_Expansion", m_expansion);
        }
    }

    public void Break()
    {
        Debug.Log("Block Break");
        Destroy(gameObject);
        // 破片
        int[] num_shuffle = { 0, 1, 2, 3, 4, 5, 6, 7 };
        for(int i = 0;i<num_shuffle.Length;i++)
        {
            int j = Random.Range(0, i + 1);
            int tmp = num_shuffle[i];
            num_shuffle[i] = num_shuffle[j];
            num_shuffle[j] = tmp;
        }
        int spawn_num = 6;
        float interval = 0.4f;
        for (int y = 0; y < 2; y++)
        {
            for (int z = 0; z < 2; z++)
            {
                for (int x = 0; x < 2; x++)
                {
                    //if (Random.Range(0.0f, 1.0f) < 0.2f) continue;
                    //if (num_shuffle[x + y * 2 + z * 4] >= spawn_num) continue;
                    BlockFragment frg = Instantiate(m_pref_block_fragment, transform.position + new Vector3((x - 0.5f) * interval, (y - 0.5f) * interval, (z - 0.5f) * interval), Quaternion.identity);
                    frg.SetOfset(new Vector3(x,y,z));
                }
            }
        }
        // 空間歪み
        Vector3 dis_pos = transform.position;
        dis_pos.y = -0.5f;  // 床の高さ
        Distortion dis = Instantiate(m_pref_distortion, dis_pos, Quaternion.identity);
        //dis.transform.localScale = new Vector3(1.5f, 0.0f, 1.5f);
        Time.timeScale = 1.0f;
        m_tama.Break();
    }

    public void HitTama(Tama tama, float time_scale, float anim_time)
    {
        m_is_hit_tama = true;
        m_tama = tama;
        Time.timeScale = time_scale;
        m_pru_dire = m_tama.GetShotDire();
        m_amplitude = 0.1f;
        DOTween.To(() => m_amplitude, (y) => m_amplitude = y, 0.3f, anim_time*time_scale).SetEase(Ease.OutQuad).OnComplete(() => { Break(); });
        m_white_ratio = 0.0f;
        DOTween.To(() => m_white_ratio, (y) => m_white_ratio = y, 0.4f, anim_time*time_scale).SetEase(Ease.OutCubic);
        m_expansion = 0.0f;
        DOTween.To(() => m_expansion, (y) => m_expansion = y, 0.08f, anim_time * time_scale).SetEase(Ease.InExpo);
    }
}
