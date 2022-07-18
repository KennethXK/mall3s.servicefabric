using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Common.Extension
{
    /// <summary>
    /// 类操作扩展
    /// </summary>
    public static class ClassExtensions
    {
        /// <summary>
        /// 检查是否为匿名类
        /// 参数为空默认返回false
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
            {
                return false;
            }
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && type.Name.Contains("AnonymousType")
                && type.Name.StartsWith("<>")
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
