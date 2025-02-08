using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class LevelButtonsLayout : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] int totalLevels = 12;
    public UnityEvent<int> onLevelButtonPressed = new UnityEvent<int>();

    [Header("Prefab")]
    [SerializeField] GameObject levelButtonPrefab;


    private void Start()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject newButton = Instantiate(levelButtonPrefab, transform);
            
            newButton.name = "LevelButton_" + i;
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = "Level " + i;

            Button buttonComponent = newButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.RemoveAllListeners();
                //buttonComponent.onClick.AddListener(() => InvokeButtonEvent(buttonComponent, levelIndex));
            }
        }
    }

    private void InvokeButtonEvent(Button button, int level)
    {
        var persistentEvent = button.onClick;
        /*
        foreach (var call in persistentEvent.GetPersistentEventCount())
        {
            if (persistentEvent.GetPersistentTarget(call) is NavigationManager navigationManager)
            {
                navigationManager.Button_SelectLevel(level);
            }
        }
        */
        button.onClick.Invoke();
    }
}
