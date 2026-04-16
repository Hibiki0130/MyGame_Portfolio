using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Vase : MonoBehaviour
{
    //VaseとVaseGhostの同じ形に同じナンバーをふる
    [SerializeField] private float number;
    private bool isSet;

    // Start is called before the first frame update
    void Start()
    {
        isSet = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //VaseGhostに渡すやつ
    public Vector3 GetRotate()
    {
        Vector3 rotation = transform.localEulerAngles;

        return rotation;
    }

    public float GetNum()
    {
        return number;
    }

    public bool SetIsSet(bool set)
    {
        isSet = set;
        return isSet;
    }
}
