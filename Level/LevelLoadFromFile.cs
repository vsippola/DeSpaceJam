using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


public class LevelLoadFromFile
{
    private static readonly string gameDataProjectFilePath = "/StreamingAssets/LevelData/";

    /*
    public static LevelMoment LoadFromFile(string file)
    {
        LevelMoment result;
        string jsonFile = "level0.json";
        string filePath = Application.dataPath + gameDataProjectFilePath + jsonFile;

        //Attempt to load json data from file.
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            try
            {
                result = JsonUtility.FromJson<LevelMoment>(dataAsJson);
            }
            catch (Exception e)
            {
                Debug.Log("Problem Loading Level: " + e.StackTrace);
                return null;
            }
        }
        else
        {
            Debug.Log("File: " + filePath + " does not exist.");
            return null;
        }

        result.BuildLevel();

        result.LogLevel();

        return result;
    }*/

}

