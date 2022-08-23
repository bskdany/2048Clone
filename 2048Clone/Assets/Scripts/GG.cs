using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GG : MonoBehaviour
{
    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void restartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
