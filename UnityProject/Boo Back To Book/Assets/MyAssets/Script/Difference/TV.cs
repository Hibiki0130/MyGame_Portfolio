using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public enum TVState
{
    NotPranked,
    Pranked,
    Ghost,
    Restored,
}
public class TV : MonoBehaviour
{
    public TVState State { get; private set; }

    [SerializeField] private TextMeshPro debugLog;
    //Loopóp
    [SerializeField] private AudioSource loopAudio;
    [Header("SE")]
    [SerializeField] private AudioClip hit;

    private bool isClose;
    private float var;

    LightAnimation lightanimation;
    InstanceBoo instanceboo;
    PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        State = TVState.NotPranked;

        lightanimation = transform.parent.gameObject.GetComponent<LightAnimation>();
        instanceboo = GetComponent<InstanceBoo>();
        playsound = GetComponent<PlaySound>();

        DifferenceManager.Instance.OnDiffrenceStateChange += CheckChosen;
    }

    // Update is called once per frame
    void Update()
    {
        SlapOrNot();
    }

    private void CheckChosen(DifferenceState newState)
    {
        /*if (newState == DifferenceState.Butler)
        {
            //ëIÇŒÇÍÇƒÇ¢ÇΩÇÁÇ¢ÇΩÇ∏ÇÁ
            if (DifferenceManager.Instance.GetChosenPlanked(Boos.Butler) == 0)
            {
                ChangeMyState(TVState.Pranked);
            }
        }*/
    }
    private void ChangeMyState(TVState newState)
    {
        State = newState;

        if (newState == TVState.Pranked)
        {
            lightanimation.SetisOnBool(true);
            loopAudio.Play();

        }
    }
    private void SlapOrNot()
    {
        //debugLog.text = "slap:" + var;
        if (isClose)
        {
            if (HandTraking.Instance.GetLeftSlap() || HandTraking.Instance.GetRightSlap())
            {
                //àÍâÒÇ…Ç¬Ç´10Ç≠ÇÁÇ¢ëùÇ¶ÇÈ
                //Ç¢ÇΩÇ∏ÇÁÇ≥ÇÍÇΩÇ∆Ç´
                if (State == TVState.Pranked)
                {
                    var++;
                }

                playsound.PlayIntarvalShot(hit);
            }
        }

        if (var > 20f)
        {
            //debugLog.text = "Clear!!";
            lightanimation.SetisOnBool(false);
            loopAudio.Stop();
            instanceboo.Clear();
            //State = TVState.Boo;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("LeftHand")|| other.gameObject.CompareTag("RightHand"))
        {
            isClose = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))Å@
        {
            isClose = false;
        }
    }
}
