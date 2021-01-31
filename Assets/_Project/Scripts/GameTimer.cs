using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    // pelin maksimiaika sekunteina
    public float MaxGameTime = 300;
    public float AlertThreshold = 60;
    public string TimeLeftText = "TIME LEFT: ";
    public string GameOverText = "GAME OVER!! YOU SUCK";
    // public GameObject ResetButton;
    public GameObject GameOverImage;
    public GameObject YouWinImage;
    float TimeLeft;
    bool LOTSASPAGHETTI = true;

    Text TimerText;
    // Start is called before the first frame update
    void Start()
    {
        // ResetButton.SetActive(false);
        GameOverImage.SetActive(false);
        YouWinImage.SetActive(false);
        TimeLeft = MaxGameTime;
        TimerText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeLeft >= 0)
        {
            TimeLeft -= Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(TimeLeft);
            TimerText.text = TimeLeftText + t.ToString(@"mm\:ss");
        } else if (LOTSASPAGHETTI)
        {
            LOTSASPAGHETTI = false;
            GameOver();
        }
    }

    void GameOver()
    {
        // TimerText.text = GameOverText;
        // ResetButton.SetActive(true);
        GameOverImage.SetActive(true);
        FindObjectOfType<CharacterControls>().LoseGame();
    }

    public void GameWin()
    {
        // ResetButton.SetActive(true);
        YouWinImage.SetActive(true);
        LOTSASPAGHETTI = false;
        TimerText.gameObject.SetActive(false);
    }


    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetGameTime()
    {
        TimeLeft = MaxGameTime;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
