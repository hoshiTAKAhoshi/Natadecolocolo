using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "MyGame/Create StageDataTable", fileName = "StageDataTable")]
public class StageDataTable : MonoBehaviour
{
    [SerializeField]
    public GameObject m_pref_ntdcc = null;
    [SerializeField]
    public Floor m_pref_floor = null;
    [SerializeField]
    public Floor m_pref_floor_hole = null;


    public struct StageCharprefData
    {
        char char_data;
        GameObject pref_data;
        public StageCharprefData(char c, GameObject p)
        {
            char_data = c;
            pref_data = p;
        }
    }

    public struct StageData
    {
        public string stage_name;
        public int challenge_move_num;
        public string[] field_data;
        public Vector3Int[] nose_dire;
        public StageData(string stname, int cnum, string[] fdata, Vector3Int[] ndire)
        {
            stage_name = stname;
            challenge_move_num = cnum;
            field_data = fdata;
            nose_dire = ndire;
        }
    }

    //public StageCharprefData[] m_yuka_char_pref_table =
    //{
    //    new StageCharprefData(' ', null),
    //    new StageCharprefData('Q', m_pref_floor),
    //    new StageCharprefData('O', m_pref_floor_hole),
    //    new StageCharprefData('G', m_pref_floor_hole)
    //};

    public Dictionary<string, GameObject> m_pref_dict = new Dictionary<string, GameObject>();
    //{
    //    {" ",null},
    //    {"Q",m_pref_floor_hole},
    //    {"O",m_pref_floor},
    //    {"G",m_pref_floor_hole},
    //};

    //public Dictionary<string, Floor> m_pref_floor_dict = new Dictionary<string, Floor>();

    public StageData[] m_stage_data =
    {
        // ステージ1
        new StageData
        (
            "はじまりのころがり",5,
            // フィールドデータ
            new[]
            {
                "SQ'Q'Q OEQ",
                " G'Q'O'Q G",
                " Q'Q'Q'Q"
            },
            // 鼻の向き
            new[]
            {
                //new Vector3Int(0,1,0),
                new Vector3Int(-1,0,0),
                //new Vector3Int(0,0,1),
                //new Vector3Int(0,0,-1),
            }
        ),
        // ステージ2
        new StageData
        (
            "はな が\nつっかえちゃう",5,
            // フィールドデータ
            new[]
            {
                " Q|Q'Q OEQ",
                " Q'QSQ'Q G",
                " Q'Q'Q|Q"
            },
            // 鼻の向き
            new[]
            {
                //new Vector3Int(0,1,0),
                new Vector3Int(-1,0,0),
                //new Vector3Int(0,0,1),
                //new Vector3Int(0,0,-1),
            }
        ),
        // ステージ3
        new StageData
        (
            "ころがる",5,
            // フィールドデータ
            new[]
            {
                "SQ' 'Q OEQ",
                " G' 'O'Q G",
                " Q'Q'Q'Q"
            },
            // 鼻の向き
            new[]
            {
                //new Vector3Int(0,1,0),
                new Vector3Int(-1,0,0),
                //new Vector3Int(0,0,1),
                //new Vector3Int(0,0,-1),
            }
        ),

    };

    public string[,] m_stage_data_ =
    {
        {
            "SQ Q",
            " Q Q O G"
        },
        {
            "SQ Q",
            " Q Q O G"
        }
    };



    //private static readonly string RESOURCE_PATH = "StageDataTable";

    //private static StageDataTable s_instance = null;
    //public static StageDataTable Instance
    //{
    //    get
    //    {
    //        if (s_instance == null)
    //        {
    //            var asset = Resources.Load(RESOURCE_PATH) as StageDataTable;
    //            if (asset == null)
    //            {
    //                // アセットが指定のパスに無い。
    //                // 誰かが勝手に移動させたか、消しやがったな！
    //                Debug.AssertFormat(false, "Missing ParameterTable! path={0}", RESOURCE_PATH);
    //                asset = CreateInstance<StageDataTable>();
    //            }

    //            s_instance = asset;
    //        }

    //        return s_instance;
    //    }
    //}

    public void Awake()
    {
        m_pref_dict.Add("S", m_pref_ntdcc);
        //m_pref_floor_dict.Add("Q", m_pref_floor_hole);
        //m_pref_floor_dict.Add("O", m_pref_floor);
        //m_pref_floor_dict.Add("G", m_pref_floor_hole);
        //Debug.Log("Awake");
    }
} // class ParameterTable