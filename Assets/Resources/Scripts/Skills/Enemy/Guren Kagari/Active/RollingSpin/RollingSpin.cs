using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "RollingSpin", menuName = "ScriptableObject/Skills/Active/Guren_Kagari/RollingSpin")]
public class RollingSpin : SkillBase
{
    private GameObject targetUnit; //임시 타겟.
    private Unit targetPlayer;

    [SerializeField] private Vector2 dangerAreaPos;
    [SerializeField] private Vector2 dangerAreaSize;

    [SerializeField] private float delayTime;

    public override bool UseSkill(ISkillCaster caster)
    {
        if (targetPlayer == null) targetPlayer = LocalGameManager.instance.unitManager.PlayerUnit;

        //경고 범위 오브제 생성.
        GameObject dangerArea = LocalGameManager.instance.objectPoolManager.poolDic["DangerArea"].GetGo("DangerAreaX");

        //위치 조정.
        dangerArea.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
        dangerArea.transform.localPosition = dangerAreaPos;

        //각도 조절.
        Vector2 dir = targetPlayer.transform.position - caster.GetCom<Transform>().position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        dangerArea.transform.parent.rotation = Quaternion.Euler(0, 0, angle);
        dangerArea.transform.localScale = new Vector2(dir.magnitude * caster.GetDirection().x, dangerAreaSize.y); //길이 조정.

        var dangerAreaCom = dangerArea.GetComponent<DangerArea>();

        var targetPos = targetPlayer.transform.position;

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

            hitBox.transform.SetParent(null);
            effect.transform.SetParent(null);

            dangerArea.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            LocalGameManager.instance.coroutineRunner.StartCoroutine(MoveToTarget(caster, targetPos));
        });

        return true;
    }

    private IEnumerator MoveToTarget(ISkillCaster caster, Vector2 targetPos)
    {
        caster.GetCom<Rigidbody2D>().DOMove(targetPos, .1f).Elapsed();
        caster.GetCom<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        yield return null;
    }
}
