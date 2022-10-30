using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Spawning")]
    [SerializeField] GameObject[] children;
    [SerializeField] List<Patrol> zones;
    [SerializeField] int minSpawn;
    [SerializeField] int maxSpawn;
    [SerializeField] float spawnRange;

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

    public float TimeLeft { get { return timer; } }

    // Start is called before the first frame update
    void Start()
    {
        timer = startTime;

        foreach (Patrol zone in zones)
        {
            SpawnChildren(zone);
        }
    }

    // Update is called once per frame
    void Update()
    {

        totalTime += Time.deltaTime;

        if (timer > 0)
        {
            Cursor.visible = false;

            timerText.text = ((int)timer).ToString();
            timer -= timerSpeed * Time.deltaTime;
        }
        else
        {
            Cursor.visible = true;

            if (!endStarted)
            {
                endStarted = true;
                endScreen.SetActive(true);
                player.active = false;

                finalCount.text = childCount.ToString();
                finalTime.text = ((int)totalTime).ToString();

                StartCoroutine(Appear(endApeearSpeed, endScreen.GetComponent<Image>(), endText));
            }
        }

        for (int i = 0; i < zones.Count; i++)
        {
            if(zones[i].needsChildren)
            {
                if(Vector3.Distance(player.transform.position, zones[i].transform.position) > zones[i].Size)
                {
                    SpawnChildren(zones[i]);
                }
            }
        }
    }

    public void ConsumeChild()
    {
        childCount += 1;
        timer += timeGainedFromConsume;
    }

    private void SpawnChildren(Patrol zone)
    {
        int spawnAmount = Random.Range(minSpawn, maxSpawn);
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject temp = Instantiate(children[Random.Range(0, children.Length)]);
            zone.AddChild(temp.GetComponent<Child>(), spawnRange);
        }
        zone.SetUpChildren();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
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
