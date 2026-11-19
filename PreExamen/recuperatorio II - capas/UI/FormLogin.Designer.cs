namespace UI
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.etiquetaUsuario = new System.Windows.Forms.Label();
            this.usuario = new System.Windows.Forms.TextBox();
            this.etiquetaClave = new System.Windows.Forms.Label();
            this.clave = new System.Windows.Forms.TextBox();
            this.entrar = new System.Windows.Forms.Button();
            this.error = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // etiquetaUsuario
            // 
            this.etiquetaUsuario.Location = new System.Drawing.Point(30, 30);
            this.etiquetaUsuario.Name = "etiquetaUsuario";
            this.etiquetaUsuario.Size = new System.Drawing.Size(80, 23);
            this.etiquetaUsuario.TabIndex = 0;
            this.etiquetaUsuario.Text = "Usuario:";
            // 
            // usuario
            // 
            this.usuario.Location = new System.Drawing.Point(120, 25);
            this.usuario.Name = "usuario";
            this.usuario.Size = new System.Drawing.Size(220, 23);
            this.usuario.TabIndex = 1;
            // 
            // etiquetaClave
            // 
            this.etiquetaClave.Location = new System.Drawing.Point(30, 70);
            this.etiquetaClave.Name = "etiquetaClave";
            this.etiquetaClave.Size = new System.Drawing.Size(80, 23);
            this.etiquetaClave.TabIndex = 2;
            this.etiquetaClave.Text = "Clave:";
            // 
            // clave
            // 
            this.clave.Location = new System.Drawing.Point(120, 65);
            this.clave.Name = "clave";
            this.clave.Size = new System.Drawing.Size(220, 23);
            this.clave.TabIndex = 3;
            this.clave.UseSystemPasswordChar = true;
            // 
            // entrar
            // 
            this.entrar.Location = new System.Drawing.Point(120, 105);
            this.entrar.Name = "entrar";
            this.entrar.Size = new System.Drawing.Size(220, 28);
            this.entrar.TabIndex = 4;
            this.entrar.Text = "Entrar";
            this.entrar.UseVisualStyleBackColor = true;
            this.entrar.Click += new System.EventHandler(this.Entrar);
            // 
            // error
            // 
            this.error.ForeColor = System.Drawing.Color.DarkRed;
            this.error.Location = new System.Drawing.Point(30, 150);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(340, 23);
            this.error.TabIndex = 5;
            // 
            // FormLogin
            // 
            this.AcceptButton = this.entrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.error);
            this.Controls.Add(this.entrar);
            this.Controls.Add(this.clave);
            this.Controls.Add(this.etiquetaClave);
            this.Controls.Add(this.usuario);
            this.Controls.Add(this.etiquetaUsuario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ingreso al sistema";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label etiquetaUsuario;
        private System.Windows.Forms.TextBox usuario;
        private System.Windows.Forms.Label etiquetaClave;
        private System.Windows.Forms.TextBox clave;
        private System.Windows.Forms.Button entrar;
        private System.Windows.Forms.Label error;
    }
}
