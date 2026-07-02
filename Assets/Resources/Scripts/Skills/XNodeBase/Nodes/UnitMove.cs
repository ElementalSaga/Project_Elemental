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
        destination = GetInputValue<Vector2>("destination", this.destination); //목적지 아웃풋을 받아옴.
        Rigidbody2D rigid = caster.GetCom<Rigidbody2D>();
        BoxCollider2D col = caster.GetCom<BoxCollider2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic; //중력을 Kinematic으로 설정.(외부 중력에 영향을 받지 않기 위함.)

        Vector2 virtualPos = rigid.position;
        Sequence moveSequence = DOTween.Sequence(); //닷트윈 시퀀스 생성.

        moveSequence.Join(DOTween.To(() => virtualPos.x, x => virtualPos.x = x, destination.x, durationX).SetEase(moveTypeX)); //X축 이동 따로.
        moveSequence.Join(DOTween.To(() => virtualPos.y, y => virtualPos.y = y, destination.y, durationY).SetEase(moveTypeY)); //Y축 이동 따로.

        moveSequence.OnUpdate(() =>
        {
            if (Physics2D.OverlapBox(virtualPos + col.offset, col.size, 0f, 1 << 3) != null) //이동하는 도중 유닛 콜라이더 크기만큼 오버랩을 생성해. 3레이어(벽)에 닿을 경우 시퀀스 취소 및 다음 노드 강제 실행.
            {
                moveSequence.Kill();
                ExecuteNode(caster);
                return;
            }
            rigid.MovePosition(virtualPos); //이동
        });

        moveSequence.SetUpdate(UpdateType.Fixed); //물리 이동은 FixedUpdate에서 수행하므로 UpdateType을 Fixed로 변경.

        //시퀀스 완료 시 자식 노드 실행.
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
