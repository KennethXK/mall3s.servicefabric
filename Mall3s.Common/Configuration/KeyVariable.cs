using Mall3s.Common.Extension;
using Mall3s.Dependency;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mall3s.Common.Configuration
{
    /// <summary>
    /// Key常量
    /// </summary>
    [SuppressSniffer]
    public class KeyVariable
    {
        /// <summary>
        /// 多租户模式
        /// </summary>
        public static bool MultiTenancy
        {
            get
            {
                var flag = App.Configuration["Mall3s_App:MultiTenancy"];
                return flag.ToBool();
            }
        }

        /// <summary>
        /// 系统文件路径
        /// </summary>
        public static string SystemPath
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["Mall3s_App:SystemPath"]) ? Directory.GetCurrentDirectory() : App.Configuration["Mall3s_App:SystemPath"];
            }
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public static List<string> AreasName
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["Mall3s_APP:CodeAreasName"]) ? new List<string>() : App.Configuration["Mall3s_APP:CodeAreasName"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 允许上传图片类型
        /// </summary>
        public static List<string> AllowImageType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["Mall3s_APP:AllowUploadImageType"]) ? new List<string>() : App.Configuration["Mall3s_APP:AllowUploadImageType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 允许上传文件类型
        /// </summary>
        public static List<string> AllowUploadFileType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["Mall3s_APP:AllowUploadFileType"]) ? new List<string>() : App.Configuration["Mall3s_APP:AllowUploadFileType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 微信允许上传文件类型
        /// </summary>
        public static List<string> WeChatUploadFileType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["Mall3s_APP:WeChatUploadFileType"]) ? new List<string>() : App.Configuration["Mall3s_APP:WeChatUploadFileType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// MinIO桶
        /// </summary>
        public static string BucketName
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["Mall3s_APP:BucketName"]) ? "" : App.Configuration["Mall3s_APP:BucketName"];
            }
        }

        /// <summary>
        /// 文件储存类型
        /// </summary>
        public static string FileStoreType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["Mall3s_APP:FileStoreType"]) ? "local" : App.Configuration["Mall3s_APP:FileStoreType"];
            }
        }
    }
}
