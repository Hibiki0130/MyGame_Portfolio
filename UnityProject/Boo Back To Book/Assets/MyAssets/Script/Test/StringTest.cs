using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringTest : MonoBehaviour
{
    Vector3 firstPos;
    Vector3 firstRot;

    // Start is called before the first frame update
    void Start()
    {
        firstPos = transform.position;
        firstRot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = firstPos;
        transform.eulerAngles = firstRot;
    }
}
