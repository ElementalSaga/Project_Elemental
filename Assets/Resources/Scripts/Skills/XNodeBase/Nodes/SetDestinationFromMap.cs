using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using XNode;

[CreateNodeMenu("Skill/SetDestinationFromMap")]
public class SetDestinationFromMap : SkillNode
{

    [Header("맵 비율 (0:왼 ~ 1:오)")]
    public float rateX;
    public float rateY;

    [Output] public Vector2 output;

    private Vector2 value;
    private CompositeCollider2D mapCol;
    private GameObject map;

    public override void Evaluate(ISkillCaster caster)
    {
        map = LocalGameManager.instance.combatManager.currentMap;
        mapCol = map.transform.GetChild(2).GetComponent<CompositeCollider2D>();

        float leftEdge = mapCol.bounds.min.x;
        float rightEdge = mapCol.bounds.max.x;

        float downEdge = mapCol.bounds.min.y;
        float upEdge = mapCol.bounds.max.y;

        float targetX = Mathf.Lerp(leftEdge, rightEdge, rateX);
        float targetY = Mathf.Lerp(downEdge, upEdge, rateY);

        value = new Vector2(targetX, targetY);
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output") return value;
        return null;
    }
}
