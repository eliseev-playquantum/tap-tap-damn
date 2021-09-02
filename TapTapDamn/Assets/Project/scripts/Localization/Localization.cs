using UnityEngine;
using System.Collections.Generic;

public class Localization
{
    const string PATH_TO_LOCALIZATION = "Data/Localization";

    private static Localization _instance;
    public static Localization instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Localization();
            }
            return _instance;
        }
    }

    public enum LocalizationLanguage
    {
        EN = 0,
        RU = 1
    }

    public LocalizationLanguage Lang;

    Dictionary<string, string> _dictionary = null;

    public LocalizationLanguage SetLanguage(string inLang, bool saveLang = true)
    {
        //Задаем язык интерфейся
        if (inLang == "")
            inLang = LanguageHelper.Get2LetterISOCodeFromSystemLanguage();

        Lang = ParseEnum(inLang);

        //Парсим файл
        //ParseXmlFile();
        ParseCSVFile();

        //---пока закомичено
        // Сохраняем выбранный язык
        if (saveLang)
            GameSettings.language = inLang;

        //Генерируем событие о смене языка
        Events.instance.Raise(new EventSetLang());

        return Lang;
    }


    #region Parse
    private void ParseCSVFile()
    {
        //Очищаем словарь
        if (_dictionary != null)
            _dictionary.Clear();
        else
            _dictionary = new Dictionary<string, string>();

        // Загружаем и парсим файл
        TextAsset loadtext = Resources.Load<TextAsset>(PATH_TO_LOCALIZATION);
        string[,] grid = CSVReader.SplitCsvGrid(loadtext.text);


        // находим столбец с выбранным языком
        int langRow = 1;
        for (int x = 1; x < grid.GetUpperBound(0); x++)
            if (grid[x, 0] == Lang.ToString())
            {
                langRow = x;
                break;
            }

        // Заносим этот столбец в словарь
        for (int y = 1; y < grid.GetUpperBound(1); y++)
        {
            if (grid[0, y] == "") continue;
            if (grid[0, y][0] == '$') continue;

            string wordCode = grid[0, y];
            string wordValue = grid[langRow, y];

            if (wordValue == "")
            {
                wordValue = grid[1, y];
                if (wordValue == "")
                    wordValue = wordCode;
            }

            DictionaryAdd(wordCode, wordValue);
        }

        loadtext = null;
        Resources.UnloadUnusedAssets();
    }

    void DictionaryAdd(string wordCode, string wordValue)
    {
        string wordCodeLower = wordCode.ToLower();
        if (!_dictionary.ContainsKey(wordCodeLower))
            _dictionary.Add(wordCodeLower, wordValue);

        //else MyDebug.LogError("Dictionary ContainsKey " + wordCodeLower + " " + wordValue);
    }
    #endregion


    public static string GetText(string key)
    {
        return instance.getTextByKey(key);
    }

    public string getTextByKey(string key)
    {
        if ((key == null) || (key == ""))
            return "";

        string keyLower = key.ToLower();
        if ((_dictionary != null) && (_dictionary.ContainsKey(keyLower)))
            return _dictionary[keyLower];

        return keyLower;
    }

    public void PrintDict()
    {
        foreach (string key in _dictionary.Keys)
        {
            Debug.Log(key + "  " + _dictionary[key]);
        }
    }

    public LocalizationLanguage SetNextLang()
    {
        int id = (int)Lang + 1;
        string nextLangName = Localization.ParseEnum(id).ToString();

        return SetLanguage(nextLangName);
    }

    private static LocalizationLanguage ParseEnum(string value)
    {
        try
        {
            LocalizationLanguage result = (LocalizationLanguage)System.Enum.Parse(typeof(LocalizationLanguage), value, true);
            return result;
        }
        catch
        {
            return LocalizationLanguage.EN;
        }

    }

    private static LocalizationLanguage ParseEnum(int value)
    {
        string[] names = System.Enum.GetNames(typeof(LocalizationLanguage));


        if (value < names.Length)
            return (LocalizationLanguage)value;
        else
            return (LocalizationLanguage)0;

    }

}
