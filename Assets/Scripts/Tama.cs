using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tama : StageObjectBase
{
    public enum TamaMode { OBJECT, INSIDE, SHOT }

    private StageMgr m_stage_mgr = null;
    private Natadecoco m_ntdcc;
    private TamaMode m_mode = TamaMode.OBJECT;

    Vector2Int m_pos_on_field = Vector2Int.zero;
    float m_yura_time = 0.0f;
    const float m_yura_amplitude = 0.2f;
    const float m_yura_speed = 2.0f;

    Vector3 m_shot_dire = Vector3.zero;
    float m_shot_speed = 4.0f;

    float m_pos_lerp_ratio = 0.5f;

    private bool m_is_hit = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_mode)
        {
            case TamaMode.OBJECT:
                m_yura_time += Time.deltaTime * m_yura_speed;
                Vector2 tama_pos = new Vector2(m_pos_on_field.x, m_pos_on_field.y);
                float amp = m_yura_amplitude;
                //Vector2 tama_pos = new Vector2(transform.position.x, transform.position.z);
                if (m_ntdcc)
                {
                    Vector2 ntdcc_pos = new Vector2(m_ntdcc.transform.position.x,m_ntdcc.transform.position.z);
                    Vector2 diff = tama_pos - ntdcc_pos;
                    float mag_min = 0.6f;//ここまでなら近づける
                    if(diff.magnitude>0 && diff.magnitude<mag_min)
                    {
                        //diff = diff*diff.magnitude;
                        tama_pos += (diff.normalized*mag_min - diff)*(0.6f+diff.magnitude)*1.0f;
                        float sx = Mathf.Abs(diff.x) * (mag_min - diff.magnitude)*0.7f;
                        float sy = Mathf.Abs(diff.y) * (mag_min - diff.magnitude)*0.7f;
                        transform.localScale = new Vector3(
                            0.3f - sx + sy,
                            0.3f + sx + sy,
                            0.3f + sx - sy
                        );
                        //Debug.Log(diff);
                        amp *= (diff.magnitude/0.6f);
                    }

                }
                transform.position = new Vector3(tama_pos.x, amp * Mathf.Sin(m_yura_time), tama_pos.y);
                break;
            case TamaMode.INSIDE:
                //Debug.Log("inside");
                if (m_ntdcc)
                {
                    transform.position = Vector3.Lerp(m_ntdcc.GetCenterPos(), transform.position, m_pos_lerp_ratio);
                    transform.rotation = Quaternion.Slerp(m_ntdcc.GetRotation(), transform.rotation, 0.0f);
                }
                break;
            case TamaMode.SHOT:
                transform.position += m_shot_dire * m_shot_speed * Time.deltaTime;
                // 弾との衝突判定
                m_pos_on_field = new Vector2Int(Mathf.RoundToInt(transform.position.x), -Mathf.RoundToInt(transform.position.z));
                //Debug.Log(m_stage_mgr);
                //if (m_stage_mgr == null) Debug.Log("no_stage_mgr");
                if (m_stage_mgr.GetObjectData(m_pos_on_field) == "E" && !m_is_hit)
                {
                    float x_in_tile = transform.position.x % 1;
                    float y_in_tile = transform.position.y % 1;
                    if ((x_in_tile > 0.6f || x_in_tile < 0.4f) && (y_in_tile > 0.6f || y_in_tile < 0.4f))
                    {
                        m_is_hit = true;
                        m_shot_speed *= 0.0f;
                        m_stage_mgr.ReplaceStageObjectData(m_pos_on_field, " ");
                        StageObjectBase obj = m_stage_mgr.GetStageObject(m_pos_on_field);
                        float time_scale = 0.2f;
                        float anim_time = 0.35f;
                        ((Block)obj).HitTama(this, time_scale, anim_time);
                        float sx = Mathf.Abs(m_shot_dire.x)*0.1f;
                        float sy = Mathf.Abs(m_shot_dire.y)*0.1f;
                        transform.DOScale(new Vector3(0.3f - sx+sy, 0.3f+sx+sy, 0.3f+sx- sy), anim_time*time_scale*2.0f).SetEase(Ease.OutQuart);
                        transform.DOMove(m_shot_dire * 0.3f, anim_time * time_scale*2.0f).SetEase(Ease.OutQuart).SetRelative();
                        //((Block)obj).Break();
                        //Break();
                    }
                }
                break;
        }
    }

    public void SetNatadecoco(Natadecoco ntdcc)
    {
        m_ntdcc = ntdcc;
    }

    public void SetMode(TamaMode mode)
    {
        m_mode = mode;
        if (m_mode == TamaMode.INSIDE)
        {
            //transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            m_pos_lerp_ratio = 1.0f;
            DOTween.To(() => m_pos_lerp_ratio, (x) => m_pos_lerp_ratio = x, 0.5f, 0.5f);
            transform.position =
                new Vector3(
                    m_pos_on_field.x + (transform.position.x - m_pos_on_field.x) * 1.0f,
                    transform.position.y,
                    m_pos_on_field.y + (transform.position.z - m_pos_on_field.y) * 1.0f
                );
        }
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetShotDire(Vector3 dire)
    {
        m_shot_dire = dire;
    }

    public void SetPosOnField(Vector2Int pos_on_field)
    {
        m_pos_on_field = pos_on_field;
    }

    public void Break()
    {
        Destroy(gameObject);
    }

    public void SetStageMgr(StageMgr stage_mgr)
    {
        Debug.Log("Set Stage Mgr");
        m_stage_mgr = stage_mgr;
    }

    public Vector3 GetShotDire()
    {
        return m_shot_dire;
    }

    // ナタデココに取り込まれた時のスケールアニメーションを再生する
    public void PlayInsideScaleAnim(Vector2Int vec)
    {
        transform.rotation = m_ntdcc.GetRotation();
        Vector3 local_vec = m_ntdcc.GetRotation() * new Vector3(vec.x, 0, vec.y);
        transform.localScale = new Vector3(
            0.25f + Mathf.Abs(local_vec.x) * 0.2f,
            0.25f + Mathf.Abs(local_vec.y) * 0.2f,
            0.25f + Mathf.Abs(local_vec.z) * 0.2f
        );
        transform.position = new Vector3(
            m_pos_on_field.x + (transform.position.x - m_pos_on_field.x) * 0.9f,
            transform.position.y,
            m_pos_on_field.y + (transform.position.z - m_pos_on_field.y) * 0.9f
        );

        //if (vec.x != 0)
        //{
        //    transform.localScale = Quaternion.Inverse(m_ntdcc.GetRotation()) * new Vector3(0.7f, 0.2f, 0.2f);
        //}
        //else if (vec.y != 0)
        //{
        //    transform.localScale = Quaternion.Inverse(m_ntdcc.GetRotation()) * new Vector3(0.2f, 0.2f, 0.7f);
        //}
        transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.35f).SetEase(Ease.OutQuad);
    }
}
