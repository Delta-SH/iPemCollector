using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iPem.Register {
    public partial class Main : Form {

        public Main() {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e) {
            try {
                expirepicker.Value = DateTime.Today.AddMonths(3);
                ucount.Value = 10;
                scount.Value = 10;
                copybutton.Enabled = false;
                exportbutton.Enabled = false;
            } catch (Exception err) {
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void makebutton_Click(object sender, EventArgs e) {
            try {
                if (String.IsNullOrWhiteSpace(customer.Text.Trim())) {
                    customer.Focus();
                    MessageBox.Show("请输入客户名称。", "系统警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (expirepicker.Value <= DateTime.Now) {
                    expirepicker.Focus();
                    MessageBox.Show("有效日期无效。", "系统警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codes = new List<string>();
                foreach (var line in device.Lines) {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var code = line.Trim();
                    if (!codes.Contains(code))
                        codes.Add(code);
                }

                if (codes.Count == 0) {
                    device.Focus();
                    MessageBox.Show("请输入授权设备。", "系统警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (var code in codes) {
                    if (!Regex.IsMatch(code, @"^[0-9a-f]{32}$", RegexOptions.IgnoreCase)) {
                        MessageBox.Show(string.Format("机器标识码({0})格式错误。", code), "系统警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var _contents = new List<string>();
                _contents.Add(string.Join(",", codes));
                _contents.Add(customer.Text.Replace(Common.Separator, "|").Trim());
                _contents.Add(expirepicker.Value.Subtract(new DateTime(2018, 3, 25)).Days.ToString());
                _contents.Add(((int)ucount.Value).ToString());
                _contents.Add(((int)scount.Value).ToString());
                
                var functions = new List<string>();
                if (function1.Checked) functions.Add(function1.Tag.ToString());
                if (function2.Checked) functions.Add(function2.Tag.ToString());
                if (function3.Checked) functions.Add(function3.Tag.ToString());
                if (function4.Checked) functions.Add(function4.Tag.ToString());
                if (function5.Checked) functions.Add(function5.Tag.ToString());
                if (function6.Checked) functions.Add(function6.Tag.ToString());
                _contents.Add(string.Join(",", functions));

                var RSAKey = Common.GetRSAKey();
                coder.Text = RSACryptoProvider.SectionEncrypt(string.Join(Common.Separator, _contents), RSAKey.Public);
                copybutton.Enabled = true;
                exportbutton.Enabled = true;
            } catch (Exception err) {
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void copybutton_Click(object sender, EventArgs e) {
            try {
                if (!String.IsNullOrWhiteSpace(coder.Text)) {
                    Clipboard.SetDataObject(coder.Text, true);
                    MessageBox.Show("注册码已复制到剪贴板。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception err) {
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportbutton_Click(object sender, EventArgs e) {
            try {
                if (!String.IsNullOrWhiteSpace(coder.Text)) {
                    if (registerFileDialog.ShowDialog() == DialogResult.OK) {
                        var regFile = new FileInfo(registerFileDialog.FileName);
                        using (var sw = regFile.CreateText()) {
                            sw.WriteLine(coder.Text);
                            sw.Close();
                        }

                        MessageBox.Show("注册码导出成功。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            } catch (Exception err) {
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
