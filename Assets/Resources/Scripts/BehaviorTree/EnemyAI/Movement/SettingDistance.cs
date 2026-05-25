using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("BT/SettingDistance")]
public class SettingDistance : BTNode
{
    Vector3 unitPos;
    Vector3 playerPos;

    public override NodeState Evaluate(AIController controller)
    {
        unitPos = controller.ParentObj.transform.position;
        playerPos = LocalGameManager.instance.unitManager.PlayerUnit.transform.position;

        blackboard.Set<Vector3>("UnitPos", unitPos);
        blackboard.Set<float>("Distance", Mathf.Abs((playerPos.x - unitPos.x)));
        //Debug.Log(blackboard.Get<float>("Distance"));
        return NodeState.Failure;
    }
}
