using System;
using UnityEngine;

[Serializable]
public class JsonLoadable<T> : IJsonLoadable 
    where T : new()
{
    public static string path = null;
    public static string backupPath = null;
    private static bool configured = false;

    public T data;

    public void Configure(string filepath, string defaultFileath)
    {
        path = filepath;
        backupPath = defaultFileath;
        configured = true;
    }

    public void Load()
    {
        if (!configured) return;

        string json;

        //Try to load the given file
        json = LoadFile(path);        
        if (JsonToData(json)) return;

        //Try to load the backup file
        json = LoadFile(backupPath);
        if (JsonToData(json)) return;

        //Otherwise default values set in the data object will be loaded
        this.data = new T();
    }

    public void Save()
    {
        //Update the backup
        FileTools.MoveFile(path, backupPath);

        string json = JsonUtility.ToJson(data);
        FileTools.TextToFile(path, json);
    }

    private bool JsonToData(string json)
    {
        //If there was a problem with the file, don't update data object
        if (json == null) return false;
        
        //Otherwise, try to parse the json into a data object
        T stringData = JsonUtility.FromJson<T>(json);

        //If there was a problem with the json parsing, don't update the data object
        if (stringData == null) return false;

        //Otherwise update the data object
        this.data = stringData;
        return true;
    }

    private string LoadFile(string file)
    {
        try
        {
            return FileTools.FileToText(file);
        }
        catch (Exception e)
        {
            Debug.Log("Problem Loading Level: " + e.StackTrace);
            return null;
        }
    }

    public bool IsConfigured()
    {
        return configured;
    }
}


