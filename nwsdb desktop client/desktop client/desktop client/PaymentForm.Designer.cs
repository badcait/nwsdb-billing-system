namespace desktop_client
{
    partial class PaymentForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.grid = new System.Windows.Forms.DataGridView();
            this.actions = new System.Windows.Forms.FlowLayoutPanel();
            this.accountLabel = new System.Windows.Forms.Label();
            this.account = new System.Windows.Forms.TextBox();
            this.lookupButton = new System.Windows.Forms.Button();
            this.token = new System.Windows.Forms.ComboBox();
            this.payButton = new System.Windows.Forms.Button();
            this.resumeButton = new System.Windows.Forms.Button();
            this.historyButton = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.actions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 95);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(1084, 438);
            this.grid.TabIndex = 0;
            // 
            // actions
            // 
            this.actions.Controls.Add(this.accountLabel);
            this.actions.Controls.Add(this.account);
            this.actions.Controls.Add(this.lookupButton);
            this.actions.Controls.Add(this.token);
            this.actions.Controls.Add(this.payButton);
            this.actions.Controls.Add(this.resumeButton);
            this.actions.Controls.Add(this.historyButton);
            this.actions.Dock = System.Windows.Forms.DockStyle.Top;
            this.actions.Location = new System.Drawing.Point(0, 0);
            this.actions.Name = "actions";
            this.actions.Padding = new System.Windows.Forms.Padding(12);
            this.actions.Size = new System.Drawing.Size(1084, 95);
            this.actions.TabIndex = 1;
            // 
            // accountLabel
            // 
            this.accountLabel.AutoSize = true;
            this.accountLabel.Location = new System.Drawing.Point(15, 12);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.accountLabel.Size = new System.Drawing.Size(111, 25);
            this.accountLabel.TabIndex = 0;
            this.accountLabel.Text = "Account number";
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(132, 15);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(180, 25);
            this.account.TabIndex = 1;
            // 
            // lookupButton
            // 
            this.lookupButton.AutoSize = true;
            this.lookupButton.Location = new System.Drawing.Point(318, 15);
            this.lookupButton.Name = "lookupButton";
            this.lookupButton.Size = new System.Drawing.Size(96, 34);
            this.lookupButton.TabIndex = 2;
            this.lookupButton.Text = "Look up bills";
            this.lookupButton.Click += new System.EventHandler(this.LookupButton_Click);
            // 
            // token
            // 
            this.token.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.token.Items.AddRange(new object[] {
            "demo-approve",
            "demo-decline"});
            this.token.Location = new System.Drawing.Point(420, 15);
            this.token.Name = "token";
            this.token.Size = new System.Drawing.Size(155, 25);
            this.token.TabIndex = 3;
            // 
            // payButton
            // 
            this.payButton.AutoSize = true;
            this.payButton.Location = new System.Drawing.Point(581, 15);
            this.payButton.Name = "payButton";
            this.payButton.Size = new System.Drawing.Size(115, 34);
            this.payButton.TabIndex = 4;
            this.payButton.Text = "Pay selected bill";
            this.payButton.Click += new System.EventHandler(this.PayButton_Click);
            // 
            // resumeButton
            // 
            this.resumeButton.AutoSize = true;
            this.resumeButton.Location = new System.Drawing.Point(702, 15);
            this.resumeButton.Name = "resumeButton";
            this.resumeButton.Size = new System.Drawing.Size(164, 34);
            this.resumeButton.TabIndex = 5;
            this.resumeButton.Text = "Resume saved payment";
            this.resumeButton.Click += new System.EventHandler(this.ResumeButton_Click);
            // 
            // historyButton
            // 
            this.historyButton.AutoSize = true;
            this.historyButton.Location = new System.Drawing.Point(872, 15);
            this.historyButton.Name = "historyButton";
            this.historyButton.Size = new System.Drawing.Size(176, 34);
            this.historyButton.TabIndex = 6;
            this.historyButton.Text = "History / recheck / receipt";
            this.historyButton.Click += new System.EventHandler(this.HistoryButton_Click);
            // 
            // status
            // 
            this.status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.status.Location = new System.Drawing.Point(0, 533);
            this.status.Name = "status";
            this.status.Padding = new System.Windows.Forms.Padding(12);
            this.status.Size = new System.Drawing.Size(1084, 58);
            this.status.TabIndex = 2;
            this.status.Text = "Look up an account to view unpaid bills.";
            // 
            // PaymentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 591);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.actions);
            this.Controls.Add(this.status);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Name = "PaymentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NWSDB · Desktop Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PaymentForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.actions.ResumeLayout(false);
            this.actions.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.FlowLayoutPanel actions;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.TextBox account;
        private System.Windows.Forms.ComboBox token;
        private System.Windows.Forms.Button lookupButton;
        private System.Windows.Forms.Button payButton;
        private System.Windows.Forms.Button resumeButton;
        private System.Windows.Forms.Button historyButton;
    }
}
