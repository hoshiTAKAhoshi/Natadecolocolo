using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectBase : MonoBehaviour
{
    public enum ObjectType { TAMA , BLOCK};

    private ObjectType m_object_type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetObjectType(ObjectType type)
    {
        m_object_type = type;
    }

    public ObjectType GetObjectType()
    {
        return m_object_type;
    }
}
