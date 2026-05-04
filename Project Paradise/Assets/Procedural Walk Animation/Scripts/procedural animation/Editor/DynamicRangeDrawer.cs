using UnityEditor;
using UnityEngine;


namespace LolopupkaAnimations2D
{
[CustomPropertyDrawer(typeof(DynamicRangeAttribute))]
public class DynamicRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DynamicRangeAttribute dynamicRange = (DynamicRangeAttribute)attribute;
        SerializedObject serializedObject = property.serializedObject;

        // Find the float variable that sets the dynamic max value
        SerializedProperty maxProperty = serializedObject.FindProperty(dynamicRange.FieldName);

        if (maxProperty != null && maxProperty.propertyType == SerializedPropertyType.Float)
        {
            float maxValue = maxProperty.floatValue;

            // Draw a slider with dynamic max value
            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(position, property, 0, maxValue, label);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [DynamicRange] with float.");
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Dynamic max value not found or not a float.");
        }
    }
}
}
