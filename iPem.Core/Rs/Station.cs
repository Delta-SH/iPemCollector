using System;

namespace iPem.Core {
    /// <summary>
    /// 站点信息
    /// </summary>
    [Serializable]
    public partial class Station {
        /// <summary>
        /// 站点编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 外部编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 站点类型
        /// </summary>
        public StationType Type { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// 海拔标高
        /// </summary>
        public string Altitude { get; set; }

        /// <summary>
        /// 市电容量
        /// </summary>
        public string CityElecCap { get; set; }

        /// <summary>
        /// 市电路数
        /// </summary>
        public int CityElectNumber { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Enabled { get; set; }
    }
}
