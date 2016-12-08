using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    [Serializable]
    public partial class WcStation {
        public Station Current { get; set; }

        public List<WcRoom> Rooms { get; set; }
    }
}
