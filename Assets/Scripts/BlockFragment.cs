using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BlockFragment : MonoBehaviour
{
    [SerializeField] private Material m_brust_material;
    [SerializeField] private AnimationCurve m_scale_curve;
    private Rigidbody m_rb;
    private Renderer m_rdr;
    private Material m_material;

    private Vector3 m_ofs = Vector3.zero;

    private bool m_is_brust = false;
    private bool m_is_fall = false;
    private float m_transparent_ratio = 0.0f;
    private float m_white_ratio = 0.0f;
    private Sequence m_seq_white;
    private Sequence m_seq_scale;


    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.1f;

        m_rb = this.GetComponent<Rigidbody>();
        m_rdr = GetComponent<Renderer>();
        m_material = m_rdr.material;
        m_white_ratio = 1.0f;
        m_material.SetFloat("_WhiteRatio", m_white_ratio);
        m_seq_white = DOTween.Sequence();
        m_seq_white.Append(DOTween.To(() => m_white_ratio, (y) => m_white_ratio = y, 0.0f, 0.3f).SetEase(Ease.OutCubic));

        m_seq_scale = DOTween.Sequence();
        m_seq_scale.Append(transform.DOScale(new Vector3(0.33f, 0.33f, 0.33f), 0.52f).SetEase(m_scale_curve));
        //transform.DOScale(Vector3.zero, 0.4f).SetDelay(0.3f + Random.Range(0.0f, 0.3f)).SetEase(Ease.InCubic);
        
        float haba = 1.0f;
        Vector3 force = new Vector3(Random.Range(-1.0f, 1.0f) * haba / 2 + (m_ofs.x * 2 - 1) * haba, Random.Range(1.5f, 2.3f) * 1 + m_ofs.y * 2.0f, Random.Range(-1.0f, 1.0f) * haba / 2 + (m_ofs.z * 2 - 1) * haba);
        //force = new Vector3(0, 3, 0);
        force *= 0.6f;
        m_rb.AddForce(force, ForceMode.Impulse);
        float t = 15.0f;
        m_rb.maxAngularVelocity = t;
        //m_rb.AddTorque(new Vector3(Random.Range(-t, t), Random.Range(-t, t), Random.Range(-t, t)), ForceMode.Impulse);
        m_rb.angularVelocity = new Vector3(Random.Range(-t, t), Random.Range(-t, t), Random.Range(-t, t));
    }

    // Update is called once per frame
    void Update()
    {
        m_material.SetFloat("_WhiteRatio", m_white_ratio);
        m_material.SetFloat("_TransparentRatio", m_transparent_ratio);
        if (m_is_brust ==true)
        {
        }
        //else if (transform.position.y < -0.6f)
        //{
        //    float tween_time = 0.4f + Random.Range(-0.2f, 0.40f);
        //    DOTween.To(() => m_transparent_ratio, (y) => m_transparent_ratio = y, 1.0f, tween_time).SetEase(Ease.InExpo).OnComplete(() => { Destroy(gameObject); });
        //    transform.DOScale(transform.localScale * 0.3f, tween_time).SetEase(m_scale_curve);
        //    m_white_ratio = 0.5f;
        //    DOTween.To(() => m_white_ratio, (y) => m_white_ratio = y, 0.0f, tween_time * 0.5f).SetEase(Ease.OutCubic);
        //    m_is_brust = true;
        //}

        if (m_rb.velocity.y < 0 && transform.position.y < -1.5f && m_is_fall == false && m_is_brust == false)
        {
            m_is_fall = true;
            //m_seq_scale.Kill();
            //transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine);
            //m_rb.drag = 4.5f;
            //m_material = null;
            //m_material = m_rdr.material = new Material(m_brust_material);

            //m_transparent_ratio = 0.3f;
            //DOTween.To(() => m_transparent_ratio, (y) => m_transparent_ratio = y, 1.0f, 0.2f).SetEase(Ease.InCubic).OnComplete(() => { Destroy(gameObject); });
            m_seq_scale.Kill();
            transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InQuint).OnComplete(() => { Destroy(gameObject); });

        }

    }

    public void SetOfset(Vector3 ofs)
    {
        m_ofs = ofs;
    }

    public void OnCollisionStay(Collision other)
    {
        //Debug.Log("Hit");
        if (m_rb.velocity.y <= 0 && m_is_brust == false&&other.gameObject.tag != "BlockFragment")
        {
            m_is_brust = true;
            m_rb.drag = 15.0f;
            m_rb.angularDrag = 15.0f;
            m_seq_scale.Kill();
            transform.DOScale(Vector3.zero, 0.35f).SetEase(Ease.InQuad).OnComplete(() => { Destroy(gameObject); });
        }

        return;
        if (m_rb.velocity.y <= 0 && m_is_brust==false && other.gameObject.tag != "BlockFragment")
        {
            m_is_brust = true;
            m_rb.isKinematic = true;
            //transform.rotation = Quaternion.identity;
            m_material = null;
            m_material = m_rdr.material = new Material(m_brust_material);

            m_seq_white.Kill();
            m_white_ratio = 0.4f;
            m_material.SetFloat("_WhiteRatio", m_white_ratio);
            Ease curve = Ease.OutQuad;
            float tween_time = 0.4f;
            m_transparent_ratio = 0.3f;
            DOTween.To(() => m_transparent_ratio, (y) => m_transparent_ratio = y, 1.0f, tween_time).SetEase(Ease.InCubic).OnComplete(()=> { Destroy(gameObject); });
            transform.DOMoveY(0.3f, tween_time).SetEase(Ease.OutSine).SetRelative();
            transform.DOScale(new Vector3(0.45f, 0.45f, 0.45f), tween_time).SetEase(curve);

        }
    }
}
