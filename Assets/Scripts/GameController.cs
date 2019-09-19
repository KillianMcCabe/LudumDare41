﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController instance = null;

    [System.NonSerialized]
    public Turret[] turrets;

    public GameObject[] accessories;
    public Item[] gifts;
    public Transform[] spawnLocations;

    public Text waveCountText;
    public Text enemyCountText;
    public GameObject gameWinScreen;
    public GameObject gameOverScreen;

    public Text nextWaveIn_Text;

    public Transform hintTransform;

    [System.NonSerialized]
    public int currentWaveIndex = 0;
    public DifficultyLevels.Wave currentWave;

    float timeBetweenWaves = 6;
    float spawnAreaRadius = 8;

    [SerializeField]
    private GameObject level1Enemy;

    [SerializeField]
    private GameObject level2Enemy;

    [SerializeField]
    private GameObject level3Enemy;

    private GameObject hint;

    [System.NonSerialized]
    private bool gameOver = false;

    int enemyCount;
    public int EnemyCount
    {
        get { return enemyCount; }
        set
        {
            enemyCount = value;
            enemyCountText.text = enemyCount.ToString();
            if (enemyCount <= 0)
            {
                StartCoroutine(SpawnNextWave());
            }
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        turrets = GameObject.FindObjectsOfType<Turret>() as Turret[];

        hint = Resources.Load("Prefabs/Hint_Text") as GameObject;
    }

    // Use this for initialization
    void Start()
    {

        // Shuffle accessories (Knuth shuffle algorithm)
        for (int t = 0; t < accessories.Length; t++)
        {
            GameObject tmp = accessories[t];
            int r = Random.Range(t, accessories.Length);
            accessories[t] = accessories[r];
            accessories[r] = tmp;
        }

        // also shuffle gifts
        for (int t = 0; t < gifts.Length; t++)
        {
            Item tmp = gifts[t];
            int r = Random.Range(t, gifts.Length);
            gifts[t] = gifts[r];
            gifts[r] = tmp;
        }

        for (var i = 0; i < turrets.Length && i < accessories.Length; i++)
        {
            GameObject accessory = accessories[i];
            turrets[i].AddAccessory(accessory);
        }

        for (var i = 0; i < turrets.Length && i < gifts.Length; i++)
        {
            turrets[i].SetLike(gifts[i].label);
            turrets[i].SetDislike(gifts[(i + 1) % gifts.Length].label);
        }

        enemyCount = 0;
        nextWaveIn_Text.gameObject.SetActive(false);
        StartCoroutine(SpawnNextWave());
    }

    public void DisplayHint(string text)
    {
        GameObject go = Instantiate(hint);
        go.GetComponent<Text>().text = text;
        go.transform.SetParent(hintTransform, false);
    }

    public void CheckIfGameOver()
    {
        foreach (Turret t in turrets)
        {
            if (t.isAlive)
            {
                return;
            }
        }
        gameOver = true;
        gameOverScreen.SetActive(true);
    }

    IEnumerator SpawnNextWave()
    {
        if (currentWaveIndex >= DifficultyLevels.waves.Length)
        {
            gameWinScreen.SetActive(true);
            // SceneController.instance.LoadScene("Main");
            yield break; // exit coroutine
        }

        currentWave = DifficultyLevels.waves[currentWaveIndex]; // TODO: win condition
        currentWaveIndex++;

        waveCountText.text = currentWaveIndex.ToString();
        if (currentWaveIndex >= DifficultyLevels.waves.Length)
        {
            waveCountText.text = "FINAL";
        }

        if (currentWaveIndex != 1)
        {
            nextWaveIn_Text.gameObject.SetActive(true);
            float t = 0;
            while (t < timeBetweenWaves)
            {
                if (currentWaveIndex >= DifficultyLevels.waves.Length)
                {
                    nextWaveIn_Text.text = "Final wave begins in " + (timeBetweenWaves - t).ToString("F0") + "..";
                }
                else
                {
                    nextWaveIn_Text.text = "Next wave begins in " + (timeBetweenWaves - t).ToString("F0") + "..";
                }
                t += Time.deltaTime;
                yield return null;
            }
            nextWaveIn_Text.gameObject.SetActive(false);
        }

        EnemyCount += currentWave.noOfLevel1Enemies;
        EnemyCount += currentWave.noOfLevel2Enemies;
        EnemyCount += currentWave.noOfLevel3Enemies;

        for (int i = 0; i < currentWave.noOfLevel1Enemies; i++)
        {
            int r = Random.Range(0, spawnLocations.Length);
            Vector3 pos = spawnLocations[r].position + new Vector3(Random.Range(-spawnAreaRadius, spawnAreaRadius), 0, Random.Range(-spawnAreaRadius, spawnAreaRadius));
            Instantiate(level1Enemy, pos, Random.rotation);

            yield return new WaitForSeconds(.25f); // wait a small time between each enemy spawn
        }
        for (int i = 0; i < currentWave.noOfLevel2Enemies; i++)
        {
            int r = Random.Range(0, spawnLocations.Length);
            Vector3 pos = spawnLocations[r].position + new Vector3(Random.Range(-spawnAreaRadius, spawnAreaRadius), 0, Random.Range(-spawnAreaRadius, spawnAreaRadius));
            Instantiate(level2Enemy, pos, Random.rotation);

            yield return new WaitForSeconds(.25f); // wait a small time between each enemy spawn
        }
        for (int i = 0; i < currentWave.noOfLevel3Enemies; i++)
        {
            int r = Random.Range(0, spawnLocations.Length);
            Vector3 pos = spawnLocations[r].position + new Vector3(Random.Range(-spawnAreaRadius, spawnAreaRadius), 5, Random.Range(-spawnAreaRadius, spawnAreaRadius));
            Instantiate(level3Enemy, pos, Random.rotation);

            yield return new WaitForSeconds(.25f); // wait a small time between each enemy spawn
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneController.instance.LoadScene("Main");
        }
    }
}
