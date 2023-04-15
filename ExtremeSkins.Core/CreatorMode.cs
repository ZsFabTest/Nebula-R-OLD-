using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExtremeSkins.Core;

public static class CreatorMode
{
    public const string CreatorModeFolder = "CreatorMode";
    public const string TranslationCsvFile = "translation.csv";
    public const string Comma = ",";

    private const string creatorModePlaceHolder = "CreatorMode = {0}";

    public enum SupportedLangs
    {
        English,
        Latam,
        Brazilian,
        Portuguese,
        Korean,
        Russian,
        Dutch,
        Filipino,
        French,
        German,
        Italian,
        Japanese,
        Spanish,
        SChinese,
        TChinese,
        Irish
    }

    public static StreamWriter CreateTranslationWriter(string amongUsPath)
    {
        CreateCreatorModeFolder(amongUsPath);

        StreamWriter transCsv = new StreamWriter(
            GetTranslationCsvPath(amongUsPath), false, new UTF8Encoding(true));

        List<string> langList = new List<string>();

        foreach (SupportedLangs enumValue in Enum.GetValues(typeof(SupportedLangs)))
        {
            langList.Add(enumValue.ToString());
        }

        transCsv.WriteLine(
            string.Format(
                "{1}{0}{2}",
                Comma,
                "TransKey",
                string.Join(Comma, langList)));

        return transCsv;
    }

    public static StreamReader GetTranslationReader(string amongUsPath)
        => new StreamReader(GetTranslationCsvPath(amongUsPath), new UTF8Encoding(true));

    public static StreamWriter GetTranslationWriter(string amongUsPath)
    {
        CreateCreatorModeFolder(amongUsPath);
        string csvFile = GetTranslationCsvPath(amongUsPath);
        bool isFileExist = IsExistTransFile(amongUsPath);

        StreamWriter transCsv = new StreamWriter(
            csvFile, isFileExist, new UTF8Encoding(true));

        if (!isFileExist)
        {
            List<string> langList = new List<string>();

            foreach (SupportedLangs enumValue in Enum.GetValues(typeof(SupportedLangs)))
            {
                langList.Add(enumValue.ToString());
            }

            transCsv.WriteLine(
                string.Format(
                    "{1}{0}{2}",
                    Comma,
                    "TransKey",
                    string.Join(Comma, langList)));
        }

        return transCsv;
    }

    public static bool IsExistTransFile(string amongUsPath)
    {
        string csvFile = Path.Combine(amongUsPath, CreatorModeFolder, TranslationCsvFile);
        return File.Exists(csvFile);
    }

    public static void SetCreatorMode(string amongUsPath, bool active)
    {
        string cfgPath = Path.Combine(amongUsPath, Config.Path);
        string text;

        using (var cfg = new StreamReader(cfgPath, new UTF8Encoding(true)))
        {
            text = cfg.ReadToEnd();
        }
        text = text.Replace(
            string.Format(creatorModePlaceHolder, (!active).ToString().ToLower()),
            string.Format(creatorModePlaceHolder, active.ToString().ToLower()));

        using var newCfg = new StreamWriter(cfgPath, false, new UTF8Encoding(true));
        newCfg.Write(text);
    }

    private static void CreateCreatorModeFolder(string targetPath)
    {
        string csvFolder = Path.Combine(targetPath, CreatorModeFolder);

        if (!Directory.Exists(csvFolder))
        {
            Directory.CreateDirectory(csvFolder);
        }
    }

    private static string GetTranslationCsvPath(string amongUsPath)
        => Path.Combine(amongUsPath, CreatorModeFolder, TranslationCsvFile);
}
