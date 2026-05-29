using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.IO.LowLevel.Unsafe;

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

    public override void Evaluate(ISkillCaster caster)
    {
        destination = GetInputValue<Vector2>("destination", this.destination);
        Rigidbody2D rigid = caster.GetCom<Rigidbody2D>();
        BoxCollider2D col = caster.GetCom<BoxCollider2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;

        Vector2 virtualPos = rigid.position;
        Sequence moveSequence = DOTween.Sequence();

        moveSequence.Join(DOTween.To(() => virtualPos.x, x => virtualPos.x = x, destination.x, durationX).SetEase(moveTypeX));
        moveSequence.Join(DOTween.To(() => virtualPos.y, y => virtualPos.y = y, destination.y, durationY).SetEase(moveTypeY));

        moveSequence.OnUpdate(() =>
        {
            if (Physics2D.OverlapBox(virtualPos + col.offset, col.size, 0f, 1 << 3) != null)
            {
                moveSequence.Kill();
                ExecuteNode(caster);
                return;
            }
            rigid.MovePosition(virtualPos);
        });

        moveSequence.SetUpdate(UpdateType.Fixed);

        moveSequence.OnComplete(() =>
        {
            ExecuteNode(caster);
        });
    }

    private void ExecuteNode(ISkillCaster caster)
    {
        try
        {
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
        finally
        {

        }
    }
}
