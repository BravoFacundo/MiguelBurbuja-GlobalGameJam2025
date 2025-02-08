using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum VolumeType { Master, SFX, Music }
public class ButtonSlider : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] VolumeType volumeType;
    [SerializeField] int startValue;
    private List<GameObject> buttons = new List<GameObject>();
    private const int MaxBubbles = 10;

    [Header("Prefabs")]
    public GameObject bubbleButtonPrefab;
    public GameObject addButtonPrefab;

    [Header("Local References")]
    public Transform parent;

    [Header("References")]
    public MusicManager musicManager;


    void Start() => InitializeButtons(startValue);
    void InitializeButtons(int initialCount)
    {
        for (int i = 0; i < initialCount; i++)
        {
            AddBubbleButton();
        }
        UpdateButtons();
    }

    public void AddBubbleButton()
    {
        if (buttons.Count < MaxBubbles)
        {
            GameObject newButton = Instantiate(bubbleButtonPrefab, parent);
            newButton.GetComponent<Button>().onClick.AddListener(() => RemoveBubbleButton(newButton));
            buttons.Add(newButton);
        }
        UpdateButtons();
    }

    public void RemoveBubbleButton(GameObject button)
    {
        buttons.Remove(button);
        Destroy(button);
        UpdateButtons();
    }

    void UpdateButtons()
    {
        // Eliminar botón agregar si existe
        buttons.RemoveAll(b => b.CompareTag("AddButton"));
        foreach (Transform child in parent)
        {
            if (child.CompareTag("AddButton"))
                Destroy(child.gameObject);
        }

        // Agregar botón agregar solo si hay menos de 10 burbujas
        if (buttons.Count < MaxBubbles)
        {
            GameObject addButton = Instantiate(addButtonPrefab, parent);
            addButton.GetComponent<Button>().onClick.AddListener(() => AddBubbleButton());
            buttons.Add(addButton);
        }

        // Actualizar el valor en el MusicManager
        //musicManager.SetVolume(volumeType, buttons.Count);
    }
}
