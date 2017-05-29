using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iPem.Configurator {
    public partial class About : Form {
        public About() {
            InitializeComponent();
            this.labelProductName.Text = iPemStore.Name;
            this.labelVersion.Text = String.Format("版本 {0}", iPemStore.Version);
            this.labelCompanyName.Text = iPemStore.CompanyName;
            this.labelCopyright.Text = iPemStore.Copyright;
        }
    }
}
