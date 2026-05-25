using System;
using System.Reflection;
using UnityEngine;

[CreateNodeMenu("BT/SetBlackboardValue")]
public class SetBlackboardValueNode : BTNode
{
    public enum ValueSource { Constant, Blackboard, UnitProperty }
    public enum OperationType { Set, Add, Subtract, Multiply, Divide, Modulo }
    public enum ValueType { Float, Int, Bool, String }

    [Header("대상 (Target)")]
    public string targetKey = "";
    public ValueType valueType;

    [Header("연산 (Operation)")]
    public OperationType operation;

    [Header("값 (Source)")]
    public ValueSource sourceMode;
    public string sourceKey = "";
    public float floatValue;
    public int intValue;
    public bool boolValue;
    public string stringValue;

    public override NodeState Evaluate(AIController controller)
    {
        if (blackboard == null) return NodeState.Failure;

        object sourceVal = GetSourceValue(controller);
        if (sourceVal == null && valueType != ValueType.String) return NodeState.Failure;

        try
        {
            switch (valueType)
            {
                case ValueType.Int:
                    int currentInt = blackboard.Get<int>(targetKey);
                    int srcInt = Convert.ToInt32(sourceVal);
                    //Debug.Log(CalculateInt(currentInt, srcInt));
                    blackboard.Set(targetKey, CalculateInt(currentInt, srcInt));
                    break;

                case ValueType.Float:
                    float currentFloat = blackboard.Get<float>(targetKey);
                    float srcFloat = Convert.ToSingle(sourceVal);
                    blackboard.Set(targetKey, CalculateFloat(currentFloat, srcFloat));
                    break;

                case ValueType.Bool:
                    blackboard.Set(targetKey, Convert.ToBoolean(sourceVal));
                    break;

                case ValueType.String:
                    blackboard.Set(targetKey, sourceVal?.ToString());
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SetBlackboardValueNode Error: {e.Message}");
            return NodeState.Failure;
        }

        return NodeState.Success;
    }

    private object GetSourceValue(AIController controller)
    {
        switch (sourceMode)
        {
            case ValueSource.Constant:
                return valueType switch
                {
                    ValueType.Float => floatValue,
                    ValueType.Int => intValue,
                    ValueType.Bool => boolValue,
                    ValueType.String => stringValue,
                    _ => null
                };
            case ValueSource.Blackboard:
                return valueType switch
                {
                    ValueType.Float => blackboard.Get<float>(sourceKey),
                    ValueType.Int => blackboard.Get<int>(sourceKey),
                    ValueType.Bool => blackboard.Get<bool>(sourceKey),
                    ValueType.String => blackboard.Get<string>(sourceKey),
                    _ => null
                };
            case ValueSource.UnitProperty:
                return GetUnitProperty(controller.ParentObj, sourceKey);
        }
        return null;
    }

    private object GetUnitProperty(GameObject obj, string propertyName)
    {
        if (obj == null) return null;
        foreach (var comp in obj.GetComponents<Component>())
        {
            if (comp == null) continue;
            Type type = comp.GetType();
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (prop != null) return prop.GetValue(comp);
            var field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) return field.GetValue(comp);
        }
        return null;
    }

    private int CalculateInt(int current, int source)
    {
        return operation switch
        {
            OperationType.Set => source,
            OperationType.Add => current + source,
            OperationType.Subtract => current - source,
            OperationType.Multiply => current * source,
            OperationType.Divide => source != 0 ? current / source : current,
            OperationType.Modulo => source != 0 ? current % source : current,
            _ => source
        };
    }

    private float CalculateFloat(float current, float source)
    {
        return operation switch
        {
            OperationType.Set => source,
            OperationType.Add => current + source,
            OperationType.Subtract => current - source,
            OperationType.Multiply => current * source,
            OperationType.Divide => Math.Abs(source) > float.Epsilon ? current / source : current,
            _ => source
        };
    }
}
