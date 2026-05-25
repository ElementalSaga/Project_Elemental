using System.Data;
using UnityEngine;
using XNode;

public abstract class SkillNode : Node
{
    [Input(dynamicPortList = true)] public BTNode parent;

    public abstract void Evaluate(ISkillCaster caster);
}
