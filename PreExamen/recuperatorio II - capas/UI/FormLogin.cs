using System;
using System.Windows.Forms;
using BLL;
using DomainModel;

namespace UI
{
    // Pantalla de ingreso. Lo visual está en FormLogin.Designer.cs
    public partial class FormLogin : Form
    {
        private readonly SeguridadServicio servicio;

        public FormLogin(SeguridadServicio servicio)
        {
            InitializeComponent();
            this.servicio = servicio;
        }

        private void Entrar(object sender, EventArgs e)
        {
            try
            {
                Sesion sesion = servicio.IniciarSesion(usuario.Text, clave.Text);

                FormPrincipal principal = new FormPrincipal(servicio, sesion);
                Hide();
                principal.ShowDialog();
                Close();
            }
            catch (SeguridadException ex)
            {
                error.Text = ex.Message;
            }
        }
    }
}
