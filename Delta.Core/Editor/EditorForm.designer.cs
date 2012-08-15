namespace Delta.Editor
{
    partial class EditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grdProperty = new System.Windows.Forms.PropertyGrid();
            this.listBoxObjects = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // grdProperty
            // 
            this.grdProperty.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grdProperty.Location = new System.Drawing.Point(0, 199);
            this.grdProperty.Name = "grdProperty";
            this.grdProperty.Size = new System.Drawing.Size(450, 324);
            this.grdProperty.TabIndex = 0;
            this.grdProperty.ToolbarVisible = false;
            this.grdProperty.Enter += new System.EventHandler(this.grdProperty_Enter);
            // 
            // listBoxObjects
            // 
            this.listBoxObjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.listBoxObjects.FormattingEnabled = true;
            this.listBoxObjects.Location = new System.Drawing.Point(0, 0);
            this.listBoxObjects.Name = "listBoxObjects";
            this.listBoxObjects.Size = new System.Drawing.Size(450, 199);
            this.listBoxObjects.TabIndex = 1;
            this.listBoxObjects.SelectedIndexChanged += new System.EventHandler(this.listBoxObjects_SelectedIndexChanged);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 523);
            this.Controls.Add(this.listBoxObjects);
            this.Controls.Add(this.grdProperty);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorForm_FormClosing);
            this.Load += new System.EventHandler(this.EditorForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PropertyGrid grdProperty;
        private System.Windows.Forms.ListBox listBoxObjects;


    }
}