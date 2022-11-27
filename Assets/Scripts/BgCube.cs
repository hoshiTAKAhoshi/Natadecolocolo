using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgCube : BgSolid
{
    private Vector3 m_add_rot;
    private float m_add_y = 0.0f;
    private float m_add_y_force = 0.0f;     // ƒS[ƒ‹—p
    private Vector2 m_force;                // Žü‚è‚ÉL‚ª‚é—p

    [SerializeField] private AnimationCurve m_force_decay_curve;
    [SerializeField] private AnimationCurve m_force_y_curve;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        float a = 5;
        m_add_rot = new Vector3(Random.Range(-a, a), Random.Range(-a, a), Random.Range(-a, a));

        m_add_y = (transform.localScale.x-0.3f) * 0.2f;// Random.Range(0.01f, 0.07f);


        //m_material.color = Color.white;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        transform.Rotate(m_add_rot * Time.deltaTime * (1+(m_force.magnitude * 30.0f + m_add_y_force*20.0f)));
        float up = 17.0f+10.0f;
        float down = 5.5f * 2.0f;
        //Camera.main.ScreenToWorldPoint();
        //Camera.main.WorldToScreenPoint();

        //m_force *= 0.95f;

//        if (-transform.position.x + transform.position.z > up)
        if (-transform.localPosition.x + transform.localPosition.z > up)
        {
                //Debug.Log(transform.position);
                transform.localPosition += new Vector3(down, 0, -down);
            //Debug.Log(transform.position);
        }
        else
        {
            //transform.position += new Vector3(-m_add_y, 0.0f, m_add_y) * Time.deltaTime;
            float add_y = m_add_y + m_add_y_force;
            transform.localPosition += new Vector3(-add_y + m_force.x - m_force.y, 0.0f, add_y + m_force.x + m_force.y) * Time.deltaTime;
        }
    }

    public void SetScale(float base_scale)
    {
        float scale = Random.Range(base_scale, base_scale+0.15f);
        transform.localScale = Vector3.one * scale;

    }

    public void AddForceBgCube(Vector2 center_screen_pos, float power, bool is_goal = true)
    {
        if (is_goal)
        {
            Debug.Log("cube");
            Vector2 cube_screen_pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 vec = cube_screen_pos - center_screen_pos;
            float mag = vec.magnitude * 0.002f;
            Vector2 force = vec.normalized / (mag * mag + 0.5f);
            force *= power;
            DOTween.To(() => m_force, (x) => m_force = x, force, 3.0f).SetEase(m_force_decay_curve);
            DOTween.To(() => m_add_y_force, (x) => m_add_y_force = x, (transform.localScale.x - 0.3f) * 15.0f * Random.Range(0.7f,1.3f), 3.0f + Random.Range(-0.3f,0.5f))
                .SetEase(m_force_y_curve).SetDelay(1.7f);
        }
    }


}
