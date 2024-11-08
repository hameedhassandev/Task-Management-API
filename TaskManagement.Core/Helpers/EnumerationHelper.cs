using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public static class EnumerationHelper
    {
        public static string GetEnumDescription<T>(string EnumValue)
        {
            var enumType = typeof(T);
            var memberInfo = enumType.GetMember(EnumValue);
            if(memberInfo == null)
                return string.Empty;

            var enumValueMemberInfo = memberInfo.FirstOrDefault(m => m.DeclaringType == enumType);
            if (enumValueMemberInfo == null)
                return string.Empty;
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var name = ((DescriptionAttribute)valueAttributes[0]).Description; 
            
            return name;
        }
    }
}
