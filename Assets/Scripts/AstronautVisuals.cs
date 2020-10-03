using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AstronautVisuals : MonoBehaviour
{
    public SpriteRenderer mySpriteRenderer;
    public GameObject myStation;
    Random rando = new Random();

    
    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer.color = new Color(rando.Next(0,255),rando.Next(0,255),rando.Next(0,255));
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(0,0,Vector3.Angle(transform.position, transform.parent.position));
        //unfinished testing if statements
        if (Input.GetKeyDown(KeyCode.A)) {
            //transform.position;
        }
    }
}
