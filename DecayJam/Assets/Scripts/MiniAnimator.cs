using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniAnimator : MonoBehaviour
{
    [SerializeField] Texture[] frames;
    [SerializeField] float time;
    [SerializeField] Renderer rend;

    private float timer;
    private int currentIndex;

    private void Start()
    {
        timer = time;
    }

    private void Update()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        if(timer <= 0)
        {
            // Next Frame 
            if(currentIndex + 1 < frames.Length)
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }
            rend.material.mainTexture = frames[currentIndex];

            timer = time;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
