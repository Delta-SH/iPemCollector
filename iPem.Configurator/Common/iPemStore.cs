﻿using System;

namespace iPem.Configurator {
    /// <summary>
    /// Represents an application store
    /// </summary>
    public static class iPemStore {
        /// <summary>
        /// Gets or sets the application identifier
        /// </summary>
        public static Guid Id {
            get { return new Guid("3f81204e-f755-4205-b488-97fb042d407e"); }
        }

        /// <summary>
        /// Gets or sets the application name
        /// </summary>
        public static string Name {
            get { return "PECSII数据处理服务"; }
        }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public static string Version {
            get { return "V1.3.5.0 Build180615"; }
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
            get { return "All Rights Reserved ©2011-2018"; }
        }
    }
}
