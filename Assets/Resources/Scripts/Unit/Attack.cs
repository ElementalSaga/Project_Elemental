using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour, IAttackable, ISkillCaster, IDataInitializeable
{
    //public event Action<int> OnHitEvent;
    public event Action UpdateSkillGage;

    [SerializeField] private Skill_Module[] basicAttacks = new Skill_Module[2]; //유닛이 땅/공중에 있을 때 교체할 기본 공격 모듈.
    [SerializeField] private GameObject parentObj;
    [SerializeField] private BoxCollider2D hitBox;
    [SerializeField] private Rigidbody2D rigid;
    private PlayerController controller;
    private Unit unit;
    private Animator anim;

    [Header("특정 키를 눌러 사용하는 액티브 스킬.")] //기본 공격도 포함.
    [SerializeField] private List<Skill_Module> activeSkills = new List<Skill_Module>(); //액티브 기본 스킬
    //[SerializeField] private List<Skill_Module> modifiedActiveSkills = new List<Skill_Module>(); //특정 캐릭터의 커맨드로 발동하는 액티브 스킬.

    [Header("특정 조건을 만족하면 자동으로 사용되는 패시브 스킬")]
    [SerializeField] private List<Skill_Module> passiveSkills = new List<Skill_Module>();

    [Header("유물 효과들")]
    [SerializeField] private List<Skill_Module> relicPowers = new List<Skill_Module>();

    [Header("피격되어 데미지를 받을 때 실행할 효과들")]
    //맞을 때 실행할 효과들은 공격자의 정보가 필요한 경우도 존재하므로, 새로운 클래스를 만들어 사용.
    [SerializeField] private List<DamagedEventBase> hitEvents = new List<DamagedEventBase>();

    [Header("공격 성공 시 적용할 이벤트 리스트들")]
    [SerializeField] private List<OnHitEventBase> onHitEvents = new List<OnHitEventBase>();

    [SerializeField] private int combo;

    public int TotalDmg { set; get; }
    public bool Attacking { set; get; }
    public bool CancleAllSkill { get; }
    public int Combo { get => combo; set => combo = value; }

    public List<Skill_Module> ActiveSkills { get => activeSkills; set => activeSkills = value; }
    public List<Skill_Module> PassiveSkills { get => passiveSkills; set => passiveSkills = value; }
    public List<Skill_Module> RelicPowers { get => relicPowers; set => relicPowers = value; }
    public List<DamagedEventBase> HitEvents { get => hitEvents; set => hitEvents = value; }
    public List<OnHitEventBase> OnHitEvents { get => onHitEvents; set => onHitEvents = value; }

    private Slider[] skillCoolSlider;

    void LateUpdate()
    {
        UpdateCoolDown();
        UsePassiveSkills();

        //ChangeAirialBasicAttack();
        //Debug.Log($"{parentObj.name}의 x가속도 : {rigid.velocity.x}");
    }

    public void DataInitialize()
    {
        unit = parentObj.GetComponent<Unit>();
        anim = parentObj.GetComponent<Animator>();
        rigid = parentObj.GetComponent<Rigidbody2D>();

        skillCoolSlider = LocalGameManager.instance.playerUIManager.SkillCools;

        SkillInit(hitEvents);
        SkillInit(activeSkills);
        SkillInit(passiveSkills);
        SkillInit(onHitEvents);
    }

    private void SkillInit<T>(List<T> skills) where T : class
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i] == null) continue;

            var original = skills[i] as UnityEngine.Object;
            if (original == null) continue;

            var newInstance = Instantiate(original);

            if (newInstance is DamagedEventBase damagedEvent)
            {
                damagedEvent.Initialize(unit);
            }
            else if (newInstance is Skill_Module skillModule)
            {
                skillModule.InitSkill();
            }
            else if (newInstance is OnHitEventBase onHitEventBase)
            {
                onHitEventBase.Initialize(unit);
            }

            skills[i] = newInstance as T;
        }
    }

    public T FindSkill<T>(T skill) where T : SkillBase
    {
        var allSkillModule = activeSkills.Concat(passiveSkills);

        foreach (var module in allSkillModule)
        {
            if (module == null) continue;

            var foundSkill = module.FindInternalSkill<T>();
            if (foundSkill != null)
            {
                return foundSkill;
            }
        }
        return null;
    }

    public OnHitEventBase FindOnHitEvent(Type type)
    {
        // 전달된 타입이 OnHitEventBase를 상속하는지 확인합니다.
        if (!typeof(OnHitEventBase).IsAssignableFrom(type))
        {
            Debug.LogWarning($"요청된 타입 '{type.Name}'은 OnHitEventBase를 상속하지 않습니다.");
            return null;
        }

        foreach (var hitevent in onHitEvents)
        {
            // 정확한 타입이 일치하는지 확인합니다.
            if (hitevent.GetType() == type)
            {
                return hitevent;
            }
        }
        return null;
    }

    private void ChangeAirialBasicAttack()
    {
        if (activeSkills.Count == 0 || basicAttacks.Length == 0) return;
        if (unit.isAirial)
        {
            ActiveSkills[0] = basicAttacks[1];
        }
        else
        {
            ActiveSkills[0] = basicAttacks[0];
        }
    }

    private void UsePassiveSkills()
    {
        foreach (var skill in new List<Skill_Module>(activeSkills))
        {
            if (skill.HavePassive)
            {
                skill.ProccessPassive(this);
            }
        }

        foreach (var skill in new List<Skill_Module>(passiveSkills))
        {
            skill.UseOnDownSkill(this);
        }
    }

    public bool PerformSkill(Skill_Module skill)
    {
        //if (attNum == -1) return false;
        bool result;
        rigid.velocity = new Vector2(0, rigid.velocity.y);
        if (unit.isAirial) //공중에서 스킬을 쓸 때.
        {
            if (!skill.CanUseAirial) return false;
            else result = skill.UseOnDownSkill(this);
        }

        else //땅 위에서 스킬을 쓸 때.
        {
            int comboAnim = Combo;
            result = skill.UseOnDownSkill(this);
        }

        //엘 게이지 업데이트.
        UpdateSkillGage?.Invoke();
        return result;
    }

    //키보드에서 손을 뗄 때 실행되는 스킬.
    public bool PerformOnReleaseSkill(Skill_Module skill)
    {
        bool result;
        rigid.velocity = new Vector2(0, rigid.velocity.y);

        result = skill.UseReleaseSkill(this);

        return result;
    }

    public void PlayAnimation(string animName)
    {
        anim.CrossFade(animName, 0f);
    }

    private void UpdateCoolDown()
    {
        for (int i = 0; i < activeSkills.Count; i++)
        {
            activeSkills[i].UpdateCoolDown(Time.deltaTime);
            if (unit.Type != UnitType.PlayableCharacter) continue;

            if (skillCoolSlider[i] != null)
            {
                skillCoolSlider[i].value = activeSkills[i].RemainingCoolDown / activeSkills[i].coolDown;
                //Debug.Log(skillCoolSlider[i].value);
            }
        }
    }

    public Vector2 GetPosition()
    {
        return parentObj.transform.position;
    }

    public Vector2 GetDirection()
    {
        return new Vector2(parentObj.transform.localScale.x, 0);
    }

    public Quaternion GetRotation()
    {
        return parentObj.transform.rotation;
    }

    public Transform GetHitBoxPos()
    {
        return this.gameObject.transform.GetChild(0).transform;
    }

    public int GetAttackPower()
    {
        return unit.Att;
    }

    public float GetGiveDmgRate()
    {
        return unit.GivingDmgRate;
    }

    public IDamageable GetDamageable()
    {
        return parentObj.GetComponent<IDamageable>();
    }

    public GameObject GetGameObject()
    {
        return parentObj;
    }

    public Transform GetCatchPos()
    {
        return this.gameObject.transform.GetChild(1).transform;
    }

    public Transform GetAttackPos()
    {
        return null;
    }

    public T GetCom<T>() => parentObj.GetComponentInChildren<T>();
}