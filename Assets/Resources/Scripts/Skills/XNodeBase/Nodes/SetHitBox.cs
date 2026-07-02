using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

[CreateNodeMenu("Skill/SetHitBox")]
public class SetHitBox : SkillNode
{
    [Output(dynamicPortList = true)] public List<SkillNode> childs;

    [Header("판정 지속 시간")]
    public float duration;

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

        //데미지 설정.
        int totalDmg = (caster.GetAttackPower() + (int)(caster.GetAttackPower() * weight)) + (int)((caster.GetAttackPower() + (int)(caster.GetAttackPower() * weight)) * (caster.GetGiveDmgRate() / 100f));

        //히트박스 생성 및 위치, 크기, 각도, 태그 설정.
        GameObject hitBox = LocalGameManager.instance.objectPoolManager.poolDic["HitBox"].GetGo("HitBox");

        hitBox.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));

        HitBox hitBoxCom = hitBox.GetComponent<HitBox>();
        hitBox.tag = caster.GetGameObject().tag;

        hitBox.transform.localScale = size;
        hitBox.transform.localPosition = pos;

        hitBox.transform.rotation = angle;

        if (!chaseCaster) //히트박스가 캐스터 움직임을 따라가지 않을 때.
        {
            hitBox.transform.SetParent(null);
        }

        hitBoxCom.Initialize(totalDmg, 0, caster, null, duration);

        //자식 노드가 존재할 경우 실행.
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
}
