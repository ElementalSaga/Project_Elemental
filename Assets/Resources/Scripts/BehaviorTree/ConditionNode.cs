using System;
using System.Reflection;
using UnityEngine;

[CreateNodeMenu("BT/ConditionNode")]
public class ConditionNode : BTNode
{
    public enum ValueSource { Constant, Blackboard, UnitProperty, AIState }
    public enum SignType { Greater, Lower, EqualGreater, EqualLower, Equal, NotEqual }
    public enum ValueType { Float, Int, Bool, Enum }

    [Header("좌항 (Value A)")]
    public ValueSource leftSource = ValueSource.Blackboard;
    public string leftKey = "Distance";
    public ValueType valueType = ValueType.Float;

    [Header("비교 연산자")]
    public SignType sign = SignType.Equal;

    [Header("우항 (Value B)")]
    public ValueSource rightSource = ValueSource.Constant;
    public string rightKey = "";
    public float floatValue;
    public int intValue;
    public bool boolValue;
    public AIController.UnitState stateValue;

    public override NodeState Evaluate(AIController controller)
    {
        object leftVal = GetValue(leftSource, leftKey, controller);
        object rightVal = GetValue(rightSource, rightKey, controller);

        if (leftVal == null || rightVal == null)
        {
            // AIState인 경우 Enum 비교를 위해 null 체크 예외 처리 가능
            if (leftSource != ValueSource.AIState && rightSource != ValueSource.AIState)
            {
                //Debug.LogWarning($"ConditionNode: One of the values is null. Left: {leftVal}, Right: {rightVal}");
                return NodeState.Failure;
            }
        }

        bool result = false;

        try
        {
            switch (valueType)
            {
                case ValueType.Float:
                    result = CompareFloat(Convert.ToSingle(leftVal), Convert.ToSingle(rightVal), sign);
                    break;
                case ValueType.Int:
                    result = CompareInt(Convert.ToInt32(leftVal), Convert.ToInt32(rightVal), sign);
                    break;
                case ValueType.Bool:
                    result = CompareBool(Convert.ToBoolean(leftVal), Convert.ToBoolean(rightVal), sign);
                    break;
                case ValueType.Enum:
                    result = CompareEnum(leftVal, rightVal, sign);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"ConditionNode Error during evaluation: {e.Message}");
            return NodeState.Failure;
        }

        return result ? NodeState.Success : NodeState.Failure;
    }

    private object GetValue(ValueSource source, string key, AIController controller)
    {
        switch (source)
        {
            case ValueSource.Constant:
                switch (valueType)
                {
                    case ValueType.Float: return floatValue;
                    case ValueType.Int: return intValue;
                    case ValueType.Bool: return boolValue;
                    case ValueType.Enum: return stateValue;
                }
                break;

            case ValueSource.Blackboard:
                if (blackboard == null) return null;
                switch (valueType)
                {
                    case ValueType.Float: return blackboard.Get<float>(key);
                    case ValueType.Int: return blackboard.Get<int>(key);
                    case ValueType.Bool: return blackboard.Get<bool>(key);
                    case ValueType.Enum: return blackboard.Get<AIController.UnitState>(key);
                }
                break;

            case ValueSource.UnitProperty:
                return GetUnitProperty(controller.ParentObj, key);

            case ValueSource.AIState:
                return controller.curState;
        }
        return null;
    }

    private object GetUnitProperty(GameObject obj, string propertyName)
    {
        if (obj == null) return null;

        // 모든 컴포넌트를 순회하며 해당 이름의 프로퍼티나 필드를 찾음
        Component[] components = obj.GetComponents<Component>();
        foreach (var comp in components)
        {
            if (comp == null) continue;
            Type type = comp.GetType();

            // 프로퍼티 검색
            PropertyInfo prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (prop != null) return prop.GetValue(comp);

            // 필드 검색
            FieldInfo field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) return field.GetValue(comp);
        }

        return null;
    }

    private bool CompareFloat(float a, float b, SignType op)
    {
        switch (op)
        {
            case SignType.Greater: return a > b;
            case SignType.Lower: return a < b;
            case SignType.EqualGreater: return a >= b;
            case SignType.EqualLower: return a <= b;
            case SignType.Equal: return Mathf.Approximately(a, b);
            case SignType.NotEqual: return !Mathf.Approximately(a, b);
        }
        return false;
    }

    private bool CompareInt(int a, int b, SignType op)
    {
        switch (op)
        {
            case SignType.Greater: return a > b;
            case SignType.Lower: return a < b;
            case SignType.EqualGreater: return a >= b;
            case SignType.EqualLower: return a <= b;
            case SignType.Equal: return a == b;
            case SignType.NotEqual: return a != b;
        }
        return false;
    }

    private bool CompareBool(bool a, bool b, SignType op)
    {
        if (op == SignType.Equal) return a == b;
        if (op == SignType.NotEqual) return a != b;
        return false;
    }

    private bool CompareEnum(object a, object b, SignType op)
    {
        if (op == SignType.Equal) return Equals(a, b);
        if (op == SignType.NotEqual) return !Equals(a, b);
        return false;
    }
}
