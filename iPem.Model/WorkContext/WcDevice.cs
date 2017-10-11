using iPem.Core;
using iPem.Core.Rs;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    [Serializable]
    public partial class WcDevice {
        public WcDevice(Device current) {
            this.Current = current;
        }

        public Device Current { get; private set; }

        public List<Signal> Signals { get; set; }
    }
}
