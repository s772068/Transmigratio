using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void OpenScene(int index)
    {
        StartCoroutine(DebugLoad(index));
    }

    private IEnumerator DebugLoad(int index)
    {
        var op = SceneManager.LoadSceneAsync(index);
        op.allowSceneActivation = true;
        while (!op.isDone)
        {
            print("progress: " + op.progress);
            yield return null;
        }
    }
}
