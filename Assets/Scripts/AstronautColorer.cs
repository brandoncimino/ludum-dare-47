using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AstronautColorer : MonoBehaviour
{
    public SpriteRenderer mySpriteRenderer;
    Random rando = new Random();
    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer.color = new Color(rando.Next(0,255),rando.Next(0,255),rando.Next(0,255));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
