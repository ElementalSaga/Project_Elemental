using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WindBallObj : SkillObjBase
{
    [SerializeField] private Rigidbody2D rigid;
    private Vector2 currentDir;
    private float moveSpeed = 10f;
    [SerializeField] private float bounceRandomness = 10f; // 반사 시 추가할 무작위 각도 범위
    private float skinWidth = 0.05f; // 벽에서 밀어낼 거리 (끼임 방지용)

    public override void ObjInit(Vector2 _dir, int _dmg, int _stunDmg, string _tag, ISkillCaster _caster)
    {
        if (rigid == null) rigid = GetComponent<Rigidbody2D>();

        currentDir = _dir == Vector2.zero ? Vector2.right : _dir.normalized;
        rigid.velocity = currentDir * moveSpeed;

        DOVirtual.DelayedCall(10, () => this.ReleaseObject());
    }

    void FixedUpdate()
    {
        ObjMovement();
    }

    public override void ObjMovement()
    {
        if (currentDir != Vector2.zero)
        {
            rigid.velocity = currentDir * moveSpeed;
        }
    }

    // 충돌 시작 시 처리
    public void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other);
    }

    // 구석에 끼는 것을 방지하기 위해 충돌 유지 중에도 체크
    public void OnCollisionStay2D(Collision2D other)
    {
        HandleCollision(other);
    }

    private void HandleCollision(Collision2D other)
    {
        // 벽 레이어(3) 확인
        if (other.gameObject.layer == 3)
        {
            bool hasReflected = false;
            Vector2 combinedNormal = Vector2.zero;

            for (int i = 0; i < other.contactCount; i++)
            {
                Vector2 contactNormal = other.GetContact(i).normal;

                // 현재 진행 방향과 벽이 마주보고 있을 때만 (이미 튕겨나가는 중이 아닐 때만) 계산
                // 구석 처리를 위해 약간의 오차(-0.1f)를 허용
                if (Vector2.Dot(currentDir, contactNormal) < -0.1f)
                {
                    combinedNormal += contactNormal;
                    hasReflected = true;
                }
            }

            if (hasReflected)
            {
                combinedNormal.Normalize();

                // 1. 반사 방향 계산 + 무작위성 추가
                Vector2 reflectDir = Vector2.Reflect(currentDir, combinedNormal);
                float randomAngle = Random.Range(-bounceRandomness, bounceRandomness);
                reflectDir = Quaternion.Euler(0, 0, randomAngle) * reflectDir;
                currentDir = reflectDir.normalized;

                // 2. 중요: 위치 보정 (벽 바깥쪽으로 살짝 밀어내어 끼임 방지)
                transform.position += (Vector3)combinedNormal * skinWidth;

                // 3. 물리 값 즉시 갱신
                rigid.velocity = currentDir * moveSpeed;
                float angle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                Debug.Log($"벽 충돌 대응 - 법선: {combinedNormal}, 위치 보정 실행");
            }
        }
    }
}
