using System;
using System.Collections;
using System.Collections.Generic;
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
		dangerAreaSize = GetInputValue<Vector2>("dangerAreaSize", this.dangerAreaSize);
		dangerAreaPos = GetInputValue<Vector2>("dangerAreaPos", this.dangerAreaPos);

		var target = LocalGameManager.instance.unitManager.PlayerUnit;
		GameObject dangerArea = LocalGameManager.instance.objectPoolManager.poolDic["DangerArea"].GetGo("DangerAreaX");

		dir = target.transform.position - caster.GetCom<Transform>().position;
		dangerArea.transform.SetParent(caster.GetGameObject().transform.GetChild(2).transform.GetChild(0));
		dangerArea.transform.localPosition = dangerAreaPos;

		if (targeting)
		{
			//각도 조절.
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			dangerAreaAngle = Quaternion.Euler(0, 0, angle);
			dangerArea.transform.parent.rotation = Quaternion.Euler(0, 0, angle);
		}
		else
		{
			dangerAreaAngle = Quaternion.Euler(0, 0, 0);
			dangerArea.transform.rotation = Quaternion.Euler(0, 0, 0);
		}

		if (autoLength)
		{
			dangerAreaSize.x = dir.magnitude * caster.GetDirection().x;
			dangerArea.transform.localScale = new Vector2(dir.magnitude * caster.GetDirection().x, dangerAreaSize.y);
		}
		else
		{
			dangerArea.transform.localScale = new Vector2(dangerAreaSize.x, dangerAreaSize.y);
		}

		var dangerAreaCom = dangerArea.GetComponent<DangerArea>();

		dangerAreaCom.Activate(delayTime, () =>
		{
			try
			{
				foreach (var port in DynamicOutputs)
				{
					var child = port.Connection.node as SkillNode;
					child.Evaluate(caster);
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