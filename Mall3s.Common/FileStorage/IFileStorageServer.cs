using Mall3s.Common.FileManage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Common.FileStorage
{
    /// <summary>
    /// 文件存储
    /// </summary>
    public interface IFileStorageServer
    {
        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="type">图片类型 </param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ </param>
        /// <returns></returns>
        public Task<IActionResult> DownloadFile(string type, string fileName);
        /// <summary>
        /// 获取下载加密链接(xxx|fileName|type|false)格式
        /// </summary>
        /// <param name="fileId">文件标识</param>
        /// <param name="type">文件类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="local">是否是本地存储（本地存储，则直接从本地文件夹读取）</param>
        /// <returns></returns>
        public string GetDownloadUrlEncryption(string fileId, string type, string fileName, bool local = false);
        /// <summary>
        /// 下载文件链接(根据加密链接字符串)
        /// </summary>
        public Task<dynamic> DownloadFile([FromQuery] string encryption);


        /// <summary>
        /// 上传文件/图片(返回文件名)
        /// </summary>
        /// <returns></returns>
        public Task<FileUploadResult> Uploader(string type, IFormFile file);
        /// <summary>
        /// 上传本地文件(返回文件名+路径）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        public FileUploadResult UploadLocalFile(string type, IFormFile file);
        /// <summary>
        /// 根据类型获取文件存储路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public string GetPathByType(string type);
        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public bool AllowFileType(string fileExtension, string type);
        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <returns></returns>
        public bool AllowImageType(string fileExtension);

        /// <summary>
        /// 复制本地文件到云存储
        /// </summary>
        /// <param name="fileFullName">本地完整路径</param>
        /// <param name="uploadPath">文件相对路径</param>
        /// <returns></returns>
        public Task<bool> CopyFileByType(string fileFullName, string uploadPath);

    }
}
