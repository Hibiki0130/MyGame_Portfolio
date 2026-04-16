using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Induction : MonoBehaviour
{
    //[SerializeField] private TextMeshPro debugLog;

    private GameObject player;
    private bool sent;
    public bool approached {  get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        approached = false;
        sent = false;

        player = GameObject.FindWithTag("MainCamera");

        if (player == null)
        {
            Debug.Log("‹ó‚¾‚æ");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (approached)
        {
            if (sent)
            {
                //‹ß‚Ã‚¢‚½î•ñ‚ğ‘—‚Á‚½‚ç‚¨–ğŒä–Æ
                Destroy(gameObject);
            }
        }
        else
        {
            CheckPlayer();
        }
    }

    //1.0fˆÈ‰º‚Ì‹——£‚Å‹ß‚Ã‚¢‚½”»’è
    private float clearDistance = 1.0f;
    private void CheckPlayer()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        //\•ª‹ß‚Ã‚¢‚½‚ç
        if (distance < clearDistance)
        {
            approached = true;
        }
    }

    public void SetSentBoolTrue()
    {
        sent = true;
    }
}
