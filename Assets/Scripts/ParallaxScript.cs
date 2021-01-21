using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class ParallaxScript : MonoBehaviour
{
    private float length;
    private float startposx,startposy;
    public GameObject cam;
    public float parallaxEffect;
    public float parallayEffect;
    private Vector3 origin;


    void Start()
    {
        origin = transform.position;
        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        startposx = transform.position.x;
        startposy = 11f;
        
    }

    private void FixedUpdate()
    {
        
        float distx = (cam.transform.position.x * parallaxEffect);
        float disty = (cam.transform.position.y * parallayEffect)+13f;

        transform.position = new Vector3(startposx + distx, startposy + disty, transform.position.z);
        if (cam.transform.position.x- transform.position.x > length/3)
        {
            startposx += length;
        }
    }

    public void resetBackground()
    {
        startposx = 0;
    }
}