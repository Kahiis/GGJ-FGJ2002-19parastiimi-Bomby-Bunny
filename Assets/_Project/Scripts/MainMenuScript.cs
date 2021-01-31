using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject MainMenu;
    public GameObject CreditsMenu;

    void Start()
    {
        CreditsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Lataa peliskeneen
    public void PlayGame()
    {
        Debug.Log("Should go to game");
        SceneManager.LoadScene(1);
    }

    public void ShowCredits()
    {
        Debug.Log("Should show credits");
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }

    public void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        CreditsMenu.SetActive(false);
    }
}
