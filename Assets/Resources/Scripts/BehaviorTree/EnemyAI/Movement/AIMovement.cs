using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("BT/Action/Movement")]
public class AIMovement : BTNode
{
    [Header("블랙보드 키")]
    public string directionKey = "Direction";

    public override NodeState Evaluate(AIController controller)
    {
        if (blackboard is null) return NodeState.Failure;

        if (blackboard.Get<float>("Distance") <= controller.ParentObj.GetComponent<EnemyCharacter>().MeleeRange)
        {
            controller.CallMoveEvent(Vector3.zero);
        }

        else controller.CallMoveEvent(Vector3.right * blackboard.Get<int>(directionKey));
        return NodeState.Success;
    }
}
