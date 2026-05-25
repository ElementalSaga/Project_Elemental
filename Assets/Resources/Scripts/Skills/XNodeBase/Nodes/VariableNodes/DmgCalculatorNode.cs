using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Skill/Variable/DmgCalculator")]
public class DmgCalculatorNode : SkillNode
{
    public float weight;
    [Output] public float output;

    public override void Evaluate(ISkillCaster caster)
    {

    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output") return weight;
        return null;
    }
}
