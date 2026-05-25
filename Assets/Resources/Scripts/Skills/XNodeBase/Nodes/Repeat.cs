using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Skill/Repeat")]
public class Repeat : SkillNode
{
    public int repeatTime;

    public override void Evaluate(ISkillCaster caster)
    {
        if (repeatTime <= 0) return;
        for (int i = 0; i < repeatTime; i++)
        {
            foreach (var port in DynamicOutputs)
            {
                var child = port.Connection.node as SkillNode;
                child.Evaluate(caster);
            }
        }
    }
}
