using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public GameObject grass;

	int mapWidth = 10;
	int mapHeight = 10;

	float tileSize = 16;

	public void GenerateMap() {
		var mapGO = new GameObject();
		mapGO.name = "Map";

		float mapOffsetX = -(((mapWidth) / 2) * tileSize) + tileSize / 2;
		float mapOffsetY = -(((mapHeight) / 2) * tileSize) + tileSize / 2;
		
		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				GameObject go = Instantiate(grass, new Vector3(x * tileSize + mapOffsetX, 0, y * tileSize + mapOffsetY), Quaternion.identity);
				go.transform.SetParent(mapGO.transform);
			}
		}
	}
}
