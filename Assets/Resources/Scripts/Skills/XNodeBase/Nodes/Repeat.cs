using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Skill/Repeat")]
public class Repeat : SkillNode
{
    [Output(dynamicPortList = true)] public List<SkillNode> childs;
    [Output(dynamicPortList = true)] public List<SkillNode> exitRepeat;
    public int repeatTime;

    public override void Evaluate(ISkillCaster caster)
    {
        if (repeatTime <= 0) return;
        for (int i = 0; i < repeatTime; i++)
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
        }

        foreach (var port in DynamicOutputs)
        {
            if (port.fieldName.StartsWith("afterRepeatNode "))
            {
                if (port.IsConnected)
                {
                    var child = port.Connection.node as SkillNode;
                    child.Evaluate(caster);
                }
            }
        }
    }
}
