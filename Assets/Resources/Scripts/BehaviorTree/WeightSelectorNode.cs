using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("BT/WeightSelectorNode")]
public class WeightSelectorNode : BTNode
{
    [Output(dynamicPortList = true)] public List<float> WeightList;
    private BTNode runningChild = null;
    private int lastFrameVisited = -1;

    public override NodeState Evaluate(AIController controller)
    {
        // 노드에 새로 진입했거나 실행이 끊겼었다면 상태 초기화
        if (Time.frameCount != lastFrameVisited + 1)
        {
            runningChild = null;
        }
        lastFrameVisited = Time.frameCount;

        if (runningChild != null)
        {
            NodeState state = runningChild.Evaluate(controller);
            if (state == NodeState.Running)
            {
                return NodeState.Running;
            }
            runningChild = null;
            return state;
        }

        //NodePort port = GetOutputPort("WeightList");
        //Debug.Log(port.ConnectionCount);
        float total = 0;
        List<int> availableIndices = new List<int>();

        // 1. 현재 실행 가능한 노드들만 골라내고 전체 가중치 합산
        for (int i = 0; i < WeightList.Count; i++)
        {
            NodePort port = GetOutputPort("WeightList " + i);
            if (port == null || !port.IsConnected) continue;

            BTNode child = port.Connection.node as BTNode;
            // 핵심: 자식 노드에게 실행 가능 여부를 물어봅니다.
            if (child != null && child.CanExecute(controller))
            {
                total += WeightList[i];
                availableIndices.Add(i); // 실행 가능한 인덱스만 저장
            }
        }

        // 모든 스킬이 쿨타임이라면 실패 반환
        if (total <= 0 || availableIndices.Count == 0) return NodeState.Failure;

        // 2. 가용한 노드들 사이에서 주사위 굴리기
        float roll = Random.Range(0, total);
        float cumulative = 0;

        foreach (int index in availableIndices)
        {
            cumulative += WeightList[index];
            if (roll <= cumulative)
            {
                NodePort port = GetOutputPort("WeightList " + index);
                BTNode child = port.Connection.node as BTNode;
                NodeState state = child.Evaluate(controller);
                if (state == NodeState.Running)
                {
                    runningChild = child;
                }
                return state;
            }
        }
        return NodeState.Failure;
    }
}
