using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private const float DisplayTurretInfoDistance = 10f;
    private const float TimeBetweenWaves = 11;
    private const float SpawnAreaRadius = 8;

    public static GameController Instance = null;

    [SerializeField]
    public Robot _robotPrefab = null;

    [SerializeField]
    public MovementMarker _movementMarkerPrefab = null;

    [SerializeField]
    public TurretInfo _turretInfo = null;

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
    private Text waveCountText = null;

    [SerializeField]
    private Text enemyCountText = null;

    [SerializeField]
    private GameObject gameWinScreen = null;

    [SerializeField]
    private GameObject gameOverScreen = null;

    [SerializeField]
    private Transform _messageUIPanel = null;

    [SerializeField]
    private Text nextWaveIn_Text = null;

    [System.NonSerialized]
    public int currentWaveIndex = 0;
    public DifficultyLevels.Wave currentWave;

    [SerializeField]
    private GameObject _pauseScreen = null;

    [SerializeField]
    private AbilityData _flirtAbilityData = null;

    [SerializeField]
    private AbilityData _giveAbilityData = null;

    [SerializeField]
    private AbilityData _throwAbilityData = null;

    [Header ("Prefabs references")]

    [SerializeField]
    private GameObject level1Enemy = null;

    [SerializeField]
    private GameObject level2Enemy = null;

    [SerializeField]
    private GameObject level3Enemy = null;

    private GameObject hint = null;
    public Robot Robot { get; private set; }

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

    private bool _gamePaused = false;
    public bool GamePaused
    {
        get { return _gamePaused; }
        set
        {
            _gamePaused = value;

            _pauseScreen.SetActive(_gamePaused);

            if (_gamePaused)
            {
                // pause gameplay
                Time.timeScale = 0;
            }
            else
            {
                // resume gameplay
                Time.timeScale = 1;
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        Robot = Instantiate(_robotPrefab, Vector3.zero, Quaternion.identity);

        _flirtAbilityData.OnAbilityActivated += HandleFlirtActivation;
        _giveAbilityData.OnAbilityActivated += HandleGiveActivation;
        _throwAbilityData.OnAbilityActivated += HandleThrowActivation;
    }

    private void Start()
    {
        CursorManager.SetCursorState(CursorLockMode.Confined);

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
            GamePaused = !GamePaused;
        }

        // Get closest turret to Player
        Turret closestTurret = null;
        float closestTurretDist = float.MaxValue;
        foreach (Turret turret in turrets)
        {
            if (turret.isAlive)
            {
                float dist = Vector3.Distance(Robot.transform.position, turret.transform.position);
                if (dist < closestTurretDist)
                {
                    closestTurret = turret;
                    closestTurretDist = dist;
                }
            }
        }

        // Check if player clicked mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse was clicked over a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                int layerMask = 1 << LayerMask.NameToLayer("Terrain");

                // Ray cast to the mouse cursor position
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                float distance = 100;
                if (Physics.Raycast (ray, out hit, distance, layerMask))
                {
                    Vector3 position = hit.point;

                    // Instantiate prefab to display where Robot is moving to
                    Instantiate(_movementMarkerPrefab, position, Quaternion.identity);

                    Robot.MoveTo(position);
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

    public void Resume()
    {
        GamePaused = false;
    }

    public void Exit()
    {
        GamePaused = false;
        SceneController.instance.LoadScene("Main");
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

        gameOverScreen.SetActive(true);
    }

    // TODO: move to Utils class
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

    private IEnumerator SpawnNextWave()
    {
        if (currentWaveIndex >= DifficultyLevels.waves.Length)
        {
            gameWinScreen.SetActive(true);

            yield return new WaitForSeconds(4);

            GamePaused = false;
            SceneController.instance.LoadScene("Success");

            yield break; // exit coroutine
        }

        currentWave = DifficultyLevels.waves[currentWaveIndex]; // TODO: win condition
        currentWaveIndex++;

        waveCountText.text = currentWaveIndex.ToString() + "/" + (DifficultyLevels.waves.Length - 1);
        if (currentWaveIndex >= DifficultyLevels.waves.Length)
        {
            waveCountText.text = "FINAL";
        }

        if (currentWaveIndex != 1)
        {
            nextWaveIn_Text.gameObject.SetActive(true);
            float t = 0;
            while (t < TimeBetweenWaves)
            {
                if (currentWaveIndex >= DifficultyLevels.waves.Length)
                {
                    nextWaveIn_Text.text = "Final wave begins in " + (TimeBetweenWaves - t).ToString("F0") + "..";
                }
                else
                {
                    nextWaveIn_Text.text = "Next wave begins in " + (TimeBetweenWaves - t).ToString("F0") + "..";
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
            Vector3 pos = spawnLocations[r].position + new Vector3(Random.Range(-SpawnAreaRadius, SpawnAreaRadius), 0, Random.Range(-SpawnAreaRadius, SpawnAreaRadius));
            Instantiate(level1Enemy, pos, Random.rotation);

            yield return new WaitForSeconds(.25f); // wait a small time between each enemy spawn
        }
        for (int i = 0; i < currentWave.noOfLevel2Enemies; i++)
        {
            int r = Random.Range(0, spawnLocations.Length);
            Vector3 pos = spawnLocations[r].position + new Vector3(Random.Range(-SpawnAreaRadius, SpawnAreaRadius), 0, Random.Range(-SpawnAreaRadius, SpawnAreaRadius));
            Instantiate(level2Enemy, pos, Random.rotation);

            yield return new WaitForSeconds(.25f); // wait a small time between each enemy spawn
        }
        for (int i = 0; i < currentWave.noOfLevel3Enemies; i++)
        {
            int r = Random.Range(0, spawnLocations.Length);
            Vector3 pos = spawnLocations[r].position + new Vector3(Random.Range(-SpawnAreaRadius, SpawnAreaRadius), 5, Random.Range(-SpawnAreaRadius, SpawnAreaRadius));
            Instantiate(level3Enemy, pos, Random.rotation);

            yield return new WaitForSeconds(.25f); // wait a small time between each enemy spawn
        }
    }

    private void HandleTurretDeath()
    {
        CheckIfGameOver();
    }

    private void HandleFlirtActivation()
    {
        if (Robot.TurretInInteractionRange != null)
        {
            Robot.TurretInInteractionRange.Flirt();
        }
        else
        {
            Debug.LogWarning("TurretInInteractionRange is null!");
        }
    }

    private void HandleGiveActivation()
    {
        Robot.GiveGift();
    }

    private void HandleThrowActivation()
    {
        Robot.ThrowCurrentlyHeldItem();
    }
}
