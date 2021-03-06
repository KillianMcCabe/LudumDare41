﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{

    public GameObject grass;

    [System.Serializable]
    public struct PropData // TODO: use ScriptableObjects
    {
        public GameObject prefab;
        public int count;
    }

    public PropData[] propData;

    [SerializeField]
    GameObject _pebblesPrefab = null;

    int mapWidth = 10;
    int mapHeight = 10;

    float tileSize = 16;

    public void GenerateMap()
    {
        GameObject oldMap = GameObject.Find("Map");
        DestroyImmediate(oldMap);

        var mapGO = new GameObject();
        mapGO.name = "Map";

        float mapOffsetX = -(((mapWidth) / 2) * tileSize) + tileSize / 2;
        float mapOffsetY = -(((mapHeight) / 2) * tileSize) + tileSize / 2;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameObject go = Instantiate(grass, new Vector3(x * tileSize + mapOffsetX, 0, y * tileSize + mapOffsetY), Quaternion.identity);
                go.transform.SetParent(mapGO.transform);
            }
        }

        // add props to map
        foreach (PropData pd in propData)
        {
            for (int i = 0; i < pd.count; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-((tileSize / 2) * mapWidth) + tileSize / 2, ((tileSize / 2) * mapWidth) - tileSize / 2),
                    0,
                    Random.Range(-((tileSize / 2) * mapHeight) + tileSize / 2, ((tileSize / 2) * mapHeight) - tileSize / 2)
                );
                GameObject go = Instantiate(pd.prefab, pos, Quaternion.Euler(0, Random.Range(0, 360f), 0));
                go.transform.SetParent(mapGO.transform);

                if (pd.prefab.name == "Rock")
                {
                    // spawn secondary rocks
                    SpawnAround(pd.prefab, go, minScale: 0.4f, maxScale: 0.8f);

                    // spawn pebbles around rock
                    SpawnAround(_pebblesPrefab, go);
                }
                else if (pd.prefab.name.Contains("Grass"))
                {
                    // spawn secondary grass
                    SpawnAround(pd.prefab, go);
                }
                else if (pd.prefab.name.Contains("Tree"))
                {
                    // spawn secondary trees
                    SpawnAround(pd.prefab, go, minScale: 0.65f, maxScale: 0.95f);
                }
            }
        }

        // add colliders to edges TODO: use fence prefab

        float colliderHeight = 10;
        var colliderGO1 = new GameObject();
        colliderGO1.transform.SetParent(mapGO.transform);
        colliderGO1.name = "collider";
        colliderGO1.transform.position = new Vector3(0, 0, -((tileSize / 2) * mapHeight));
        colliderGO1.transform.localEulerAngles = new Vector3(0, 0, 0);
        BoxCollider boxCollider1 = colliderGO1.AddComponent<BoxCollider>();
        boxCollider1.size = new Vector3(mapWidth * tileSize, colliderHeight, 1);

        var colliderGO2 = new GameObject();
        colliderGO2.transform.SetParent(mapGO.transform);
        colliderGO2.name = "collider";
        colliderGO2.transform.position = new Vector3(-((tileSize / 2) * mapWidth), 0, 0);
        colliderGO2.transform.localEulerAngles = new Vector3(0, 90, 0);
        BoxCollider boxCollider2 = colliderGO2.AddComponent<BoxCollider>();
        boxCollider2.size = new Vector3(mapWidth * tileSize, colliderHeight, 1);

        var colliderGO3 = new GameObject();
        colliderGO3.transform.SetParent(mapGO.transform);
        colliderGO3.name = "collider";
        colliderGO3.transform.position = new Vector3(0, 0, ((tileSize / 2) * mapHeight));
        colliderGO3.transform.localEulerAngles = new Vector3(0, 0, 0);
        BoxCollider boxCollider3 = colliderGO3.AddComponent<BoxCollider>();
        boxCollider3.size = new Vector3(mapWidth * tileSize, colliderHeight, 1);

        var colliderGO4 = new GameObject();
        colliderGO4.transform.SetParent(mapGO.transform);
        colliderGO4.name = "collider";
        colliderGO4.transform.position = new Vector3(((tileSize / 2) * mapWidth), 0, 0);
        colliderGO4.transform.localEulerAngles = new Vector3(0, 90, 0);
        BoxCollider boxCollider4 = colliderGO4.AddComponent<BoxCollider>();
        boxCollider4.size = new Vector3(mapWidth * tileSize, colliderHeight, 1);

        NavMeshSurface navMesh = mapGO.AddComponent<NavMeshSurface>();

        LayerMask mask = ~0; // all layers
        mask = mask & ~(1 << LayerMask.NameToLayer("SmallTerrainProp")); // ignore SmallTerrainProp layer
        navMesh.layerMask = mask;

        navMesh.BuildNavMesh();
    }

    private void SpawnAround(GameObject prefabToSpawn, GameObject parent, float minScale = 1f, float maxScale = 1f)
    {
        float chanceOfSpawn = 0.6f;
        while (Random.Range(0f, 1f) < chanceOfSpawn)
        {
            Renderer rend = prefabToSpawn.GetComponent<Renderer>();
            float bounds = rend.bounds.extents.magnitude;
            float clusterOffsetMin = bounds;
            float clusterOffsetMax = bounds * 2;
            Vector3 offset = new Vector3(Random.Range(clusterOffsetMin, clusterOffsetMax), 0, 0);
            offset = Quaternion.Euler(0, Random.Range(0, 360f), 0) * offset;

            GameObject pebblesGO = Instantiate(prefabToSpawn);
            pebblesGO.transform.SetParent(parent.transform);
            pebblesGO.transform.localPosition = offset;
            pebblesGO.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            float scale = Random.Range(minScale, maxScale);
            pebblesGO.transform.localScale = new Vector3(scale, scale, scale);

            // reduce chance to spawn more pebbles
            chanceOfSpawn = (chanceOfSpawn / 2) - 0.05f;
        }
    }
}
