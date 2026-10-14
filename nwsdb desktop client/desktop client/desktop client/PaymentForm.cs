using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using desktop_client.Data;
using desktop_client.DTO;
using desktop_client.Model;
using desktop_client.Services;
namespace desktop_client
{
    public partial class PaymentForm : Form
    {
        private readonly PartnerApiClient api;
        private readonly DraftStore drafts;
        public PaymentForm()
        {
            InitializeComponent();
            token.SelectedIndex = 0;
        }
        public PaymentForm(PartnerApiClient api, DraftStore drafts) : this()
        {
            this.api = api;
            this.drafts = drafts;
        }
        private async void LookupButton_Click(object sender, EventArgs e) { await RunAsync(LookupAsync); }
        private async void PayButton_Click(object sender, EventArgs e) { await RunAsync(PayAsync); }
        private async void ResumeButton_Click(object sender, EventArgs e) { await RunAsync(ResumeAsync); }
        private async void HistoryButton_Click(object sender, EventArgs e)
        {
            await RunAsync(() => { using (var form = new PaymentStatusForm(api)) form.ShowDialog(this); return Task.CompletedTask; });
        }
        private void PaymentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!actions.Enabled) { e.Cancel = true; status.Text = "Wait for the current request to finish."; }
        }
        private async Task RunAsync(Func<Task> action)
        {
            actions.Enabled = false;
            try { await action(); }
            catch (Exception ex) { status.Text = ex.Message; }
            finally { actions.Enabled = true; }
        }
        public async Task LookupAsync()
        {
            if (string.IsNullOrWhiteSpace(account.Text)) { status.Text = "Enter an account number."; return; }
            var bills = await api.LookupBillsAsync(account.Text.Trim()); grid.DataSource = bills;
            status.Text = bills.Length == 0 ? "No unpaid bills found." : "Select one bill. Each payment settles its full amount.";
        }
        public async Task PayAsync()
        {
            if (drafts.Load() != null) { status.Text = "A saved payment exists. Use Resume saved payment first."; return; }
            var bill = grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as BillLookupDto;
            if (bill == null) { status.Text = "Select an unpaid bill."; return; }
            if (bill.IsProcessing) { status.Text = "This bill already has a pending payment. Check history."; return; }
            if (MessageBox.Show(this, "Submit LKR " + bill.Amount.ToString("N2") + " for " + bill.AccountNumber + "?", "Confirm payment", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var draft = new PaymentDraft { OwnerUserId = drafts.OwnerUserId, OwnerPartnerId = drafts.OwnerPartnerId, BillId = bill.BillId,
                Amount = bill.Amount, ClientReference = Guid.NewGuid(), MockPaymentToken = token.Text };
            drafts.Save(draft);
            await SendAsync(draft, true);
        }
        private async Task SendAsync(PaymentDraft draft, bool firstSend)
        {
            try
            {
                var result = await api.SubmitPaymentAsync(new CreatePaymentDto { BillId = draft.BillId, Amount = draft.Amount,
                    ClientReference = draft.ClientReference, MockPaymentToken = draft.MockPaymentToken });
                if (!firstSend && result.Status == AttemptStatus.Pending) result = await api.ReconcileAsync(result.AttemptId);
                Finish(result);
            }
            catch (ApiException ex)
            {
                if (firstSend && (ex.StatusCode == 400 || ex.StatusCode == 401 || ex.StatusCode == 403 || ex.StatusCode == 404 || (ex.StatusCode == 409 && ex.Code == "BillUnavailable"))) drafts.Clear();
                status.Text = ex.Message + (drafts.Load() != null ? " Saved reference retained; use Resume." : " Correct the request or sign in again.");
            }
            catch (Exception) { status.Text = "Outcome uncertain. Saved reference retained. Check API connection, then use Resume."; }
        }
        private void Finish(PaymentStatusDto result)
        {
            status.Text = result.Message;
            if (result.Status == AttemptStatus.Pending) return;
            if (result.Status == AttemptStatus.Completed && result.Receipt != null) PaymentStatusForm.DisplayReceipt(this, result.Receipt);
            drafts.Clear();
        }
        private async Task ResumeAsync()
        {
            var draft = drafts.Load(); if (draft == null) { status.Text = "No saved payment to resume."; return; }
            var known = (await api.GetHistoryAsync()).FirstOrDefault(x => x.ClientReference == draft.ClientReference);
            if (known == null) { await SendAsync(draft, false); return; }
            Finish(known.Status == AttemptStatus.Pending ? await api.ReconcileAsync(known.AttemptId) : known);
        }
    }
}
