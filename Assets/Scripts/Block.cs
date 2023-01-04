using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : StageObjectBase
{
    enum OTTO_STATE { NEUTRAL, PUSH, RELEASE }

    [SerializeField] private BlockFragment m_pref_block_fragment;
    [SerializeField] private Distortion m_pref_distortion;

    [SerializeField] private float m_otto_amplitude_max;
    [SerializeField] private float m_otto_time_in;
    [SerializeField] private float m_otto_time_out;
    [SerializeField] private AnimationCurve m_curve_otto_in;
    [SerializeField] private AnimationCurve m_curve_otto_out;

    private Vector3 m_pru_dire = Vector3.zero;
    private float m_amplitude = 0.0f;
    private float m_amplitude_past = 0.0f;
    private float m_white_ratio = 0.0f;
    private float m_expansion = 0.0f;
    private Sequence m_seq_amplitude;
    private Material m_material;
    private bool m_is_hit_tama = false;

    private Natadecoco m_ntdcc = null;
    private Tama m_tama = null;
    private Sequence m_seq_otto;

    private OTTO_STATE m_state = OTTO_STATE.NEUTRAL;
    private float m_yure = 0.0f;
    private float m_yure_past = 0.0f;
    private Vector3 m_ntdcc_dire;

    //private BoxCollider m_collider;

    // Start is called before the first frame update
    void Start()
    {
        SetObjectType(ObjectType.BLOCK);
        m_material = GetComponent<Renderer>().material;
        //m_collider= GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(m_is_hit_tama)
        {
            m_material.SetVector("_Amplitude", m_pru_dire * m_amplitude);
            m_material.SetFloat("_WhiteRatio", m_white_ratio);
            m_material.SetFloat("_Expansion", m_expansion);
        }

        switch (m_state)
        {
            case OTTO_STATE.NEUTRAL:
                break;
            case OTTO_STATE.PUSH:
                {
                    if (m_ntdcc)
                    {

                        //Time.timeScale = 0.2f;
                        Vector3 ntdcc_rot = m_ntdcc.GetRot();
                        Vector2Int to_pos = m_ntdcc.GetToPos();
                        Vector3 pru_vec = m_ntdcc.GetPruVec();
                        float x = -ntdcc_rot.z + pru_vec.x * 60;// * to_pos.x;
                        float y = -ntdcc_rot.x - pru_vec.z * 60;// * to_pos.y;
                                                                //Debug.Log(m_ntdcc.GetPruVec());
                                                                //Debug.Log(x.ToString("F") + "," + y.ToString("F"));
                        m_yure_past = m_yure;
                        if (to_pos.x != 0)
                            m_yure = x;
                        else
                            m_yure = y;


                        if (Mathf.Abs(m_yure) > 12.0f)
                        {
                            m_seq_otto.Kill();
                            m_amplitude = (Mathf.Abs(m_yure) - 12.0f) * 0.02f;// * Mathf.Sign(y);
                            if (Mathf.Abs(m_yure) < Mathf.Abs(m_yure_past))
                            {
                                //m_amplitude = 0.0f;
                                m_seq_otto = DOTween.Sequence();
                                m_seq_otto.Append(DOTween.To(() => m_amplitude, (val) => m_amplitude = val, 0, m_otto_time_out).SetEase(m_curve_otto_out));//.OnComplete(() => { m_state = OTTO_STATE.NEUTRAL; });
                                m_ntdcc = null;
                                m_state = OTTO_STATE.RELEASE;
                            }
                        }
                    }
                }
                break;
            case OTTO_STATE.RELEASE:
                {

                }
                break;
        }
        
    }

    public void Break()
    {
        //Debug.Log("Block Break");
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

        //m_collider.enabled = false;

        float interval = 0.4f;
        for (int y = 0; y < 2; y++)
        {
            for (int z = 0; z < 2; z++)
            {
                for (int x = 0; x < 2; x++)
                {
                    //if (Random.Range(0.0f, 1.0f) < 0.2f) continue;
                    //if (num_shuffle[x + y * 2 + z * 4] >= spawn_num) continue;
                    BlockFragment frg = Instantiate(m_pref_block_fragment, transform.position + new Vector3((x - 0.5f) * interval, (y + 1.5f) * interval, (z - 0.5f) * interval), Quaternion.identity);
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

    public void PlayOttoPru(Vector2Int dire)
    {
        m_pru_dire = new Vector3(dire.x,0,-dire.y);
        m_seq_amplitude.Kill();
        m_seq_amplitude = DOTween.Sequence();
        m_seq_amplitude.Append(DOTween.To(() => m_amplitude, (x) => m_amplitude = x, m_otto_amplitude_max, m_otto_time_in).SetEase(m_curve_otto_in));
        m_seq_amplitude.Append(DOTween.To(() => m_amplitude, (x) => m_amplitude = x, 0.0f, m_otto_time_out).SetEase(m_curve_otto_out));

    }

    // ナタデココおっとプル用
    public void SetNatadecoco(Natadecoco ntdcc)
    {
        m_ntdcc = ntdcc;
        //m_amplitude_past = 0.0f;
        //m_amplitude = 0.0f;
        if (ntdcc == null)
            return;
        m_state = OTTO_STATE.PUSH;
        m_ntdcc_dire = ntdcc.GetPruDire();
        Vector2Int to_pos = ntdcc.GetToPos();
        m_pru_dire = new Vector3(to_pos.x,0,-to_pos.y);

    }
}
