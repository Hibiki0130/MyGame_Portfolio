using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    [SerializeField] private GameObject kirakiraEffect;
    [SerializeField] private GameObject attentionBack;
    [SerializeField] private AudioClip catchAGhost;
    [SerializeField] private AudioClip closeABook;
    private enum HandState
    {
        None,
        Open,
        Close,
        Finish,
    }
    private HandState state;

    private bool closeSound;
    private bool ableCatch;
    private float thumb_Distance;
    private float index_Distance;

    PlaySound playsound;
    AttentionManager attentionmanager;
    // Start is called before the first frame update
    void Start()
    {
        ableCatch = false;
        closeSound = true;
        ChangeMyState(HandState.Open);
        playsound = GetComponent<PlaySound>();
        attentionmanager = attentionBack.GetComponent<AttentionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.Attention)
        {
            if (state != HandState.Finish)
            {
                GetMiddle();
                OpenAndClose();
            }
        }
    }
    private void ChangeMyState(HandState newState)
    {
        if (newState == state) return;

        switch (newState)
        {
            case HandState.Open:
                closeSound = true;
                break;
            case HandState.Close:
                ableCatch = true;
                StartCoroutine(SetAbleCatch());
                break;
            default:
                break;
        }
        state = newState;
    }
    Vector3 Pos;
    private void GetMiddle()
    {
        Pos = (HandTraking.Instance.GetLeftHandPinkyPos()
            + HandTraking.Instance.GetRightHandPinkyPos()) / 2;
        Pos.y += 0.05f;

        transform.position = Pos;
        //Debug.Log("mannaka");
    }
    private void OpenAndClose()
    {
        //薬指同士がどの程度近づいているか
        index_Distance = Vector3.Distance(HandTraking.Instance.GetLeftHandIndexPos(), HandTraking.Instance.GetRightHandIndexPos());
        //debugLog.text = "" + index_Distance;

        //親指同士がどの程度近づいているか
        thumb_Distance = Vector3.Distance(HandTraking.Instance.GetLeftHandThumbPos(), HandTraking.Instance.GetRightHandThumbPos());
        //debugLog.text = "" + thumb_Distance;

        //近づいているとき
        //本が閉じる
        if (thumb_Distance <= 0.1f)
        {
            ChangeMyState(HandState.Close);
        }
        //離れているとき
        //本が開く
        else if (thumb_Distance > 0.12f)
        {
            ChangeMyState(HandState.Open);
        }
    }
    private IEnumerator SetAbleCatch()
    {
        //閉じてから2秒たったら捕まえ無効
        yield return new WaitForSeconds(2f);

        ableCatch = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (state != HandState.Finish)
        {
            if (ableCatch)
            {
                if (other.gameObject.CompareTag("Boo"))
                {
                    closeSound = true;
                    Instantiate(kirakiraEffect, other.transform.position, Quaternion.identity);
                    if (closeSound)
                    {
                        playsound.PlayOneShot(catchAGhost);
                        closeSound = false;
                    }
                    Destroy(other.gameObject);
                    attentionmanager.StateChangeSet();
                    ChangeMyState(HandState.Finish);
                }
                else
                {
                    /*if (closeSound)
                    {
                        playsound.PlayOneShot(closeABook);
                        closeSound = false;
                    }*/
                }
            }
        }
    }
}
