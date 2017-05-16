// ********************************************************************************************************* *

// Copyright (c) 2017 Michal Kelnar

// SPDX-License-Identifier: BSD-3-Clause
// The BSD-3-Clause license for this file can be found in the LICENSE.txt file included with this distribution
// or at https://spdx.org/licenses/BSD-3-Clause.html#licenseText

// ********************************************************************************************************* *

using System;
using System.Windows;
using System.Xml;
using System.Xml.XPath;

namespace xmlEraser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (this.tbRequest.Text != string.Empty)
            {
                try
                {
                    XPathExpression.Compile(tbRequest.Text);
                    Properties.Settings.Default.Save();

                    XmlDocument xml = new XmlDocument();
                    xml.Load(this.filePath.Text);
                    xml.Save(this.filePath.Text + ".backup");

                    foreach (XmlNode item in xml.SelectNodes(this.tbRequest.Text))
                    {
                        item.ParentNode.RemoveChild(item);
                    }

                    xml.Save(this.filePath.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease check request XPath syntax.", "Error", MessageBoxButton.OK);
                }
            }
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "XML files (.xml)|*.xml|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (System.IO.File.Exists(this.filePath.Text))
            {
                ofd.FileName = System.IO.Directory.GetParent(this.filePath.Text).FullName;
            }
            if (ofd.ShowDialog().Value)
            {
                this.filePath.Text = ofd.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
