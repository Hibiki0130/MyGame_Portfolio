using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBook : MonoBehaviour
{
    public bool ableTalk { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        ableTalk = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(ableTalk);
    }

    public void SetAbleTalkTrue()
    {
        ableTalk = true;
    }

    public void SetAbleTalkFalse()
    {
        ableTalk = false;
    }
}
