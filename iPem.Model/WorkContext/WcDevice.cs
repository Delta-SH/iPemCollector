﻿using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    [Serializable]
    public partial class WcDevice {
        public Device Current { get; set; }

        public List<WcPoint> Points { get; set; }
    }
}
