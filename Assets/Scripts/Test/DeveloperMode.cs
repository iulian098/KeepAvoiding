using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NiobiumStudios;

[System.Serializable]
public class DeveloperMode
{
    public bool Enabled;
    public List<GameObject> _objects;
    public DailyRewardsInterface dri;

    public void EnableDisable(bool b){
        Enabled = b;
        dri.isDebug = Enabled;
        foreach (GameObject obj in _objects)
        {
            obj.SetActive(Enabled);
        }
        
    }
}
