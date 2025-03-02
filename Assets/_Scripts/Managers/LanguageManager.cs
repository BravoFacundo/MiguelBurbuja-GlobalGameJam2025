using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class LanguageManager : MonoBehaviour
{
    [Header("Configuration")]
    public int currentLanguage = 0;
    [SerializeField] bool generateCSV = false;

    [Header("CSV")]
    public TextAsset csvFile;
    public string csvFileName = "localization";

    [Header("References")]
    public List<TextMeshProUGUI> texts;
    public Dictionary<string, string[]> translations = new();

    private void Awake() => LoadCSV();
    public void SetLanguage(int languageID)
    {
        currentLanguage = languageID;

        foreach (var text in texts)
        {
            string key = text.transform.parent != null ? text.transform.parent.name : text.gameObject.name;

            if (translations.ContainsKey(key))
                text.text = translations[key][currentLanguage];
        }
    }

    //---------- CSV ------------------------------------------------------------------------------------------------------------------

    private void LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);
        if (csvFile == null)
        {
            Debug.LogError($"No se encontró el archivo CSV en Resources: {csvFileName}.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] columns = line.Split(',');

            if (columns.Length < 3)
            {
                Debug.LogWarning($"Skipping malformed CSV line: {line}");
                continue;
            }

            string key = columns[0].Trim();
            string englishText = columns[1].Trim();
            string spanishText = columns[2].Trim();

            translations[key] = new string[] { englishText, spanishText };
        }
    }
#if UNITY_EDITOR
    private void Start() { if (generateCSV) GenerateCSV(); }
    public void GenerateCSV()
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", csvFileName + ".csv");

        List<string> lines = new() { "Key,English,Spanish" };

        foreach (var text in texts)
        {
            string key = text.transform.parent != null ? text.transform.parent.name : text.gameObject.name;
            string englishText = text.text; // Tomamos el texto actual (en inglés) del componente TextMeshProUGUI
            lines.Add($"{key},{englishText},"); // Añadimos la key y el texto en inglés vacío para la columna en español
        }

        File.WriteAllLines(filePath, lines);
        Debug.Log($"CSV generado en: {filePath}");
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
