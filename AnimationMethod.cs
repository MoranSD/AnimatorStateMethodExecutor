using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utils.Animation
{
    //    [System.Serializable]
    //    public abstract class AnimationMethodArgument
    //    {
    //        public abstract object GetValue();
    //    }
    //    [System.Serializable]
    //    public class FloatArgument : AnimationMethodArgument
    //    {
    //        public float value;

    //        public override object GetValue() => value;
    //    }

    //    [System.Serializable]
    //    public class IntArgument : AnimationMethodArgument
    //    {
    //        public int value;

    //        public override object GetValue() => value;
    //    }

    //    [System.Serializable]
    //    public class StringArgument : AnimationMethodArgument
    //    {
    //        public string value;

    //        public override object GetValue() => value;
    //    }

    //    [System.Serializable]
    //    public class Vector3Argument : AnimationMethodArgument
    //    {
    //        public Vector3 value;

    //        public override object GetValue() => value;
    //    }

    //    [System.Serializable]
    //    public class BoolArgument : AnimationMethodArgument
    //    {
    //        public bool value;

    //        public override object GetValue() => value;
    //    }

    //    [System.Serializable]
    //    public class GameObjectArgument : AnimationMethodArgument
    //    {
    //        public GameObject value;

    //        public override object GetValue() => value;
    //    }

    [Serializable]
    public class SerializableArgument
    {
        public enum ArgumentType
        {
            Float,
            Int,
            String,
            Vector3,
            Bool,
            GameObject
        }

        public ArgumentType Type;
        public float FloatValue;
        public int IntValue;
        public string StringValue;
        public Vector3 Vector3Value;
        public bool BoolValue;
        public GameObject GameObjectValue;

        public SerializableArgument() { }

        public object GetValue()
        {
            switch (Type)
            {
                case ArgumentType.Float: return FloatValue;
                case ArgumentType.Int: return IntValue;
                case ArgumentType.String: return StringValue;
                case ArgumentType.Vector3: return Vector3Value;
                case ArgumentType.Bool: return BoolValue;
                case ArgumentType.GameObject: return GameObjectValue;
                default: return null;
            }
        }

        public void SetValue(object value)
        {
            if (value is float f)
            {
                Type = ArgumentType.Float;
                FloatValue = f;
            }
            else if (value is int i)
            {
                Type = ArgumentType.Int;
                IntValue = i;
            }
            else if (value is string s)
            {
                Type = ArgumentType.String;
                StringValue = s;
            }
            else if (value is Vector3 v)
            {
                Type = ArgumentType.Vector3;
                Vector3Value = v;
            }
            else if (value is bool b)
            {
                Type = ArgumentType.Bool;
                BoolValue = b;
            }
            else if (value is GameObject g)
            {
                Type = ArgumentType.GameObject;
                GameObjectValue = g;
            }
        }
    }

    [Serializable]
    public class AnimationMethod
    {
        public string AnimationStateName;
        public AnimatorStateEventType ExecuteTime;

        public string SelectedMethodName;
        public List<SerializableArgument> SelectedMethodArguments = new List<SerializableArgument>();

        [HideInInspector] public string PreviousSelectedMethodName;

        public void Execute()
        {
            if (string.IsNullOrEmpty(SelectedMethodName) || SelectedMethodArguments == null)
                return;

            var method = typeof(AnimationHelper)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(x => x.Name == SelectedMethodName);

            object[] args = SelectedMethodArguments.Select(arg => arg.GetValue()).ToArray();
            method.Invoke(null, args);
        }
    }

    //[System.Serializable]
    //public class ObjectSerializable : ISerializationCallbackReceiver
    //{
    //    public ObjectSerializable(object _val)
    //    {
    //        val = _val;
    //    }

    //    // our value of interest
    //    private object val;

    //    [SerializeField, HideInInspector]
    //    private string valSerialized;


    //    public object GetValue()
    //    {
    //        return val;
    //    }

    //    public void OnBeforeSerialize()
    //    {
    //        if (val == null)
    //        {
    //            valSerialized = "n";
    //            return;
    //        }
    //        var type = val.GetType();
    //        if (type == typeof(int))
    //            valSerialized = "i" + val.ToString();
    //        else if (type == typeof(float))
    //            valSerialized = "f" + val.ToString();
    //        else if (type == typeof(Color))
    //        {
    //            Color32 c = (Color)val;
    //            uint v = (uint)c.r + ((uint)c.g << 8) + ((uint)c.b << 16) + ((uint)c.a << 24);
    //            valSerialized = "c" + v;
    //        }
    //        else if (type == typeof(Vector2))
    //        {
    //            Vector2 v = (Vector2)val;
    //            valSerialized = "w" + v.x + "|" + v.y;
    //        }
    //        else if (type == typeof(Vector3))
    //        {
    //            Vector3 v = (Vector3)val;
    //            valSerialized = "v" + v.x + "|" + v.y + "|" + v.z;
    //        }
    //        else if (type == typeof(bool))
    //        {
    //            bool v = (bool)val;
    //            valSerialized = "b" + v;
    //        }
    //    }

    //    public void OnAfterDeserialize()
    //    {
    //        if (valSerialized.Length == 0)
    //            return;
    //        char type = valSerialized[0];
    //        if (type == 'n')
    //            val = null;
    //        else if (type == 'i')
    //            val = int.Parse(valSerialized.Substring(1));
    //        else if (type == 'f')
    //            val = float.Parse(valSerialized.Substring(1));
    //        else if (type == 'c')
    //        {
    //            uint v = uint.Parse(valSerialized.Substring(1)); //
    //            Color c = new Color32((byte)(v & 0xFF), (byte)((v >> 8) & 0xFF), (byte)((v >> 16) & 0xFF), (byte)((v >> 24) & 0xFF));
    //            val = c;
    //        }
    //        else if (type == 'w')
    //        {
    //            string[] v = valSerialized.Substring(1).Split('|');
    //            val = new Vector2(float.Parse(v[0]), float.Parse(v[1]));
    //        }
    //        else if (type == 'v')
    //        {
    //            string[] v = valSerialized.Substring(1).Split('|');
    //            val = new Vector3(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]));
    //        }
    //        else if (type == 'b')
    //        {
    //            val = bool.Parse(valSerialized.Substring(1));
    //        }
    //    }
    //}
}