using Newtonsoft.Json;
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
    public partial class ExForm : Form {
        private string _taskId;
        public ExForm(string taskId) {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(taskId)) throw new ArgumentNullException("taskId");
            this._taskId = taskId;
        }

        private void ExForm_Load(object sender, EventArgs e) {
            startDate.Value = DateTime.Today.AddDays(-1);
            endDate.Value = DateTime.Today;
        }

        private void exButton_Click(object sender, EventArgs e) {
            try {
                var start = startDate.Value;
                var end = endDate.Value;
                if (start >= end) {
                    MessageBox.Show("开始时间必须小于结束时间。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (end.Subtract(start).TotalDays > 180) {
                    MessageBox.Show("执行时段必须小于180天", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("您确定要执行计划任务吗？", "确认对话框", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK) {
                    var _registry = new Registry(Application.StartupPath);
                    var _orderId = this.GetOrderId(_taskId);
                    if (_orderId == OrderId.Null) throw new Exception("无效的计划任务");
                    _registry.SaveOrders(new List<OrderEntity> { new OrderEntity { Id = _orderId, Param = JsonConvert.SerializeObject(new ExTask { Id = _taskId, Start = start, End = end }) } });
                    MessageBox.Show("执行计划任务命令已下发", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception err) {
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private OrderId GetOrderId(string task) {
            if (this._taskId == "T001")
                return OrderId.ExTask001;
            if (this._taskId == "T002")
                return OrderId.ExTask002;
            if (this._taskId == "T003")
                return OrderId.ExTask003;
            if (this._taskId == "T004")
                return OrderId.ExTask004;
            if (this._taskId == "T005")
                return OrderId.ExTask005;
            if (this._taskId == "T006")
                return OrderId.ExTask006;
            return OrderId.Null;
        }
    }
}
