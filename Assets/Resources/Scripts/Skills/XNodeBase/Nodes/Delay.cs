using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateNodeMenu("Skill/Delay")]
public class Delay : SkillNode
{
    [Output(dynamicPortList = true)] public List<SkillNode> childs;
    public float delayTime;

    public override void Evaluate(ISkillCaster caster)
    {
        DOVirtual.DelayedCall(delayTime, () =>
        {
            foreach (var port in DynamicOutputs)
            {
                if (port.fieldName.StartsWith("childs "))
                {
                    if (port.IsConnected)
                    {
                        var child = port.Connection.node as SkillNode;
                        child.Evaluate(caster);
                    }
                }
            }
        });
    }
}
