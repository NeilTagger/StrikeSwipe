﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBarScript : MonoBehaviour
{
    public Image powerBarMask, powerBarGlow;
    public float barChangeSpeed = 1f;
    public float GlowSpeed;
    float maxPowerBarValue = 100;
    float maxPowerBarSpeed = 10;
    float barThreashold = 92.5f;

    float goodTheashold = 50, greatThreashold = 80;

    float currentPowerBarValue;
    bool powerIsIncreasing;
    bool powerBarOn;
    float growth=1.25f;
    public float finalPower = 0;
    public AudioClip BadSound;
    public AudioClip GoodSound;
    public AudioClip GreatSound;
    private AudioSource sound;

    public void Initialize()
    {
        sound = GetComponent<AudioSource>();
        sound.pitch = 1f;
        finalPower = 0;
        barChangeSpeed = 1.5f;
        currentPowerBarValue = 0;
        powerIsIncreasing = true;
        powerBarOn = true;
        StartCoroutine(UpdatePowerBar());

    }
    IEnumerator UpdatePowerBar()
    {
        while (powerBarOn) 
        {
            if (!powerIsIncreasing)
            {
                currentPowerBarValue -= barChangeSpeed;
                if (currentPowerBarValue <= 0)
                {
                    powerIsIncreasing = true;
                }
            }
            if (powerIsIncreasing)
            {
                if (currentPowerBarValue >= maxPowerBarValue)
                {
                    powerIsIncreasing = false;
                }
                currentPowerBarValue += barChangeSpeed;
            }
            
            float fill = currentPowerBarValue / maxPowerBarValue;
            powerBarMask.fillAmount = fill;
            yield return new WaitForSeconds(0.02f);
        }
        
        yield return null;


    }
    void Update()
    {

    }

    public void AddPower()
    {
        float actualPower = currentPowerBarValue;

        
        powerBarOn = false;
        if (currentPowerBarValue  > barThreashold)
        {
            actualPower = 100;
        }

        if (currentPowerBarValue < goodTheashold)
        {
            StartCoroutine(startBarGlow(Color.red));
            sound.PlayOneShot(BadSound);
        }
        else if (currentPowerBarValue < greatThreashold)
        {
            StartCoroutine(startBarGlow(Color.yellow));
            sound.PlayOneShot(GoodSound);
        }
        else
        {
            StartCoroutine(startBarGlow(Color.green));
            sound.PlayOneShot(GreatSound);
        }


        if (barChangeSpeed <= maxPowerBarSpeed)
        {
            barChangeSpeed = barChangeSpeed + (float)Math.Pow((actualPower / 100), 2);
        }
        else
        {
            barChangeSpeed = maxPowerBarSpeed;
        }

        finalPower += (float)(Math.Pow(actualPower, growth)) * (actualPower / 100);
        GameController.Instance.Ball.GetComponent<Renderer>().material.SetFloat("Power", finalPower);


        barReset();
    }

    public IEnumerator startBarGlow(Color C)
    {
        Color Copy = C;
        C.a = 0;
        powerBarGlow.color = C;
        Copy.a = 1;

        float tick = 0;

        while (powerBarGlow.color.a != 1)
        {
            powerBarGlow.color = Color.Lerp(C, Copy, tick);
            tick += Time.deltaTime * GlowSpeed;

            yield return null;

        }

        yield return new WaitForSeconds(0.01f);

        while (powerBarGlow.color.a != 0)
        {
            powerBarGlow.color = Color.Lerp(C, Copy, tick);
            tick -= Time.deltaTime * GlowSpeed;

            yield return null;
        }

        powerBarGlow.color = new Color(0, 0, 0, 0);

    }


    void barReset()
    {
        currentPowerBarValue = 0;
        powerIsIncreasing = true;
        powerBarOn = true;
    }
}
