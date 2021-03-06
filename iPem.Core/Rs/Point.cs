﻿using System;

namespace iPem.Core {
    /// <summary>
    /// 信号信息
    /// </summary>
    [Serializable]
    public partial class Point {
        /// <summary>
        /// 信号编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 外部编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 信号名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 信号类型
        /// </summary>
        public EnmPoint Type { get; set; }

        /// <summary>
        /// 单位/描述
        /// </summary>
        public string UnitState { get; set; }

        /// <summary>
        /// 顺序号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 标准告警编号
        /// </summary>
        public string AlarmId { get; set; }

        /// <summary>
        /// 网管告警编号
        /// </summary>
        public string NMAlarmId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 逻辑分类
        /// </summary>
        public LogicType LogicType { get; set; }

        /// <summary>
        /// 告警对设备的影响
        /// </summary>
        public string DeviceEffect { get; set; }

        /// <summary>
        /// 告警对业务的影响
        /// </summary>
        public string BusiEffect { get; set; }

        /// <summary>
        /// 信号说明
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 信号解释
        /// </summary>
        public string Interpret { get; set; }

        /// <summary>
        /// 扩展设置
        /// </summary>
        public string ExtSet { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Enabled { get; set; }
    }
}
