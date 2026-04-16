using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    Rigidbody rb;
    Vector3 firstPos;
    Vector3 firstRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        firstPos = transform.position;
        firstRot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        //ìÆÇ¢ÇΩÇÁ
        if (transform.position != firstPos)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void PutBack()
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.position = firstPos;
        transform.eulerAngles = firstRot;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            //Debug.Log("ñﬂÇ¡ÇΩÅI");
            PutBack();
        }
    }
}
