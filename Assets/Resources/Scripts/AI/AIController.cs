using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AIController : MonoBehaviour, IDataInitializeable, IControllable
{
    public enum UnitState { Idle, Attacking, Moving }

    [Header("AI의 현재 상태")]
    [SerializeField] private UnitState _curState;
    private float lastStateChangeTime;

    public UnitState curState
    {
        get => _curState;
        set
        {
            if (_curState != value)
            {
                _curState = value;
                lastStateChangeTime = Time.time;
            }
        }
    }

    [SerializeField] private GameObject parentObj;
    public GameObject ParentObj { get => parentObj; set => value = parentObj; }
    private Rigidbody2D rigid;
    private Animator anim;

    private IMovable movement;
    public IAttackable attack;

    public event Action<Vector2> moveInput;
    public event Action<int> attackInput;
    public event Action jumpInput;
    public event Action interaction;

    [Header("AI모듈")]
    public BehaviorTreeGraph behaviorTree;
    private BehaviorTreeGraph runTimeTree;
    private BTNode root;

    [SerializeField] private bool runningBT = false;

    void Update()
    {
        //if (runningBT) 
        root.Evaluate(this);
        UpdateUnitState();
    }

    public void CallMoveEvent(Vector2 dir)
    {
        if (curState != UnitState.Moving) curState = UnitState.Moving;
        moveInput?.Invoke(dir);
    }

    public void CallAttackEvent()
    {
        attackInput?.Invoke(0);
    }

    public void DataInitialize()
    {
        // if (parentObj.GetComponent<PlayableCharacter>().controlState == PlayableCharacter.ControlState.Player)
        // {
        //     playerInput.onActionTriggered -= ActionTrigger;
        //     playerInput.onActionTriggered += ActionTrigger;
        // }

        // movement = parentObj.GetComponentInChildren<IMovable>();
        // attack = parentObj.GetComponentInChildren<IAttackable>();
        // anim = parentObj.GetComponent<Animator>();
        // isAirial = false;

        runTimeTree = behaviorTree.Copy() as BehaviorTreeGraph;
        runTimeTree.blackboard = new BlackBoard();
        root = runTimeTree.rootNode;

        rigid = parentObj.GetComponent<Rigidbody2D>();
        anim = parentObj.GetComponentInChildren<Animator>();
        movement = parentObj.GetComponentInChildren<IMovable>();
        attack = parentObj.GetComponentInChildren<IAttackable>();

        runningBT = true;
    }

    private void UpdateUnitState()
    {
        if (curState == UnitState.Attacking)
        {
            // 공격 상태 관리는 이제 PerformAttack 노드의 코루틴이나 외부 로직에서 명시적인 기간(skill.Duration) 동안 수행됩니다.
            /*
            if (Time.time - lastStateChangeTime < 0.2f) return;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                curState = UnitState.Idle;
                Debug.Log("공격 종료.");
                CallMoveEvent(Vector2.zero);
            }
            */
        }

        else
        {
            if (rigid.velocity.x == 0)
            {
                curState = UnitState.Idle;
                anim.CrossFade("Idle", 0f);
            }
            else
            {
                curState = UnitState.Moving;
                anim.CrossFade("Run", 0f);
            }
        }
    }

    public void StopMovement()
    {
        moveInput?.Invoke(Vector2.zero);
    }
}
