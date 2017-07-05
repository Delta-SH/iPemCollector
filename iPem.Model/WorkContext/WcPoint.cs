using iPem.Core;
using System;

namespace iPem.Model {
    [Serializable]
    public partial class WcPoint {
        public Point Current { get; set; }

        public SubPoint SubPoint { get; set; }
    }
}
