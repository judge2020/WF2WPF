using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Convert_to_WPF
{
//**************************************************************************************
    public partial class mainForm : Form
    {
    //**********************************************************************************
        FileInfo        source_project      = null;
        FileInfo        destination_project = null;
        ArrayList       projects            = new ArrayList();
    //**********************************************************************************
        public mainForm()
        {
            InitializeComponent();
        }
    //**********************************************************************************
        private void bSourceProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg   = new OpenFileDialog();
        //******************************************************************************
            dlg.Filter           = Properties.Settings.Default.OpenFilter;
            dlg.FilterIndex      = Properties.Settings.Default.OpenFilterCount;
            dlg.RestoreDirectory = true;
        //******************************************************************************
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                source_project = new FileInfo(dlg.FileName);
                tbSource.Text  = dlg.FileName;
            //**************************************************************************
                if (tbDest.Text.Length == 0)
                {
                    string dest_dir = source_project.DirectoryName;
                //**********************************************************************
                    dest_dir += "_WPF\\";
                    tbDest.Text = dest_dir + source_project.Name;
                    destination_project = new FileInfo(tbDest.Text);
                }
            }
        }
    //**********************************************************************************
        private void bConvert_Click(object sender, EventArgs e)
        {
            bool project_error = false;
        //******************************************************************************
        // Clear out any previous data...
        //******************************************************************************
            rtbOutput.Text = String.Empty;
            projects.Clear();
        //******************************************************************************
        // Make sure source and destination exist...
        //******************************************************************************
            if (tbSource.Text.Trim().Length == 0 ||
                tbDest.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please enter source and destination");
                return;
            }
        //******************************************************************************
        // Read project file
        //   At this level we only care about top level elements in the file (projects)
        //******************************************************************************
            output("Parsing project file");
            try
            {
                Project p = parseProject(source_project.FullName);
            //**************************************************************************
            // Check if project had an error...
            //**************************************************************************
                if (p.status != String.Empty)
                {
                    output(p.status);
                    output("Done.");
                    return;
                }
            }
            catch (Exception ex)
            {
                output("Could not parse file: " + ex.Message);
                output("Done.");
                return;
            }
        //******************************************************************************
        // Give the user some feedback...
        //******************************************************************************
            output(String.Format("Projects: {0:N0}", projects.Count));
        //******************************************************************************
        // Create destination directory if it doesn't exist
        //******************************************************************************
            try
            {
                if (destination_project.Directory.Exists)
                {
                    output("Output directory exists - deleting");
                    output(destination_project.DirectoryName);
                    try
                    {
                        destination_project.Directory.Delete(true);
                    }
                    catch(Exception ex)
                    {
                        output("Error deleting directory: " + ex.Message);
                        output("Aborting");
                        return;
                    }
                }
                output("Creating output directory: ");
                output(destination_project.DirectoryName);
                destination_project.Directory.Create();
                output("Directory created");
            }
            catch (Exception ex)
            {
                output("Could not create output directory: " + ex.Message);
                output("Done.");
                return;
            }
        //******************************************************************************
        // Output project file
        //******************************************************************************
            output("Creating new Visual C# project");
            try
            {
                create_WPF_Project();
            }
            catch(Exception ex)
            {
                output("Could not convert file: " + ex.Message);
            }
            output("Done.");
        }
    //**********************************************************************************
        private Project parseProject(string filename)
        {
            bool          project_error = false;
            XmlTextReader reader        = new XmlTextReader(filename);
            Project       p             = new Project("dummy");
                                                     
            p.status = "Undefined project";
            while (reader.Read() && project_error == false) 
            { 
                switch (reader.NodeType) 
                { 
                    case XmlNodeType.Element:
                    {
                    //******************************************************************
                    // Create a configuration node and parse children
                    //******************************************************************
                        p = new Project(reader.Name);
                        p.parse(reader);
                    //******************************************************************
                    // Add to our projects list
                    //******************************************************************
                        project_error = analyzeProject(p);
                        projects.Add(p);
                        break; 
                    }
                } 
            }
            reader.Close();
            return p;
        }
    //**********************************************************************************
        private bool analyzeProject(Project project)
        {
            bool      project_error     = false;
            ArrayList designer_files    = new ArrayList();
        //******************************************************************************
        // Search through children nodes...
        //******************************************************************************
            foreach (cfgNode c in project.children)
            {
                if (project_error) break;               // stop parsing
                if (c.name != "ItemGroup") continue;
            //**************************************************************************
            // Found "ItemGroup" node
            //**************************************************************************
                foreach (cfgNode item in c.children)
                {
                //**********************************************************************
                // Look for source files and WPF indicators...
                //**********************************************************************
                    switch (item.name)
                    {
                    //******************************************************************
                    // #TODO - need to determine files in a project to copy.
                    //******************************************************************
                        case "None":
                        case "Content":
                        case "EmbeddedResource":
                        //**************************************************************
                        // Find the source file name...
                        //**************************************************************
                            foreach (attribute a in item.attributes)
                            {
                                if (a.name == "Include")
                                    project.files.Add(a.value);
                            }
                            break;
                    //******************************************************************
                    // Compile statements refer to source files...
                    //******************************************************************
                        case "Compile":
                            string filename = String.Empty;
                        //**************************************************************
                        // Find the source file name...
                        //**************************************************************
                            foreach (attribute a in item.attributes)
                            {
                                if (a.name == "Include")
                                {
                                    filename = a.value;
                                //******************************************************
                                // Add project file...
                                //******************************************************
                                    if (project.addSourceFile(filename) == false)
                                    {
                                        project_error = true;
                                        break;
                                    }
                                //******************************************************
                                // Is it a designer file?
                                //******************************************************
                                    if (a.value.Contains(".Designer.") &&
                                        a.value.Contains("\\") == false)    // skip subdirectories
                                        designer_files.Add(a.value);
                                }
                            }
                        //**************************************************************
                        // Determine if this a "Form" source file
                        //   - Node contains a "SubType" element of "Form"
                        //**************************************************************
                            foreach (cfgNode s in item.children)
                            {
                                if (s.name == "SubType")
                                {
                                    if (s.text == "Form")
                                    {
                                        cForm form = new cForm();
                                        form.definition_file = filename;
                                        project.forms.Add(form);
                                    }
                                }
                            }
                            break;
                    //******************************************************************
                    // Check if WPF components already in Project file...
                    //******************************************************************
                        case "Page":
                        case "ApplicationDefinition":
                            project.status = "Contains WPF project - cannot convert";
                            project_error  = true;
                            break;
                    }
                }
            }
        //******************************************************************************
        // Try to match designer files with Forms...
        //******************************************************************************
            foreach (cForm f in project.forms)
            {
                string df = Project.formFile(f.definition_file, ".Designer.cs");
                foreach (string s in designer_files)
                {
                    if (df == s)
                    {
                        f.designer_file = s;
                        break;
                    }
                }
            }
        //******************************************************************************
        // Return WPF project status
        //******************************************************************************
            return project_error;
        }
    //**********************************************************************************
        private void createForm(cForm form)
        {
            string          sText;
            string          fn          = source_project.DirectoryName + "\\" + 
                                          form.designer_file;
            StreamReader    file        = new System.IO.StreamReader(fn);
            StringBuilder   source_text = new StringBuilder();
            string          filename    = form.definition_file;
        //******************************************************************************
        // Walk through the designer file looking for known controls
        //******************************************************************************
            while(file.EndOfStream == false)
            {
                string line = file.ReadLine();
                if (line == null) break;
                source_text.Append(line);
                if (cButton.findControl(line))              // Button Control
                {
                    cButton b = new cButton(line, filename);
                    form.controls.Add(b);
                }
                else if (cLabel.findControl(line))          // Label Control
                {
                    cLabel b = new cLabel(line, filename);
                    form.controls.Add(b);
                } 
                else if (cListBox.findControl(line))        // ListBox Control
                {
                    cListBox b = new cListBox(line, filename);
                    form.controls.Add(b);
                } 
                else if (cTextBox.findControl(line))        // TextBox Control
                {
                    cTextBox b = new cTextBox(line, filename);
                    form.controls.Add(b);
                } 
                else if (cComboBox.findControl(line))       // ComboBox Control
                {
                    cComboBox b = new cComboBox(line, filename);
                    form.controls.Add(b);
                } 
                else if (cCheckbox.findControl(line))       // CheckBox Control
                {
                    cCheckbox b = new cCheckbox(line, filename);
                    form.controls.Add(b);
                } 
                else if (cRichTextBox.findControl(line))    // RichTextBox Control
                {
                    cRichTextBox b = new cRichTextBox(line, filename);
                    form.controls.Add(b);
                } 
                else if (cDataGridView.findControl(line))   // DataGridView Control
                {
                    cDataGridView b = new cDataGridView(line, filename);
                    form.controls.Add(b);
                } 
            }
        //******************************************************************************
            file.Close();
        //******************************************************************************
        // Create a string of the source code file
        //   This is so we don't have to read the file again while parsing settings.
        //******************************************************************************
            sText = source_text.ToString();
            form.Settings(sText);
        //******************************************************************************
        // Parse the control settings
        //******************************************************************************
            foreach (cBase b in form.controls)
                b.Settings(sText);
        }
    //**********************************************************************************
        private void output(string text)
        {
            rtbOutput.Text += text + "\n";
        }
    //**********************************************************************************
        private void create_WPF_Project()
        {
            XmlTextWriter w = new XmlTextWriter(destination_project.FullName,
                                                Encoding.UTF8);
            XmlDocument d = new XmlDocument();
            int         i = 1;
            foreach (Project p in projects)
            {
                output("Create project: " + i++);
            //**************************************************************************
            // Parse the designer files - builds form object module...
            //**************************************************************************
                foreach(cForm f in p.forms)
                {
                    createForm(f);
                    string filename = destination_project.DirectoryName + "\\" +
                                      Project.formFile(f.definition_file, ".xaml");
                    XAML.output(f, filename);
                    Conversion.convertFormFile(f, source_project, destination_project);
                    p.ns = f.sNameSpace;
                }
            //**************************************************************************
                Conversion.convertConfiguration(p);
                XmlNode n = addNode(d, p);
                d.AppendChild(n);
                copyProjectfiles(p);
            }                
            w.Formatting = Formatting.Indented;
            w.WriteStartDocument();
            d.Save(w);
            w.Close();
        }
    //**********************************************************************************
        private void copyProjectfiles(Project p)
        {
        //******************************************************************************
        // Make sure there are forms in this project
        //******************************************************************************
            if (p.forms.Count == 0)
            {
            //#TODO - return error
            }
            else 
            {
            //****************************************************************************
            // Select first form as the default window.
            //   Improvement - ask user which form or try to determine it.
            //****************************************************************************
                if (p.forms.Count > 0)
                {
                    cForm f = (cForm)p.forms[0];
                    p.default_form = Project.formFile(f.definition_file, ".xaml");
                }
            }
        //******************************************************************************
        // Walk project files and output to new project file...            
        //******************************************************************************
            foreach (string f in p.files)
            {
            //**************************************************************************
            // skip form files
            //**************************************************************************
                if (p.isForm(f)) continue;      
            //**************************************************************************
            // skip only form-based designer files
            //**************************************************************************
                if (f.Contains(".Designer.cs") && 
                    f.Contains("Properties\\") == false) continue;   
            //**************************************************************************
                string   sfn = source_project.DirectoryName + "//" + f;
                string   dfn = destination_project.DirectoryName + "//" + f;
                FileInfo sfi = new FileInfo(sfn);
                FileInfo dfi = new FileInfo(dfn);
            //**************************************************************************
            // Copy this file to destination - also make sure the directory exists.
            //**************************************************************************
                if (dfi.Directory.Exists == false)
                    dfi.Directory.Create();
                sfi.CopyTo(dfi.FullName, true);
            }
        //******************************************************************************
        // Create new XAML application files based on our default files
        //******************************************************************************
            change_default_ns(p, ".\\App.xaml.cs.txt", 
                              destination_project.DirectoryName + "\\App.xaml.cs");
            change_default_ns(p, ".\\App.xaml.txt", 
                              destination_project.DirectoryName + "\\App.xaml");
        }
    //**********************************************************************************
        private void change_default_ns(Project p, string iFile, string oFile)
        {
            StreamReader r = new StreamReader(iFile);
            StreamWriter w = new StreamWriter(oFile);
        //******************************************************************************
        // Walk through file and replacing default information with project info
        //******************************************************************************
            while (r.EndOfStream == false)
            {
                string line = r.ReadLine();
            //**************************************************************************
                line = line.Replace("WpfApplication1", p.ns);
                line = line.Replace("StartupUri=\"Window1.xaml",
                                    "StartupUri=\"" + p.default_form);
            //**************************************************************************
                w.WriteLine(line);
            }
        //******************************************************************************
            r.Close();
            w.Close();
        }
    //**********************************************************************************
        private XmlNode addNode(XmlDocument doc, cfgNode cn)
        {
        //******************************************************************************
        // Add an internal configuration node (cfgNode) to XML document
        //******************************************************************************
            XmlNode n = doc.CreateElement(cn.name);
        //******************************************************************************
        // Add text
        //******************************************************************************
            if (cn.text != String.Empty)
                n.InnerText = cn.text;
        //******************************************************************************
        // Add any attributes
        //******************************************************************************
            foreach (attribute a in cn.attributes)
            {
                XmlAttribute attr = doc.CreateAttribute(a.name);
            //**************************************************************************
                attr.Value = a.value;
                n.Attributes.Append(attr);
            }
        //******************************************************************************
        // Add children - recursive
        //******************************************************************************
            foreach (cfgNode child in cn.children)
            {
                XmlNode childNode = addNode(doc, child);
            //**************************************************************************
                n.AppendChild(childNode);
            }
        //******************************************************************************
        // Return node...
        //******************************************************************************
            return n;
        }
    }
}
