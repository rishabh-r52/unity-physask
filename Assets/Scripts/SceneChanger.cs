using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("nextLevel", 2f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void nextLevel()
    {
        SceneManager.LoadScene(0);
    }
}

