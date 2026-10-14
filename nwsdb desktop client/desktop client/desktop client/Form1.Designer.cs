namespace desktop_client
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.panel = new System.Windows.Forms.FlowLayoutPanel();
            this.heading = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.login = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.panel.SuspendLayout();
            this.panel.Name = "panel";
            this.heading.Name = "heading";
            this.usernameLabel.Name = "usernameLabel";
            this.username.Name = "username";
            this.passwordLabel.Name = "passwordLabel";
            this.password.Name = "password";
            this.login.Name = "login";
            this.BackColor = System.Drawing.Color.White;
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panel.Padding = new System.Windows.Forms.Padding(35);
            this.panel.WrapContents = false;
            this.heading.Text = "Partner collections";
            this.heading.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.heading.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.heading.AutoSize = true;
            this.usernameLabel.AutoSize = true;
            this.passwordLabel.AutoSize = true;
            this.usernameLabel.Text = "Username";
            this.passwordLabel.Text = "Password";
            this.login.Text = "Sign in";
            this.username.Size = new System.Drawing.Size(290, 25);
            this.password.Size = new System.Drawing.Size(290, 25);
            this.password.UseSystemPasswordChar = true;
            this.login.Size = new System.Drawing.Size(290, 38);
            this.login.Click += new System.EventHandler(this.SignIn);
            this.AcceptButton = this.login;
            this.panel.Controls.Add(this.heading);
            this.panel.Controls.Add(this.usernameLabel);
            this.panel.Controls.Add(this.username);
            this.panel.Controls.Add(this.passwordLabel);
            this.panel.Controls.Add(this.password);
            this.panel.Controls.Add(this.login);
            this.Controls.Add(this.panel);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ClientSize = new System.Drawing.Size(394, 331);
            this.Name = "Form1";
            this.Text = "NWSDB · Partner sign in";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.FlowLayoutPanel panel;
        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Button login;
    }
}
