using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static GameController instance = null;

	[System.NonSerialized]
	public Turret[] turrets;

	public GameObject[] accessories;
	public GameObject[] gifts;
	public Transform[] spawnLocations;

	public Text enemyCountText;
	public Text nextWaveIn_Text;

	[System.NonSerialized]
	public int currentWaveIndex = 0;
	public DifficultyLevels.Wave currentWave;

	float timeBetweenWaves = 5;
	float spawnAreaRadius = 8;
	GameObject level1Enemy;

	int enemyCount;
	public int EnemyCount
	{
		get { return enemyCount; }
		set
		{
			enemyCount = value;
			enemyCountText.text = enemyCount.ToString();
			if (enemyCount <= 0) {
				StartCoroutine(SpawnNextWave());
			}
		}
	}

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this);
			return;
		}

		turrets = GameObject.FindObjectsOfType<Turret>() as Turret[];

		level1Enemy = Resources.Load("Prefabs/Bomb") as GameObject;
	}

	// Use this for initialization
	void Start () {

		// Shuffle accessories (Knuth shuffle algorithm)
        for (int t = 0; t < accessories.Length; t++ )
        {
            GameObject tmp  = accessories[t];
            int r = Random.Range(t, accessories.Length);
            accessories[t] = accessories[r];
            accessories[r] = tmp;
        }

		// also shuffle gifts
		for (int t = 0; t < gifts.Length; t++ )
        {
            GameObject tmp  = gifts[t];
            int r = Random.Range(t, gifts.Length);
            gifts[t] = gifts[r];
            gifts[r] = tmp;
        }

		for (var i =0; i < turrets.Length && i < accessories.Length; i++) {
			GameObject accessory = accessories[i];
			turrets[i].AddAccessory(accessory);
		}

		for (var i = 0; i < turrets.Length && i < gifts.Length; i++) {
			turrets[i].SetLike(gifts[i]);
			turrets[i].SetDislike(gifts[(i+1)%gifts.Length]);
		}

		enemyCount = 0;
		nextWaveIn_Text.gameObject.SetActive(false);
		StartCoroutine(SpawnNextWave());
	}
	
	IEnumerator SpawnNextWave() {
		if (currentWaveIndex >= DifficultyLevels.waves.Length) {
			yield break; // exit coroutine
		}

		currentWave = DifficultyLevels.waves[currentWaveIndex]; // TODO: win condition
		currentWaveIndex++;

		if (currentWaveIndex != 1) {
			nextWaveIn_Text.gameObject.SetActive(true);
			float t = 0;
			while (t < timeBetweenWaves) {
				if (currentWaveIndex >= DifficultyLevels.waves.Length) {
					nextWaveIn_Text.text = "Final wave begins in " + (timeBetweenWaves-t).ToString("F0") + "..";
				} else {
					nextWaveIn_Text.text = "Next wave begins in " + (timeBetweenWaves-t).ToString("F0") + "..";
				}
				t += Time.deltaTime;
				yield return null;
			}
			nextWaveIn_Text.gameObject.SetActive(false);
		}

		EnemyCount += currentWave.noOfLevel1Enemies;

		for (int i = 0; i < currentWave.noOfLevel1Enemies; i++) {
			int r = Random.Range(0, spawnLocations.Length);
			Vector3 pos = spawnLocations[r].position + new Vector3(Random.Range(-spawnAreaRadius, spawnAreaRadius), 0, Random.Range(-spawnAreaRadius, spawnAreaRadius));
			Instantiate(level1Enemy, pos, Random.rotation);

			yield return new WaitForSeconds(.2f); // wait a small time between each enemy spawn
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
