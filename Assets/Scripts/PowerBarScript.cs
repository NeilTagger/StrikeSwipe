using System;
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
    float maxPowerBarSpeed = 15;
    float barThreashold = 92.5f;

    float goodTheashold = 50, greatThreashold = 80;

    float currentPowerBarValue;
    bool powerIsIncreasing;
    bool powerBarOn;
    float growth=1.25f;
    public float finalPower = 0;
    public void Initialize()
    {
        finalPower = 0;
        barChangeSpeed = 3.5f;
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

        Debug.Log("power stored");
        powerBarOn = false;
        if (currentPowerBarValue  > barThreashold)
        {
            actualPower = 100;
        }

        if (currentPowerBarValue < goodTheashold)
        {
            StartCoroutine(startBarGlow(Color.red));
        }
        else if (currentPowerBarValue < greatThreashold)
        {
            StartCoroutine(startBarGlow(Color.yellow));
        }
        else
        {
            StartCoroutine(startBarGlow(Color.green));
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
        Debug.Log(powerBarOn);
    }
}
