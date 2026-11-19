// =====================================================================
//  PARCIAL 2 - Login/Logout + permisos con funcionalidades compuestas
//  Proyecto: Aplicación de Windows Forms (.NET Framework 4.8)
//
//  CÓMO USARLO: crear "Aplicación de Windows Forms (.NET Framework)",
//  borrar Form1.cs y Program.cs, agregar este archivo y ejecutar.
//  Los formularios se arman por código (sin diseñador) para que se
//  vea todo en un lugar.
//
//  Separado por CAPAS con namespaces. En el examen: un proyecto
//  (Biblioteca de clases) por namespace: BE, DAL, BLL, Servicios, y el
//  WinForms es la UI. Referencias: UI -> BLL -> DAL, y todos -> BE.
//
//  PATRONES (y la frase del enunciado que los pide):
//   - COMPOSITE -> "funcionalidades compuestas = conjunto de simples, y
//                  pueden contener otras compuestas" (árbol)
//                  Componente (abstracto) / Patente (hoja) / Familia (compuesto)
//   - SINGLETON -> la SESIÓN: hay un solo usuario logueado a la vez
//
//  DIAGRAMA DE CLASES (resumen para dibujar):
//
//          Componente (abstracta) <----------------+
//           ^              ^                       | 0..*  (hijos)
//        Patente        Familia -------------------+
//
//   Usuario ---- tiene 0..* ----> Componente   (su perfil)
//   SesionManager (Singleton) ---> Usuario logueado
//   FrmLogin -> UsuarioBLL -> UsuarioDAL
// =====================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

// =====================================================================
//  CAPA BE (Entidades)
// =====================================================================
namespace BE
{
    // ---- COMPOSITE: la clase base. Hoja y compuesto se tratan IGUAL ----
    public abstract class Componente
    {
        public string Nombre { get; set; }

        public abstract List<Componente> Hijos { get; }
        public abstract void Agregar(Componente c);

        // ¿Este componente (o alguno de adentro) es el permiso buscado?
        public abstract bool TienePermiso(string nombre);
    }

    // HOJA: funcionalidad simple / atómica (una opción de menú)
    public class Patente : Componente
    {
        public Patente(string nombre) { Nombre = nombre; }

        public override List<Componente> Hijos
        {
            get { return new List<Componente>(); }   // no tiene hijos
        }

        public override void Agregar(Componente c)
        {
            throw new InvalidOperationException("Una patente no puede tener hijos.");
        }

        public override bool TienePermiso(string nombre)
        {
            return Nombre == nombre;
        }
    }

    // COMPUESTO: conjunto de patentes y/o de otras familias
    public class Familia : Componente
    {
        private readonly List<Componente> hijos = new List<Componente>();

        public Familia(string nombre) { Nombre = nombre; }

        public override List<Componente> Hijos
        {
            get { return hijos; }
        }

        public override void Agregar(Componente c)
        {
            if (c == this)
                throw new InvalidOperationException("Una familia no puede contenerse a sí misma.");
            hijos.Add(c);
        }

        // RECURSIÓN: pregunta a cada hijo, sea hoja o compuesto
        public override bool TienePermiso(string nombre)
        {
            if (Nombre == nombre) return true;
            foreach (Componente h in hijos)
                if (h.TienePermiso(nombre)) return true;
            return false;
        }
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }

        // El PERFIL del usuario: sus familias y/o patentes
        public List<Componente> Permisos { get; set; }

        public Usuario() { Permisos = new List<Componente>(); }

        public bool TienePermiso(string nombre)
        {
            foreach (Componente c in Permisos)
                if (c.TienePermiso(nombre)) return true;
            return false;
        }
    }
}

// =====================================================================
//  CAPA SERVICIOS: cosas que usan todas las capas (sesión, encriptación)
// =====================================================================
namespace Servicios
{
    using BE;

    // ---- SINGLETON: una sola sesión en toda la aplicación ----
    public class SesionManager
    {
        private static SesionManager instancia;
        private static readonly object candado = new object();

        private SesionManager() { }            // constructor privado

        public static SesionManager Instancia
        {
            get
            {
                lock (candado)
                {
                    if (instancia == null)
                        instancia = new SesionManager();
                    return instancia;
                }
            }
        }

        public Usuario Usuario { get; private set; }

        public bool EstaLogueado
        {
            get { return Usuario != null; }
        }

        public void Login(Usuario u)
        {
            if (EstaLogueado)
                throw new InvalidOperationException("Ya hay una sesión iniciada.");
            Usuario = u;
        }

        public void Logout()
        {
            Usuario = null;
        }
    }

    // Las contraseñas NO se guardan en texto plano
    public static class Encriptador
    {
        public static string Hash(string texto)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}

// =====================================================================
//  CAPA DAL (Datos): persistencia EN MEMORIA (lo permite el enunciado)
// =====================================================================
namespace DAL
{
    using BE;
    using Servicios;

    public class UsuarioDAL
    {
        // static: la "base de datos" vive mientras corre el programa
        private static readonly List<Usuario> usuarios = CargarDatos();

        public Usuario BuscarPorNombre(string nombre)
        {
            return usuarios.Find(u => u.NombreUsuario == nombre);
        }

