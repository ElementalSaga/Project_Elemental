using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("BT/ControlDirection")]
public class ControlDirection : BTNode
{
    [Header("블랙보드 키")]
    public string directionKey = "Direction";
    private Transform unitTransform;

    public override NodeState Evaluate(AIController controller)
    {
        if (blackboard is null) return NodeState.Failure;
        blackboard.Set<Vector3>("UnitPos", controller.ParentObj.transform.position);

        Vector3 unitPos = blackboard.Get<Vector3>("UnitPos");
        Vector3 playerPos = LocalGameManager.instance.unitManager.PlayerUnit.transform.position;

        blackboard.Set<int>(directionKey, (int)Mathf.Sign((playerPos.x - unitPos.x)));

        controller.ParentObj.transform.localScale = new Vector3(Mathf.Sign(playerPos.x - unitPos.x), 1, 1);

        return NodeState.Success;
    }
}
