using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace PrefabViewer.Inspector
{
    public static class ComponentPropertyReader
    {
        static readonly HashSet<Type> SupportedTypes = new HashSet<Type>
        {
            typeof(int), typeof(float), typeof(double), typeof(bool), typeof(string),
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            typeof(uint), typeof(long), typeof(ulong),
            typeof(Vector2), typeof(Vector3), typeof(Vector4),
            typeof(Color), typeof(Color32), typeof(Quaternion),
            typeof(Rect), typeof(Bounds), typeof(LayerMask), typeof(Enum)
        };

        public static List<ComponentInfo> Read(GameObject target)
        {
            var result = new List<ComponentInfo>();
            if (target == null)
                return result;

            var components = target.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null)
                    continue;
                if (component is Transform)
                    continue;

                var info = new ComponentInfo
                {
                    typeName = component.GetType().Name,
                    enabled = GetEnabled(component)
                };

                ReadMembers(component, info.properties);
                result.Add(info);
            }

            return result;
        }

        static bool GetEnabled(Component component)
        {
            if (component is Behaviour behaviour)
                return behaviour.enabled;
            if (component is Renderer renderer)
                return renderer.enabled;
            if (component is Collider collider)
                return collider.enabled;
            return true;
        }

        static void ReadMembers(object target, List<PropertyInfoDto> output)
        {
            var type = target.GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var field in type.GetFields(flags))
            {
                if (field.IsStatic)
                    continue;
                if (field.IsDefined(typeof(NonSerializedAttribute), true) && !field.IsDefined(typeof(SerializeField), true))
                    continue;
                if (field.IsDefined(typeof(HideInInspector), true))
                    continue;

                AddField(field, target, output);
            }

            foreach (var property in type.GetProperties(flags))
            {
                if (!property.CanRead || property.GetIndexParameters().Length > 0)
                    continue;
                if (property.GetMethod == null || !property.GetMethod.IsPublic)
                    continue;

                AddProperty(property, target, output);
            }
        }

        static void AddField(FieldInfo field, object target, List<PropertyInfoDto> output)
        {
            object value;
            try
            {
                value = field.GetValue(target);
            }
            catch
            {
                return;
            }

            output.Add(BuildDto(field.Name, field.FieldType, value));
        }

        static void AddProperty(PropertyInfo property, object target, List<PropertyInfoDto> output)
        {
            object value;
            try
            {
                value = property.GetValue(target);
            }
            catch
            {
                return;
            }

            output.Add(BuildDto(property.Name, property.PropertyType, value));
        }

        static PropertyInfoDto BuildDto(string name, Type type, object value)
        {
            var dto = new PropertyInfoDto { name = name, typeName = type.Name };
            PopulateDisplay(dto, value, type);
            return dto;
        }

        static void PopulateDisplay(PropertyInfoDto dto, object value, Type type)
        {
            if (value == null && typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                dto.displayKind = PropertyDisplayKind.ObjectReference;
                dto.value = $"None ({type.Name})";
                return;
            }

            if (type == typeof(bool))
            {
                dto.displayKind = PropertyDisplayKind.Bool;
                dto.boolValue = (bool)value;
                return;
            }

            if (type == typeof(int) || type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) || type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong))
            {
                dto.displayKind = PropertyDisplayKind.Integer;
                dto.value = value.ToString();
                return;
            }

            if (type == typeof(float) || type == typeof(double))
            {
                dto.displayKind = PropertyDisplayKind.Float;
                dto.value = value is double d ? d.ToString("0.####") : ((float)value).ToString("0.####");
                return;
            }

            if (type == typeof(string))
            {
                dto.displayKind = PropertyDisplayKind.String;
                dto.value = (string)value ?? "";
                return;
            }

            if (type.IsEnum)
            {
                dto.displayKind = PropertyDisplayKind.Enum;
                dto.value = value.ToString();
                return;
            }

            if (type == typeof(Vector2))
            {
                var v = (Vector2)value;
                dto.displayKind = PropertyDisplayKind.Vector2;
                dto.vectorX = v.x;
                dto.vectorY = v.y;
                return;
            }

            if (type == typeof(Vector3))
            {
                var v = (Vector3)value;
                dto.displayKind = PropertyDisplayKind.Vector3;
                dto.vectorX = v.x;
                dto.vectorY = v.y;
                dto.vectorZ = v.z;
                return;
            }

            if (type == typeof(Vector4))
            {
                var v = (Vector4)value;
                dto.displayKind = PropertyDisplayKind.Vector4;
                dto.vectorX = v.x;
                dto.vectorY = v.y;
                dto.vectorZ = v.z;
                dto.vectorW = v.w;
                return;
            }

            if (type == typeof(Color))
            {
                var c = (Color)value;
                dto.displayKind = PropertyDisplayKind.Color;
                dto.vectorX = c.r;
                dto.vectorY = c.g;
                dto.vectorZ = c.b;
                dto.vectorW = c.a;
                return;
            }

            if (type == typeof(LayerMask))
            {
                dto.displayKind = PropertyDisplayKind.LayerMask;
                var mask = ((LayerMask)value).value;
                dto.value = mask == 0 ? "Nothing" : mask.ToString();
                return;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                var obj = (UnityEngine.Object)value;
                dto.displayKind = PropertyDisplayKind.ObjectReference;
                dto.value = obj != null ? $"{obj.name} ({type.Name})" : $"None ({type.Name})";
                return;
            }

            var formatted = FormatValue(value, type);
            if (formatted == "[Unsupported type]" || formatted == "[UnityEvent]" || formatted == "[Dictionary]" ||
                (formatted.StartsWith("[") && formatted.EndsWith("]")))
            {
                dto.displayKind = PropertyDisplayKind.Unsupported;
                dto.value = formatted;
                return;
            }

            dto.displayKind = PropertyDisplayKind.Default;
            dto.value = formatted;
        }

        static string FormatValue(object value, Type type)
        {
            if (value == null)
                return "None";

            if (type.IsEnum || (type.IsGenericType && Nullable.GetUnderlyingType(type)?.IsEnum == true))
                return value.ToString();

            if (type == typeof(string))
                return (string)value;

            if (type == typeof(bool))
                return ((bool)value) ? "true" : "false";

            if (type == typeof(int) || type == typeof(float) || type == typeof(double) ||
                type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
                type == typeof(ushort) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong))
                return value.ToString();

            if (type == typeof(Vector2))
            {
                var v = (Vector2)value;
                return $"({v.x:0.###}, {v.y:0.###})";
            }

            if (type == typeof(Vector3))
            {
                var v = (Vector3)value;
                return $"({v.x:0.###}, {v.y:0.###}, {v.z:0.###})";
            }

            if (type == typeof(Vector4))
            {
                var v = (Vector4)value;
                return $"({v.x:0.###}, {v.y:0.###}, {v.z:0.###}, {v.w:0.###})";
            }

            if (type == typeof(Color))
            {
                var c = (Color)value;
                return $"RGBA({c.r:0.###}, {c.g:0.###}, {c.b:0.###}, {c.a:0.###})";
            }

            if (type == typeof(Color32))
            {
                var c = (Color32)value;
                return $"RGBA({c.r}, {c.g}, {c.b}, {c.a})";
            }

            if (type == typeof(Quaternion))
            {
                var e = ((Quaternion)value).eulerAngles;
                return $"Euler({e.x:0.#}, {e.y:0.#}, {e.z:0.#})";
            }

            if (type == typeof(Rect))
            {
                var r = (Rect)value;
                return $"(x:{r.x:0.###}, y:{r.y:0.###}, w:{r.width:0.###}, h:{r.height:0.###})";
            }

            if (type == typeof(Bounds))
            {
                var b = (Bounds)value;
                return $"center:{b.center} size:{b.size}";
            }

            if (type == typeof(LayerMask))
                return ((LayerMask)value).value.ToString();

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                var obj = (UnityEngine.Object)value;
                return obj != null ? obj.name : "None";
            }

            if (value is UnityEvent)
                return "[UnityEvent]";

            if (value is IList list)
                return FormatList(list);

            if (value is IDictionary)
                return "[Dictionary]";

            if (IsSupportedPrimitive(type))
                return value.ToString();

            return "[Unsupported type]";
        }

        static bool IsSupportedPrimitive(Type type)
        {
            if (type.IsEnum)
                return true;
            if (SupportedTypes.Contains(type))
                return true;
            return false;
        }

        static string FormatList(IList list)
        {
            if (list.Count == 0)
                return "[]";
            if (list.Count > 8)
                return $"[{list.Count} items]";

            var parts = new List<string>();
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                parts.Add(item == null ? "null" : FormatValue(item, item.GetType()));
            }
            return "[" + string.Join(", ", parts) + "]";
        }
    }
}
