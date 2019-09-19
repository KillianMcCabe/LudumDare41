using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinPanel : MonoBehaviour
{
    float t = 0;
    public GameObject text;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        text.transform.localScale += new Vector3(1, 1, 1) * 0.02f * Time.deltaTime;

        t += Time.deltaTime;
        if (t > 4)
        {
            SceneController.instance.LoadScene("Success");
            Destroy(this);
        }
    }
}
