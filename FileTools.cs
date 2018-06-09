using System;
using System.IO;
using UnityEngine;

class FileTools
{
    private static string appliationPath;
    private static string dataPath;

    private static bool configured = false;

    public static void ConfigureTools(string aPath, string dPath)
    {
        appliationPath = aPath;
        dataPath = dPath;

        configured = true;
    }

    public static string FileToText(string filepath)
    {
        //Yeah yeah, throw an error here :P
        if (!configured) return null;

        String path = appliationPath + dataPath + filepath;

        //Attempt to load json data from file.
        if (File.Exists(path))
        {            
            return File.ReadAllText(path);
        }
        else
        {
            Debug.Log("File: " + path + " does not exist.");
            return null;
        }
    }

    public static void TextToFile(string filepath, string data)
    {


        //Yeah yeah, throw an error here :P
        if (!configured) return;

        String path = appliationPath + dataPath + filepath;

        File.WriteAllText(path, data);
    }
    
    public static void MoveFile(string filesource, string filedest)
    {        
        String pathS = appliationPath + dataPath + filesource;
        String pathD = appliationPath + dataPath + filedest;

        if (File.Exists(pathD)) File.Delete(pathD);

        File.Move(pathS, pathD);
    }
}

