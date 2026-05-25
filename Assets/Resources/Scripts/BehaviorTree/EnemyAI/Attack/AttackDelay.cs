using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("BT/Action/AttackDelay")]
public class AttackDelay : BTNode
{
    public float minDuration;
    public float maxDuration;
    public bool useBlackboard;
    public string durationKey;

    private float duration;
    private float startTime;
    private int lastFrameVisited = -1;

    public override NodeState Evaluate(AIController controller)
    {
        // 이전 프레임에 방문하지 않았다면 (새로 시작하거나 다시 들어온 경우) 시작 시간 초기화
        if (Time.frameCount != lastFrameVisited + 1)
        {
            if (useBlackboard && !string.IsNullOrEmpty(durationKey) && blackboard.HasKey(durationKey))
            {
                duration = blackboard.Get<float>(durationKey);
            }
            else
            {
                duration = Random.Range(minDuration, maxDuration);
            }

            startTime = Time.time;
            //Debug.Log($"[AttackDelay] 딜레이 시작. Duration: {duration}");
        }
        lastFrameVisited = Time.frameCount;

        float elapsed = Time.time - startTime;
        if (elapsed >= duration)
        {
            //Debug.Log($"[AttackDelay] 딜레이 완료 (Elapsed: {elapsed}) -> Success");
            return NodeState.Success;
        }

        // Debug.Log($"[AttackDelay] 대기 중... (Elapsed: {elapsed}/{duration})");
        return NodeState.Running;
    }
}
