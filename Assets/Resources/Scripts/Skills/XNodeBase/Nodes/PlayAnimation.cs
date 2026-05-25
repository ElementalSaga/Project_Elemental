using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Skill/PlayAnimation")]
public class PlayAnimation : SkillNode
{
    public string animName;

    public override void Evaluate(ISkillCaster caster)
    {
        caster.PlayAnimation(animName);
    }
}
