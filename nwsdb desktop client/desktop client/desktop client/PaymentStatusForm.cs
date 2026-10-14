using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using desktop_client.DTO;
using desktop_client.Services;
namespace desktop_client
{
    public partial class PaymentStatusForm : Form
    {
        private readonly PartnerApiClient api;
        public PaymentStatusForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
        }
        public PaymentStatusForm(PartnerApiClient api) : this()
        {
            this.api = api;
            Shown += PaymentStatusForm_Shown;
        }
        private async void PaymentStatusForm_Shown(object sender, EventArgs e) { await RunAsync(RefreshAsync); }
        private async void RefreshButton_Click(object sender, EventArgs e) { await RunAsync(RefreshAsync); }
        private async void RecheckButton_Click(object sender, EventArgs e) { await RunAsync(RecheckAsync); }
        private async void ReceiptButton_Click(object sender, EventArgs e) { await RunAsync(ReceiptAsync); }
        private async Task RunAsync(Func<Task> action) { bar.Enabled = false; try { await action(); } catch (Exception ex) { MessageBox.Show(this, ex.Message); } finally { bar.Enabled = true; } }
        public async Task RefreshAsync() { grid.DataSource = await api.GetHistoryAsync(); if (grid.Columns["Receipt"] != null) grid.Columns["Receipt"].Visible = false; }
        private PaymentStatusDto Selected() { return grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as PaymentStatusDto; }
        public async Task RecheckAsync() { var selected = Selected(); if (selected == null) return; var r = await api.ReconcileAsync(selected.AttemptId); MessageBox.Show(this, r.Message); await RefreshAsync(); }
        private async Task ReceiptAsync() { var selected = Selected(); if (selected == null || selected.Receipt == null) { MessageBox.Show(this, "Select a completed payment."); return; } ShowReceipt(await api.GetReceiptAsync(selected.Receipt.ReceiptNumber)); }
        public void ShowReceipt(ReceiptDto receipt) { DisplayReceipt(this, receipt); }
        public static void DisplayReceipt(IWin32Window owner, ReceiptDto receipt)
        {
            using (var form = new Form { Text = "Payment receipt", Size = new Size(620, 400), StartPosition = FormStartPosition.CenterParent })
            using (var document = new System.Drawing.Printing.PrintDocument())
            {
                var text = "NWSDB PAYMENT RECEIPT\r\n\r\nReceipt: " + receipt.ReceiptNumber + "\r\nAccount: " + receipt.AccountNumber +
                    "\r\nBill: " + receipt.BillId + "\r\nAmount: LKR " + receipt.Amount.ToString("N2") + "\r\nPaid at (UTC): " + receipt.PaidAt.ToString("yyyy-MM-dd HH:mm") + "\r\nBank reference: " + receipt.BankReference;
                form.Controls.Add(new TextBox { Text = text, Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, Font = new Font("Consolas", 11) });
                var print = new Button { Text = "Print receipt", Dock = DockStyle.Bottom, Height = 40 }; form.Controls.Add(print);
                document.PrintPage += (s, e) => { using (var font = new Font("Arial", 11)) e.Graphics.DrawString(text, font, Brushes.Black, e.MarginBounds); };
                print.Click += (s, e) => { using (var dialog = new PrintDialog { Document = document, UseEXDialog = true }) if (dialog.ShowDialog(form) == DialogResult.OK) document.Print(); };
                form.ShowDialog(owner);
            }
        }
    }
}
