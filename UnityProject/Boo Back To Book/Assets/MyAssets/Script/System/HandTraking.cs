using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandTraking : MonoBehaviour
{
    //LightLine参照
    public static HandTraking Instance { get; private set; }

    [SerializeField] private TextMeshPro debugLog;
    [Header("0..左手 1..右手")]
    //掌
    [SerializeField] private GameObject[] palms;
    //親指第一関節
    [SerializeField] private GameObject[] thumbs;
    //中指と人差し指の第2関節
    [SerializeField] private GameObject[] indexs;
    [SerializeField] private GameObject[] middles;
    //中指の先
    [SerializeField] private GameObject[] middleTips;
    //小指の付け根
    [SerializeField] private GameObject[] pinkys;

    float PtoMdistance = 0.1f;

    private bool leftishold;
    private bool rightishold;
    private bool leftslap;
    private bool rightslap;
    private bool leftrub;
    private bool rightrub;
    private void Awake()
    {
        //シングルトン初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        leftishold = false;
        rightishold = false;
        leftslap = false;
        rightslap = false;
        leftrub = false;
        rightrub = false;
    }

    // Update is called once per frame
    void Update()
    {
        //float rot = Vector3.Angle(palms[0].transform.forward, Vector3.up);
        //debugLog.text = "$" + rot;

        HandCheck();
    }

    Vector3 leftlastPos;
    Vector3 rightlastPos;
    private void HandCheck()
    {
        //握る挙動//
        float left_hold_distance = Vector3.Distance(palms[0].transform.position, middleTips[0].transform.position);
        float right_hold_distance = Vector3.Distance(palms[1].transform.position, middleTips[1].transform.position);

        //debugLog.text = "left" + leftishold;

        if (left_hold_distance < PtoMdistance)
        {
            leftishold = true;
        }
        else
        {
            leftishold = false;
        }

        if (right_hold_distance < PtoMdistance)
        {
            rightishold = true;
        }
        else
        {
            rightishold = false;
        }
        //握る挙動//

        //叩く挙動//
        //debugLog.text = "left" + leftslap;

        //現在の位置を保存
        Vector3 leftcurrentPos = palms[0].transform.position;
        Vector3 rightcurrentPos = palms[1].transform.position;
        //それぞれの手の早さ
        Vector3 leftvelocity = (leftcurrentPos - leftlastPos) / Time.deltaTime;
        Vector3 rightvelocity = (rightcurrentPos - rightlastPos) / Time.deltaTime;

        //下向きにたたいているか
        if (leftlastPos.y - leftcurrentPos.y > 0.01f)
        {
            if (leftvelocity.magnitude > 1.5f)
            {
                leftslap = true;
            }
        }
        else
        {
            leftslap = false;
        }

        if (rightlastPos.y - rightcurrentPos.y > 0.01f)
        {
            if (rightvelocity.magnitude > 1.5f)
            {
                rightslap = true;
            }
        }
        else
        {
            rightslap = false;
        }

        //位置を保存
        leftlastPos = leftcurrentPos;
        rightlastPos = rightcurrentPos;

        //叩く挙動//

        //こする挙動//
        if (leftvelocity.magnitude > 1.5f)
        {
            leftrub = true;
        }
        else
        {
            leftrub = false;
        }

        if (rightvelocity.magnitude > 1.5f)
        {
            rightrub = true;
        }
        else
        {
            rightrub = false;
        }
        //こする挙動//
    }

    //握る挙動//
    public bool GetLeftHold()
    {
        return leftishold;
    }
    public bool GetRightHold() 
    { 
        return rightishold; 
    }
    //握る挙動//

    //叩く挙動//
    public bool GetLeftSlap()
    {
        return leftslap;
    }
    public bool GetRightSlap()
    {
        return rightslap;
    }
    //叩く挙動//

    //こする挙動//
    public bool GetLeftRub()
    {
        return leftrub;
    }
    public bool GetRightRub()
    {
        return rightrub ;
    }
    //こする挙動//

    //手の位置//
    public Vector3 GetLeftHandPos()
    {
        Vector3 pos = palms[0].transform.position;

        return pos;
    }
    public Vector3 GetRightHandPos()
    {
        Vector3 pos = palms[1].transform.position;

        return pos;
    }
    //手の位置//

    //親指の位置//
    public Vector3 GetLeftHandThumbPos()
    {
        Vector3 pos = thumbs[0].transform.position;

        return pos;
    }
    public Vector3 GetRightHandThumbPos()
    {
        Vector3 pos = thumbs[1].transform.position;

        return pos;
    }
    //親指の位置//

    //人差し指の位置//
    public Vector3 GetLeftHandIndexPos()
    {
        Vector3 pos = indexs[0].transform.position;

        return pos;
    }
    public Vector3 GetRightHandIndexPos()
    {
        Vector3 pos = indexs[1].transform.position;

        return pos;
    }
    //人差し指の位置//

    //小指の位置//
    public Vector3 GetLeftHandPinkyPos()
    {
        Vector3 pos = pinkys[0].transform.position;

        return pos;
    }
    public Vector3 GetRightHandPinkyPos()
    {
        Vector3 pos = pinkys[1].transform.position;

        return pos;
    }
    //小指の位置//

    //手先の方向//
    public Vector3 GetLeftHandUp()
    {
        Vector3 rot = palms[0].transform.forward.normalized;

        return rot;
    }
    public Vector3 GetRightHandUp()
    {
        Vector3 rot = palms[1].transform.forward.normalized;

        return rot;
    }
    //手先の方向//

    //手の甲の方向//
    public Vector3 GetLeftHandBack()
    {
        Vector3 rot = palms[0].transform.up.normalized;

        return rot;
    }
    public Vector3 GetRightHandBack()
    {
        Vector3 rot = palms[1].transform.up.normalized;

        return rot;
    }
    //手の甲の方向//
}
