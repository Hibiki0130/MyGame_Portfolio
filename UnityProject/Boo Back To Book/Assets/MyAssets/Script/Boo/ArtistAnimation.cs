using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtistAnimation : MonoBehaviour
{
    public bool AbleMove { get; private set; }
    private GameObject parent;
    private Vector3 parentPos;
    // Start is called before the first frame update
    void Start()
    {
        AbleMove = false;
        parent = transform.parent.gameObject;
        parentPos = parent.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    //appearanceend
    public void StartMove()
    {
        AbleMove = true;
    }
    //catchend
    public void OnAnimationEnd()
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           ;
        DifferenceManager.Instance.SetFixedPlank(Boos.Artist);
        Destroy(parent);
        //parent.transform.localScale = Vector3.zero;
    }
}
