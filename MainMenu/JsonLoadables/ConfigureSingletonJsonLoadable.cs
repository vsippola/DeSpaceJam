using System;
using UnityEngine;

[Serializable]
public class ConfigureSingletonJsonLoadable<T>
    where T: new()
{
    public string path;
    public string pathBkp;
    public DataPairs type;

    SingletonJsonLoadable<T> loadable;

    public ConfigureSingletonJsonLoadable(string path, string pathBkp)
    {
        this.path = path;
        this.pathBkp = pathBkp;
    }

    public void Configure()
    {        
        loadable = SingletonJsonLoadable<T>.Instance;
        if (loadable.IsConfigured()) return;
        loadable.Configure(path, pathBkp);

        //Throw error if not configured        
    }

    //Seperated as may want to order loading.
    //May want to be another class
    public void Load()
    {
        if (loadable.data == null) loadable.Load();
    }

}

