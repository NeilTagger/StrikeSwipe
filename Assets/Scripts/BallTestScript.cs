using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTestScript : MonoBehaviour
{
    public AudioClip landSound;
    public AudioClip flySound;
    private AudioSource aSource;
    public PowerBarScript powerBar;
    private bool notLanded = false;

    
    public Rigidbody rb;
    public float power = 1f;
   
    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

   
    void Update()
    {
        if (rb.position.y >= 5.55)
        {
            notLanded = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (notLanded)
        {
            PlaySound(2);
            notLanded = false;
        }
          
    }
    public void PlaySound(int sound)
    {
        if (sound == 1)
        {
            aSource.PlayOneShot(flySound);
        }
        if (sound == 2)
        {
            aSource.PlayOneShot(landSound);
        }
    }
}
