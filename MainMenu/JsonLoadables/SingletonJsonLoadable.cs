using System;

//Extends json loadable class to a singleton
[Serializable]
public class SingletonJsonLoadable<T> : JsonLoadable<T>
    where T: new()
{
     private static SingletonJsonLoadable<T> instance;
     private static readonly System.Object padlock = new System.Object();

     protected SingletonJsonLoadable() { }

     public static SingletonJsonLoadable<T> Instance
     {
         get
         {
             if(instance == null)
             {
                 lock(padlock)
                 {
                     if(instance == null)
                     {
                         instance = new SingletonJsonLoadable<T>();
                     }
                 }
             }

             return instance;
         }
     }

}

