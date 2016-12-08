using System;

namespace iPem.Core {
    [Serializable]
    public partial class DivSwitElecSour : Device {
        public double RatedOutputVolt { get; set; }

        public string MoniModuleModel { get; set; }

        public string ExisRModuleCount { get; set; }

        public string RModuleModel { get; set; }

        public int RModuleRatedWorkVolt { get; set; }

        public string SingRModuleRatedOPCap { get; set; }

        public string SingGBattGFuseCap { get; set; }

        public int BattGFuseGNumber { get; set; }

        public string OPDistBoardModel { get; set; }

        public int OPDistBoardNumber { get; set; }
    }
}
