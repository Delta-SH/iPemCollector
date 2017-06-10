using iPem.Core;
using System;

namespace iPem.Model {
    public partial class ParamEntity {
        public ParamId Id { get; set; }

        public string Value { get; set; }

        public DateTime Time { get; set; }
    }
}
