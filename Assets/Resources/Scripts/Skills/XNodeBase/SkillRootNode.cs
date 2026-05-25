using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Skill/RootNode")]
public class SkillRootNode : SkillNode
{
    [Output(dynamicPortList = true)] public List<SkillNode> childs;

    public override void Evaluate(ISkillCaster caster)
    {
        //Debug.Log(childs.Count);
        foreach (var port in DynamicOutputs)
        {
            SkillNode child = port.Connection.node as SkillNode;
            child.Evaluate(caster);
        }
    }
}
