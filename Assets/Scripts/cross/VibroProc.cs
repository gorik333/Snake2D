using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibroProc : MonoBehaviour
{
    public static VibroProc Instance;

    private int[] m_mask;
    private int m_repeat;
    private bool isVibrate = false;


    private void Awake()
    {
        Instance = this;
    }

    public void StartVibro(int[] mask, int repeats, bool isNew = true)
    {
        m_mask = mask;
        m_repeat = repeats;

        if(isNew)
        {
            StopVibro();
            StartCoroutine("Vibrate");
        }
        else
        {
            if(!isVibrate)
            {
                StopVibro();
                StartCoroutine("Vibrate");
            }
        }
    }

    public void StopVibro()
    {
        StopCoroutine("Vibrate");
        isVibrate = false;
    }


    public IEnumerator Vibrate()
    {
        isVibrate = true;
        int[] mask = m_mask;
        int repeats = m_repeat;

        int currentRepeat = 0;
        int i = 1;

        float frequency = mask[0] / 1000f;
        float timer = frequency;

        while (i < mask.Length)
        {
            timer += Time.deltaTime;

            if (timer > frequency)
            {
                timer = 0;

				if (mask[i] == 1)
                {
                    Vibro.Light();
                }

                if (mask[i] == 2)
                {
                    Vibro.Medium();
                }

                if (mask[i] == 3)
                {
                    Vibro.Heavy();
                }

                //print("vibro " + i + " / power = " + mask[i] + " / currentRepeat = " + currentRepeat);

                i++;

                if((i == mask.Length))
                {
                    if(repeats != 0)
                    {
                        currentRepeat++;

                        if (currentRepeat < repeats)
                        {
                            i = 1;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i = 1;
                    }
                }
            }

            yield return null;
        }

        isVibrate = false;
    }
}

//int[] mask = { 400, 1, 2, 3, 0, 1, 0, 2, 0, 3, 0, 2, 0, 1  };

                    //VibroProc.Instance.StartVibro(mask, 2);