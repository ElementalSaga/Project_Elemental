using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Skill/Delay")]
public class Delay : SkillNode
{
    [Output(dynamicPortList = true)] public List<SkillNode> childs;
    public float delayTime;

    public override void Evaluate(ISkillCaster caster)
    {
        LocalGameManager.instance.coroutineRunner.StartCoroutine(PerformDelay(delayTime));

        foreach (var port in DynamicOutputs)
        {
            var child = port.Connection.node as SkillNode;
            child.Evaluate(caster);
        }
    }

    private IEnumerator PerformDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }
}
