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
        screens = canvas.transform.Cast<Transform>().Select(child => child.gameObject).ToList();
        Utilities.DeactivateAllChildrens(canvas.transform);

    }

    //---------- SCREENS ------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Activa una pantalla por id.
    /// </summary>
    /// 
    public void ActivateScreen(int index)
    {
        foreach (GameObject screen in screens) { screen.SetActive(false); }
        screens[index-1].SetActive(true);
    }

    //---------- BUTTONS ------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Delayed action para los botones.
    /// porque pinga esta 2 veces esto?
    /// </summary>
    private void Delayed_Action(GameState gameState, AudioClip sfx) 
        => StartCoroutine(Delayed_Action(gameState, pressDelay, sfx));

    private IEnumerator Delayed_Action(GameState gameState, float delay, AudioClip sfx)
    {
        Utilities.PlaySoundAndDestroy(sfx);
        yield return new WaitForSeconds(delay);
        gameManager.SetGameState(gameState);
    }

    //PLAY
    public void Button_Play() => Delayed_Action(GameState.Game, buttonSFX);
    //LVL SELECTOR 
    public void Button_LevelSelector() => Delayed_Action(GameState.LevelSelector, buttonSFX);
    //CREDITOS
    public void Button_Credits() => Delayed_Action(GameState.CreditsScreen, buttonSFX);
    //SIGUIENTE
    public void Button_Next() => Delayed_Action(GameState.Game, buttonSFX);
    //REINTENTAR
    public void Button_Retry() => Delayed_Action(GameState.Game, buttonSFX);
    //VOLVER
    public void Button_Back()
    {
        if (gameManager.currentGameState == GameState.TutorialScreen)
            Delayed_Action(GameState.LoreScreen, buttonSFX);
        else Delayed_Action(GameState.Menu, buttonSFX);
    }

    /// <summary>
    /// distintos comportamientos de continuar
    /// </summary>
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
    public void Button_Exit() => Delayed_Action(GameState.Exit, exitSFX);

    public void Button_SelectLevel(int levelIndex)
    {
        gameManager.LoadLevel(levelIndex - 1);
        Delayed_Action(GameState.Game, buttonSFX);
    }

}
