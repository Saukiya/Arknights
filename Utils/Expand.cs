using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Expand {
    // 标签名字
    public static string GetName(this Enum value) {
        Type temType = value.GetType();
        MemberInfo[] memberInfos = temType.GetMember(value.ToString());
        if (memberInfos.Length == 0) return value.ToString();
        object[] objs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute),false);
        return objs.Length > 0 ? ((DescriptionAttribute) objs[0]).Description : value.ToString();
    }

    // 查找子物体的组件
    public static T GetComponent<T>(this Transform value, string path) where T : Object {
        return value.Find(path).GetComponent<T>();
    }

    public static Vector3 Round(this Vector3 vector3) {
        return new Vector3(Mathf.Round(vector3.x), Mathf.Round(vector3.y), Mathf.Round(vector3.z));
    }
}