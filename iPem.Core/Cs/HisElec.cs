using System;

namespace iPem.Core {
    [Serializable]
    public partial class HisElec {
        public string Id { get; set; }

        public EnmOrganization Type { get; set; }

        public EnmFormula FormulaType { get; set; }

        public DateTime Period { get; set; }

        public double Value { get; set; }
    }
}
