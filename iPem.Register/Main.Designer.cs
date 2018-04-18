namespace iPem.Register {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.MainGroupBox = new System.Windows.Forms.GroupBox();
            this.RegisterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.customerlbl = new System.Windows.Forms.Label();
            this.customer = new System.Windows.Forms.TextBox();
            this.expirelbl = new System.Windows.Forms.Label();
            this.ucountlbl = new System.Windows.Forms.Label();
            this.expirepicker = new System.Windows.Forms.DateTimePicker();
            this.ucount = new System.Windows.Forms.NumericUpDown();
            this.scountlbl = new System.Windows.Forms.Label();
            this.scount = new System.Windows.Forms.NumericUpDown();
            this.functionlbl = new System.Windows.Forms.Label();
            this.devicelbl = new System.Windows.Forms.Label();
            this.coderlbl = new System.Windows.Forms.Label();
            this.FunctionLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.function1 = new System.Windows.Forms.CheckBox();
            this.function2 = new System.Windows.Forms.CheckBox();
            this.function4 = new System.Windows.Forms.CheckBox();
            this.function5 = new System.Windows.Forms.CheckBox();
            this.device = new System.Windows.Forms.TextBox();
            this.coder = new System.Windows.Forms.TextBox();
            this.function3 = new System.Windows.Forms.CheckBox();
            this.function6 = new System.Windows.Forms.CheckBox();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.copybutton = new System.Windows.Forms.Button();
            this.makebutton = new System.Windows.Forms.Button();
            this.exportbutton = new System.Windows.Forms.Button();
            this.registerFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.MainLayoutPanel.SuspendLayout();
            this.MainGroupBox.SuspendLayout();
            this.RegisterLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scount)).BeginInit();
            this.FunctionLayoutPanel.SuspendLayout();
            this.BottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayoutPanel
            // 
            this.MainLayoutPanel.ColumnCount = 1;
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.Controls.Add(this.MainGroupBox, 0, 0);
            this.MainLayoutPanel.Controls.Add(this.BottomPanel, 0, 1);
            this.MainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainLayoutPanel.Name = "MainLayoutPanel";
            this.MainLayoutPanel.Padding = new System.Windows.Forms.Padding(5);
            this.MainLayoutPanel.RowCount = 2;
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainLayoutPanel.Size = new System.Drawing.Size(434, 461);
            this.MainLayoutPanel.TabIndex = 0;
            // 
            // MainGroupBox
            // 
            this.MainGroupBox.Controls.Add(this.RegisterLayout);
            this.MainGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainGroupBox.Location = new System.Drawing.Point(8, 9);
            this.MainGroupBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MainGroupBox.Name = "MainGroupBox";
            this.MainGroupBox.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.MainGroupBox.Size = new System.Drawing.Size(418, 393);
            this.MainGroupBox.TabIndex = 0;
            this.MainGroupBox.TabStop = false;
            this.MainGroupBox.Text = "注册信息";
            // 
            // RegisterLayout
            // 
            this.RegisterLayout.ColumnCount = 3;
            this.RegisterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.RegisterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.RegisterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RegisterLayout.Controls.Add(this.customerlbl, 0, 0);
            this.RegisterLayout.Controls.Add(this.customer, 2, 0);
            this.RegisterLayout.Controls.Add(this.expirelbl, 0, 2);
            this.RegisterLayout.Controls.Add(this.ucountlbl, 0, 4);
            this.RegisterLayout.Controls.Add(this.expirepicker, 2, 2);
            this.RegisterLayout.Controls.Add(this.ucount, 2, 4);
            this.RegisterLayout.Controls.Add(this.scountlbl, 0, 6);
            this.RegisterLayout.Controls.Add(this.scount, 2, 6);
            this.RegisterLayout.Controls.Add(this.functionlbl, 0, 8);
            this.RegisterLayout.Controls.Add(this.devicelbl, 0, 10);
            this.RegisterLayout.Controls.Add(this.coderlbl, 0, 12);
            this.RegisterLayout.Controls.Add(this.FunctionLayoutPanel, 2, 8);
            this.RegisterLayout.Controls.Add(this.device, 2, 10);
            this.RegisterLayout.Controls.Add(this.coder, 2, 12);
            this.RegisterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegisterLayout.Location = new System.Drawing.Point(8, 22);
            this.RegisterLayout.Name = "RegisterLayout";
            this.RegisterLayout.RowCount = 13;
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RegisterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.RegisterLayout.Size = new System.Drawing.Size(402, 363);
            this.RegisterLayout.TabIndex = 0;
            // 
            // customerlbl
            // 
            this.customerlbl.AutoSize = true;
            this.customerlbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customerlbl.Location = new System.Drawing.Point(3, 0);
            this.customerlbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.customerlbl.Name = "customerlbl";
            this.customerlbl.Size = new System.Drawing.Size(97, 25);
            this.customerlbl.TabIndex = 0;
            this.customerlbl.Text = "客户名称(&U)";
            this.customerlbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // customer
            // 
            this.customer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customer.Location = new System.Drawing.Point(105, 2);
            this.customer.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.customer.MaxLength = 20;
            this.customer.Name = "customer";
            this.customer.Size = new System.Drawing.Size(297, 23);
            this.customer.TabIndex = 1;
            // 
            // expirelbl
            // 
            this.expirelbl.AutoSize = true;
            this.expirelbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expirelbl.Location = new System.Drawing.Point(3, 30);
            this.expirelbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.expirelbl.Name = "expirelbl";
            this.expirelbl.Size = new System.Drawing.Size(97, 25);
            this.expirelbl.TabIndex = 0;
            this.expirelbl.Text = "有效日期(&E)";
            this.expirelbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ucountlbl
            // 
            this.ucountlbl.AutoSize = true;
            this.ucountlbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucountlbl.Location = new System.Drawing.Point(3, 60);
            this.ucountlbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ucountlbl.Name = "ucountlbl";
            this.ucountlbl.Size = new System.Drawing.Size(97, 25);
            this.ucountlbl.TabIndex = 0;
            this.ucountlbl.Text = "用户数量(&N)";
            this.ucountlbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // expirepicker
            // 
            this.expirepicker.CustomFormat = "yyyy年MM月dd日";
            this.expirepicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expirepicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.expirepicker.Location = new System.Drawing.Point(105, 32);
            this.expirepicker.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.expirepicker.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.expirepicker.MinDate = new System.DateTime(2018, 1, 1, 0, 0, 0, 0);
            this.expirepicker.Name = "expirepicker";
            this.expirepicker.Size = new System.Drawing.Size(297, 23);
            this.expirepicker.TabIndex = 2;
            this.expirepicker.Value = new System.DateTime(2018, 1, 1, 0, 0, 0, 0);
            // 
            // ucount
            // 
            this.ucount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucount.Location = new System.Drawing.Point(105, 62);
            this.ucount.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.ucount.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.ucount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ucount.Name = "ucount";
            this.ucount.Size = new System.Drawing.Size(297, 23);
            this.ucount.TabIndex = 3;
            this.ucount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // scountlbl
            // 
            this.scountlbl.AutoSize = true;
            this.scountlbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scountlbl.Location = new System.Drawing.Point(3, 90);
            this.scountlbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.scountlbl.Name = "scountlbl";
            this.scountlbl.Size = new System.Drawing.Size(97, 25);
            this.scountlbl.TabIndex = 0;
            this.scountlbl.Text = "站点数量(&S)";
            this.scountlbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scount
            // 
            this.scount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scount.Location = new System.Drawing.Point(105, 92);
            this.scount.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.scount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.scount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.scount.Name = "scount";
            this.scount.Size = new System.Drawing.Size(297, 23);
            this.scount.TabIndex = 4;
            this.scount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // functionlbl
            // 
            this.functionlbl.AutoSize = true;
            this.functionlbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.functionlbl.Location = new System.Drawing.Point(3, 120);
            this.functionlbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.functionlbl.Name = "functionlbl";
            this.functionlbl.Size = new System.Drawing.Size(97, 50);
            this.functionlbl.TabIndex = 0;
            this.functionlbl.Text = "软件功能(&F)";
            this.functionlbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // devicelbl
            // 
            this.devicelbl.AutoSize = true;
            this.devicelbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicelbl.Location = new System.Drawing.Point(3, 175);
            this.devicelbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.devicelbl.Name = "devicelbl";
            this.devicelbl.Size = new System.Drawing.Size(97, 60);
            this.devicelbl.TabIndex = 0;
            this.devicelbl.Text = "授权设备(&D)";
            this.devicelbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // coderlbl
            // 
            this.coderlbl.AutoSize = true;
            this.coderlbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.coderlbl.Location = new System.Drawing.Point(3, 240);
            this.coderlbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.coderlbl.Name = "coderlbl";
            this.coderlbl.Size = new System.Drawing.Size(97, 123);
            this.coderlbl.TabIndex = 0;
            this.coderlbl.Text = "注册码(&R)";
            this.coderlbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FunctionLayoutPanel
            // 
            this.FunctionLayoutPanel.ColumnCount = 3;
            this.FunctionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.FunctionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.FunctionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.FunctionLayoutPanel.Controls.Add(this.function1, 0, 0);
            this.FunctionLayoutPanel.Controls.Add(this.function2, 1, 0);
            this.FunctionLayoutPanel.Controls.Add(this.function4, 0, 2);
            this.FunctionLayoutPanel.Controls.Add(this.function5, 1, 2);
            this.FunctionLayoutPanel.Controls.Add(this.function3, 2, 0);
            this.FunctionLayoutPanel.Controls.Add(this.function6, 2, 2);
            this.FunctionLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FunctionLayoutPanel.Location = new System.Drawing.Point(105, 120);
            this.FunctionLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FunctionLayoutPanel.Name = "FunctionLayoutPanel";
            this.FunctionLayoutPanel.RowCount = 3;
            this.FunctionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.FunctionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.FunctionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.FunctionLayoutPanel.Size = new System.Drawing.Size(297, 50);
            this.FunctionLayoutPanel.TabIndex = 5;
            // 
            // function1
            // 
            this.function1.AutoSize = true;
            this.function1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.function1.Location = new System.Drawing.Point(0, 0);
            this.function1.Margin = new System.Windows.Forms.Padding(0);
            this.function1.Name = "function1";
            this.function1.Size = new System.Drawing.Size(98, 22);
            this.function1.TabIndex = 0;
            this.function1.Tag = "1";
            this.function1.Text = "虚拟界面";
            this.function1.UseVisualStyleBackColor = true;
            // 
            // function2
            // 
            this.function2.AutoSize = true;
            this.function2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.function2.Location = new System.Drawing.Point(98, 0);
            this.function2.Margin = new System.Windows.Forms.Padding(0);
            this.function2.Name = "function2";
            this.function2.Size = new System.Drawing.Size(98, 22);
            this.function2.TabIndex = 0;
            this.function2.Tag = "2";
            this.function2.Text = "系统报表";
            this.function2.UseVisualStyleBackColor = true;
            // 
            // function4
            // 
            this.function4.AutoSize = true;
            this.function4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.function4.Location = new System.Drawing.Point(0, 27);
            this.function4.Margin = new System.Windows.Forms.Padding(0);
            this.function4.Name = "function4";
            this.function4.Size = new System.Drawing.Size(98, 23);
            this.function4.TabIndex = 0;
            this.function4.Tag = "4";
            this.function4.Text = "视频功能";
            this.function4.UseVisualStyleBackColor = true;
            // 
            // function5
            // 
            this.function5.AutoSize = true;
            this.function5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.function5.Location = new System.Drawing.Point(98, 27);
            this.function5.Margin = new System.Windows.Forms.Padding(0);
            this.function5.Name = "function5";
            this.function5.Size = new System.Drawing.Size(98, 23);
            this.function5.TabIndex = 0;
            this.function5.Tag = "5";
            this.function5.Text = "能耗功能";
            this.function5.UseVisualStyleBackColor = true;
            // 
            // device
            // 
            this.device.Dock = System.Windows.Forms.DockStyle.Fill;
            this.device.Location = new System.Drawing.Point(105, 175);
            this.device.Margin = new System.Windows.Forms.Padding(0);
            this.device.Multiline = true;
            this.device.Name = "device";
            this.device.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.device.Size = new System.Drawing.Size(297, 60);
            this.device.TabIndex = 5;
            // 
            // coder
            // 
            this.coder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.coder.Location = new System.Drawing.Point(105, 240);
            this.coder.Margin = new System.Windows.Forms.Padding(0);
            this.coder.Multiline = true;
            this.coder.Name = "coder";
            this.coder.ReadOnly = true;
            this.coder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.coder.Size = new System.Drawing.Size(297, 123);
            this.coder.TabIndex = 6;
            // 
            // function3
            // 
            this.function3.AutoSize = true;
            this.function3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.function3.Location = new System.Drawing.Point(196, 0);
            this.function3.Margin = new System.Windows.Forms.Padding(0);
            this.function3.Name = "function3";
            this.function3.Size = new System.Drawing.Size(101, 22);
            this.function3.TabIndex = 0;
            this.function3.Tag = "3";
            this.function3.Text = "系统指标";
            this.function3.UseVisualStyleBackColor = true;
            // 
            // function6
            // 
            this.function6.AutoSize = true;
            this.function6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.function6.Location = new System.Drawing.Point(196, 27);
            this.function6.Margin = new System.Windows.Forms.Padding(0);
            this.function6.Name = "function6";
            this.function6.Size = new System.Drawing.Size(101, 23);
            this.function6.TabIndex = 0;
            this.function6.Tag = "6";
            this.function6.Text = "电池功能";
            this.function6.UseVisualStyleBackColor = true;
            // 
            // BottomPanel
            // 
            this.BottomPanel.Controls.Add(this.exportbutton);
            this.BottomPanel.Controls.Add(this.makebutton);
            this.BottomPanel.Controls.Add(this.copybutton);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomPanel.Location = new System.Drawing.Point(8, 406);
            this.BottomPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(418, 50);
            this.BottomPanel.TabIndex = 1;
            // 
            // copybutton
            // 
            this.copybutton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.copybutton.Location = new System.Drawing.Point(0, 10);
            this.copybutton.Margin = new System.Windows.Forms.Padding(0);
            this.copybutton.Name = "copybutton";
            this.copybutton.Size = new System.Drawing.Size(100, 30);
            this.copybutton.TabIndex = 0;
            this.copybutton.Text = "复制注册码(&C)";
            this.copybutton.UseVisualStyleBackColor = true;
            this.copybutton.Click += new System.EventHandler(this.copybutton_Click);
            // 
            // makebutton
            // 
            this.makebutton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.makebutton.Location = new System.Drawing.Point(318, 10);
            this.makebutton.Margin = new System.Windows.Forms.Padding(0);
            this.makebutton.Name = "makebutton";
            this.makebutton.Size = new System.Drawing.Size(100, 30);
            this.makebutton.TabIndex = 0;
            this.makebutton.Text = "生成注册码(&G)";
            this.makebutton.UseVisualStyleBackColor = true;
            this.makebutton.Click += new System.EventHandler(this.makebutton_Click);
            // 
            // exportbutton
            // 
            this.exportbutton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.exportbutton.Location = new System.Drawing.Point(110, 10);
            this.exportbutton.Margin = new System.Windows.Forms.Padding(0);
            this.exportbutton.Name = "exportbutton";
            this.exportbutton.Size = new System.Drawing.Size(100, 30);
            this.exportbutton.TabIndex = 0;
            this.exportbutton.Text = "导出注册码(&T)";
            this.exportbutton.UseVisualStyleBackColor = true;
            this.exportbutton.Click += new System.EventHandler(this.exportbutton_Click);
            // 
            // registerFileDialog
            // 
            this.registerFileDialog.DefaultExt = "key";
            this.registerFileDialog.FileName = "register.key";
            this.registerFileDialog.Filter = "授权文件|*.key|所有文件|*.*";
            this.registerFileDialog.Title = "文件另存为";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 461);
            this.Controls.Add(this.MainLayoutPanel);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "动力环境集中监控平台 - 软件注册机";
            this.Load += new System.EventHandler(this.Main_Load);
            this.MainLayoutPanel.ResumeLayout(false);
            this.MainGroupBox.ResumeLayout(false);
            this.RegisterLayout.ResumeLayout(false);
            this.RegisterLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scount)).EndInit();
            this.FunctionLayoutPanel.ResumeLayout(false);
            this.FunctionLayoutPanel.PerformLayout();
            this.BottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private System.Windows.Forms.GroupBox MainGroupBox;
        private System.Windows.Forms.TableLayoutPanel RegisterLayout;
        private System.Windows.Forms.Label customerlbl;
        private System.Windows.Forms.TextBox customer;
        private System.Windows.Forms.Label expirelbl;
        private System.Windows.Forms.Label ucountlbl;
        private System.Windows.Forms.DateTimePicker expirepicker;
        private System.Windows.Forms.NumericUpDown ucount;
        private System.Windows.Forms.Label scountlbl;
        private System.Windows.Forms.NumericUpDown scount;
        private System.Windows.Forms.Label functionlbl;
        private System.Windows.Forms.Label devicelbl;
        private System.Windows.Forms.Label coderlbl;
        private System.Windows.Forms.TableLayoutPanel FunctionLayoutPanel;
        private System.Windows.Forms.CheckBox function1;
        private System.Windows.Forms.CheckBox function2;
        private System.Windows.Forms.CheckBox function4;
        private System.Windows.Forms.CheckBox function5;
        private System.Windows.Forms.TextBox device;
        private System.Windows.Forms.TextBox coder;
        private System.Windows.Forms.CheckBox function3;
        private System.Windows.Forms.CheckBox function6;
        private System.Windows.Forms.Panel BottomPanel;
        private System.Windows.Forms.Button copybutton;
        private System.Windows.Forms.Button exportbutton;
        private System.Windows.Forms.Button makebutton;
        private System.Windows.Forms.SaveFileDialog registerFileDialog;
    }
}