﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MockSchoolManagement.Extensions
{
    /// <summary>
    /// 枚举的扩展类
    /// </summary>
    public static class EnumExtension
    {
        public static string GetDisplayName(this System.Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memberInfo = type.GetMember(en.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute),true);
                if (attrs != null && attrs.Length > 0)
                { 
                    return ((DisplayAttribute)attrs[0]).Name; 
                }
            }
            return en.ToString();
        }
    }
}
