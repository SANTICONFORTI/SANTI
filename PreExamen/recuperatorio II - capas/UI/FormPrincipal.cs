using System;
using System.Windows.Forms;
using BLL;
using DomainModel;

namespace UI
{
    // Pantalla principal: el menu se arma segun el perfil del usuario.
    // Lo visual está en FormPrincipal.Designer.cs
    public partial class FormPrincipal : Form
    {
        private readonly SeguridadServicio servicio;
        private readonly Sesion sesion;

        public FormPrincipal(SeguridadServicio servicio, Sesion sesion)
        {
            InitializeComponent();
            this.servicio = servicio;
            this.sesion = sesion;

            quien.Text = "Usuario: " + sesion.Usuario.Nombre +
                         "   -   Perfil: " + sesion.Usuario.Perfil.Nombre;
            CargarMenu();
        }

        // Pide el menu ya filtrado por el perfil.
        private void CargarMenu()
        {
            MenuServicio servicioMenu = new MenuServicio();
            Funcionalidad catalogo = CatalogoFuncionalidades.Armar();
            Funcionalidad visible = servicioMenu.ArmarMenu(catalogo, sesion.Usuario.Perfil);

            menu.Nodes.Clear();
            AgregarNodos(visible, menu.Nodes);
            menu.ExpandAll();
        }

        private void AgregarNodos(Funcionalidad funcionalidad, TreeNodeCollection destino)
        {
            foreach (Funcionalidad hijo in funcionalidad.ObtenerHijos())
            {
                TreeNode nodo = destino.Add(hijo.Nombre);

                if (hijo.TieneHijos())
                {
                    AgregarNodos(hijo, nodo.Nodes);
                }
            }
        }

        private void Abrir(object sender, TreeViewEventArgs e)
        {
            // Solo las simples abren algo.
            if (e.Node.Nodes.Count == 0)
            {
                abierto.Text = "Abriendo: " + e.Node.Text;
            }
        }

        private void Salir(object sender, EventArgs e)
        {
            servicio.CerrarSesion();
            Close();
        }
    }
}
