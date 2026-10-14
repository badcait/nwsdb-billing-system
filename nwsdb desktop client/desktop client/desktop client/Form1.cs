using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using desktop_client.DTO;
using desktop_client.Data;
using desktop_client.Services;
namespace desktop_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private async void SignIn(object sender, EventArgs e)
        {
            login.Enabled = false;
            var api = new PartnerApiClient(ConfigurationManager.AppSettings["NwsdbApiUrl"] ?? "https://localhost:7180/");
            try
            {
                var user = await api.LoginAsync(new LoginRequestDto { Username = username.Text.Trim(), Password = password.Text });
                password.Clear(); Hide();
                using (var form = new PaymentForm(api, new DraftStore(user.UserId, user.PartnerId.Value))) form.ShowDialog();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Sign-in failed"); Show(); }
            finally { api.Dispose(); login.Enabled = true; }
        }
    }
}
