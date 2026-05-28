using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using XNode;

[CreateNodeMenu("Skill/SetDestination")]
public class SetDestination : SkillNode
{
    public enum TargetingType { Variable, Player }

    [Header("목적지 타입 (개발자 지정 / 플레이어)")]
    public TargetingType targetTypeX;
    public TargetingType targetTypeY;
    public float rateX;
    public float rateY;
    public Vector2 target;

    private Vector2 value;
    [Output] public Vector2 output;

    public override void Evaluate(ISkillCaster caster)
    {
        float x = 0f;
        float y = 0f;

        switch (targetTypeX)
        {
            case TargetingType.Player:
                x = Mathf.Lerp(caster.GetPosition().x, LocalGameManager.instance.unitManager.PlayerUnit.gameObject.transform.position.x, rateX);
                break;

            case TargetingType.Variable:
                x = caster.GetCom<Transform>().position.x + (target.x * caster.GetDirection().x);
                break;
        }

        switch (targetTypeY)
        {
            case TargetingType.Player:
                y = Mathf.Lerp(caster.GetPosition().y, LocalGameManager.instance.unitManager.PlayerUnit.gameObject.transform.position.y, rateY);
                break;

            case TargetingType.Variable:
                y = caster.GetCom<Transform>().position.y + target.y;
                break;
        }

        value = new Vector2(x, y);
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output") return value;
        return null;
    }
}
