﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class ParallaxScript : MonoBehaviour
{
    public float length;
    private float startposx,startposy;
    public GameObject cam;
    public float parallaxEffect;
    public float parallayEffect;


    void Start()
    {
        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x*1.5f;
        startposx = transform.position.x;
        startposy = 5.5f;
        
    }

    private void FixedUpdate()
    {
        
        float distx = (cam.transform.position.x * parallaxEffect);
        float disty = (cam.transform.position.y * parallayEffect)+13f;

        transform.position = new Vector3(startposx + distx, startposy + disty, transform.position.z);
        if (cam.transform.position.x- transform.position.x > length/3)
        {
            startposx += (length/3)*2f;
        }
    }
}