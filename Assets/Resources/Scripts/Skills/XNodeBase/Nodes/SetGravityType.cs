using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Skill/SetBodyType")]
public class SetGravityType : SkillNode
{
    [SerializeField] private RigidbodyType2D _bodyType;

    public override void Evaluate(ISkillCaster caster)
    {
        caster.GetCom<Rigidbody2D>().bodyType = _bodyType;
    }
}
