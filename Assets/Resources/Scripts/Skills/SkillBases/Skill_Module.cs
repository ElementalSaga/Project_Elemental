using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObject/Skill/Skill Module")]
public class Skill_Module : ScriptableObject
{
    public IBlackBoard blackBoard; //스킬 데이터를 담을 블랙보드.
    public enum ActiveType { OnDown, OnHold };
    [SerializeField] private string skillName; //최종 스킬 이름
    [SerializeField] private string skillDetail; //최종 스킬의 상세 설명
    public float coolDown; //최종 스킬의 쿨타임
    public int consumption;
    [SerializeField] private float duration; //사용한 스킬의 공격 상태 지속 시간.

    private float remainingCoolDown; //남은 쿨타임
    [SerializeField] private bool basicAttack; //기본 공격 모듈이지 체크.
    [SerializeField] private bool attackable; //공격 판정이 존재하는 스킬 체크
    [SerializeField] private bool cancleDelay; //기본 공격의 후딜레이를 캔슬하고 작동하는 스킬 체크
    [SerializeField] private bool havePassive; //기본 지속 효과를 가지고 있는지 여부 체크
    [SerializeField] private bool canUseAirial; //공중에서 사용이 가능한지의 여부 체크


    [SerializeField] private List<SkillBase> activeSkills = new List<SkillBase>(); //이 스킬을 실행할 때 같이 실행되는 액티브 스킬들.
    [SerializeField] private List<SkillBase> passiveSkills = new List<SkillBase>(); //이 스킬을 착용하고 있을 때 발동하는 패시브 스킬들.
    [SerializeField] private List<SkillBase> releaseSkills = new List<SkillBase>(); //키 입력을 종료할 때 실행되는 액티브 스킬들. 주로 차징형에 사용.
    public ActiveType activeType;

    //외부에서 접근할 수 있게 하는 프로퍼티
    #region Property
    public bool OnCoolDown => remainingCoolDown > 0;
    public float RemainingCoolDown => remainingCoolDown;
    public bool Attackable => attackable;
    public bool CancleDelay => cancleDelay;
    public bool HavePassive => havePassive;
    public bool CanUseAirial => canUseAirial;
    public bool BasicAttack => basicAttack;
    public float Duration => duration;

    public List<SkillBase> ReleaseSkills => releaseSkills;
    #endregion EndProperty

    void Awake()
    {
        blackBoard = new BlackBoard();
    }

    public void InitSkill()
    {
        //data.SkillName = skillName;
        //data.SkillDetail = skillDetail;

        //Debug.Log("스킬 초기화 완료");
        blackBoard.Set("Condition", true);

        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (activeSkills[i] != null)
            {
                activeSkills[i] = Instantiate(activeSkills[i]);
                activeSkills[i].Initialize(this);
            }
        }

        for (int i = 0; i < releaseSkills.Count; i++)
        {
            if (releaseSkills[i] != null)
            {
                releaseSkills[i] = Instantiate(releaseSkills[i]);
                releaseSkills[i].Initialize(this);
            }
        }

        for (int i = 0; i < passiveSkills.Count; i++)
        {
            if (passiveSkills[i] != null)
            {
                passiveSkills[i] = Instantiate(passiveSkills[i]);
                passiveSkills[i].Initialize(this);
            }
        }
    }

    //스킬 사용을 시도하는 메서드 
    //플레이어가 키보드 버튼을 누를 시 발동.
    //caster.Attacking은 플레이어가 공격 중인지 체크하는 boolean.
    //공격 판정이 아닌 스킬도 존재하기에 (버프 스킬), attacking을 조정하는 코드는 스킬 개인 모듈에 넣어야 함.
    public bool UseOnDownSkill(ISkillCaster caster)
    {
        Unit unit = caster.GetCom<Unit>();
        if (unit.curGage - consumption < 0) return false;

        if (OnCoolDown || !blackBoard.Get<bool>("Condition"))
        {
            Debug.Log($"쿨타임 상황 : {OnCoolDown}, 조건 상황 : {blackBoard.Get<bool>("Condition")}.");
            return false;
        }

        unit.curGage -= consumption;
        // 쿨다운이 아니라면 모든 스킬을 실행
        foreach (var skill in new List<SkillBase>(activeSkills))
        {
            if (skill != null) skill.UseSkill(caster);
        }

        // 쿨다운 시작
        remainingCoolDown = coolDown;
        return true;
    }

    public bool UseReleaseSkill(ISkillCaster caster)
    {
        Unit unit = caster.GetCom<Unit>();
        foreach (var skill in new List<SkillBase>(releaseSkills))
        {
            if (skill != null) skill.UseSkill(caster);
        }

        return true;
    }

    // 매 프레임 쿨다운을 업데이트하는 메서드
    public void UpdateCoolDown(float deltaTime)
    {
        if (!OnCoolDown) return;
        foreach (var skill in new List<SkillBase>(passiveSkills))
        {
            IPassiveSkills passiveSkill = skill as IPassiveSkills;
            if (passiveSkill != null) passiveSkill.SkillOff();
        }
        remainingCoolDown -= deltaTime;
    }

    public void ProccessPassive(ISkillCaster caster) //패시브 스킬 사용.
    {
        //Debug.Log("패시브 스킬 사용");
        foreach (var skill in new List<SkillBase>(passiveSkills))
        {
            skill.UseSkill(caster); //패시브 스킬들은 계속 실행되어야 함.
        }
    }

    public void ChangeSkillModule(SkillBase previousSkill, SkillBase currentSkill) //현재 스킬, 바꿀 스킬
    {
        int index = activeSkills.IndexOf(previousSkill);

        if (index != -1)
        {
            //Debug.Log("0이 나와야함 = " + index);
            activeSkills[index] = currentSkill;
            currentSkill.Initialize(this);
        }
        else
        {
            // 교체하려는 스킬을 찾지 못하면 오류가 발생하므로, 경고를 출력하고 실행을 중단합니다.
            Debug.LogWarning($"Warning: Skill '{previousSkill?.name}' not found in module '{this.name}' and could not be replaced by '{currentSkill?.name}'.");
        }
    }

    public T FindInternalSkill<T>() where T : SkillBase
    {
        return activeSkills.OfType<T>().FirstOrDefault() ?? passiveSkills.OfType<T>().FirstOrDefault();
    }

    public void ResetCoolDown()
    {
        remainingCoolDown = 0;
    }
}
