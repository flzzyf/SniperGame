using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T Instance;
    public static T instance
    {
        get
        {
            if(Instance == null) {
                T[] temps = FindObjectsOfType<T>();

                if (temps.Length == 0) {
                    Debug.LogError("未能找到" + typeof(T).Name + "的实例。");
                    return null;
                }

                //总是返回最后一个，也就是最新的（从之前场景保留下来的
                Instance = temps[temps.Length - 1];
            }

            return Instance;
        }
    }

    void Awake()
    {
        //已有实例则自毁
        if(FindObjectsOfType<T>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
