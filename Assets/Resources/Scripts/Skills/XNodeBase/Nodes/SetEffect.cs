using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Skill/SetEffect")]
public class SetEffect : SkillNode
{
    [Input] public Vector2 size;
    [Input] public Vector2 pos;

    public override void Evaluate(ISkillCaster caster)
    {
        size = GetInputValue<Vector2>("size", this.size);
        pos = GetInputValue<Vector2>("pos", this.pos);

        GameObject effect = LocalGameManager.instance.objectPoolManager.poolDic["Effect"].GetGo("Effect");

        effect.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
        Effect effectCom = effect.GetComponent<Effect>();

        effect.transform.localScale = size;
        effect.transform.localPosition = pos;
        effect.transform.localRotation = Quaternion.Euler(0, 0, 0);

        effectCom.Initialize(.2f);
    }
}
