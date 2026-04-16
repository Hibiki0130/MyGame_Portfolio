using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPEffect : MonoBehaviour
{
    [SerializeField] private GameObject book;

    [Header("Effects")]
    [SerializeField] private GameObject appearance;
    [SerializeField] private GameObject smoke;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AppearanceEffect()
    {
        Instantiate(appearance, book.transform.position, Quaternion.identity);
        Instantiate(smoke, book.transform.position, Quaternion.identity);
    }

    public void SmokeEffect()
    {
        Instantiate(smoke, book.transform.position, Quaternion.identity);
    }
}
