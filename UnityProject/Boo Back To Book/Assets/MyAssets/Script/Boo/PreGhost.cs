using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GhostManager;

public class PreGhost : MonoBehaviour
{
    private GameObject player;
    private float time;
    private bool ableCountDown;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        time = 0f;
        ableCountDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            Destroy(gameObject);
            //ableCountDown = true;
        }

        GhostControll();
    }

    //半径
    private float r = 0.25f;//(本番は)0.2f;
    //オブジェクトの「現在の角度位置」を制御する独立した変数（時間とともに変化）
    //プレイヤーからおばけまでひいた線とX軸でできる角度＜＜ではない！！＞＞
    //とりあえず7/22の時点では90度（正面）スタート
    private float angle = 90f;
    //まわるスピード
    private float speed = 1.0f;
    private float upDownSpeed = 0.3f;
    //後ろに行った時と前にいるとき
    //stateでやるか？
    private bool isBack;
    private bool isFront;
    private bool isIn;
    private bool isOut;
    private void GhostControll()
    {
        //お化けが存在できるX座標の最小と最大（没）
        //float xmin = middle.x - r;   
        //float xmax = middle.x + r;

        //円の真ん中
        Vector3 middle = player.transform.position;
        //Debug.Log(middle);

        //お化けが存在できるY座標の最小と最大
        float minY = middle.y + 0.01f;
        float maxY = middle.y + 0.2f;
        //マイナスで右回り
        angle -= speed * Time.deltaTime;
        //後ろに行ったら方向を変える（一回だけ）
        if (isBack)
        {
            speed *= -1.0f;
            isBack = false;
        }

        //↓↓お化けのポジション設定↓↓
        float ghostposX = middle.x + Mathf.Cos(angle) * r;
        float ghostposZ = middle.z + Mathf.Sin(angle) * r;

        //上下の存在可能範囲に入ってる
        if (transform.position.y >= minY && transform.position.y <= maxY)
        {
            isIn = true;
            isOut = false;
        }
        //入っていない
        else if (transform.position.y < minY|| transform.position.y > maxY)
        {
            if (isIn)
            {
                upDownSpeed *= -1.0f;
                isOut = true;
                isIn = false;
            }
        }

        float ghostposY = transform.position.y + (upDownSpeed * Time.deltaTime);
           

        //プレイヤーよりも後ろにいったら
        if (transform.position.z < middle.z)
        {
            if (isFront)
            {
                isBack = true;
                isFront = false;
            }
        }
        //前にいる
        else
        {
            isFront = true;
        }

        transform.position = new Vector3(ghostposX, ghostposY, ghostposZ);
    }
}
