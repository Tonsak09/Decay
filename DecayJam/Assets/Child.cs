using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    [Header("Capturing")]
    [SerializeField] Transform bar;
    [SerializeField] float captureSpeed;

    private float captureAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(captureAmount > 0)
        {
            bar.transform.localScale = new Vector3(captureAmount, 0.2f, 1);
        }


    }

    public bool CaptureChild(Transform witch)
    {
        if(captureAmount > 1)
        {
            return true;
        }

        captureAmount += captureSpeed * Time.deltaTime;
        bool childCaptured = captureAmount >= 1;

        if(childCaptured)
        {
            // Cease movment 

            // Turn to spectar 

            // Follow witch 
        }

        return childCaptured;
    }
}
