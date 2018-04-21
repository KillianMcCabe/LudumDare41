using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public GameObject grass;

	int mapWidth = 8;
	int mapHeight = 8;

	public void GenerateMap() {
		var mapGO = new GameObject();
		mapGO.name = "map";
		
		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				GameObject go = Instantiate(grass, new Vector3(x * 16, 0, y * 16), Quaternion.identity);
				go.transform.SetParent(mapGO.transform);
			}
		}
	}
}
