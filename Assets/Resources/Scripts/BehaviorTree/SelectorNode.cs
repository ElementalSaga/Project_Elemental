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
        foreach (var port in DynamicOutputs) //자식 아웃풋 포트에 연결되어 있는 모든 노드들을 검사.
        {
            BTNode child = port.Connection.node as BTNode; //해당 노드들을 BTNode로 타입변환 후 가져옴.
            switch (child.Evaluate(controller)) //기존 행동 트리 로직.
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
