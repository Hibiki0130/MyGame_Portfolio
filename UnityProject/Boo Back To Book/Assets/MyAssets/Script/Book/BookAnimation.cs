using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRPlugin;

public class BookAnimation : MonoBehaviour
{
    [SerializeField] private GameObject eyeMaskBoo;
    [SerializeField] private GameObject sneezeEffect;
    [SerializeField] private AudioClip sneezeSound;
    private Animator animator;
    PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playsound = GetComponent<PlaySound>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Sneeze()
    {
        Instantiate(sneezeEffect, transform.position, Quaternion.identity);

        if (GameManager.Instance.state == GameState.LookAround)
        {
            playsound.PlayOneShot(sneezeSound);
            Instantiate(eyeMaskBoo, transform.position, Quaternion.identity);
        }
    }
}
