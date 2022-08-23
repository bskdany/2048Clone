using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject pointsText;
    public void Setup(int Score)
    {
        gameObject.SetActive(true);
        //pointsText.GetComponent<Text>().text = Score +" POINTS";
    }

    public void restartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
