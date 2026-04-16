using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaseGhost : MonoBehaviour
{
    //VaseとVaseGhostの同じ形に同じナンバーをふる
    [SerializeField] private float number;

    Vector3 Rotation;
    // Start is called before the first frame update
    void Start()
    {
        Rotation = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

    }

    Vector3 pieceRotate = new Vector3();
    private float pieceNum;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pieces"))
        {
            Vase vase = other.GetComponent<Vase>();

            //rotateとナンバーが同じだったら一致
            pieceRotate = vase.GetRotate();
            pieceNum = vase.GetNum();

            if (number == pieceNum)
            {
                //if (Rotation == pieceRotate)
                
                    //Debug.Log("一致！");
                    vase.SetIsSet(true);
                    other.transform.position = transform.position;
                    Destroy(gameObject);
            }
        }
    }
}