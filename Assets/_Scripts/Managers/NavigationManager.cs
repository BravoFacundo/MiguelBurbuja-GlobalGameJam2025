using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] List<GameObject> screens = new();
    Canvas canvas;

    [Header("Buttons")]
    [SerializeField] float pressDelay = 1f;
    [SerializeField] AudioClip buttonSFX;
    [SerializeField] AudioClip exitSFX;

    [Header("References")]
    GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponentInParent<GameManager>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

        GetAllScreens();
    }

    //---------- SCREENS ------------------------------------------------------------------------------------------------------------------

    public void GetAllScreens()
    {
        screens = canvas.transform.Cast<Transform>().Select(child => child.gameObject).ToList();
        Utilities.DeactivateAllChildrens(canvas.transform);
    }
    public void DisableAllScreens()
    {
        foreach (GameObject screen in screens) { screen.SetActive(false); }
    }
    public void SetScreen(int index) => screens[index].SetActive(true);

    //---------- BUTTONS ------------------------------------------------------------------------------------------------------------------

    private void Delayed_Action(GameState gameState, AudioClip sfx) => 
        StartCoroutine(Delayed_Action(gameState, pressDelay, sfx));
    private IEnumerator Delayed_Action(GameState gameState, float delay, AudioClip sfx)
    {
        Utilities.PlaySoundAndDestroy(sfx);
        yield return new WaitForSeconds(delay);
        gameManager.SetGameState(gameState);
    }

    public void Button_Play() => Delayed_Action(GameState.LoreScreen, buttonSFX);
    public void Button_LevelSelector() => Delayed_Action(GameState.LevelSelector, buttonSFX);
    public void Button_Credits() => Delayed_Action(GameState.CreditsScreen, buttonSFX);
    public void Button_Next() => Delayed_Action(GameState.Game, buttonSFX);
    public void Button_Retry() => Delayed_Action(GameState.Game, buttonSFX);
    public void Button_Exit() => StartCoroutine(Delayed_Action(GameState.Game, pressDelay * 2, buttonSFX));
    public void Button_Back()
    {
        if (gameManager.currentGameState == GameState.TutorialScreen)
            Delayed_Action(GameState.LoreScreen, buttonSFX);
        else Delayed_Action(GameState.Menu, buttonSFX);
    }

    public void Button_Continue()
    {
        if (gameManager.currentGameState == GameState.LoreScreen)
            Delayed_Action(GameState.TutorialScreen, buttonSFX);
        else if (gameManager.currentGameState == GameState.TutorialScreen)
            Delayed_Action(GameState.Game, buttonSFX);
        else if (gameManager.currentGameState == GameState.WinScreen || gameManager.currentGameState == GameState.LoseScreen)
            Delayed_Action(GameState.Menu, buttonSFX);
        else gameManager.SetGameState(GameState.Game);
    }

}
