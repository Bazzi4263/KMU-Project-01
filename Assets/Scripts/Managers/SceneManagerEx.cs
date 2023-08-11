using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene QurrentScene { get{ return GameObject.FindObjectOfType<BaseScene>(); } }
    public string NextScene;
    public void LoadScene(Define.Scenes type)
    {
        QurrentScene.Clear(type);
        SceneManager.LoadScene(GetSceneName(type));
    }

    public void LoadingScene(Define.Scenes type)
    {
        QurrentScene.Clear(type);
        NextScene = GetSceneName(type);
        SceneManager.LoadScene("TestScene");
    }

    string GetSceneName(Define.Scenes type)
    {
        return System.Enum.GetName(typeof(Define.Scenes), type);
    }
}
