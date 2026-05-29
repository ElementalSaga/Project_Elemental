using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using XNode;

[CreateNodeMenu("Skill/SetDangerArea")]
public class SetDangerArea : SkillNode
{
	[Output(dynamicPortList = true)] public List<SkillNode> childs;

	[Header("경고 시간")]
	[SerializeField] private float delayTime;

	[Header("경고 박스 크기 및 위치와 각도")]
	[Input] public Vector2 dangerAreaSize;
	[Input] public Vector2 dangerAreaPos;
	[Output] public Vector2 dangerAreaSizeOutput;
	[Output] public Vector2 dangerAreaPosOutput;
	[Output] public Quaternion dangerAreaAngle;

	[Header("플레이어 위치에 따라 각도가 변화하는지 여부.")]
	[SerializeField] private bool targeting;

	[Header("자동 길이 조절.")]
	[SerializeField] private bool autoLength;

	private Vector2 dir;

	public override void Evaluate(ISkillCaster caster)
	{
		//경고 지대 크기 및 위치를 다른 노드에서 받아옴.
		dangerAreaSize = GetInputValue<Vector2>("dangerAreaSize", this.dangerAreaSize);
		dangerAreaPos = GetInputValue<Vector2>("dangerAreaPos", this.dangerAreaPos);

		//타깃 오브젝트 체크
		var target = LocalGameManager.instance.unitManager.PlayerUnit;

		//오브젝트 풀에서 위험 표시기 가져오기.
		GameObject dangerArea = LocalGameManager.instance.objectPoolManager.poolDic["DangerArea"].GetGo("DangerAreaX");

		//시전자의 Transform
		var casterTransform = caster.GetCom<Transform>();

		//시전자에서 타겟을 향한 방향 체크. 목적지 위치 - 자신 위치
		dir = target.transform.position - casterTransform.position;

		//부모 설정 및, 부모 오브젝트 위치를 기준으로 경고 영역 위치 설정
		dangerArea.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
		dangerArea.transform.localPosition = dangerAreaPos;

		if (targeting) //타겟의 위치에 따라 자동으로 각도를 조정할 때.
		{
			//각도 계산
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			dangerAreaAngle = Quaternion.Euler(0, 0, angle); //각도 설정.
			dangerArea.transform.rotation = dangerAreaAngle; //아웃풋에 전달.
		}
		else //고정된 각도를 사용할 때
		{
			dangerAreaAngle = Quaternion.identity;
			dangerArea.transform.localRotation = Quaternion.identity;
		}

		if (autoLength)
		{
			dangerAreaSize.x = dir.magnitude * (targeting ? 1 : caster.GetDirection().x) * caster.GetDirection().x;
			dangerArea.transform.localScale = new Vector2(dangerAreaSize.x, dangerAreaSize.y);
		}
		else
		{
			dangerArea.transform.localScale = new Vector2(dangerAreaSize.x, dangerAreaSize.y);
		}

		// 부모의 Flip(Scale -1) 영향을 받지 않도록 월드 좌표계로 분리합니다. (SetHitBox와 동일한 방식)


		var dangerAreaCom = dangerArea.GetComponent<DangerArea>();

		dangerAreaCom.Activate(delayTime, () =>
		{
			try
			{
				foreach (var port in DynamicOutputs)
				{
					if (port.fieldName.StartsWith("childs "))
					{
						if (port.IsConnected)
						{
							var child = port.Connection.node as SkillNode;
							child.Evaluate(caster);
						}
					}
				}
			}
			finally
			{
				Debug.Log("차징 완료");
			}
		});
	}

	public override object GetValue(NodePort port)
	{
		if (port.fieldName == "dangerAreaAngle") return dangerAreaAngle;
		if (port.fieldName == "dangerAreaSizeOutput") return dangerAreaSize;
		if (port.fieldName == "dangerAreaPosOutput") return dangerAreaPos;
		return null;
	}
}