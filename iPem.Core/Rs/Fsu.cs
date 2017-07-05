using System;

namespace iPem.Core {
    /// <summary>
    /// FSU信息表
    /// </summary>
    [Serializable]
    public partial class Fsu : Device {
        /// <summary>
        /// 所属厂家编号
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 密码(MD5加密)
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// FTP用户名
        /// </summary>
        public string FtpUid { get; set; }

        /// <summary>
        /// FTP密码
        /// </summary>
        public string FtpPwd { get; set; }

        /// <summary>
        /// FTP文件路径
        /// </summary>
        public string FtpFilePath { get; set; }

        /// <summary>
        /// FTP操作权限
        /// </summary>
        public int FtpAuthority { get; set; }
    }
}
