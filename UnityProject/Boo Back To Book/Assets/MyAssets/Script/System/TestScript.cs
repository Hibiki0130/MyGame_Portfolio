using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static SoundManager;

public class TestScript : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject book;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TestScript が動いています！");
    }

    // Update is called once per frame
    void Update()
    {
        //Sounds();
        HandContoller();
        BookRotation();
        GetMiddle();
        MakeRay();
    }

    private float value = 0.01f;
    private void HandContoller()
    {
        //最初の位置に戻す
        if(Input.GetKey(KeyCode.F))
        {
            leftHand.transform.position = new Vector3(-1.0f, 0f, 0f);
            rightHand.transform.position = new Vector3(1.0f, 0f, 0f);
        }

        //左手
        //前後左右上下
        if(Input.GetKey(KeyCode.W))
        {
            leftHand.transform.position += new Vector3(0f, 0f, value);
        }
        if (Input.GetKey(KeyCode.S))
        {
            leftHand.transform.position -= new Vector3(0f, 0f, value);
        }
        if (Input.GetKey(KeyCode.A))
        {
            leftHand.transform.position -= new Vector3(value, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            leftHand.transform.position += new Vector3(value, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            leftHand.transform.position += new Vector3(0f, value, 0f);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            leftHand.transform.position -= new Vector3(0f, value, 0f);
        }

        //右手
        //前後左右上下
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rightHand.transform.position += new Vector3(0f, 0f, value);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rightHand.transform.position -= new Vector3(0f, 0f, value);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rightHand.transform.position -= new Vector3(value, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightHand.transform.position += new Vector3(value, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.P))
        {
            rightHand.transform.position += new Vector3(0f, value, 0f);
        }
        if (Input.GetKey(KeyCode.L))
        {
            rightHand.transform.position -= new Vector3(0f, value, 0f);
        }
    }

    //手の真ん中とる
    Vector3 middleOfHands;
    Vector3 Pos;
    private void GetMiddle()
    {
        middleOfHands = (leftHand.transform.position + rightHand.transform.position) / 2;
        book.transform.position = middleOfHands;
        //中心がずれているので手直し
        //Pos += new Vector3(0.05f, 0, 0);
    }

    private void Sounds()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            Debug.Log("Z!!");
            SoundManager.Instance.PlaySE(clip);
        }
    }

    private float rotationSpeed = 5.0f;
    private void BookRotation()
    {
        // X軸をdirectionに合わせる回転を計算（Y軸は上向き）
        //Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        //book.transform.rotation = Quaternion.LookRotation(Vector3.forward); // 一旦デフォルト方向
        // directionがX軸（右向き）になるようにQuaternionを設定
        //book.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up); // 初期化
        //float angle = Vector3.Angle(hand, book.transform.right);
        /*Vector3 toup = Vector3.Cross(leftHand.transform.position, rightHand.transform.position);
       book.transform.rotation = Quaternion.FromToRotation(Vector3.right, hand);
       book.transform.rotation = Quaternion.FromToRotation(Vector3.forward, toup);*/

        Vector3 hand = (leftHand.transform.position - rightHand.transform.position).normalized;

        //本のZ軸を決める
        //二つのベクトルと直行（垂直）になるベクトルを求める
        Vector3 zAxis = Vector3.Cross(Vector3.down, hand).normalized;

        //本のY軸を zAxis と hand の外積で作る（直交するベクトル）
        Vector3 yAxis = Vector3.Cross(zAxis, hand).normalized;

        // 回転を作る（X軸 = hand, Y軸 = yAxis）
        Quaternion rotation = Quaternion.LookRotation(zAxis, yAxis);
        book.transform.rotation = Quaternion.Slerp(book.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }

    Vector3 raydirection = Vector3.forward;
    private void MakeRay()
    {
        UnityEngine.Ray ray = new UnityEngine.Ray(leftHand.transform.position, raydirection);
        Debug.DrawRay(leftHand.transform.position, raydirection * 30);
    }
}
