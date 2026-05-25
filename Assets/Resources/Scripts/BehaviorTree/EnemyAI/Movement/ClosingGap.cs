using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateNodeMenu("BT/Action/ClosingGap")]
public class ClosingGap : BTNode
{
    private bool isSuccess = false;

    public override NodeState Evaluate(AIController controller)
    {
        if (blackboard is null) return NodeState.Failure;

        if (blackboard.Get<float>("Distance") > controller.ParentObj.GetComponent<EnemyCharacter>().MeleeRange)
        {
            controller.CallMoveEvent(Vector3.right * blackboard.Get<int>("Direction"));
            Debug.Log("거리 좁히는 중.");
            return NodeState.Running;
        }
        else
        {
            Debug.Log("이미 거리가 좁혀짐.");
            return NodeState.Success;
        }
    }
}
