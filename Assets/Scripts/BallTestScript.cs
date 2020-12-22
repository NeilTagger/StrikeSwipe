using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTestScript : MonoBehaviour
{
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    public Rigidbody rb;
    public float power = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                    checkSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                checkSwipe();
            }
        }



        if (Input.GetKeyDown("space"))
        {
            print("powah!");
            rb.AddForce(power,power,0);
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
            Debug.Log("Detected Swipe");

            return Vector2.Angle(baseDir, swipeVector);
        }

        return -1;
    }

    float verticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float horizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }


}
