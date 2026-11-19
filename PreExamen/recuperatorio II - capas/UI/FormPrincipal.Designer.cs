namespace UI
{
    partial class FormPrincipal
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
            this.quien = new System.Windows.Forms.Label();
            this.menu = new System.Windows.Forms.TreeView();
            this.abierto = new System.Windows.Forms.Label();
            this.salir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // quien
            // 
            this.quien.Location = new System.Drawing.Point(20, 15);
            this.quien.Name = "quien";
            this.quien.Size = new System.Drawing.Size(500, 23);
            this.quien.TabIndex = 0;
            this.quien.Text = "Usuario:";
            // 
            // menu
            // 
            this.menu.HideSelection = false;
            this.menu.Location = new System.Drawing.Point(20, 50);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(310, 330);
            this.menu.TabIndex = 1;
            this.menu.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Abrir);
            // 
            // abierto
            // 
            this.abierto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.abierto.Location = new System.Drawing.Point(350, 50);
            this.abierto.Name = "abierto";
            this.abierto.Size = new System.Drawing.Size(340, 330);
            this.abierto.TabIndex = 2;
            this.abierto.Text = "Elegi una opcion del menu.";
            // 
            // salir
            // 
            this.salir.Location = new System.Drawing.Point(350, 390);
            this.salir.Name = "salir";
            this.salir.Size = new System.Drawing.Size(160, 28);
            this.salir.TabIndex = 3;
            this.salir.Text = "Cerrar sesion";
            this.salir.UseVisualStyleBackColor = true;
            this.salir.Click += new System.EventHandler(this.Salir);
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 431);
            this.Controls.Add(this.salir);
            this.Controls.Add(this.abierto);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.quien);
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pantalla principal";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label quien;
        private System.Windows.Forms.TreeView menu;
        private System.Windows.Forms.Label abierto;
        private System.Windows.Forms.Button salir;
    }
}