        private static List<Usuario> CargarDatos()
        {
            // Patentes (funcionalidades simples)
            Patente altaUsuario = new Patente("Alta de usuario");
            Patente bajaUsuario = new Patente("Baja de usuario");
            Patente verBitacora = new Patente("Ver bitácora");
            Patente asignarPermisos = new Patente("Asignar permisos");
            Patente facturar = new Patente("Facturar");
            Patente verClientes = new Patente("Ver clientes");
            Patente reportes = new Patente("Reportes");

            // Familia DENTRO de otra familia (lo que pide el enunciado)
            Familia seguridad = new Familia("Seguridad");
            seguridad.Agregar(asignarPermisos);
            seguridad.Agregar(verBitacora);

            Familia administracion = new Familia("Administración");
            administracion.Agregar(altaUsuario);
            administracion.Agregar(bajaUsuario);
            administracion.Agregar(seguridad);      // compuesta dentro de compuesta

            Familia ventas = new Familia("Ventas");
            ventas.Agregar(facturar);
            ventas.Agregar(verClientes);

            Usuario admin = new Usuario { Id = 1, NombreUsuario = "admin", PasswordHash = Encriptador.Hash("123") };
            admin.Permisos.Add(administracion);
            admin.Permisos.Add(ventas);
            admin.Permisos.Add(reportes);           // una patente suelta también vale

            Usuario vendedor = new Usuario { Id = 2, NombreUsuario = "vendedor", PasswordHash = Encriptador.Hash("123") };
            vendedor.Permisos.Add(ventas);

            return new List<Usuario> { admin, vendedor };
        }
    }
}

// =====================================================================
//  CAPA BLL (Negocio): reglas del login
// =====================================================================
namespace BLL
{
    using BE;
    using DAL;
    using Servicios;

    public class UsuarioBLL
    {
        private readonly UsuarioDAL dal = new UsuarioDAL();

        public Usuario Login(string nombre, string password)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Complete usuario y contraseña.");

            Usuario u = dal.BuscarPorNombre(nombre);
            if (u == null || u.PasswordHash != Encriptador.Hash(password))
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");

            SesionManager.Instancia.Login(u);
            return u;
        }

        public void Logout()
        {
            SesionManager.Instancia.Logout();
        }
    }
}

// =====================================================================
//  CAPA UI (WinForms)
// =====================================================================
namespace UI
{
    using BE;
    using BLL;
    using Servicios;

    public class FrmLogin : Form
    {
        private readonly TextBox txtUsuario = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly UsuarioBLL bll = new UsuarioBLL();

        public FrmLogin()
        {
            Text = "Login";
            ClientSize = new Size(300, 200);
            StartPosition = FormStartPosition.CenterScreen;

            Controls.Add(new Label { Text = "Usuario:", Location = new Point(20, 25), AutoSize = true });
            txtUsuario.Location = new Point(110, 22);
            txtUsuario.Width = 160;
            Controls.Add(txtUsuario);

            Controls.Add(new Label { Text = "Contraseña:", Location = new Point(20, 60), AutoSize = true });
            txtPassword.Location = new Point(110, 57);
            txtPassword.Width = 160;
            txtPassword.PasswordChar = '*';
            Controls.Add(txtPassword);

            Button btnIngresar = new Button { Text = "Ingresar", Location = new Point(110, 95), Width = 160 };
            btnIngresar.Click += BtnIngresar_Click;
            Controls.Add(btnIngresar);
            AcceptButton = btnIngresar;

            Controls.Add(new Label
            {
                Text = "Pruebas: admin / 123  ó  vendedor / 123",
                Location = new Point(20, 150),
                AutoSize = true,
                ForeColor = Color.Gray
            });
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario u = bll.Login(txtUsuario.Text, txtPassword.Text);
                txtPassword.Clear();
                Hide();
                FrmPrincipal principal = new FrmPrincipal(u);
                principal.FormClosed += (s, ev) =>
                {
                    bll.Logout();          // al cerrar el principal = logout
                    txtUsuario.Clear();
                    Show();                // vuelve el login para otro usuario
                };
                principal.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    public class FrmPrincipal : Form
    {
        public FrmPrincipal(Usuario u)
        {
            Text = "Pantalla principal - " + u.NombreUsuario;
            ClientSize = new Size(500, 300);
            StartPosition = FormStartPosition.CenterScreen;

            // El menú se GENERA a partir del perfil (Composite).
            // Lo que el usuario no tiene, directamente no existe en el menú.
            MenuStrip menu = new MenuStrip();
            foreach (Componente c in u.Permisos)
                ArmarMenu(menu.Items, c);
            MainMenuStrip = menu;
            Controls.Add(menu);

            Controls.Add(new Label
            {
                Text = "Bienvenido, " + SesionManager.Instancia.Usuario.NombreUsuario,
                Location = new Point(20, 50),
                AutoSize = true,
                Font = new Font("Segoe UI", 12)
            });

            Button btnLogout = new Button { Text = "Cerrar sesión", Location = new Point(20, 90), Width = 120 };
            btnLogout.Click += (s, e) => Close();     // el FormClosed del login hace el logout
            Controls.Add(btnLogout);
        }

        // RECURSIVO, igual que el Composite:
        //  Familia -> ítem con submenú (y se llama a sí mismo con cada hijo)
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
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());
        }
    }
}
