using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateNodeMenu("Skill/UnitMove")]
public class UnitMove : SkillNode
{
    public enum DestType { Variable, Dynamic }

    [Output(dynamicPortList = true)] public List<SkillNode> childs;


    [Header("이동 타입(X축 Y축 따로)")]
    public Ease moveTypeX;
    public Ease moveTypeY;

    public float durationX;
    public float durationY;

    [Header("목적지")]
    [Input] public Vector2 destination;

    private Transform casterTransform;

    public override void Evaluate(ISkillCaster caster)
    {
        destination = GetInputValue<Vector2>("destination", this.destination);

        Debug.Log($"목적지 : {destination.y}");

        caster.GetCom<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        casterTransform = caster.GetCom<Transform>();
        Sequence moveSequence = DOTween.Sequence();

        moveSequence.Join(caster.GetCom<Transform>().DOMoveX(destination.x, durationX).SetEase(moveTypeX));
        moveSequence.Join(caster.GetCom<Transform>().DOMoveY(destination.y, durationY).SetEase(moveTypeY));

        moveSequence.OnComplete(() =>
        {
            try
            {
                foreach (var port in DynamicOutputs)
                {
                    var child = port.Connection.node as SkillNode;
                    child.Evaluate(caster);
                }
            }
            finally
            {

            }
        });
    }
}
