using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "You delivered " + PlayerPrefs.GetInt("Points", 0) + " liters of blood to your master";
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
