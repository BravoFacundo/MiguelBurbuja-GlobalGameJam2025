using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavigationManager : MonoBehaviour
{
    Canvas canvas;
    Dictionary<string, GameObject> screenD = new();

    [Header("Configuration")]
    [SerializeField] float pressDelay = 1f;

    [Header("Sounds")]
    [SerializeField] AudioClip buttonSFX;
    [SerializeField] AudioClip exitSFX;

    [Header("References")]
    [SerializeField] TMP_Text playButtonText;
    [SerializeField] TMP_Text TutorialButtonText;
    
    GameManager gameManager;

    void Awake()
    {
        gameManager = transform.parent.GetComponentInChildren<GameManager>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        
        GetScreens(canvas);
        Utilities.DeactivateAllChildrens(canvas.transform);
    }

    private void Update() => Handle_Inputs();

    //---------- INPUTS ------------------------------------------------------------------------------------------------------------------

    private void Handle_Inputs()
    {
        if (Input.anyKeyDown)
        {
            if (gameManager.currentGameState == GameState.Lose)
                Delayed_Action(GameState.Game, buttonSFX);
            else if (gameManager.currentGameState != GameState.Menu)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                    Delayed_Action(GameState.Menu, exitSFX);
            }
        }
    }

    //---------- SCREENS ------------------------------------------------------------------------------------------------------------------

    private void GetScreens(Canvas canvas)
    {
        foreach (Transform child in canvas.transform)
        {
            GameObject screen = child.gameObject;
            string[] parts = screen.name.Split('_');
            if (parts.Length == 2)
            {
                string id = parts[0];
                string name = parts[1];

                if (!screenD.ContainsKey(id)) screenD.Add(id, screen);
                if (!screenD.ContainsKey(name)) screenD.Add(name, screen);
            }
        }
    }

    public void ActivateScreen(object screenIdentifier)
    {
        foreach (GameObject screen in screenD.Values) screen.SetActive(false);

        string key = screenIdentifier is int index ? index.ToString() : screenIdentifier.ToString();
        if (screenD.ContainsKey(key))
        {
            screenD[key].SetActive(true);
            string[] parts = screenD[key].name.Split('_'); //Debug.Log(parts[1]);
            gameManager.currentScreen = parts[1];
        }
        else Debug.LogError($"Pantalla no encontrada: {screenIdentifier}");
    }

    //---------- ACTIONS ------------------------------------------------------------------------------------------------------------------

    private void Delayed_Action(object target, AudioClip sfx)
        => StartCoroutine(Delayed_Action(target, pressDelay, sfx));
    private IEnumerator Delayed_Action(object target, float delay, AudioClip sfx)
    {
        SoundManager.PlaySoundAndDestroy(sfx);
        yield return new WaitForSeconds(delay);

        if (target is GameState gameState) gameManager.SetGameState(gameState);
        else if (target is int index) ActivateScreen(index.ToString());
        else if (target is string str)
        {
            if (int.TryParse(str, out int numIndex)) ActivateScreen(numIndex.ToString());
            else ActivateScreen(str);
        }
        else Debug.LogError($"Tipo de parámetro no válido en Delayed_Action: {target}");
    }

    //---------- BUTTONS ------------------------------------------------------------------------------------------------------------------
    
    public void Button_Play() => Delayed_Action("Lore", buttonSFX);
    public void Button_LevelSelector() => Delayed_Action("LevelSelector", buttonSFX);
    public void Button_Configuration() => Delayed_Action("Configuration", buttonSFX);
    public void Button_Credits() => Delayed_Action("Credits", buttonSFX);
    public void Button_Next()
    {
        if(gameManager.currentScreen == "Lore")
        {
            if (gameManager.playerData.maxLevelReach > 1)
            {
                Delayed_Action("Tutorial", buttonSFX);
                return;
            }
        }
        Delayed_Action(GameState.Game, buttonSFX);
    }
    public void Button_Back()
    {
        if (gameManager.currentScreen == "Tutorial") Delayed_Action("Lore", buttonSFX);
        else Delayed_Action("Menu", buttonSFX);
    }
    public void Button_Exit() => StartCoroutine(gameManager.ExitGame());
    public void Button_SelectLevel(int levelNumber)
    {
        gameManager.playerData.currentLevel = levelNumber;
        Delayed_Action(GameState.Game, buttonSFX);
    }

    public void Button_Continue() => Delayed_Action(GameState.Game, buttonSFX);
    public void Button_Retry() => Delayed_Action(GameState.Game, buttonSFX);

    public void HasPreviousProgress()
    {
        playButtonText.text = "Continuar";
        TutorialButtonText.text = "Jugar";
    }

}
