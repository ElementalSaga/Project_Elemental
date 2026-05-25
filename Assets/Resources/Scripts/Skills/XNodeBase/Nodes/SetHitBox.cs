using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

[CreateNodeMenu("Skill/SetHitBox")]
public class SetHitBox : SkillNode
{
    [Input] public Vector2 size;
    [Input] public Vector2 pos;

    [Input] public float weight;
    [Input] public Quaternion angle;

    [Header("히트박스가 캐스터의 움직임과 함께 하는지 여부.")]
    public bool chaseCaster;

    public override void Evaluate(ISkillCaster caster)
    {
        size = GetInputValue<Vector2>("size", this.size);
        pos = GetInputValue<Vector2>("pos", this.pos);
        weight = GetInputValue<float>("weight", this.weight);
        angle = GetInputValue<Quaternion>("angle", this.angle);

        Debug.Log(angle);

        int totalDmg = (caster.GetAttackPower() + (int)(caster.GetAttackPower() * weight)) + (int)((caster.GetAttackPower() + (int)(caster.GetAttackPower() * weight)) * (caster.GetGiveDmgRate() / 100f));

        GameObject hitBox = LocalGameManager.instance.objectPoolManager.poolDic["HitBox"].GetGo("HitBox");

        if (chaseCaster) hitBox.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
        HitBox hitBoxCom = hitBox.GetComponent<HitBox>();
        hitBox.tag = caster.GetGameObject().tag;

        hitBox.transform.localScale = size;
        hitBox.transform.localPosition = pos;
        hitBox.transform.localRotation = angle;

        hitBoxCom.Initialize(totalDmg, 0, caster, null, .15f);
    }
}
