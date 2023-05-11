using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer titleRenderer;
    [SerializeField] Color fadeColor;
    [SerializeField] GameObject pressKeyTxt;

    [SerializeField] int timer;
    
    void Start()
    {
        timer = -25;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0 && timer < 50)
        {
            titleRenderer.color += fadeColor;
        } else if (timer > 100 && timer < 130)
        {
            titleRenderer.color -= fadeColor;
        } else if (timer > 175 && timer < 203)
        {
            if (timer % 3 == 0)
            {
                pressKeyTxt.SetActive(!pressKeyTxt.activeSelf);
            }
        }

        if (Input.anyKey)
        {
            SceneManager.LoadScene("SampleScene");
        }

        timer++;
    }
}
