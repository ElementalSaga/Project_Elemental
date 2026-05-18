using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laijutsu", menuName = "ScriptableObject/Skills/Active/Guren_Kagari/Laijutsu")]
public class Laijutsu : SkillBase
{
    [SerializeField] private float delayTime;
    [SerializeField] private Vector2 dangerAreaSize;
    [SerializeField] private Vector2 dangerAreaPos;

    public override bool UseSkill(ISkillCaster caster)
    {
        //Debug.Log("발도 준비");
        GameObject dangerArea = LocalGameManager.instance.objectPoolManager.poolDic["DangerArea"].GetGo("DangerAreaX");

        dangerArea.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
        dangerArea.transform.localPosition = dangerAreaPos;

        dangerArea.transform.localScale = new Vector2(dangerAreaSize.x, dangerAreaSize.y);

        var dangerAreaCom = dangerArea.GetComponent<DangerArea>();

        dangerAreaCom.Activate(delayTime, () =>
        {
            GameObject hitBox = LocalGameManager.instance.objectPoolManager.poolDic["HitBox"].GetGo("HitBox");
            GameObject effect = LocalGameManager.instance.objectPoolManager.poolDic["Effect"].GetGo("Effect");

            hitBox.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
            effect.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));

            HitBox hitBoxCom = hitBox.GetComponent<HitBox>();
            Effect effectCom = effect.GetComponent<Effect>();

            hitBox.tag = caster.GetGameObject().tag;

            hitBox.transform.localScale = dangerArea.transform.localScale;
            hitBox.transform.localPosition = dangerAreaPos;
            hitBox.transform.localRotation = Quaternion.Euler(0, 0, 0);

            effect.transform.localScale = dangerArea.transform.localScale;
            effect.transform.localPosition = dangerAreaPos;
            effect.transform.localRotation = Quaternion.Euler(0, 0, 0);

            hitBoxCom.Initialize(dmgCalculater.Calculate(caster), stunDmg, caster, null, .15f);
            effectCom.Initialize(.2f);

            caster.PlayAnimation(animName);
        });

        return true;
    }
}
