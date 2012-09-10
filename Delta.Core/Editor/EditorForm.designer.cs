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
            this.buttonLoadTMX = new System.Windows.Forms.Button();
            this.buttonSaveTMX = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // grdProperty
            // 
            this.grdProperty.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grdProperty.Location = new System.Drawing.Point(0, 233);
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
            // buttonLoadTMX
            // 
            this.buttonLoadTMX.Location = new System.Drawing.Point(12, 205);
            this.buttonLoadTMX.Name = "buttonLoadTMX";
            this.buttonLoadTMX.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadTMX.TabIndex = 2;
            this.buttonLoadTMX.Text = "Load TMX";
            this.buttonLoadTMX.UseVisualStyleBackColor = true;
            this.buttonLoadTMX.Click += new System.EventHandler(this.buttonLoadTMX_Click);
            // 
            // buttonSaveTMX
            // 
            this.buttonSaveTMX.Location = new System.Drawing.Point(93, 205);
            this.buttonSaveTMX.Name = "buttonSaveTMX";
            this.buttonSaveTMX.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveTMX.TabIndex = 3;
            this.buttonSaveTMX.Text = "Save TMX";
            this.buttonSaveTMX.UseVisualStyleBackColor = true;
            this.buttonSaveTMX.Click += new System.EventHandler(this.buttonSaveTMX_Click);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 557);
            this.Controls.Add(this.buttonSaveTMX);
            this.Controls.Add(this.buttonLoadTMX);
            this.Controls.Add(this.listBoxObjects);
            this.Controls.Add(this.grdProperty);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorForm_FormClosing);
            this.Load += new System.EventHandler(this.EditorForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PropertyGrid grdProperty;
        private System.Windows.Forms.ListBox listBoxObjects;
        private System.Windows.Forms.Button buttonLoadTMX;
        private System.Windows.Forms.Button buttonSaveTMX;


    }
}