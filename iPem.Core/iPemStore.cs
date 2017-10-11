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
            get { return new Guid("0121f3f2-ab54-4105-a6de-c645d9da5c8b"); }
        }

        /// <summary>
        /// Gets or sets the application name
        /// </summary>
        public static string Name {
            get { return "PECS-II数据处理服务"; }
        }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public static string Version {
            get { return "V1.1.0 Build170925"; }
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
            get { return "All Rights Reserved ©2011-2017"; }
        }
    }
}
