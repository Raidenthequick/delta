using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Delta.Tiled;

namespace Delta.Editor
{
    public partial class EditorForm : Form
    {
        private System.Xml.XmlDocument tmxBase = null;

        public EditorForm()
        {
            InitializeComponent();
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.UserClosing:
                    Hide();
                    e.Cancel = true;
                    break;
            }

        }

        private void EditorForm_Load(object sender, EventArgs e)
        {
        }

        private void grdProperty_Enter(object sender, EventArgs e)
        {
            grdProperty.Refresh();
        }

        //adds objects to the editor form's list
        public void AddObjects(List<object> objs)
        {
            //uses ToString() for the name
            objs.ForEach((o) => listBoxObjects.Items.Add(o));
        }

        //clears object list
        public void ClearObjects()
        {
            listBoxObjects.Items.Clear();
        }

        //selects an object in the listbox which then, based on event, selects in grid
        public void SelectObject(object obj)
        {
            listBoxObjects.SelectedItem = obj;
        }

        private void listBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxObjects.SelectedIndex < 0)
            {
                return;
            }

            grdProperty.SelectedObject = listBoxObjects.SelectedItem;
        }

        /// <summary>
        /// dependencies for editor loading/saving
        /// </summary>
        public static void LoadDependencies()
        {
            foreach (var f in Directory.GetFiles(Environment.CurrentDirectory + "/../..", "*.*", SearchOption.AllDirectories))
            {
                FileInfo file = new FileInfo(f);
                switch (file.Extension.ToLower())
                {
                    case ".stylesheet":
                        Delta.Tiled.Map.LoadStyleSheet(file.FullName);
                        break;
                }
            }
        }

        private void buttonSaveTMX_Click(object sender, EventArgs e)
        {
            if (tmxBase == null)
            {
                MessageBox.Show("Select TMX to load as a base first");
                return;
            }

            var sfd = new SaveFileDialog() { InitialDirectory = System.IO.Path.GetFullPath(@"..\..\Delta.Examples\Delta.ExamplesContent\Maps") };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Delta.Tiled.Map.Instance.SaveToTMX(tmxBase, sfd.FileName);
            }
        }

        private void buttonLoadTMX_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog() { InitialDirectory = System.IO.Path.GetFullPath(@"..\..\Delta.Examples\Delta.ExamplesContent\Maps") };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tmxBase = new System.Xml.XmlDocument();
                tmxBase.Load(new FileStream(ofd.FileName, FileMode.Open));

                ////backup old map
                //var oldMap = Map.Instance;

                ////construct new map which reinits Instance
                //var map = new Map(ofd.FileName);

                ////turn back Instance
                //Map.Instance = oldMap;

                ////copy new layers
                //Map.Instance._children.Clear();
                //Map.Instance._children = map._children;
            }
        }
    }
}
