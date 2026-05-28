using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

[CreateNodeMenu("Skill/SummonObject")]
public class SummonObj : SkillNode
{
    //[Input] public Vector2 summonPos;
    public string objName;

    [Input] public float weight;

    public override void Evaluate(ISkillCaster caster)
    {
        var dmg = (int)(weight * caster.GetAttackPower());
        var obj = LocalGameManager.instance.objectPoolManager.poolDic["BossObj"].GetGo(objName);
        obj.transform.position = caster.GetHitBoxPos().position;

        obj.GetComponent<SkillObjBase>().ObjInit(caster.GetDirection(), dmg, 0, "Enemy", caster);
    }
}
