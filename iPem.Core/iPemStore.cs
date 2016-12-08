using System;

namespace iPem.Core {
    /// <summary>
    /// Represents an application store
    /// </summary>
    public static class iPemStore {
        /// <summary>
        /// Gets or sets the application identifier
        /// </summary>
        public static Guid Id {
            get { return new Guid("0121F3F2-AB54-4105-A6DE-C645D9DA5C8B"); }
        }

        /// <summary>
        /// Gets or sets the application name
        /// </summary>
        public static string Name {
            get { return "智能监控数据处理服务"; }
        }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public static string Version {
            get { return "V1.0.0 Build161118"; }
        }

        /// <summary>
        /// Gets or sets the company name
        /// </summary>
        public static string CompanyName {
            get { return "Delta GreenTech(China) Co., Ltd."; }
        }

        /// <summary>
        /// Gets or sets the copyright
        /// </summary>
        public static string Copyright {
            get { return "All Rights Reserved ©2016"; }
        }
    }
}
