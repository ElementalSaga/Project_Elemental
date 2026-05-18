using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "CrushBlow", menuName = "ScriptableObject/Skills/Active/YeonHaRyeon/CrushBlow")]
public class CrushBlow : SkillBase
{
    private IAttackable attack;
    private HeatSeal heatSeal;
    private int attackCount;

    [SerializeField] private Vector2 hitBoxOffset;
    [SerializeField] private Vector2 hitBoxSize;

    private string[] comboAnim = { "CrushBlow_1", "CrushBlow_2", "CrushBlow_Final" };

    public override bool UseSkill(ISkillCaster caster)
    {
        if (attack is null) attack = caster.GetCom<IAttackable>();
        if (heatSeal is null) heatSeal = attack.FindSkill(heatSeal);

        attackCount = heatSeal.heatSeal == 3 ? 3 : 2;

        LocalGameManager.instance.coroutineRunner.StartCoroutine(Combo(caster));

        return true;
    }

    private IEnumerator Combo(ISkillCaster caster)
    {
        int count = 0;
        while (true)
        {
            if (count >= attackCount) break;

            animName = count switch
            {
                0 => comboAnim[0],
                1 => comboAnim[1],
                2 => comboAnim[2],
                _ => ""
            };

            caster.PlayAnimation(animName);
            caster.GetCom<Rigidbody2D>().DOMoveX(caster.GetGameObject().transform.position.x + (2f * caster.GetDirection().x), .1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GameObject hitBox = LocalGameManager.instance.objectPoolManager.poolDic["HitBox"].GetGo("HitBox");

                hitBox.transform.position = caster.GetHitBoxPos().position;
                hitBox.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));

                HitBox hitBoxCom = hitBox.GetComponent<HitBox>();

                hitBox.tag = caster.GetGameObject().tag;

                hitBox.transform.localScale = hitBoxSize;
                hitBox.transform.localPosition = hitBoxOffset;

                //var hitBoxCol = hitBoxCom.GetComponent<BoxCollider2D>();

                //hitBoxCol.size = hitBoxSize;
                //hitBoxCol.offset = hitBoxOffset;

                hitBoxCom.Initialize(dmgCalculater.Calculate(caster), stunDmg, caster, null, .15f);
                count++;
            });

            yield return new WaitForSeconds(.25f);
        }

        heatSeal.heatSeal = 0;
        yield return null;
    }
}
