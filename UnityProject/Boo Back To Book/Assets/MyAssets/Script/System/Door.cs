using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private GameObject lookAround;
    [SerializeField] private GameObject hallWay;
    [Header("SE")]
    [SerializeField] private AudioClip Open;
    [SerializeField] private AudioClip Close;
    private enum DoorState
    {
        Open,
        Close,
    }
    private DoorState state;

    Rigidbody rb;
    BoxCollider bc;
    public bool IsInRoom {  get; private set; }

    private bool firstOpen;
    private bool closeSE;
    private float firstOpenAngle;

    Vector3 rot = new Vector3();
    Quaternion targetRot;
    Quaternion closeRot;

    PlaySound playsound;
    LookAround lookaround;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChange += OpenSound;

        IsInRoom = false;
        firstOpen = false;
        closeSE = false;
        firstOpenAngle = 35f;

       state = DoorState.Close;

        rb = GetComponent<Rigidbody>();
        bc= GetComponent<BoxCollider>();
        playsound = GetComponent<PlaySound>();
        lookaround = lookAround.GetComponent<LookAround>();

        bc.enabled = false;

        //最初開ける角度
        targetRot = Quaternion.Euler(0f, firstOpenAngle, 0f);
        //ここまであけたらフリーズ
        closeRot = Quaternion.Euler(0f, 90f, 0f);
    }
    //
    // Update is called once per frame
    void Update()
    {
        rot = transform.eulerAngles;

        //チュートリアルが終わったら動ける
        if (state == DoorState.Open)
        {
            //フリーズ解除
            rb.constraints = RigidbodyConstraints.None;
            closeSE = true;

            CheckAngle();
        }
        else if (state == DoorState.Close)
        {
            if(closeSE)
            {
                playsound.PlayOneShot(Close);
                closeSE = false;
            }

            transform.rotation = closeRot;
            Freeze();
        }

        if(GameManager.Instance.state==GameState.LookAround)
        {
            if(IsInRoom)
            {
                hallWay.SetActive(false);
            }
        }
    }
    //
    //
    private void Freeze()
    {
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        bc.enabled = false;
    }
    private void CheckAngle()
    {
        //最初ちょっと開けるor入ってないのに閉められたら開ける
        if (!firstOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);

            if (rot.y <= firstOpenAngle)
            {
                firstOpen = true;
            }
        }
        else
        {
            //入ってないのに閉じちゃった
            if (!IsInRoom)
            {
                if (rot.y >= 90f)
                {
                    firstOpen = false;
                }
            }
            //開け切った
            if (rot.y <= 0f)
            {
                Freeze();
            }
        }
    }
    private void OpenSound(GameState newState)
    {
        if (newState == GameState.LookAround)
        {
            state = DoorState.Open;
            playsound.PlayOneShot(Open);
            bc.enabled = true;
        }
    }
    public void CloseDoor()
    {
        firstOpen = true;

        if (state == DoorState.Close)
        {
            IsInRoom = true;
        }
        else
        {
            if (!IsInRoom)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, closeRot, Time.deltaTime * 2f);

                if (rot.y >= 80f)
                {
                    playsound.PlayOneShot(Close);
                    transform.rotation = closeRot;
                    IsInRoom = true;
                }
            }
            else
            {
                state = DoorState.Close;
            }
        }
    }
}
