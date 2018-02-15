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
    public class Conversion
    {
    //**********************************************************************************
    // This method will walk through the base Form source file and convert it to
    // the XAML source file equivalent.
    //**********************************************************************************
        public static void convertFormFile(cForm form, FileInfo source_project,
                                                       FileInfo destination_project)
        {
            string src = source_project.DirectoryName + "\\" + form.definition_file;
            string dst = destination_project.DirectoryName + "\\" + 
                         Project.formFile(form.definition_file, ".xaml.cs");
        //******************************************************************************
            StreamReader r        = new StreamReader(src);
            StreamWriter w        = new StreamWriter(dst);
            bool         found_ns = false;
        //******************************************************************************
        // Walk through source file
        //******************************************************************************
            while (r.EndOfStream == false)
            {
                string line = r.ReadLine();
            //**************************************************************************
            // Look for MessageBox from Windows.Forms
            //**************************************************************************
                if (line.Contains(" MessageBox.Show("))
                {
                //**********************************************************************
                // Add System.Windows.Forms prefix.
                //**********************************************************************
                    line = line.Replace(" MessageBox.Show(", " System.Windows.Forms.MessageBox.Show(");
                    w.WriteLine(line);
                }
            //**************************************************************************
            // Look for DialogResult from Windows.Forms
            //**************************************************************************
                else if (line.Contains(" DialogResult."))
                {
                //**********************************************************************
                // Add System.Windows.Forms prefix.
                //**********************************************************************
                    line = line.Replace(" DialogResult.", " System.Windows.Forms.DialogResult.");
                    w.WriteLine(line);
                }
            //**************************************************************************
            // Look for namespace declaration
            //**************************************************************************
                else if (line.Contains("namespace " + form.sNameSpace) && 
                         found_ns == false)
                {
                //**********************************************************************
                // Add WPF references...
                //**********************************************************************
                    w.WriteLine("using System.Windows;");
                    w.WriteLine("using System.Windows.Controls;");
                    w.WriteLine("using System.Windows.Data;");
                    w.WriteLine("using System.Windows.Documents;");
                    w.WriteLine("using System.Windows.Input;");
                    w.WriteLine("using System.Windows.Media;");
                    w.WriteLine("using System.Windows.Media.Imaging;");
                    w.WriteLine("using System.Windows.Navigation;");
                    w.WriteLine("using System.Windows.Shapes;");
                //**********************************************************************
                // Write namespace declaration out also...
                //**********************************************************************
                    w.WriteLine();
                    w.WriteLine(line);
                    found_ns = true;
                }
            //**************************************************************************
            // Look for class definition
            //**************************************************************************
                else if (line.Contains("partial class " + form.name))
                {
                    int len = line.IndexOf(':');
                //**********************************************************************
                // Change "Form" parent class to "Window" parent class
                //**********************************************************************
                    line = line.Substring(0, len);
                    line += ": Window";
                //**********************************************************************
                // Write out line...
                //**********************************************************************
                    w.WriteLine(line);
                }
            //**************************************************************************
            // Just write out line...
            //**************************************************************************
                else 
                {
                    string nl = form.convert(line);
                    w.WriteLine(nl);
                }
            }
        //******************************************************************************
            r.Close();
            w.Close();
        }
    //**********************************************************************************
    // This method creates a new configuration file - csproj - for WPF.
    //**********************************************************************************
        public static void convertConfiguration(Project project)
        {
        //******************************************************************************
        // Walk through the project children
        //******************************************************************************
            foreach (cfgNode c in project.children)
            {
            //**************************************************************************
            // ItemGroup - contains references and source files
            //**************************************************************************
                if (c.name == "ItemGroup")
                {
                    bool      add_xaml_code = false;
                    bool      add_xaml_ref  = false;
                    ArrayList remove        = new ArrayList();
                //**********************************************************************
                // Look for statements we need to convert
                //**********************************************************************
                    foreach (cfgNode item in c.children)
                    {
                        switch (item.name)
                        {
                            case "Reference":   // Reference group
                            //**********************************************************
                            // add WPF references...
                            //**********************************************************
                                add_xaml_ref = true;
                                break;
                            case "Compile":     // Source group
                            //**********************************************************
                            // remove Form files and add WPF files/defs
                            //**********************************************************
                                add_xaml_code = true;
                                if (item.attributes.Count > 0)
                                {
                                    attribute a  = (attribute)item.attributes[0];
                                //******************************************************
                                // Skip any property files...
                                //******************************************************
                                    if (a.value.Contains("Properties\\")) break;
                                //******************************************************
                                // Check for files to remove...
                                //******************************************************
                                    if (a.value == "Program.cs" ||
                                        a.value.Contains(".Designer.cs"))
                                    {
                                        remove.Add(item);
                                    }
                                //******************************************************
                                // Also remove any form sources - already converted to 
                                // XAML equivalent
                                //******************************************************
                                    foreach (cfgNode cn in item.children)
                                    {
                                        if (cn.name == "SubType" &&
                                            cn.text == "Form")
                                        {
                                            remove.Add(item);
                                            break;
                                        }
                                    }
                                }
                                break;
                            case "EmbeddedResource":    // Resource
                            //**********************************************************
                            // remove Form resources
                            //**********************************************************
                                foreach (attribute a in item.attributes)    
                                {
                                    if (project.isFormResx(a.value))
                                    {
                                        remove.Add(item);
                                    }
                                }

                                break;
                        }
                    }
                //**********************************************************************
                // Remove the items we found...
                //**********************************************************************
                    foreach (cfgNode item in remove)
                        c.children.Remove(item);
                //**********************************************************************
                // Add code and references for WPF/XAML
                //**********************************************************************
                    if (add_xaml_code) addXAMLcode(c, project);
                    if (add_xaml_ref)  addXAMLreferences(c);
                }
            }
        }
    //**********************************************************************************
        private static void addXAMLreferences(cfgNode itemGroup)
        {
            cfgNode   n;
            attribute attr;
        //******************************************************************************
        // Add references to support XAML and WPF
        //******************************************************************************
            n          = new cfgNode("Reference");
            attr       = new attribute();
            attr.name  = "Include";
            attr.value = "WindowsBase";
            n.attributes.Add(attr);
            itemGroup.children.Add(n);

            n          = new cfgNode("Reference");
            attr       = new attribute();
            attr.name  = "Include";
            attr.value = "PresentationCore";
            n.attributes.Add(attr);
            itemGroup.children.Add(n);

            n          = new cfgNode("Reference");
            attr       = new attribute();
            attr.name  = "Include";
            attr.value = "PresentationFramework";
            n.attributes.Add(attr);
            itemGroup.children.Add(n);
        }
    //**********************************************************************************
        private static void addXAMLcode(cfgNode itemGroup, Project p)
        {
            int       i = 0;
            cfgNode   n;
            cfgNode   cn;
            attribute attr;
        //******************************************************************************
        // Add Application definition files
        //******************************************************************************
            n          = new cfgNode("ApplicationDefinition");
            attr       = new attribute();
            attr.name  = "Include";
            attr.value = "App.xaml";
            n.attributes.Add(attr);
            cn         = new cfgNode("Generator");
            cn.text    = "MSBuild:Compile";
            n.children.Add(cn);
            cn         = new cfgNode("SubType");
            cn.text    = "Designer";
            n.children.Add(cn);
            itemGroup.children.Insert(i++, n);

            n          = new cfgNode("Compile");
            attr       = new attribute();
            attr.name  = "Include";
            attr.value = "App.xaml.cs";
            n.attributes.Add(attr);
            cn         = new cfgNode("DependentUpon");
            cn.text    = "App.xaml";
            n.children.Add(cn);
            cn         = new cfgNode("SubType");
            cn.text    = "Code";
            n.children.Add(cn);
            itemGroup.children.Insert(i++, n);
        //******************************************************************************
        // Add converted form source files...
        //******************************************************************************
            foreach (cForm f in p.forms)
            {
            //**************************************************************************
            // XAML definition - replaces designer file
            //**************************************************************************
                n          = new cfgNode("Page");
                attr       = new attribute();
                attr.name  = "Include";
                attr.value = Project.formFile(f.definition_file, ".xaml");
                n.attributes.Add(attr);
                cn         = new cfgNode("Generator");
                cn.text    = "MSBuild:Compile";
                n.children.Add(cn);
                cn         = new cfgNode("SubType");
                cn.text    = "Designer";
                n.children.Add(cn);
                itemGroup.children.Insert(i++, n);
            //**************************************************************************
            // CS file - replaces form source file
            //**************************************************************************
                n          = new cfgNode("Compile");
                attr       = new attribute();
                attr.name  = "Include";
                attr.value = Project.formFile(f.definition_file, ".xaml.cs");
                n.attributes.Add(attr);
                cn         = new cfgNode("DependentUpon");
                cn.text    = Project.formFile(f.definition_file, ".xaml");
                n.children.Add(cn);
                cn         = new cfgNode("SubType");
                cn.text    = "Code";
                n.children.Add(cn);
                itemGroup.children.Insert(i++, n);
            }
        }
    }
}
