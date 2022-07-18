using Mall3s.Dependency;

namespace Mall3s.Authorization
{
    /// <summary>
    /// 常量、公共方法配置类
    /// </summary>
    [SuppressSniffer]
    internal static class Penetrates
    {
        /// <summary>
        /// 授权策略前缀
        /// </summary>
        internal const string AppAuthorizePrefix = "<Mall3s.Authorization.AppAuthorizeRequirement>";
    }
}