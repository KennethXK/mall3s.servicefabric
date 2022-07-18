using Mall3s.Common.Extension;
using Mall3s.Common.Helper;
using Mall3s.Core.System.Manager;
using Mall3s.DataEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace Mall3s.Core.System.Service.Impl
{
    public class FileCoreService : IFileCoreService
    {
        private readonly IFileStorageCoreServer _fileStorageServer;
        private readonly IUserCoreManager _userManager;

        public FileCoreService(IFileStorageCoreServer fileServer, IUserCoreManager userManager)
        {
            _fileStorageServer = fileServer;
            _userManager = userManager;
        }
        #region 导入导出

        /// <summary>
        /// 导出(本地导出文件并上传七牛云）
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [NonAction]
        public dynamic Export(string jsonStr, string name)
        {
            var _fileName = name + Ext.GetTimeStamp + ".Json";
            var fileType = _fileStorageServer.GetPathByType("");
            var _filePath = App.WebHostEnvironment.WebRootPath + fileType;

            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);
            var byteList = new UTF8Encoding(true).GetBytes(jsonStr.ToCharArray());
            //注意全部要用英文名
            FileHelper.CreateFile(_filePath + _fileName, byteList);
            var result = _fileStorageServer.CopyFileByType(_filePath + _fileName, fileType + _fileName).Result;
            var fileName = _userManager.UserId + "|" + fileType + _fileName + "|json";
            var output = new
            {
                name = _fileName,
                url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(fileName, "Mall3s")
            };
            return output;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [NonAction]
        public string Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var byteList = new byte[file.Length];
            stream.Read(byteList, 0, (int)file.Length);
            stream.Position = 0;
            var sr = new StreamReader(stream, Encoding.Default);
            var json = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return json;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        [NonAction]
        public dynamic UploadLocalFile(string type, IFormFile file)
        {
            return _fileStorageServer.UploadLocalFile(type, file);
        }

        /// <summary>
        /// 根据类型获取目录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetPathByType(string type)
        {
            var result = _fileStorageServer.GetPathByType(type);
            return result;
        }
        #endregion
    }
}
