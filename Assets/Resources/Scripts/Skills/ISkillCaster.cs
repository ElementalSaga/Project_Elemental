using UnityEngine;
using System.Collections.Generic;

public interface ISkillCaster
{
    int TotalDmg { get; set; }
    bool Attacking { get; set; }
    bool CancleAllSkill { get; }
    void PlayAnimation(string animName);
    //int CurrentGage { get; set; }
    //string GetTag();

    //void SetScale(int dir);
    Vector2 GetPosition();
    Vector2 GetDirection();

    Quaternion GetRotation();

    int GetAttackPower();
    float GetGiveDmgRate();

    IDamageable GetDamageable();
    GameObject GetGameObject();
    Transform GetHitBoxPos();
    Transform GetCatchPos(); //잡기할 때 Enemy의 위치.
    Transform GetAttackPos(); //공격하거나 투사체를 소환할 때의 위치.

    T GetCom<T>();
}
