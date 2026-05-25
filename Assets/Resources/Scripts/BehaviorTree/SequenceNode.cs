using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;

[CreateNodeMenu("BT/SequenceNode")]
public class SequenceNode : BTNode
{
    [Output(dynamicPortList = true)] public List<BTNode> childs;

    private int currentChildIndex = 0;
    private int lastFrameVisited = -1;

    public override NodeState Evaluate(AIController controller)
    {
        //Debug.Log("호놀룰루");
        // 연속된 프레임 방문이 아니면 인덱스 초기화
        if (Time.frameCount != lastFrameVisited + 1)
        {
            currentChildIndex = 0;
        }
        lastFrameVisited = Time.frameCount;

        var ports = DynamicOutputs.ToList();

        for (int i = currentChildIndex; i < ports.Count; i++)
        {
            BTNode child = ports[i].Connection.node as BTNode;
            NodeState state = child.Evaluate(controller);

            switch (state)
            {
                case NodeState.Success:
                    continue;

                case NodeState.Failure:
                    currentChildIndex = 0;
                    return NodeState.Failure;

                case NodeState.Running:
                    currentChildIndex = i;
                    return NodeState.Running;
            }
        }

        currentChildIndex = 0;
        return NodeState.Success;
    }
}
