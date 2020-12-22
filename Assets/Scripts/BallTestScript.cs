using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTestScript : MonoBehaviour
{

    public PowerBarScript powerBar;

    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    public Rigidbody rb;
    public float power = 1f;
   
    void Start()
    {
        
    }

   
    void Update()
    {
        float angle = 0;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    angle = checkSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                angle = checkSwipe();
            }
        }

        if (angle > 0)
        {

            Debug.Log(angle);
        }

        if (Input.GetKeyDown("space"))
        {
            print("Use stored power");
            rb.AddForce(powerBar.finalPower, powerBar.finalPower, 0);
        }


    }

    float checkSwipe()
    {
        Vector2 swipeVector;
        Vector2 baseDir = Vector2.right;

        swipeVector.x = verticalMove();

        swipeVector.y = horizontalValMove();

        if (swipeVector.magnitude > SWIPE_THRESHOLD * SWIPE_THRESHOLD)
        {
            Debug.Log(swipeVector.normalized);

            return FindDegree(swipeVector.x, swipeVector.y);
        }

        return -1;
    }

    public float FindDegree(float x, float y)
    {
        float value = (float)((Mathf.Atan2(x, y) / Math.PI) * 180f);
        if (value < 0) value += 360f;

        return value;
    }


    float verticalMove()
    {
        return fingerDown.y - fingerUp.y;
    }

    float horizontalValMove()
    {
        return fingerDown.x - fingerUp.x;
    }


}
