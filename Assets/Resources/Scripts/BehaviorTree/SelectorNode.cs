using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("BT/SelectorNode")]
public class SelectorNode : BTNode
{
    [Output(dynamicPortList = true)] public List<BTNode> childs;

    public override NodeState Evaluate(AIController controller)
    {
        //Debug.Log("할렐루야");
        foreach (var port in DynamicOutputs)
        {
            BTNode child = port.Connection.node as BTNode;
            switch (child.Evaluate(controller))
            {
                case NodeState.Success:
                    return NodeState.Success;

                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Failure:
                    continue;
            }
        }
        return NodeState.Failure;
    }
}
