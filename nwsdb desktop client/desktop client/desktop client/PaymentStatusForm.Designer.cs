namespace desktop_client
{
    partial class PaymentStatusForm
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
            this.bar = new System.Windows.Forms.FlowLayoutPanel();
            this.refreshButton = new System.Windows.Forms.Button();
            this.recheckButton = new System.Windows.Forms.Button();
            this.receiptButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.bar.SuspendLayout();
            this.grid.Name = "grid";
            this.bar.Name = "bar";
            this.refreshButton.Name = "refreshButton";
            this.recheckButton.Name = "recheckButton";
            this.receiptButton.Name = "receiptButton";
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.ReadOnly = true;
            this.grid.AllowUserToAddRows = false;
            this.grid.MultiSelect = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.bar.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar.Height = 50;
            this.bar.Padding = new System.Windows.Forms.Padding(6);
            this.refreshButton.Text = "Refresh";
            this.refreshButton.AutoSize = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            this.bar.Controls.Add(this.refreshButton);
            this.recheckButton.Text = "Recheck selected";
            this.recheckButton.AutoSize = true;
            this.recheckButton.Click += new System.EventHandler(this.RecheckButton_Click);
            this.bar.Controls.Add(this.recheckButton);
            this.receiptButton.Text = "View / reprint receipt";
            this.receiptButton.AutoSize = true;
            this.receiptButton.Click += new System.EventHandler(this.ReceiptButton_Click);
            this.bar.Controls.Add(this.receiptButton);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.bar);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ClientSize = new System.Drawing.Size(1034, 481);
            this.Name = "PaymentStatusForm";
            this.Text = "Payment history and receipts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.bar.ResumeLayout(false);
            this.bar.PerformLayout();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.FlowLayoutPanel bar;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button recheckButton;
        private System.Windows.Forms.Button receiptButton;
    }
}
