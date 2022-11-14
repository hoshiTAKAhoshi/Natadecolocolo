using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TamaShot : MonoBehaviour
{
    Vector3 m_shot_dire = Vector3.zero;
    float m_shot_speed = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_shot_dire * m_shot_speed * Time.deltaTime;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetShotDire(Vector3 dire)
    {
        m_shot_dire = dire;
    }
}
