using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{
    float t = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * 2f * Time.deltaTime;

        t += Time.deltaTime;
        if (t > 4)
        {
            Destroy(gameObject);
        }
    }
}
