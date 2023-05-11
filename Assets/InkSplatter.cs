using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkSplatter : MonoBehaviour
{
    [SerializeField] float duration, size;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color fade;
    Vector3 scaleRate;
    Transform trfm;
    int timer, splatterTimer;
    // Start is called before the first frame update
    void Start()
    {
        trfm = transform;
        timer = Mathf.RoundToInt(duration * 50);
        scaleRate = Vector3.one * size / 7;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (splatterTimer < 7)
        {
            trfm.localScale += scaleRate;
            splatterTimer++;
        }

        if (timer > 0)
        {
            if (timer < 50)
            {
                spriteRenderer.color -= fade;
            }

            timer--;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
