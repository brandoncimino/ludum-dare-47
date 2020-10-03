using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSpin : MonoBehaviour
{
    public float rotateIntensity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotateIntensity, 0);
    }
}
