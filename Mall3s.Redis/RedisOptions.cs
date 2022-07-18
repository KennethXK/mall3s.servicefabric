using System;
using System.Collections.Generic;
using System.Text;

namespace Mall3s.Core.Redis
{
    public class RedisOptions
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public int Defaultdatabase { get; set; }
    }
}