using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMgr : MonoBehaviour
{
    [SerializeField]
    private StageDataTable m_table = null;
    [SerializeField]
    private Natadecoco m_pref_ntdcc = null;
    [SerializeField]
    private Tama m_pref_tama = null;
    [SerializeField]
    private Block m_pref_block = null;

    [SerializeField]
    private Floor m_pref_floor = null;
    [SerializeField]
    private Floor m_pref_floor_hole = null;
    [SerializeField]
    private Floor m_pref_floor_hole_goal = null;

    private List<string> m_floor_data = new List<string>();
    private Dictionary<Vector2Int, Floor> m_floor_list = new Dictionary<Vector2Int, Floor>();
    private List<string> m_object_data = new List<string>();
    private Dictionary<Vector2Int, StageObjectBase> m_object_list = new Dictionary<Vector2Int, StageObjectBase>();
    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0.0f, -24.0f, 0.0f);
        CreateStage(0);
        Debug.Log(UnityEditorInternal.InternalEditorUtility.unityPreferencesFolder);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateStage(int stage_num)
    {
        StageDataTable.StageData stage_data = m_table.m_stage_data[stage_num];
        Debug.Log(stage_data.stage_name);
        for (int y = 0; y < stage_data.field_data.Length; y++)
        {
            m_floor_data.Add(null);
            m_object_data.Add(null);
            for (int x = 0; x < stage_data.field_data[y].Length; x++)
            {
                string data = stage_data.field_data[y].Substring(x, 1);
                if (x % 2 == 0)
                {//オブジェクト
                 //GameObject pref = m_table.m_pref_dict[data];
                    switch (data)
                    {
                        case "S":
                            Natadecoco obj = Instantiate(m_pref_ntdcc, new Vector3((int)(x / 2), 0.0f, -y), Quaternion.identity);
                            obj.SetPos(new Vector2Int((int)(x / 2), -y));
                            obj.InitNose(stage_data.nose_dire);
                            obj.SetStageMgr(this);
                            obj.transform.parent = this.transform;
                            break;
                        case "'":
                            //Debug.Log(new Vector2Int(x / 2, y));
                            Tama tama = Instantiate(m_pref_tama, new Vector3((int)(x / 2), 0.0f, -y), Quaternion.identity);
                            tama.SetObjectType(StageObjectBase.ObjectType.TAMA);
                            tama.SetPosOnField(new Vector2Int(x / 2, -y));
                            tama.SetStageMgr(this);
                            tama.transform.parent = this.transform;
                            m_object_list.Add(new Vector2Int(x / 2, y), tama);

                            break;
                        case "E":
                            Block block = Instantiate(m_pref_block, new Vector3((int)(x / 2), -0.1f, -y), Quaternion.identity);
                            block.transform.parent = this.transform;
                            m_object_list.Add(new Vector2Int(x / 2, y), block);
                            break;
                        default:
                            break;
                    }
                    m_object_data[y] += data;
                }
                else
                {//床
                 //Floor pref = m_table.m_pref_floor_dict[data];
                    switch (data)
                    {
                        case "Q":
                            {
                                Floor floor = Instantiate(m_pref_floor_hole, new Vector3((int)(x / 2), -0.501f, -y), Quaternion.identity);
                                floor.transform.parent = this.transform;
                                m_floor_list.Add(new Vector2Int(x / 2, y), floor);
                                break;
                            }
                        case "O":
                            {
                                Floor floor = Instantiate(m_pref_floor, new Vector3((int)(x / 2), -0.501f, -y), Quaternion.identity);
                                floor.transform.parent = this.transform;
                                m_floor_list.Add(new Vector2Int(x / 2, y), floor);
                                break;
                            }
                        case "G":
                            {
                                Floor floor = Instantiate(m_pref_floor_hole_goal, new Vector3((int)(x / 2), -0.501f, -y), Quaternion.identity);
                                floor.transform.parent = this.transform;
                                m_floor_list.Add(new Vector2Int(x / 2, y), floor);
                                break;
                            }
                        default:
                            break;
                    }
                    m_floor_data[y] += data;
                }
                
            }
        }
    }

    public string GetFloorData(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;

        // 範囲外
        if (x < 0 || y < 0)
            return " ";
        if (y >= m_floor_data.Count)
            return " ";
        if (x >= m_floor_data[y].Length)
            return " ";

        return m_floor_data[y].Substring(x, 1);
    }

    public string GetObjectData(Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;

        // 範囲外
        if (x < 0 || y < 0)
            return " ";
        if (y >= m_object_data.Count)
            return " ";
        if (x >= m_object_data[y].Length)
            return " ";

        return m_object_data[y].Substring(x, 1);
    }

    public void ReplaceStageObjectData(Vector2Int pos, string data)
    {
        //Debug.Log(m_object_data[pos.y]);
        m_object_data[pos.y] = m_object_data[pos.y].Remove(pos.x, 1).Insert(pos.x, data);
        //Debug.Log(m_object_data[pos.y]);
    }

    public void TamaIntoNtdcc(Vector2Int pos, Natadecoco ntdcc)
    {
        Tama tama = (Tama)m_object_list[pos];
        tama.SetMode(Tama.TamaMode.INSIDE);
        tama.SetNatadecoco(ntdcc);
    }

    public StageObjectBase GetStageObject(Vector2Int pos)
    {
        if (m_object_list.ContainsKey(pos))
        {
            return m_object_list[pos];
        }
        return null;
    }


    public void StartFloorSinking(Vector2Int pos, bool is_otto)
    {
        //Debug.Log("Sinking");
        if (m_floor_list.ContainsKey(pos))
        {
            Floor floor = m_floor_list[pos];
            if (floor)
            {
                floor.StartSinking(is_otto);
            }
        }
    }

    public float GetFloorSinking(Vector2Int pos)
    {
        if (m_floor_list.ContainsKey(pos))
        {
            Floor floor = m_floor_list[pos];
            if (floor)
            {
                return floor.GetSinking();
            }
        }
        return 0.0f;
    }
}
