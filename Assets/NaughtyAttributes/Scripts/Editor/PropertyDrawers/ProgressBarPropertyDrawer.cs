using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Data;

namespace NaughtyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarPropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            ProgressBarAttribute progressBarAttribute = PropertyUtility.GetAttribute<ProgressBarAttribute>(property);
            var maxValue = GetMaxValue(property, progressBarAttribute);

            return IsNumber(property) && IsNumber(maxValue)
                ? GetPropertyHeight(property)
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (!IsNumber(property))
            {
                string message = string.Format("Field {0} is not a number", property.name);
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
                return;
            }

            ProgressBarAttribute progressBarAttribute = PropertyUtility.GetAttribute<ProgressBarAttribute>(property);
            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
            var valueFormatted = property.propertyType == SerializedPropertyType.Integer ? value.ToString() : string.Format("{0:0.00}", value);
            var maxValue = GetMaxValue(property, progressBarAttribute);

            if (maxValue != null && IsNumber(maxValue))
            {
                var fillPercentage = value / CastToFloat(maxValue);
                var barLabel = (!string.IsNullOrEmpty(progressBarAttribute.Name) ? "[" + progressBarAttribute.Name + "] " : "") + valueFormatted + "/" + maxValue;
                Color barColor = GetColor(progressBarAttribute, property);
                var labelColor = Color.white;

                var indentLength = NaughtyEditorGUI.GetIndentLength(rect);
                Rect barRect = new Rect()
                {
                    x = rect.x + indentLength,
                    y = rect.y,
                    width = rect.width - indentLength,
                    height = EditorGUIUtility.singleLineHeight
                };

                HandleInput(barRect, property, maxValue);
                DrawBar(barRect, Mathf.Clamp01(fillPercentage), barLabel, barColor, labelColor);
            }
            else
            {
                string message = string.Format(
                    "The provided dynamic max value for the progress bar is not correct. Please check if the '{0}' is correct, or the return type is float/int",
                    nameof(progressBarAttribute.MaxValueName));

                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private void HandleInput(Rect rect, SerializedProperty property, object maxValue)
        {
            bool changed = false;
            float minValue = 0f;
            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
            var controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition) ||
                GUIUtility.hotControl == controlId && (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag))
            {
                // Update value based on mouse position.
                GUIUtility.hotControl = controlId;
                value = minValue + Mathf.Abs(CastToFloat(maxValue) - minValue) *
                    Mathf.Clamp01((Event.current.mousePosition.x - rect.xMin) / rect.width);

                changed = true;
            }
            else if (GUIUtility.hotControl == controlId && Event.current.rawType == EventType.MouseUp)
            {
                // Release hot control.
                GUIUtility.hotControl = 0;
            }

            if (changed)
            {
                GUI.changed = true;

                value = value <= minValue ? minValue : value >= CastToFloat(maxValue) ? CastToFloat(maxValue) : value;
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        property.intValue = (int)value;
                        break;
                    case SerializedPropertyType.Float:
                        property.floatValue = value;
                        break;
                }

                Event.current.Use();
            }
        }

        private object GetMaxValue(SerializedProperty property, ProgressBarAttribute progressBarAttribute)
        {
            if (string.IsNullOrEmpty(progressBarAttribute.MaxValueName))
            {
                return progressBarAttribute.MaxValue;
            }
            else
            {
                object target = PropertyUtility.GetTargetObjectWithProperty(property);

                FieldInfo valuesFieldInfo = ReflectionUtility.GetField(target, progressBarAttribute.MaxValueName);
                if (valuesFieldInfo != null)
                {
                    return valuesFieldInfo.GetValue(target);
                }

                PropertyInfo valuesPropertyInfo = ReflectionUtility.GetProperty(target, progressBarAttribute.MaxValueName);
                if (valuesPropertyInfo != null)
                {
                    return valuesPropertyInfo.GetValue(target);
                }

                MethodInfo methodValuesInfo = ReflectionUtility.GetMethod(target, progressBarAttribute.MaxValueName);
                if (methodValuesInfo != null &&
                    (methodValuesInfo.ReturnType == typeof(float) || methodValuesInfo.ReturnType == typeof(int)) &&
                    methodValuesInfo.GetParameters().Length == 0)
                {
                    return methodValuesInfo.Invoke(target, null);
                }

                return null;
            }
        }

        private void DrawBar(Rect rect, float fillPercent, string label, Color barColor, Color labelColor)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            var fillRect = new Rect(rect.x, rect.y, rect.width * fillPercent, rect.height);

            EditorGUI.DrawRect(rect, new Color(0.13f, 0.13f, 0.13f));
            EditorGUI.DrawRect(fillRect, barColor);

            // set alignment and cache the default
            var align = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.UpperCenter;

            // set the color and cache the default
            var c = GUI.contentColor;
            GUI.contentColor = labelColor;

            // calculate the position
            var labelRect = new Rect(rect.x, rect.y - 2, rect.width, rect.height);

            // draw~
            EditorGUI.DropShadowLabel(labelRect, label);

            // reset color and alignment
            GUI.contentColor = c;
            GUI.skin.label.alignment = align;
        }

        private bool IsNumber(SerializedProperty property)
        {
            bool isNumber = property.propertyType == SerializedPropertyType.Float || property.propertyType == SerializedPropertyType.Integer;
            return isNumber;
        }

        private bool IsNumber(object obj)
        {
            return (obj is float) || (obj is int);
        }

        private float CastToFloat(object obj)
        {
            if (obj is int)
            {
                return (int)obj;
            }
            else
            {
                return (float)obj;
            }
        }
        
        private Color GetColor(ProgressBarAttribute attribute, SerializedProperty property)
        {
            if (attribute.Color != EColor.Custom)
            {
                return attribute.Color.GetColor();
            }
            else if (!string.IsNullOrEmpty(attribute.ColorName))
            {
                return PropertyUtility.GetValue<Color>(property, attribute.ColorName);
            }
            else if (!string.IsNullOrEmpty(attribute.HexColor))
            {
                if (ColorUtility.TryParseHtmlString(attribute.HexColor, out Color color))
                {
                    return color;
                }
            }

            throw new InvalidExpressionException($"Invalid color configuration for ProgressBarAttribute on field {property.name}. Please check if the Color is set to Custom and the ColorName or HexColor is correct.");
        }
    }
}
