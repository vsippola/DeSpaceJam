using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


public class LevelLoadFromFile
{
    private static string gameDataProjectFilePath = "/_Data/LevelData/";

    public static LevelMoment LoadFromFile(string file)
    {
        LevelMoment result;
        string jsonFile = "level0.json";
        string filePath = Application.dataPath + gameDataProjectFilePath + jsonFile;

        //Attempt to load json data from file.
        if (File.Exists(filePath))
        {
            Debug.Log("File: " + filePath + " exists. Attempting to parse.");
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

        Debug.Log(result.x + ", " + result.y + ", " + result.z);
        Debug.Log(result.levelAsString);
        Debug.Log(JsonUtility.ToJson(result));

        result.BuildLevel();

        result.LogLevel();

        //Load spawner location/capacity/frequency/queue (later)

        return result;
    }

}

