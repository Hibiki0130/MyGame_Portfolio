using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LightLine : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugLog;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PullAnimation();
    }

    private void PullAnimation()
    {
        float lefthanddistance = Vector3.Distance(HandTraking.Instance.GetLeftHandPos(), transform.position);
        float righthanddistance= Vector3.Distance(HandTraking.Instance.GetRightHandPos(), transform.position);

        //debugLog.text = "left"+lefthanddistance;

        if (HandTraking.Instance.GetLeftHold())
        {
            //debugLog.text = "lefthold";

            if (lefthanddistance < 0.05f)
            {
                transform.position = HandTraking.Instance.GetLeftHandPos();
            }
        }

        if (righthanddistance < 0.05f && HandTraking.Instance.GetRightHold())
        {
            //debugLog.text = "righthold";

            if (righthanddistance < 0.05f)
            {
                transform.position = HandTraking.Instance.GetRightHandPos();
            }
        }
    }
}
