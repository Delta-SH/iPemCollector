using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// 数据库信息
    /// </summary>
    public partial class DbEntity {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType Type { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 数据库端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 登录用户
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Db { get; set; }
    }
}
