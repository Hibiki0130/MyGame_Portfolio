using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinsAnimation : MonoBehaviour
{
    public bool OldIsFirstAppearance { get; private set; }
    public bool YoungIsFirstAppearance { get; private set; }
    public bool OldIsAppearance { get; private set; }
    public bool YoungIsAppearance {  get; private set; }
    public bool OldAbleMove { get; private set; }
    public bool YoungAbleMove { get; private set; }

    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        OldIsFirstAppearance = false;
        YoungIsFirstAppearance = false;
        OldIsAppearance = true;
        YoungIsAppearance = true;
        OldAbleMove = false;
        YoungAbleMove = false;

        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //appearanceAnimationÇÃç≈å„
    public void OldFirstAppearance()
    {
        OldIsFirstAppearance = true;
    }
    public void YoungFirstAppearance()
    {
        YoungIsFirstAppearance = true;
    }
    public void OldAppearance()
    {
        OldIsAppearance = true;
    }
    public void OldDisappear()
    {
        OldIsAppearance = false;
    }
    public void YoungAppearance()
    {
        YoungIsAppearance = true;
    }
    public void YoungDisappear()
    {
        YoungIsAppearance = false;
    }
    public void OldDestroy()
    {
        Destroy(parent);
        //parent.transform.localScale = Vector3.zero;
    }
    public void YoungDestroy()
    {
        Destroy(parent);
        //parent.transform.localScale = Vector3.zero;
    }
}
