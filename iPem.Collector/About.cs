using iPem.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace iPem.Collector {
    partial class About : Form {
        public About() {
            InitializeComponent();
            this.labelProductName.Text = iPemStore.Name;
            this.labelVersion.Text = String.Format("版本 {0}", iPemStore.Version);
            this.labelCompanyName.Text = iPemStore.CompanyName;
            this.labelCopyright.Text = iPemStore.Copyright;
        }
    }
}
