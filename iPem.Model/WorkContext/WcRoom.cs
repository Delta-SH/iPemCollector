using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    [Serializable]
    public partial class WcRoom {
        public Room Current { get; set; }

        public List<WcFsu> Fsus { get; set; }

        public List<WcDevice> Devices { get; set; }
    }
}
