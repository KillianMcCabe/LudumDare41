using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private const float DisplayTurretInfoDistance = 10f;

    public static GameController instance = null;

    [SerializeField]
    public Player _player;

    [SerializeField]
    public TurretInfo _turretInfo;

    [System.NonSerialized]
    public Turret[] turrets;

    public GameObject[] accessories;
    public Item[] gifts;
    private string[] _turretNames = {
        "Darragh",
        "Mike",
        "Turk",
        "Archimedes",
        "Lucifer"
    };
    public Transform[] spawnLocations;

    [Header("External components")]

    [SerializeField]
    private Text waveCountText;

    [SerializeField]
    private Text enemyCountText;

    [SerializeField]
    private GameObject gameWinScreen;

    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private Transform _messageUIPanel;

    [SerializeField]
    private Text nextWaveIn_Text;

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

    private void Awake()
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
        foreach (Turret turret in turrets)
        {
            turret.OnDeath += HandleTurretDeath;
        }

        hint = Resources.Load("Prefabs/Hint_Text") as GameObject;
    }

    // shuffle an array using Knuth shuffle algorithm
    private T[] ShuffleArray<T>(T[] array)
    {
        for (int t = 0; t < array.Length; t++)
        {
            T tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }

        return array;
    }

    private void Start()
    {
        // Shuffle accessories, gifts and names
        accessories = ShuffleArray(accessories);
        gifts = ShuffleArray(gifts);
        _turretNames = ShuffleArray(_turretNames);

        // Assign each turret an accessory, like and dislike, and name
        for (var i = 0; i < turrets.Length && i < gifts.Length; i++)
        {
            GameObject accessory = accessories[i];
            turrets[i].AddAccessory(accessory);

            turrets[i].SetLike(gifts[i].Label);
            turrets[i].SetDislike(gifts[(i + 1) % gifts.Length].Label);

            turrets[i].Name = _turretNames[i];
        }

        enemyCount = 0;
        nextWaveIn_Text.gameObject.SetActive(false);
        StartCoroutine(SpawnNextWave());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneController.instance.LoadScene("Main");
        }

        // Get closest turret to Player
        Turret closestTurret = null;
        float closestTurretDist = float.MaxValue;
        foreach (Turret turret in turrets)
        {
            if (turret.isAlive)
            {
                float dist = Vector3.Distance(_player.transform.position, turret.transform.position);
                if (dist < closestTurretDist)
                {
                    closestTurret = turret;
                    closestTurretDist = dist;
                }
            }
        }

        if (closestTurret != null && closestTurretDist < DisplayTurretInfoDistance)
        {
            // display info for closest turret
            _turretInfo.Show();
            _turretInfo.DisplayInfoForTurret(closestTurret);
        }
        else
        {
            _turretInfo.Hide();
        }
    }

    public void DisplayHint(string text)
    {
        GameObject go = Instantiate(hint);
        go.GetComponent<Text>().text = text;
        go.transform.SetParent(_messageUIPanel, false);
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

    private void HandleTurretDeath()
    {
        CheckIfGameOver();
    }
}
