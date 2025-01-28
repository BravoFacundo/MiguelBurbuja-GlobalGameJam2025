using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavigationManager : MonoBehaviour
{
    GameManager gameManager;

    [Header("Screens")]
    [SerializeField] Canvas canvas;
    [SerializeField] private List<GameObject> screens = new();

    [Header("Buttons")]
    [SerializeField] float pressDelay = 1f;
    [SerializeField] AudioClip buttonSFX;
    [SerializeField] AudioClip exitSFX;

    [Header("References")]
    [SerializeField] Button playButton;
    [SerializeField] Button continueButton;

    void Awake()
    {
        gameManager = transform.parent.GetComponentInChildren<GameManager>();
        GetScreens();
    }

    // Método para verificar el progreso previo del jugador
    public void CheckPreviousPlay(int maxLevelReached)
    {
        TMP_Text playButtonText = playButton.GetComponentInChildren<TMP_Text>();
        TMP_Text continueButtonText = continueButton.GetComponentInChildren<TMP_Text>();
        if (maxLevelReached == 1)
        {
            playButtonText.text = "Jugar";
            continueButtonText.text = "Jugar";
        }
        else
        {
            playButtonText.text = "Continuar";
            continueButtonText.text = "Continuar";
        }
    }

    // Método para obtener todas las pantallas y desactivarlas
    public void GetScreens()
    {
        foreach (Transform child in canvas.transform)
        {
            screens.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    // Método para activar una pantalla específica
    public void SetScreen(int index) => screens[index].SetActive(true);

    // Método para desactivar todas las pantallas
    public void DisableAllScreens()
    {
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
    }

    // Métodos para manejar los botones con retraso
    public void Button_Play() => StartCoroutine(nameof(Delayed_Play));
    public IEnumerator Delayed_Play()
    {
        Utilities.PlaySoundAndDestroy(buttonSFX);
        yield return new WaitForSeconds(pressDelay);
        gameManager.SetGameState(GameState.LoreScreen);
    }

    public void Button_LevelSelector() => StartCoroutine(nameof(Delayed_LevelSelector));
    public IEnumerator Delayed_LevelSelector()
    {
        Utilities.PlaySoundAndDestroy(buttonSFX);
        yield return new WaitForSeconds(pressDelay);
        gameManager.SetGameState(GameState.LevelSelector);
    }

    public void Button_Credits() => StartCoroutine(nameof(Delayed_Credits));
    public IEnumerator Delayed_Credits()
    {
        Utilities.PlaySoundAndDestroy(buttonSFX);
        yield return new WaitForSeconds(pressDelay);
        gameManager.SetGameState(GameState.CreditsScreen);
    }

    public void Button_Back() => StartCoroutine(nameof(Delayed_Back));
    public IEnumerator Delayed_Back()
    {
        Utilities.PlaySoundAndDestroy(exitSFX);
        yield return new WaitForSeconds(pressDelay);
        if (gameManager.currentGameState == GameState.TutorialScreen)
            gameManager.SetGameState(GameState.LoreScreen);
        else gameManager.SetGameState(GameState.Menu);
    }

    public void Button_Continue() => StartCoroutine(nameof(Delayed_Continue));
    public IEnumerator Delayed_Continue()
    {
        Utilities.PlaySoundAndDestroy(buttonSFX);
        yield return new WaitForSeconds(pressDelay);
        if (gameManager.currentGameState == GameState.LoreScreen)
            gameManager.SetGameState(GameState.TutorialScreen);
        else if (gameManager.currentGameState == GameState.TutorialScreen)
            gameManager.SetGameState(GameState.Game);
        else if (gameManager.currentGameState == GameState.WinScreen || gameManager.currentGameState == GameState.LoseScreen)
            gameManager.SetGameState(GameState.Menu);
        else gameManager.SetGameState(GameState.Game);
    }

    public void Button_Next() => StartCoroutine(nameof(Delayed_Next));
    public IEnumerator Delayed_Next()
    {
        Utilities.PlaySoundAndDestroy(buttonSFX);
        yield return new WaitForSeconds(pressDelay);
        gameManager.SetGameState(GameState.Game);
    }

    public void Button_Exit() => StartCoroutine(nameof(Delayed_Exit));
    public IEnumerator Delayed_Exit()
    {
        Utilities.PlaySoundAndDestroy(exitSFX);
        gameManager.SavePlayerProgress();
        yield return new WaitForSeconds(pressDelay * 2);
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
