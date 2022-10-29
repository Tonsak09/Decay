using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Values")]
    [SerializeField] float timeGainedFromConsume;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float startTime;
    [SerializeField] float timerSpeed;

    [Header("End")]
    [SerializeField] Player player;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject endText;
    [SerializeField] float endApeearSpeed;
    [SerializeField] TextMeshProUGUI finalCount;
    [SerializeField] TextMeshProUGUI finalTime;

    private float timer;
    private bool endStarted;

    private float totalTime;
    private int childCount;

    // Start is called before the first frame update
    void Start()
    {
        timer = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;

        if (timer > 0)
        {
            timerText.text = ((int)timer).ToString();
            timer -= timerSpeed * Time.deltaTime;
        }
        else
        {
            if (!endStarted)
            {
                endStarted = true;
                endScreen.SetActive(true);

                finalCount.text = childCount.ToString();
                finalTime.text = ((int)totalTime).ToString();

                StartCoroutine(Appear(endApeearSpeed, endScreen.GetComponent<Image>(), endText));
            }
        }
    }

    public void ConsumeChild()
    {
        childCount += 1;
    }

    private IEnumerator Appear(float speed, Image image, GameObject textToAppear)
    {
        float lerp = 0;
        while (lerp <= 1)
        {
            lerp += Time.deltaTime * speed;
            image.color = new Color(image.color.r, image.color.g, image.color.b, lerp);
            yield return null;
        }

        textToAppear.SetActive(true);
    }
}
