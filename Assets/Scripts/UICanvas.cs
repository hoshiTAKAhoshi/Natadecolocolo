using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    private Vector3 m_pos_camera_to_ui = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        m_pos_camera_to_ui = transform.position - Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position + m_pos_camera_to_ui;
    }
}
