using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataBase : MonoBehaviour
{
    protected string[] m_char_data;
    public int[,] m_yuka_data;
    struct StageCharIntData
    {
        char char_data;
        int int_data;
        public StageCharIntData(char c, int i)
        {
            char_data = c;
            int_data = i;
        }
    }

    private StageCharIntData[] m_yuka_char_int_table =
        {
            new StageCharIntData(' ', 0),
            new StageCharIntData('Q', 1),
            new StageCharIntData('O', 2),
            new StageCharIntData('G', 3)
        };

    // コンストラクタ
    public StageDataBase(string[] char_data)
    {
        m_char_data = char_data;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
