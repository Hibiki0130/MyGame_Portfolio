using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookBag : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 waistPos;
    private float waistNum;
    private float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        waistNum = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, player.transform.rotation, speed * Time.deltaTime);

        //waistPos = playerPos.transform.position - new Vector3(0f, waistNum, 0.3f);

        //transform.position = waistPos;
    }
}
