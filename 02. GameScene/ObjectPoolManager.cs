using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public struct PoolInfo
    {
        public Transform parent;
        public GameObject originObj;
        public Queue<GameObject> pool;
    }

    public Dictionary<string, PoolInfo> poolList = new Dictionary<string, PoolInfo>();

    public void Init(string keyName, Transform parent, GameObject originObj)
    {
        PoolInfo poolInfo = new PoolInfo();
        poolInfo.parent = parent;
        poolInfo.originObj = originObj;
        poolInfo.pool = new Queue<GameObject>();

        /*
        for (int i = 0; i < parent.childCount; i++)
        {
            poolInfo.pool.Enqueue(parent.GetChild(i).GetComponent<GameObject>());
        }
        */
        poolInfo.originObj.SetActive(false);

        poolList.Add(keyName, poolInfo);
    }

    public GameObject ShowObjectPool(string keyName, Transform target)
    {
        GameObject obj;
        PoolInfo poolInfo = poolList[keyName];

        if (poolInfo.pool.Count == 0)
        {            
            obj = Instantiate(poolInfo.originObj, target.position, target.rotation, poolInfo.parent);
        }
        else
        {
            obj = poolInfo.pool.Dequeue();
            obj.transform.position = target.position;
        }

        obj.SetActive(true);
        StartCoroutine(Hide());

        IEnumerator Hide()
        {
            yield return new WaitForSeconds(1.5f);
            obj.SetActive(false);
            poolInfo.pool.Enqueue(obj);
        }

        return obj;
    }
}
