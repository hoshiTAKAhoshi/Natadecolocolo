using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Natadecoco : MonoBehaviour
{
    private enum NtdccState
    {
        IDOL = 0,
        ROTATING = 1,
        ROT_FINISH = 2,
        GOAL = 3,
        GOAL_JUMP = 4,
        
        NTDCC_STATE_MAX = 5
    }

    private enum PruState
    {
        IDLE = 0,
        PRU = 1
    }

    private enum Button
    {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3,

        SHOOT = 4,

        BUTTON_MAX
    }

    private Vector2Int[] m_to_pos_list = new Vector2Int[]
    {
        new Vector2Int( 0, -1 ),
        new Vector2Int( 0, 1 ),
        new Vector2Int( -1, 0 ),
        new Vector2Int( 1, 0 )
    };

    struct Otto
    {
        public float time;
        public AnimationCurve curve;
        public Otto(float time,AnimationCurve curve)
        {
            this.time = time;
            this.curve = curve;
        }
    }

    public Distortion m_pref_distortion_eff;            // 空間歪みエフェクト
    public Nose m_pref_nose;
    public Tama m_pref_tama;

    //[SerializeField]
    private Material m_material;    // プルプルシェーダー

    private float m_rot_time = 0.6f;                    // 回転する時間
    private Ease[] m_rot_ease = new Ease[2] { Ease.OutQuad, Ease.InQuad };
    private Vector3 m_rot = Vector3.zero;               // 回転開始から回転終了までの回転量
    private Vector3 m_fixed_rot = Vector3.zero;         // 回転終了して確定した回転量
    private Vector3 m_fixed_pos = Vector3.zero;         // 回転終了して確定した座標
    private Vector2Int m_to_pos = Vector2Int.zero;      // 移動方向のベクトル
    private Vector2Int m_pos_on_field = Vector2Int.zero;// フィールド上の座標
    private bool m_is_otto = false;
    private float m_otto_time = 0.4f;                   // 回転する時間
    private float m_otto_block_time = 0.35f;            // 回転する時間
    private float m_otto_floor_time = 0.49f;
    private float m_otto_tama_time = 0.77f;
    [SerializeField] private AnimationCurve m_otto_curve = null;
    [SerializeField] private AnimationCurve m_otto_block_curve = null;
    [SerializeField] private AnimationCurve m_otto_floor_curve = null;
    [SerializeField] private AnimationCurve m_otto_tama_curve = null;
    private string m_otto_obj = "";                     // 移動しようとした先のオブジェクト
    private Dictionary<string, Otto> m_otto_dict = new Dictionary<string, Otto>();

    private NtdccState m_state = NtdccState.IDOL;       // ナタデココの現在の状態

    // 回転終わりのプルプル
    private const float m_pru_amplitude_max = 0.2f;     // プルプルの振幅
    private float m_pru_amplitude = 0.0f;               // プルプルの振幅
    private Vector3 m_pru_dir = Vector3.zero;           // プルプルの方向
    private float m_pru_time = 0.0f;                    // プルプルする時間
    private Sequence m_seq_pru_time;                    // プルプルする時間のDOTweenのSequence
    private Sequence m_seq_pru_amplitude;               // プルプルする振幅のDOTweenのSequence
    private PruState m_pru_state = PruState.IDLE;       // プルプルの状態
    // 回転始めのもちっと
    private float m_mochi_amplitude = 0.25f;             // もちっとの振幅
    private Vector3 m_mochi_dir = Vector3.zero;         // もちっとの方向
    private float m_mochi_time = 0.0f;                  // もちっとする時間
    private Sequence m_seq_mochi_time;                  // もちっとする時間のDOTweenのSequence
    private Sequence m_seq_mochi_amplitude;             // もちっとする振幅のDOTweenのSequence

    private Sequence m_seq_rot;                         // 回転量のDOTweenのSequence

    // 弾を打った時の膨らみ
    private float m_expansion = 0.0f;
    private Sequence m_seq_expansion;
    [SerializeField] private AnimationCurve m_expansion_curve = null;

    //public Vector3Int[] m_nose_dire_def;                // 鼻の向きの定義
    private List<Nose> m_nose_list = new List<Nose>();  // 鼻のインスタンスリスト
    //private List<Vector3Int> m_nose_dire_list = new List<Vector3Int>();                   // 回転後の鼻の向き

    private Tama m_tama_inside;                         // 中に取り込んだ弾
    private bool m_is_tama_inside = false;

    private float m_goal_rot_y = 0.0f;                  // ゴール時の回転
    private float m_goal_hight = 0.0f;
    private float m_goal_pru_time = 0.0f;
    private float m_goal_pru_amplitude = 0.02f;
    [SerializeField] private AnimationCurve m_goal_rot_y_curve;
    private StageMgr m_stage_mgr;

    // Start is called before the first frame update
    void Start()
    {
        m_material = GetComponent<Renderer>().material;

        m_otto_dict.Add("",  new Otto(m_otto_time, m_otto_curve));
        m_otto_dict.Add(" ", new Otto(m_otto_time, m_otto_curve));
        m_otto_dict.Add("E", new Otto(m_otto_block_time, m_otto_block_curve));
        m_otto_dict.Add("'", new Otto(m_otto_tama_time, m_otto_tama_curve));
        m_otto_dict.Add("O", new Otto(m_otto_floor_time, m_otto_floor_curve));

        //Time.timeScale = 0.3f;
        //m_tama_inside = Instantiate(m_pref_tama);
        //m_tama_inside.SetMode(Tama.TamaMode.INSIDE);
        //m_tama_inside.SetNatadecoco(this);
        //m_is_tama_inside = true;
    }

    public void InitNose(Vector3Int[] in_nose_dire)
    {
        // 鼻を生成
        foreach (Vector3Int nose_dire in in_nose_dire)
        {
            Nose nose = Instantiate(m_pref_nose, transform.position - (Vector3)nose_dire * 0.01f, Quaternion.FromToRotation(Vector3.up, nose_dire));
            nose.transform.parent = this.transform;
            m_nose_list.Add(nose);

            nose.m_nose_dire_def = nose_dire;
            nose.m_nose_dire = nose_dire;
        }
    }

    public void SetStageMgr(StageMgr stage_mgr)
    {
        m_stage_mgr = stage_mgr;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.position);

        // 現在の姿勢を計算、反映する
        CalcTransform();

        switch (m_state)
        {
            case NtdccState.IDOL:
                {
                    // 現在押されているボタン
                    Button now_btn = PushedButton();

                    // 移動ボタンが押されていたら回転を開始する
                    if (now_btn <= Button.RIGHT)
                    {
                        StartRot(now_btn);
                    }
                    // 弾を撃つ
                    else if (now_btn == Button.SHOOT)
                    {
                        Shoot();
                    }

                }
                break;
            case NtdccState.ROT_FINISH:
                {
                    // 鼻の向きを更新
                    UpdateNoseDire();
                    // 定まった姿勢を保存
                    FixTransform();
                    // 床を沈ませる
                    m_stage_mgr.StartFloorSinking(m_pos_on_field, m_is_otto);
                    // 弾があったら弾取得
                    CheckStageObject();
                    // ゴールマスか
                    if (IsGoal(m_pos_on_field))
                    {
                        Debug.Log("ゴール");
                        PlayGoalAnim();
                        m_state = NtdccState.GOAL;
                    }
                    else
                    {
                        // プルプル開始
                        StartPru();

                        // 入力待ち状態に移行
                        m_state = NtdccState.IDOL;
                    }
                    //Debug.Log(m_stage_mgr.GetFloorData(m_pos_on_field.x, m_pos_on_field.y));

                }
                break;
            case NtdccState.GOAL:
                {
                    //m_rot.y++;
                }
                break;
            case NtdccState.GOAL_JUMP:
                {

                }
                break;
            default:
                break;
        }
        // シェーダーに情報を渡す
        ApplyShader();
    }

    // プルプルシェーダーに値を渡す
    void ApplyShader()
    {
        Vector3 puru_vec = new Vector3(m_pru_amplitude * m_pru_dir.x * Mathf.Sin(m_pru_time), 0, m_pru_amplitude * m_pru_dir.z * Mathf.Sin(m_pru_time));
        Vector3 mochi_vec = new Vector3(m_mochi_amplitude * m_mochi_dir.x * Mathf.Sin(m_mochi_time), 0, m_mochi_amplitude * m_mochi_dir.z * Mathf.Sin(m_mochi_time));
        Vector3 vec = puru_vec + mochi_vec;
        m_material.SetVector("_Amplitude", vec);
        m_material.SetFloat("_Expansion", m_expansion);
        foreach (Nose nose in m_nose_list)
        {
            nose.SetPuruDire(vec);
            nose.SetExpansion(m_expansion);
        }
    }

    // ナタデココの回転動作を開始する
    void StartRot(Button btn)
    {
        if (btn <= Button.RIGHT)
        {
            m_to_pos = m_to_pos_list[(int)btn];
        }

        m_rot = Vector3.zero;
        m_seq_rot = DOTween.Sequence();
        m_state = NtdccState.ROTATING;
        m_otto_obj = CanMove(m_to_pos);
        if (m_otto_obj == "")
        {
            m_is_otto = false;
            if (m_to_pos.y != 0)
            {
                m_seq_rot.Append(DOTween.To(() => m_rot.x, (x) => m_rot.x = x, m_rot.x - 45.0f * m_to_pos.y, m_rot_time / 2).SetEase(m_rot_ease[0]));
                m_seq_rot.Append(DOTween.To(() => m_rot.x, (y) => m_rot.x = y, m_rot.x - 90.0f * m_to_pos.y, m_rot_time / 2).SetEase(m_rot_ease[1]).OnComplete(() => { m_state = NtdccState.ROT_FINISH; }));
            }
            else if (m_to_pos.x != 0)
            {
                m_seq_rot.Append(DOTween.To(() => m_rot.z, (x) => m_rot.z = x, m_rot.z - 45.0f * m_to_pos.x, m_rot_time / 2).SetEase(m_rot_ease[0]));
                m_seq_rot.Append(DOTween.To(() => m_rot.z, (y) => m_rot.z = y, m_rot.z - 90.0f * m_to_pos.x, m_rot_time / 2).SetEase(m_rot_ease[1]).OnComplete(() => { m_state = NtdccState.ROT_FINISH; }));
            }
        }
        else
        {
            m_is_otto = true;
            Otto otto = m_otto_dict[m_otto_obj];

            if (m_to_pos.y != 0)
            {
                m_seq_rot.Append(DOTween.To(() => m_rot.x, (val) => m_rot.x = val, m_rot.x - 90.0f * m_to_pos.y, otto.time).SetEase(otto.curve).OnComplete(() => { m_state = NtdccState.ROT_FINISH; }));
            }
            else if (m_to_pos.x != 0)
            {
                m_seq_rot.Append(DOTween.To(() => m_rot.z, (val) => m_rot.z = val, m_rot.z - 90.0f * m_to_pos.x, otto.time).SetEase(otto.curve).OnComplete(() => { m_state = NtdccState.ROT_FINISH; }));
            }
        }

        m_seq_rot.Play();

        //m_seq_mochi_time.Kill();
        //m_seq_mochi_time = DOTween.Sequence();
        //m_mochi_time = 0.0f;
        //m_seq_mochi_time.Append(DOTween.To(() => m_mochi_time, (x) => m_mochi_time = x, Mathf.PI, 0.9f).SetEase(Ease.OutSine));
        //m_seq_mochi_time.Play();

        //m_mochi_dir = -m_to_pos;

        // 移動先の弾を動かす
        if(!m_is_otto)
        {
            Vector2Int new_pos = m_pos_on_field + m_to_pos;
            Tama tama = m_stage_mgr.GetStageObject(new_pos) as Tama;
            if(tama && tama.GetObjectType() == StageObjectBase.ObjectType.TAMA)
            {
                tama.SetNatadecoco(this);
            }
        }
        else
        { // おっと時に移動先のオブジェクトに干渉する
            switch(m_otto_obj)
            {
                case "E":
                    {
                        Block otto_block = m_stage_mgr.GetStageObject(m_pos_on_field + m_to_pos) as Block;
                        otto_block.SetNatadecoco(this);
                    }
                    break;
                case "'":
                    {
                        Tama tama = m_stage_mgr.GetStageObject(m_pos_on_field+m_to_pos) as Tama;
                        tama.SetNatadecoco(this);
                    }
                    break;
                case "O":
                    {

                    }
                    break;
                default:
                    break;
            }
        }

    }

    // 押されたボタンを返す
    Button PushedButton()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            return Button.UP;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.A))
            return Button.DOWN;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
            return Button.LEFT;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.S))
            return Button.RIGHT;

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.J))
            return Button.SHOOT;

        return Button.BUTTON_MAX;
    }

    // 現在のナタデココの姿勢を計算し適用する
    void CalcTransform()
    {
        transform.eulerAngles = m_fixed_rot;
        transform.Rotate(m_rot.x, m_rot.y, m_rot.z, Space.World);
        float hight = -0.5f + 1.41421356f / 2.0f * (Mathf.Sin(Mathf.Deg2Rad * (Mathf.Abs((m_rot.z + m_rot.x + 360) % 90 + 45.0f)))) + m_stage_mgr.GetFloorSinking(m_pos_on_field) + m_goal_hight;
        //transform.position = new Vector3(m_fixed_pos.x + m_to_pos.x + m_to_pos.x * (0.70710678f * Mathf.Cos(Mathf.Deg2Rad * (135 - Mathf.Abs(m_rot.z))) - 0.5f), hight, m_fixed_pos.z + m_to_pos.z + m_to_pos.z * (0.70710678f * Mathf.Cos(Mathf.Deg2Rad * (135 - Mathf.Abs(m_rot.x))) - 0.5f));
        transform.localPosition = new Vector3
            (
            m_pos_on_field.x + m_to_pos.x + m_to_pos.x * (0.70710678f * Mathf.Cos(Mathf.Deg2Rad * (135 - Mathf.Abs(m_rot.z))) - 0.5f) + m_goal_pru_amplitude*Mathf.Sin(m_goal_pru_time),
            hight,
            -m_pos_on_field.y - m_to_pos.y - m_to_pos.y * (0.70710678f * Mathf.Cos(Mathf.Deg2Rad * (135 - Mathf.Abs(m_rot.x))) - 0.5f) + m_goal_pru_amplitude * Mathf.Sin(m_goal_pru_time)
            );
        //transform.position += new Vector3(m_stage_mgr.transform.position.x,0.0f, m_stage_mgr.transform.position.z);
        //transform.position += m_stage_mgr.transform.position;
    }

    // ナタデココを揺らす動作を開始する
    void StartPru()
    {
        Vector3 new_pru_dir = new Vector3(m_to_pos.x, 0.0f, -m_to_pos.y);
        if (m_is_otto) new_pru_dir *= -1;

        if (m_pru_state == PruState.IDLE)
        {   // プルが終わっていたら
            m_pru_dir = new_pru_dir;
        }
        else
        {   // プルの途中だったら
            //DOTween.To(() => m_pru_dir, (x) => m_pru_dir = x, new_pru_dir, 0.0001f);
            m_pru_dir = new_pru_dir;
        }

        StartPruDOTween(3.5f, 0.9f);
        m_pru_state = PruState.PRU;

        //Debug.Log("Pru");
    }

    void StartPruDOTween(float period, float time, float amplitude = m_pru_amplitude_max)
    {
        m_seq_pru_amplitude.Kill();
        m_seq_pru_amplitude = DOTween.Sequence();
        m_pru_amplitude = amplitude;
        //Debug.Log(amplitude);
        m_seq_pru_amplitude.Append(DOTween.To(() => m_pru_amplitude, (x) => m_pru_amplitude = x, 0, time).SetEase(Ease.OutQuad));
        m_seq_pru_amplitude.Play();

        m_seq_pru_time.Kill();
        m_seq_pru_time = DOTween.Sequence();
        if (m_pru_state == PruState.IDLE)
        {
            m_pru_time = 0.0f;
        }
        else
        {
            //while(true)
            //{
            //    m_pru_time -= 2 * Mathf.PI;
            //    if (m_pru_time <= 0)
            //        break;
            //}
            m_pru_time = 0.0f;

        }
        m_seq_pru_time.Append(DOTween.To(() => m_pru_time, (x) => m_pru_time = x, period * Mathf.PI, time).SetEase(Ease.Linear)).OnComplete(() => { CompletePru(); });
        m_seq_pru_time.Play();
    }

    void CompletePru()
    {
        m_pru_state = PruState.IDLE;
        Debug.Log("Complete Pru");
    }

    // 回転後の姿勢を保存
    void FixTransform()
    {
        //m_fixed_pos = transform.position;
        if (!m_is_otto)
            m_pos_on_field += m_to_pos;
        m_fixed_rot = transform.eulerAngles;
        m_rot = Vector3.zero;
    }

    void CheckStageObject()
    {
        string data = m_stage_mgr.GetObjectData(m_pos_on_field);
        //Debug.Log(data);
        switch (data)
        {
            case "'":
                //Debug.Log("Tama");
                //m_stage_mgr.TamaIntoNtdcc(m_pos_on_field, this);
                if (!m_is_tama_inside)
                {
                    m_tama_inside = (Tama)m_stage_mgr.GetStageObject(m_pos_on_field);
                    m_tama_inside.SetNatadecoco(this);
                    m_tama_inside.SetMode(Tama.TamaMode.INSIDE);
                    m_tama_inside.PlayInsideScaleAnim(m_to_pos);
                    m_is_tama_inside = true;
                    m_stage_mgr.ReplaceStageObjectData(m_pos_on_field, " ");
                    //Debug.Log("yes");
                }
                else
                {
                    //zDebug.Log("no");
                }
                break;
            default:
                break;
        }
        
    }

    // ゴールマスか調べる
    bool IsGoal(Vector2Int pos)
    {
        string data = m_stage_mgr.GetFloorData(pos);
        if(data == "G")
        {
            return true;
        }
        return false;
    }

    // 弾を撃つ
    void Shoot()
    {
        // 膨らむアニメーション
        m_seq_expansion.Kill();
        m_seq_expansion = DOTween.Sequence();
        m_expansion = 0.0f;
        m_seq_expansion.Append(DOTween.To(() => m_expansion, (x) => m_expansion = x, 0.15f, 0.5f).SetEase(m_expansion_curve));
        m_seq_expansion.Play();

        m_pru_dir = Vector3.zero;

        if (m_is_tama_inside)
        {// 弾を撃つ
            //Debug.Log(m_tama_inside);
            Destroy(m_tama_inside.gameObject);
            m_is_tama_inside = false;
            //Instantiate(m_pref_distortion_eff, m_fixed_pos + new Vector3(0.0f, 0.0f, 0.8f), m_pref_distortion_eff.transform.rotation);
            //Quaternion inverse = Quaternion.AngleAxis(180.0f, new Vector3(1.0f, 0.0f, 0.0f));// Quaternion.Euler(0.0f,180.0f,0.0f);
            foreach (Nose nose in m_nose_list)
            {
                Instantiate(m_pref_distortion_eff, nose.GetEffPos(), nose.GetEffRot());
                m_pru_dir += -nose.m_nose_dire;

                Tama tama = Instantiate(m_pref_tama, transform.position + ((Vector3)nose.m_nose_dire) * 0.3f, Quaternion.identity);
                tama.SetMode(Tama.TamaMode.SHOT);
                tama.SetShotDire(nose.m_nose_dire);
                tama.SetStageMgr(m_stage_mgr);
            }
            m_pru_dir = m_pru_dir.normalized * 1.3f;
            StartPruDOTween(3.5f, 0.9f);
        }
        else
        {// 空撃ち
            foreach (Nose nose in m_nose_list)
            {
                m_pru_dir += nose.m_nose_dire;
            }
            m_pru_dir = m_pru_dir.normalized * 0.7f;
            StartPruDOTween(2.5f, 0.6f);
        }

        // 撃ち終わりに弾があったら弾を取得
        CheckStageObject();
    }

    public void SetPos(Vector2Int pos)
    {
        m_pos_on_field = pos;
    }

    // 鼻の向きを更新する
    void UpdateNoseDire()
    {
        foreach(Nose nose in m_nose_list)
        {
            Vector3 vec = Quaternion.Euler(m_rot.x, 0, m_rot.z) * nose.m_nose_dire;
            nose.m_nose_dire = new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
            //Debug.Log(nose.m_nose_dire);
        }
    }

    // 移動できるか("":移動できる, それ以外:移動できない
    string CanMove(Vector2Int dire)
    {
        //Debug.Log(dire);
        string floor = m_stage_mgr.GetFloorData(m_pos_on_field + dire);

        // 弾が入ってて移動先が弾
        if(m_is_tama_inside)
        {
            string obj = m_stage_mgr.GetObjectData(m_pos_on_field + dire);
            if (obj == "'")
                return "'";
        }

        // ブロック
        {
            string obj = m_stage_mgr.GetObjectData(m_pos_on_field + dire);
            if (obj == "E")
                return "E";

        }
        if (floor != " ")
        {
            if (floor == "O")
            {
                foreach (Nose nose in m_nose_list)
                {
                    //Debug.Log("nose:" + nose.m_nose_dire);
                    if (nose.m_nose_dire == new Vector3Int(dire.x, 0, -dire.y))
                        return "O"; // 鼻、移動先が穴なし床
                }
            }
        }
        else
        {
            return " "; // 移動先に床がない
        }

        return "";
    }

    // ゴールしたときの動きを再生
    void PlayGoalAnim()
    {
        m_stage_mgr.AddForceBgCube(Camera.main.WorldToScreenPoint(transform.position), 2.0f);
        m_pru_dir = new Vector3(m_to_pos.x, 0.0f, -m_to_pos.y);
        StartPruDOTween(1.0f, 0.5f, 1.0f);
        //DOTween.To(() => m_pru_time, (x) => m_pru_time = x, period * Mathf.PI, time).SetEase(Ease.Linear);

        // 回転
        Sequence seq_goal_rot = DOTween.Sequence();
        float time0 = 0.3f;
        seq_goal_rot.Append(DOTween.To(() => m_rot.y, (x) => m_rot.y = x, m_rot.y + 30.0f, time0).SetEase(Ease.InQuint));//SetEase(m_goal_rot_y_curve);
        seq_goal_rot.Append(DOTween.To(() => m_rot.y, (x) => m_rot.y = x, m_rot.y + 360.0f, 1.5f- time0).SetEase(Ease.OutQuint));//SetEase(m_goal_rot_y_curve);

        // 時間経過遅く
        m_stage_mgr.PlayGoalAnim();

        // つぶす
        Vector3 up_vec = Vector3.up;
        up_vec = transform.rotation * up_vec;
        Ease curve = Ease.OutSine;
        float ease_time = 1.7f;
        float delay_time = 0.3f;
        float yoko = 1.15f;
        float tate = 0.7f;
        float sx = yoko - Mathf.Abs(up_vec.x) * (yoko - tate);
        float sy = yoko - Mathf.Abs(up_vec.y) * (yoko - tate);
        float sz = yoko - Mathf.Abs(up_vec.z) * (yoko - tate);
        transform.DOScale(new Vector3(sx, sy, sz), ease_time).SetEase(curve).SetDelay(delay_time).OnComplete(() => { PlayGoalJumpAnim(); });
        // つぶした分だけ下げる
        DOTween.To(() => m_goal_hight, (x) => m_goal_hight = x, -(1.0f-tate)/2, ease_time).SetEase(curve).SetDelay(delay_time);
        
        DOTween.To(() => m_goal_pru_time, (x) => m_goal_pru_time = x, 16 * Mathf.PI, ease_time).SetEase(Ease.InSine).SetDelay(delay_time);
        m_goal_pru_amplitude = 0.0f;
        DOTween.To(() => m_goal_pru_amplitude, (x) => m_goal_pru_amplitude = x, 0.01f, ease_time).SetEase(Ease.InSine).SetDelay(delay_time);

        //transform.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), 0.8f);
    }

    void PlayGoalJumpAnim()
    {
        m_state = NtdccState.GOAL_JUMP;
        DOTween.To(() => m_goal_hight, (x) => m_goal_hight = x, 15.0f, 3.0f).SetEase(Ease.OutQuart);

        Vector3 up_vec = Vector3.up;
        up_vec = transform.rotation * up_vec;
        Ease curve = Ease.OutCubic;
        float ease_time = 0.6f;
        float yoko = 0.9f;
        float tate = 1.5f;
        float sx = yoko - Mathf.Abs(up_vec.x) * (yoko - tate);
        float sy = yoko - Mathf.Abs(up_vec.y) * (yoko - tate);
        float sz = yoko - Mathf.Abs(up_vec.z) * (yoko - tate);
        transform.DOScale(new Vector3(sx, sy, sz), ease_time).SetEase(curve);
    }

    public Vector3 GetCenterPos()
    {
        return transform.position + GetPruVec()*0.7f;
    }

    public Vector3 GetPruVec()
    {
        return m_pru_dir * m_pru_amplitude * Mathf.Sin(m_pru_time);
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    // 0~1に変換
    public float GetPruAmplitude()
    {
        return m_pru_amplitude/ m_pru_amplitude_max;
    }

    public Vector3 GetRot()
    {
        return m_rot;
    }

    public Vector3 GetPruDire()
    {
        return m_pru_dir;
    }

    public Vector2Int GetToPos()
    {
        return m_to_pos;
    }
}
