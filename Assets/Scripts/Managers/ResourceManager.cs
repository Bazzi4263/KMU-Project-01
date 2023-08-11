using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }

    //Prefabs 산하 폴더에 있는 prefab 중 원하는 것을 꺼내옴
    public GameObject Instantiate(string path, Vector3 pos = default(Vector3), Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        //if (original.GetComponent<Poolable>() != null)
        //    return Manager.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        go.transform.position = pos;
        return go;
    }

    public string[] GetAllName(string path, Vector3 pos = default(Vector3), Transform parent = null)
    {
        GameObject[] original = LoadAll<GameObject>($"Prefabs/{path}");
        string[] names = new string[original.Length];
        if (original == null)
        {
            Debug.Log($"Failed to load all prefabs name : {path}");
            return null;
        }

        //if (original.GetComponent<Poolable>() != null)
        //    return Manager.Pool.Pop(original, parent).gameObject;
        for (int i = 0; i < original.Length; i++)
        {
            names[i] = original[i].name;
        }
        return names;
    }

    public void Destroy(GameObject go, float num = 0.0f)
    {
        if (go == null)
            return;

        //Poolable poolable = go.GetComponent<Poolable>();
        //if (poolable != null)
        //{
        //    Managers.Pool.Push(poolable);
        //    return;
        //}
        //num초 후 파괴
        Object.Destroy(go, num);
    }
}
