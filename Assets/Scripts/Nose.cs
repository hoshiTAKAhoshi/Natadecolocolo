using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nose : MonoBehaviour
{
    private Material m_material;
    public Vector3Int m_nose_dire_def;
    public Vector3Int m_nose_dire;

    // Start is called before the first frame update
    void Awake()
    {
        m_material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(m_nose_dire);
    }

    public void SetExpansion(float expansion)
    {
        m_material.SetFloat("_Expansion", expansion);
    }

    public void SetPuruDire(Vector3 puru_dire)
    {
        m_material.SetVector("_Amplitude", puru_dire);
    }

    public Vector3 GetEffPos()
    {
        return transform.position + (transform.rotation) * Vector3.up * 0.7f;
    }

    public Quaternion GetEffRot()
    {
        return transform.rotation;
    }

    public void CulcDireVec(Vector3 angle)
    {
        Vector3 dire = (Quaternion.Euler(angle) * m_nose_dire_def);
        m_nose_dire = new Vector3Int(Mathf.RoundToInt(dire.x), Mathf.RoundToInt(dire.y), Mathf.RoundToInt(dire.z));
    }
}
