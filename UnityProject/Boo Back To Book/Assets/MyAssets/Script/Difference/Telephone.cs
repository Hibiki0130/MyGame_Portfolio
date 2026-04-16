using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Telephone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugLog;
    [SerializeField] private GameObject handSet;
    [SerializeField] private GameObject handSetGhost;

    [Header("効果音")]
    [SerializeField] private AudioClip ringtone;
    [SerializeField] private AudioClip hangdown;

    private bool ring;
    PlaySound playsound;

    private float time = 0f;
    private GameObject handGrab;
    private ConfigurableJoint joint;
    private Rigidbody handSetRB;

    // Start is called before the first frame update
    void Start()
    {
        ring = false;
        playsound = GetComponent<PlaySound>();
        TutorialManager.Instance.OnTutorialStateChange += RingPhone;

        //joint.connectedBody = null;
        /*handGrab = handSet.transform.GetChild(0).gameObject;
        handGrab.SetActive(true);
        handSetRB = handSet.GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();*/
    }

    // Update is called once per frame
    void Update()
    {
        RingController();
    }

    private void RingPhone(TutorialState newstate)
    {
        if (newstate == TutorialState.TelePhone)
        {
            playsound.Play(ringtone);
            ring = true;
        }
    }

    private void RingController()
    {
        if (!ring)
        {
            playsound.Stop();
        }
    }
    public void StopRing()
    {
        ring = false;
    }

    public void PlayDown()
    {
        playsound.PlayOneShot(hangdown);
    }

    //dontuse//

    /*private void CheckDistance()
    {
        float subtitleDistance = Vector3.Distance(handSet.transform.position, handSetGhost.transform.position);

        //受話器が受け皿に乗っているか
        if (subtitleDistance < 0.1f)
        {
            float anglediff = Quaternion.Angle(handSet.transform.rotation, handSetGhost.transform.rotation);
            //正解（傾き）
            if (anglediff < 5f)
            {
                //joint.connectedBody = null;
                //handGrab.SetActive(false);

                //位置揃え
                //handSet.transform.position = handSetGhost.transform.position;
                //handSet.transform.rotation = handSetGhost.transform.rotation;

               isHangUp = false;

                //デストロイしたら何回もできなくなるからそのまま
                /*Destroy(handSetGhost);
                handSetGhost = null;
            }
            else
            {
                joint.connectedBody = handSetRB;
                isHangUp = true;
            }
        }
        else
        {
            joint.connectedBody = handSetRB;
            isHangUp = true;
        }
    }*/

    //Debug.Log(isHangUp);
    //CheckDistance();

    //持ち上げられたら
    /*if (handSet.transform.position != handSetGhost.transform.position)
    {
        joint.connectedBody = handSetRB;
        isHangUp = true;
    }
    else
    {

    }*/

    /*if (isHangUp)
    {
        lightTime += Time.deltaTime;
        if (lightTime > 1.0f)
        {
            handGrab.SetActive(true);
        }


    }
    else
    {
        joint.connectedBody = null;

        handSet.transform.position = handSetGhost.transform.position;
        handSet.transform.rotation = handSetGhost.transform.rotation;
    }*/
}
