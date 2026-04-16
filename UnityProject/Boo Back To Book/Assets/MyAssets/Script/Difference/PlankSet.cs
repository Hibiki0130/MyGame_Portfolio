using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ThisPlankState
{
    Nomal,
    Planked,
    Ghost,
    Restored,
}
public enum PlankBoo
{
    Butler,
    Twins,
    Artist,
    Cat,
}
public class PlankSet : MonoBehaviour
{
    [SerializeField] private PlankBoo PlankBoo;
    public ThisPlankState State { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckPlank()
    {

    }
}
