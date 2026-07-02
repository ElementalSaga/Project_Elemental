using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using XNode;

[CreateNodeMenu("Skill/Repeat")]
public class Repeat : SkillNode
{
    [Output(dynamicPortList = true)] public List<SkillNode> childs; //반복할 자식 노드들
    [Output(dynamicPortList = true)] public List<SkillNode> exitRepeat; //반복이 끝난 후 한 번 실행할 자식 노드들.

    [Header("반복 횟수")]
    public int repeatTime;

    [Header("반복 간격")]
    public float interval;

    private int currentCount = 0;

    public bool backToRoot = false;

    public override void Evaluate(ISkillCaster caster)
    {
        if (repeatTime <= 0) //반복 하지 않으면 바로 다음 노드 실행.
        {
            ExecutePorts(caster, "exitRepeat ");
        }

        if (backToRoot) //앞의 노드 흐름이 완료된 후 다시 반복하려면.
        {
            currentCount++;
            if (currentCount < repeatTime)
            {
                var rootNode = graph.nodes.OfType<SkillRootNode>().FirstOrDefault();
                rootNode?.Evaluate(caster);
            }
            else
            {
                currentCount = 0;
                ExecutePorts(caster, "exitRepeat "); //반복 이후 노드 실행.
            }
        }
        else //반복 노드 뒤에 위치한 노드 흐름이 특정 인터벌 간격으로 반복하게 하려면.
        {
            for (int i = 0; i < repeatTime; i++) //원하는 횟수를 인터벌 간격만큼 반복.
            {
                float delay = i * interval;
                DOVirtual.DelayedCall(delay, () => { ExecutePorts(caster, "childs "); });
            }

            float finalInterval = (repeatTime - 1) * interval;
            if (finalInterval > 0) DOVirtual.DelayedCall(finalInterval, () => ExecutePorts(caster, "exitRepeat "));
            else ExecutePorts(caster, "exitRepeat ");
        }
    }

    private void ExecutePorts(ISkillCaster caster, string portName)
    {
        foreach (var port in DynamicOutputs)
        {
            if (port.fieldName.StartsWith(portName) && port.IsConnected)
            {
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    var child = port.Connection.node as SkillNode;
                    child?.Evaluate(caster);
                }
            }
        }
    }
}
