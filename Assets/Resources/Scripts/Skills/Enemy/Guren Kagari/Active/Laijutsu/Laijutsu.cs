using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laijutsu", menuName = "ScriptableObject/Skills/Active/Guren_Kagari/Laijutsu")]
public class Laijutsu : SkillBase
{
    [SerializeField] private float delayTime;
    [SerializeField] private SkillBase actionSkill;
    [SerializeField] private Vector2 dangerAreaSize;
    [SerializeField] private Vector2 dangerAreaPos;

    public override bool UseSkill(ISkillCaster caster)
    {
        //Debug.Log("발도 준비");
        GameObject dangerArea = LocalGameManager.instance.objectPoolManager.poolDic["DangerArea"].GetGo("DangerAreaX");

        dangerArea.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
        dangerArea.transform.localPosition = dangerAreaPos;
        dangerArea.transform.localScale = new Vector2(dangerAreaSize.x * caster.GetDirection().x, dangerAreaSize.y);

        var dangerAreaCom = dangerArea.GetComponent<DangerArea>();

        // Activate 호출 시 콜백을 인자로 넘겨주어 이벤트 누적 문제를 방지합니다.
        dangerAreaCom.Activate(delayTime, () => actionSkill?.UseSkill(caster));

        return true;
    }
}
