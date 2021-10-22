using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelsGenerator : MonoBehaviour {
    public Transform container;
    public int startIndex;
    public int endIndex;
    public int minLevels;
    public int maxLevels;
    public float StartingY;
    public float levelsOffset;
    public List<GameObject> levels;
    public GameObject startingLevel;
    public GameObject endingLevel;

    LevelsManager levelsManager;

    #if UNITY_EDITOR
    public void GenerateMap () {
        levelsManager = GameObject.FindObjectOfType(typeof(LevelsManager)) as LevelsManager;
        for (int i = startIndex; i < endIndex; i++) {

            GameObject levelContainer = new GameObject (i.ToString ());
            levelContainer.transform.localPosition = Vector3.zero;
            levelContainer.transform.SetParent(container);

            int levelsNr = Random.Range (minLevels, maxLevels);

            for (int j = 0; j < levelsNr; j++) {
                GameObject go;

                if (j < 2) {
                    go = PrefabUtility.InstantiatePrefab(startingLevel) as GameObject; //Instantiate (startingLevel);
                    
                } else if (j >= 2 && j < levelsNr - 2) {
                    go = PrefabUtility.InstantiatePrefab(levels[Random.Range(0, levels.Count)]) as GameObject;
                    
                } else if (j == levelsNr - 2) {
                    go = PrefabUtility.InstantiatePrefab(endingLevel) as GameObject;//Instantiate (startingLevel);
                } else {
                    go = PrefabUtility.InstantiatePrefab(startingLevel) as GameObject;//Instantiate (endingLevel);
                }


                go.transform.SetParent (levelContainer.transform);

                go.transform.position = new Vector3 (0, (j-1) * levelsOffset, 0);
            }

            if(levelsManager)
                levelsManager.Levels.Add(levelContainer);
        }
    }
    #endif
}