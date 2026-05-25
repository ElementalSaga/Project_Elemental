using UnityEngine;
using XNode;

[CreateNodeMenu("Skill/Variable/Vector2")]
public class Vector2Node : SkillNode
{
    public Vector2 value;
    [Output] public Vector2 output;

    public override void Evaluate(ISkillCaster caster)
    {

    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output") return value;
        return null;
    }
}
