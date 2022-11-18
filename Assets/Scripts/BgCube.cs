using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgCube : BgSolid
{
    private Vector3 m_add_rot;
    private float m_add_y;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        float a = 5;
        m_add_rot = new Vector3(Random.Range(-a, a), Random.Range(-a, a), Random.Range(-a, a));

        m_add_y = Random.Range(0.01f, 0.07f);

        float scale = Random.Range(0.4f, 0.9f);
        transform.localScale = Vector3.one * scale;

        //m_material.color = Color.white;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        transform.Rotate(m_add_rot * Time.deltaTime);
        float up = 17.0f;
        float down = 5.5f * 2.0f;

        if (-transform.position.x + transform.position.z > up)
        {
            Debug.Log(transform.position);
            transform.position += new Vector3(down, 0, -down);
            Debug.Log(transform.position);
        }
        else
        {
            transform.position += new Vector3(-m_add_y, 0.0f, m_add_y) * Time.deltaTime;
        }
    }
}
