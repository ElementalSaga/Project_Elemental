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
                var child = port.Connection.node as SkillNode;
                child.Evaluate(caster);
            }
        });
    }
}
