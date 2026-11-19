using System;
using System.Windows.Forms;
using DomainModel;
using Services;

namespace UI
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal(Usuario u)
        {
            InitializeComponent();

            Text = "Pantalla principal - " + u.NombreUsuario;
            lblBienvenido.Text = "Bienvenido, " + SesionManager.Instancia.Usuario.NombreUsuario;

            // El menú se GENERA a partir del perfil (Composite):
            // lo que el usuario no tiene, no aparece.
            foreach (Componente c in u.Permisos)
                ArmarMenu(menuPrincipal.Items, c);
        }

        // Recursivo, igual que el Composite:
        //  Familia -> ítem con submenú
        //  Patente -> ítem que se puede clickear
        private void ArmarMenu(ToolStripItemCollection destino, Componente c)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(c.Nombre);

            if (c is Familia)
            {
                foreach (Componente hijo in c.Hijos)
                    ArmarMenu(item.DropDownItems, hijo);
            }
            else
            {
                item.Click += (s, e) => MessageBox.Show("Abriendo: " + c.Nombre);
            }

            destino.Add(item);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Close();   // el login escucha el FormClosed y hace el Logout
        }
    }
}
