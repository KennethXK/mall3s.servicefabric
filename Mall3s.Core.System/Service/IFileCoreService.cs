using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    public interface IFileCoreService
    {
        #region 导入导出

        /// <summary>
        /// 导出(本地导出文件并上传七牛云）
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        dynamic Export(string jsonStr, string name);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string Import(IFormFile file);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        dynamic UploadLocalFile(string type, IFormFile file);

        /// <summary>
        /// 根据类型获取目录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetPathByType(string type);
        #endregion
    }
}
