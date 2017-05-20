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
using System.Text.RegularExpressions;

using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace xmlEraser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Regex r = new Regex("pattern");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            PDDocument doc = null;

            try
            {
                doc = PDDocument.load(Properties.Settings.Default.PdfPath);
                PDFTextStripper stripper = new PDFTextStripper();
                string data = stripper.getText(doc);

                Regex r = new Regex("FA\\d{8}", RegexOptions.IgnoreCase);
                MatchCollection match = Regex.Matches(data, "FA\\d{8}");
                if (match.Count > 0)
                {
                    try
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(this.filePath.Text);
                        xml.Save(this.filePath.Text + ".backup");
                        var manager = new XmlNamespaceManager(xml.NameTable);
                        manager.AddNamespace("dat", "http://www.stormware.cz/schema/version_2/data.xsd");

                        int count = 0;
                        foreach (var item in match)
                        {
                            string request = "/dat:dataPack/dat:dataPackItem[@id=\"" + item + "\"]";
                            try
                            {
                                foreach (XmlNode node in xml.SelectNodes(request, manager))
                                {
                                    node.ParentNode.RemoveChild(node);
                                    count++;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        xml.Save(this.filePath.Text);
                        MessageBox.Show("Erased " + count + " items.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\nPlease check request XPath syntax.", "Error", MessageBoxButton.OK);
                    }
                }
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }

            //if (this.tbRequest.Text != string.Empty)
            //{
            //    try
            //    {
            //        XPathExpression.Compile(tbRequest.Text);
            //        Properties.Settings.Default.Save();

            //        XmlDocument xml = new XmlDocument();
            //        xml.Load(this.filePath.Text);
            //        xml.Save(this.filePath.Text + ".backup");

            //        foreach (XmlNode item in xml.SelectNodes(this.tbRequest.Text))
            //        {
            //            item.ParentNode.RemoveChild(item);
            //        }

            //        xml.Save(this.filePath.Text);
            //    }
            //    catch(Exception ex)
            //    {
            //        MessageBox.Show(ex.Message + "\nPlease check request XPath syntax.", "Error", MessageBoxButton.OK);
            //    }
            //}
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "XML files (.xml)|*.xml";
            ofd.Multiselect = false;
            if (System.IO.File.Exists(this.filePath.Text))
            {
                ofd.FileName = System.IO.Directory.GetParent(this.filePath.Text).FullName;
            }
            if (ofd.ShowDialog().Value)
            {
                Properties.Settings.Default.FilePath = this.filePath.Text = ofd.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void openPdf_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Pdf files (.pdf)|*.pdf";
            ofd.Multiselect = false;
            if (System.IO.File.Exists(this.pdfPath.Text))
            {
                ofd.FileName = System.IO.Directory.GetParent(this.pdfPath.Text).FullName;
            }
            if (ofd.ShowDialog().Value)
            {
                Properties.Settings.Default.PdfPath = this.pdfPath.Text = ofd.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
