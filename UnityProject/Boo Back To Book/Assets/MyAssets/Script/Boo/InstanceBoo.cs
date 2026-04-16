using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstanceBoo : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private GameObject boo;
    [SerializeField] private Transform instancePos;

    private GameObject _boo;
    private Animator booAnimator;
    private bool clear;
    private bool once;
    private bool instance;
    // Start is called before the first frame update
    void Start()
    {
        booAnimator = boo.GetComponent<Animator>();

        once = false;
        instance = true;
        clear = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        if (Input.GetKey(KeyCode.G))
        {
            if (!once)
            {
                Clear();
                once = true;
            }
        }
        //

        if (clear)
        {
            if (instance)
            {
                _boo = Instantiate(boo, instancePos.position, Quaternion.identity);
                instance = false;
            }
        }
    }

    public void Clear()
    {
        if (GameManager.Instance.state == GameState.Game)
        {
            DifferenceManager.Instance.ChangeState(DifferenceState.Catch);
        }

        clear = true;
        //debugLog.text = "Instance!";
    }
}
