using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class UI
{ 
    public GameObject panelInfoPrefab;
    
    public void Init()
    {
        
    }
    public void ShowInfoPanel()
    {
        Object.Instantiate(panelInfoPrefab, new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("Main Canvas").transform);
    }
}
