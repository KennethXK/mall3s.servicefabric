using Mall3s.Common.Configuration;
using Mall3s.Common.Enum;
using Mall3s.Common.FileManage;
using Mall3s.DataEncryption;
using Mall3s.Dependency;
using Mall3s.FriendlyException;
using Mall3s.RemoteRequest.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnceMi.AspNetCore.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Mall3s.Common.FileStorage
{
    /// <summary>
    /// 文件存储服务辅助类
    /// </summary>
    public class FileStorageServer : IFileStorageServer, IScoped
    {
        private static IOSSServiceFactory _oSSServiceFactory;

        public FileStorageServer(IOSSServiceFactory oSSServiceFactory)
        {
            _oSSServiceFactory = oSSServiceFactory;
        }
        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="type">图片类型 </param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ </param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadFile(string type, string fileName)
        {
            var filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
            var serverAddr = filePath.Replace("\\", "/");
            return await DownloadFileByType(serverAddr, fileName);

        }


        /// <summary>
        /// 获取下载加密链接(xxx|fileName|type|false)格式
        /// </summary>
        /// <param name="fileId">文件标识</param>
        /// <param name="type">文件类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="local">是否是本地存储（本地存储，则直接从本地文件夹读取）</param>
        /// <returns></returns>
        public string GetDownloadUrlEncryption(string fileId, string type, string fileName, bool local = false)
        {
            var url = fileId + "|" + fileName + "|" + type + "|" + local;
            var encryptStr = DESCEncryption.Encrypt(url, "Mall3s");
            return encryptStr;
        }

        /// <summary>
        /// 下载文件链接(根据加密链接字符串)
        /// </summary>
        public async Task<dynamic> DownloadFile([FromQuery] string encryption)
        {
            var decryptStr = DESCEncryption.Decrypt(encryption, "Mall3s");
            var paramsList = decryptStr.Split("|").ToList();
            if (paramsList.Count > 0)
            {
                var fileName = paramsList.Count > 1 ? paramsList[1] : "";
                string type = paramsList.Count > 2 ? paramsList[2] : "";
                var local = paramsList.Count > 3 ? Convert.ToBoolean(paramsList[3]) : false;
                var filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
                var fileDownloadName = fileName.Replace(GetPathByType(type), "");
                return await DownloadFileByType(filePath, fileDownloadName, local);
            }
            else
            {
                throw Mall3sException.Oh(ErrorCode.D8000);
            }
        }



        /// <summary>
        /// 上传文件/图片(返回文件名)
        /// </summary>
        /// <returns></returns>
        public async Task<FileUploadResult> Uploader(string type, IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!AllowFileType(fileType, type))
                throw Mall3sException.Oh(ErrorCode.D1800);
            var _filePath = GetPathByType(type);
            var _fileName = DateTime.Now.ToString("yyyyMMdd") + "_" + YitIdHelper.NextId().ToString() + Path.GetExtension(file.FileName);
            await UploadFileByType(file, _filePath, _fileName);
            return new FileUploadResult() { Name = _fileName };
        }

        #region 上传本地文件
        /// <summary>
        /// 上传本地文件(返回文件名+路径）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        public FileUploadResult UploadLocalFile(string type, IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!AllowFileType(fileType, type))
                throw Mall3sException.Oh(ErrorCode.D1800);
            var _filePath = App.WebHostEnvironment.WebRootPath + GetPathByType(type);

            var _fileName = file.FileName;
            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);
            using (var stream = File.Create(Path.Combine(_filePath, _fileName)))
            {
                file.CopyTo(stream);
            }

            return new FileUploadResult { Name = _fileName, Url = _filePath };
        }
        #endregion

        #region 根据存储类型上传文件
        /// <summary>
        /// 根据存储类型上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task UploadFileByType(IFormFile file, string filePath, string fileName)
        {
            try
            {
                var bucketName = KeyVariable.BucketName;
                var fileStoreType = KeyVariable.FileStoreType;
                var uploadPath = Path.Combine(filePath, fileName);
                var stream = file.OpenReadStream();
                switch (fileStoreType)
                {
                    case "minio":
                        await _oSSServiceFactory.Create().PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                    case "aliyun-oss":
                        await _oSSServiceFactory.Create("Aliyun").PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                    case "tencent-cos":
                        await _oSSServiceFactory.Create("QCloud").PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                    case "qiniu-oss":
                        await _oSSServiceFactory.Create().PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                    default:
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);
                        using (var stream4 = File.Create(uploadPath))
                        {
                            await file.CopyToAsync(stream4);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw Mall3sException.Oh(ErrorCode.D8003);
            }
        }
        #endregion

        #region 复制本地文件到云存储
        /// <summary>
        /// 复制本地文件到云存储
        /// </summary>
        /// <param name="fileFullName">本地完整路径</param>
        /// <param name="uploadPath">文件相对路径</param>
        /// <returns></returns>
        public async Task<bool> CopyFileByType(string fileFullName, string uploadPath)
        {
            //本地文件拷贝复制到七牛云提供下载链接
            try
            {
                var bucketName = KeyVariable.BucketName;
                var fileStoreType = KeyVariable.FileStoreType;
                var result = false;
                switch (fileStoreType)
                {
                    case "minio":
                        result = await _oSSServiceFactory.Create().PutObjectAsync(bucketName, uploadPath, fileFullName);
                        break;
                    case "aliyun-oss":
                        result = await _oSSServiceFactory.Create().PutObjectAsync(bucketName, uploadPath, fileFullName);
                        break;
                    case "tencent-cos":
                        result = await _oSSServiceFactory.Create().PutObjectAsync(bucketName, uploadPath, fileFullName);
                        break;
                    case "qiniu-oss":

                        result = await _oSSServiceFactory.Create().PutObjectAsync(bucketName, uploadPath, fileFullName);
                        break;
                    default:

                        break;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 根据存储类型下载文件
        /// <summary>
        /// 根据存储类型下载文件
        /// </summary>
        /// <param name="filePath">完整路径</param>
        /// <param name="fileDownLoadName">本地文件名</param>
        /// <returns></returns>
        private async Task<FileStreamResult> DownloadFileByType(string filePath, string fileDownLoadName, bool local = false)
        {
            try
            {
                var bucketName = KeyVariable.BucketName;
                var fileStoreType = KeyVariable.FileStoreType;

                var serverAddr = filePath.Replace("\\", "/");
                if (local || filePath.Contains("\\")) //如果是本地文件且路径是本地格式
                {
                    fileStoreType = "local";
                }
                //上传云服务器文件是不加根目录/前缀的
                if (fileStoreType != "local" && serverAddr.StartsWith("/"))
                {
                    serverAddr = serverAddr.Substring(1);
                }
                switch (fileStoreType)
                {
                    case "minio":
                        var url1 = await _oSSServiceFactory.Create().PresignedGetObjectAsync(bucketName, serverAddr, 86400);
                        var stream1 = await url1.GetAsStreamAsync();
                        return new FileStreamResult(stream1, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    case "aliyun-oss":
                        var url2 = await _oSSServiceFactory.Create("Aliyun").PresignedGetObjectAsync(bucketName, serverAddr, 86400);
                        var stream2 = await url2.GetAsStreamAsync();
                        return new FileStreamResult(stream2, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    case "tencent-cos":
                        var url3 = await _oSSServiceFactory.Create("QCloud").PresignedGetObjectAsync(bucketName, serverAddr, 86400);
                        var stream3 = await url3.GetAsStreamAsync();
                        return new FileStreamResult(stream3, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    case "qiniu-oss":
                        var url4 = await _oSSServiceFactory.Create().PresignedGetObjectAsync(bucketName, serverAddr, 86400);
                        var stream4 = await url4.GetAsStreamAsync();
                        return new FileStreamResult(stream4, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    default:
                        return new FileStreamResult(new FileStream(filePath, FileMode.Open), "application/octet-stream") { FileDownloadName = fileDownLoadName };
                }
            }
            catch (Exception ex)
            {
                throw Mall3sException.Oh(ErrorCode.D8003);
            }
        }
        #endregion


        /// <summary>
        /// 根据类型获取文件存储路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public string GetPathByType(string type)
        {
            switch (type)
            {
                case "userAvatar":
                    return FileVariable.UserAvatarFilePath;
                case "mail":
                    return FileVariable.EmailFilePath;
                case "IM":
                    return FileVariable.IMContentFilePath;
                case "weixin":
                    return FileVariable.MPMaterialFilePath;
                case "workFlow":
                    return FileVariable.SystemFilePath;
                case "annex":
                    return FileVariable.SystemFilePath;
                case "annexpic":
                    return FileVariable.SystemFilePath;
                case "document":
                    return FileVariable.DocumentFilePath;
                //case "dataBackup":
                //    return ConfigurationFileConst.DataBackupFilePath;
                case "preview":
                    return FileVariable.DocumentPreviewFilePath;
                case "screenShot":
                case "banner":
                case "bg":
                case "border":
                case "source":
                    return FileVariable.BiVisualPath;
                case "template":
                    return FileVariable.TemplateFilePath;
                case "codeGenerator":
                    return FileVariable.GenerateCodePath;
                default:
                    return FileVariable.TemporaryFilePath;
            }
        }

        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public bool AllowFileType(string fileExtension, string type)
        {
            var allowExtension = KeyVariable.AllowUploadFileType;
            if (type.Equals("weixin"))
            {
                allowExtension = KeyVariable.WeChatUploadFileType;
            }
            var isExist = allowExtension.Find(a => a == fileExtension.ToLower());
            if (!string.IsNullOrEmpty(isExist))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <returns></returns>
        public bool AllowImageType(string fileExtension)
        {
            var allowExtension = KeyVariable.AllowImageType;
            var isExist = allowExtension.Find(a => a == fileExtension.ToLower());
            if (!string.IsNullOrEmpty(isExist))
                return true;
            else
                return false;
        }

    }
}
