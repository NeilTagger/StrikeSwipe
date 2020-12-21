using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBarScript : MonoBehaviour
{
    public Image powerBarMask;
    public float barChangeSpeed = 1f;
    float maxPowerBarValue = 100;
    float currentPowerBarValue;
    bool powerIsIncreasing;
    bool powerBarOn;
    public float finalPower = 0;
    void Start()
    {
        finalPower = 0;
        barChangeSpeed = 2;
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
        if (Input.GetKeyDown("a")) // replace if with: Input.touchCount > 0
        {
            Debug.Log("power stored");
            powerBarOn = false;
            barChangeSpeed = barChangeSpeed *1.1f;
            finalPower += currentPowerBarValue;
            barReset();
        }
    }
    void barReset()
    {
        currentPowerBarValue = 0;
        powerIsIncreasing = true;
        powerBarOn = true;
        Debug.Log(powerBarOn);
    }
}
