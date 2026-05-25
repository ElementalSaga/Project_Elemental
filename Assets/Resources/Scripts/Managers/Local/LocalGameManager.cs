using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager instance;
    public PlayerUIManager playerUIManager;
    public DialogueManager dialogueManager;
    public CoroutineRunner coroutineRunner;
    public ObjectPoolManager objectPoolManager;
    //public TimeLineManager timeLineManager;
    public UnitManager unitManager; //현재 조종 중인 현재 플레이어 유닛에 관련된 매니저.

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        foreach (var data in GetComponentsInChildren<IDataInitializeable>())
        {
            data.DataInitialize();
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     dialogueManager.dialogueRunner.RunDialogue();
        // }
    }
}
