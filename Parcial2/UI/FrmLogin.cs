using System;
using System.Windows.Forms;
using BLL;
using DomainModel;

namespace UI
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioBLL bll = new UsuarioBLL();

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario u = bll.Login(txtUsuario.Text, txtPassword.Text);
                txtPassword.Clear();
                Hide();

                FrmPrincipal principal = new FrmPrincipal(u);
                principal.FormClosed += Principal_FormClosed;
                principal.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Cuando se cierra la pantalla principal = Logout y vuelve el login
        private void Principal_FormClosed(object sender, FormClosedEventArgs e)
        {
            bll.Logout();
            txtUsuario.Clear();
            Show();
        }
    }
}
