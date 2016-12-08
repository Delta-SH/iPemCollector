using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    [Serializable]
    public partial class WcProtocol {
        public Protocol Current { get; set; }

        public List<Point> Points { get; set; }
    }
}
