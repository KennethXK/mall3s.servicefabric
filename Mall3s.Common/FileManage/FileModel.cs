using System;

namespace Mall3s.Common.FileManage
{
    /// <summary>
    /// 附件模型
    /// 版 本：V3.0.0
    /// 版 权：Mall3s开发
    /// 作 者：Mall3s开发平台组
    /// </summary>
    public class FileModel
    {
        public string FileId { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public DateTime FileTime { get; set; }

        public string FileState { get; set; }

        public string FileType { get; set; }
    }


    public class FileUploadResult
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
