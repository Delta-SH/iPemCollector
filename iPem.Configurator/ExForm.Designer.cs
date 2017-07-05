namespace iPem.Configurator {
    partial class ExForm {
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
            this.MainContainer = new System.Windows.Forms.TableLayoutPanel();
            this.startDateLabel = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.exButton = new System.Windows.Forms.Button();
            this.endDateLabel = new System.Windows.Forms.Label();
            this.startDate = new System.Windows.Forms.DateTimePicker();
            this.endDate = new System.Windows.Forms.DateTimePicker();
            this.MainContainer.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainContainer
            // 
            this.MainContainer.ColumnCount = 3;
            this.MainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.MainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.MainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainContainer.Controls.Add(this.startDateLabel, 0, 0);
            this.MainContainer.Controls.Add(this.bottomPanel, 0, 5);
            this.MainContainer.Controls.Add(this.endDateLabel, 0, 2);
            this.MainContainer.Controls.Add(this.startDate, 2, 0);
            this.MainContainer.Controls.Add(this.endDate, 2, 2);
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.Location = new System.Drawing.Point(0, 0);
            this.MainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.MainContainer.Name = "MainContainer";
            this.MainContainer.Padding = new System.Windows.Forms.Padding(10);
            this.MainContainer.RowCount = 6;
            this.MainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.MainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.MainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.MainContainer.Size = new System.Drawing.Size(284, 141);
            this.MainContainer.TabIndex = 0;
            // 
            // startDateLabel
            // 
            this.startDateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startDateLabel.Location = new System.Drawing.Point(10, 10);
            this.startDateLabel.Margin = new System.Windows.Forms.Padding(0);
            this.startDateLabel.Name = "startDateLabel";
            this.startDateLabel.Size = new System.Drawing.Size(80, 25);
            this.startDateLabel.TabIndex = 4;
            this.startDateLabel.Text = "开始日期:";
            this.startDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bottomPanel
            // 
            this.MainContainer.SetColumnSpan(this.bottomPanel, 3);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.exButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel.Location = new System.Drawing.Point(10, 91);
            this.bottomPanel.Margin = new System.Windows.Forms.Padding(0);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(264, 40);
            this.bottomPanel.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(189, 10);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(0);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // exButton
            // 
            this.exButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.exButton.Location = new System.Drawing.Point(102, 10);
            this.exButton.Margin = new System.Windows.Forms.Padding(0);
            this.exButton.Name = "exButton";
            this.exButton.Size = new System.Drawing.Size(75, 25);
            this.exButton.TabIndex = 1;
            this.exButton.Text = "执行";
            this.exButton.UseVisualStyleBackColor = true;
            this.exButton.Click += new System.EventHandler(this.exButton_Click);
            // 
            // endDateLabel
            // 
            this.endDateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.endDateLabel.Location = new System.Drawing.Point(10, 45);
            this.endDateLabel.Margin = new System.Windows.Forms.Padding(0);
            this.endDateLabel.Name = "endDateLabel";
            this.endDateLabel.Size = new System.Drawing.Size(80, 25);
            this.endDateLabel.TabIndex = 4;
            this.endDateLabel.Text = "结束日期:";
            this.endDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startDate
            // 
            this.startDate.CustomFormat = "yyyy-MM-dd";
            this.startDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDate.Location = new System.Drawing.Point(100, 10);
            this.startDate.Margin = new System.Windows.Forms.Padding(0);
            this.startDate.MaxDate = new System.DateTime(2999, 12, 31, 0, 0, 0, 0);
            this.startDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.startDate.Name = "startDate";
            this.startDate.Size = new System.Drawing.Size(174, 23);
            this.startDate.TabIndex = 5;
            this.startDate.Value = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            // 
            // endDate
            // 
            this.endDate.CustomFormat = "yyyy-MM-dd";
            this.endDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.endDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDate.Location = new System.Drawing.Point(100, 45);
            this.endDate.Margin = new System.Windows.Forms.Padding(0);
            this.endDate.MaxDate = new System.DateTime(2999, 12, 31, 0, 0, 0, 0);
            this.endDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.endDate.Name = "endDate";
            this.endDate.Size = new System.Drawing.Size(174, 23);
            this.endDate.TabIndex = 5;
            this.endDate.Value = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            // 
            // ExForm
            // 
            this.AcceptButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Controls.Add(this.MainContainer);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 180);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 180);
            this.Name = "ExForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "执行计划任务";
            this.Load += new System.EventHandler(this.ExForm_Load);
            this.MainContainer.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainContainer;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button exButton;
        private System.Windows.Forms.Label startDateLabel;
        private System.Windows.Forms.Label endDateLabel;
        private System.Windows.Forms.DateTimePicker startDate;
        private System.Windows.Forms.DateTimePicker endDate;
    }
}