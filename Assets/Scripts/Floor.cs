using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_sinking_curve = null;
    [SerializeField] private float m_sinking_time = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSinking(bool is_otto)
    {
        //Debug.Log("Sinking");
        transform.position = new Vector3(transform.position.x, -0.501f, transform.position.z);
        if(!is_otto)
            transform.DOLocalMoveY(-0.4f, m_sinking_time).SetEase(m_sinking_curve);
        else
            transform.DOLocalMoveY(-0.2f, m_sinking_time*0.6f).SetEase(m_sinking_curve);
    }

    public float GetSinking()
    {
        return transform.position.y + 0.51f;
    }
}
