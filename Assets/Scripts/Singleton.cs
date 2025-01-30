/*****************************************************************************
// File Name :         Singleton.cs
// Author :            Kyle Grenier
// Creation Date :     09/29/2021
//
// Brief Description : Defines a class with a single instance.
*****************************************************************************/
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
        }
        else
        {
            Destroy(this);
        }
    }
}