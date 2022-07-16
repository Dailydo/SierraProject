using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class OnChangedCallback : PropertyAttribute
{
    public string methodName;
    public OnChangedCallback(string methodNameNoArguments)
    {
        methodName = methodNameNoArguments;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(OnChangedCallback))]
public class OnChangedCallAttributePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label);
        if (EditorGUI.EndChangeCheck())
        {
            OnChangedCallback at = attribute as OnChangedCallback;
            MethodInfo method = property.serializedObject.targetObject.GetType().GetMethods().Where(m => m.Name == at.methodName).First();

            if (method != null && method.GetParameters().Count() == 0)// Only instantiate methods with 0 parameters
                method.Invoke(property.serializedObject.targetObject, null);
        }
    }
}

#endif
