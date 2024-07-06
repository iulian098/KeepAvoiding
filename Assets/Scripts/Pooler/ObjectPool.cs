using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject _object;
        public int size;
    }

    public static ObjectPool instance;

    public Transform parent;

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake(){
        instance = this;
        

        SetTags();
    }
    
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool p in pools){
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < p.size; i++){

                GameObject go = Instantiate(p._object);
                if(parent)
                    go.transform.SetParent(parent);
                    
                go.SetActive(false);
                objectPool.Enqueue(go);

            }

            poolDictionary.Add(p.tag, objectPool);
        }
    }

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation){

        if(!poolDictionary.ContainsKey(tag))
            return null;

        GameObject go = poolDictionary[tag].Dequeue();

        go.SetActive(true);
        go.transform.position = position;
        go.transform.rotation = rotation;
        
        IObjectPool[] pooledObj = go.GetComponents<IObjectPool>();
        if(pooledObj != null){
            foreach(IObjectPool op in pooledObj){
                op.OnObjectSpawned();
            }
        }
        poolDictionary[tag].Enqueue(go);

        return go;
    }

    void SetTags(){
        foreach(Pool p in pools){
            p.tag = p._object.name;
        }
    }
}
