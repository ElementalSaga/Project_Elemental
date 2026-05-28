using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour, IDataInitializeable
{
    public Dictionary<string, ObjectPooler> poolDic = new Dictionary<string, ObjectPooler>();

    public void DataInitialize()
    {
        //Debug.Log("초기화");
        //poolDic.Add("Rooms", transform.GetChild(0).GetComponent<ObjectPooler>());
        poolDic.Add("UI", transform.GetChild(0).GetComponent<ObjectPooler>());
        poolDic.Add("Units", transform.GetChild(1).GetComponent<ObjectPooler>());
        poolDic.Add("Effect", transform.GetChild(2).GetComponent<ObjectPooler>());
        poolDic.Add("HitBox", transform.GetChild(3).GetComponent<ObjectPooler>());
        poolDic.Add("DangerArea", transform.GetChild(4).GetComponent<ObjectPooler>());
        poolDic.Add("BossObj", transform.GetChild(5).GetComponent<ObjectPooler>());
        //poolDic.Add("Amend", transform.GetChild(5).GetComponent<ObjectPooler>());
        //poolDic.Add("Door", transform.GetChild(6).GetComponent<ObjectPooler>());
    }
}
