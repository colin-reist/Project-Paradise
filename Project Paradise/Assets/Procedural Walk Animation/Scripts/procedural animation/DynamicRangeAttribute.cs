using UnityEngine;

namespace LolopupkaAnimations2D
{
public class DynamicRangeAttribute : PropertyAttribute
{
    public string FieldName { get; private set; }

    public DynamicRangeAttribute(string fieldName)
    {
        FieldName = fieldName;
    }
}
}