using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTestScript : MonoBehaviour
{
    public PowerBarScript powerBar;
    public Rigidbody rb;
    public float power = 1f;
   
    void Start()
    {
        
    }

   
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            print("Use stored power");
            rb.AddForce(powerBar.finalPower, powerBar.finalPower, 0);
        }
    }
    
}
