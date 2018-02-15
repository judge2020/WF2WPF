using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;
//**************************************************************************************
namespace Convert_to_WPF
{
    public class XAML
    {
        public static void output(cForm form, string outputfile)
        {
            Size         s = new Size();
            StreamWriter w = new StreamWriter(outputfile);
        //******************************************************************************
        // WPF has a different width - adjust our panel width
        //******************************************************************************
            s        = form.size;
            s.Width += 18;
        //******************************************************************************
        // Write the XAML header information plus window specifications
        //******************************************************************************
            w.WriteLine("<Window x:Class=\"" + form.sNameSpace + "." + form.name + "\"");
            w.WriteLine("\txmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            w.WriteLine("\txmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            w.WriteLine("\tTitle=\"" + encodeText(form.text) + "\" Height=\"" + s.Height + 
                        "\" Width=\"" + s.Width + "\">");
        //******************************************************************************
        // Create a Grid and add each control we found...
        //******************************************************************************
            w.WriteLine("\t<Grid>");
            foreach (cBase b in form.controls) b.xaml(w, s);
            w.WriteLine("\t</Grid>");
        //******************************************************************************
        // Close up and return
        //******************************************************************************
            w.WriteLine("</Window>");
            w.Close();
        }
        public static string encodeText(string text)
        {
            XmlDocument doc = new XmlDocument();
        //******************************************************************************
        // Easiest way to encode text is create an XML node and return encoding
        //******************************************************************************
            doc.PreserveWhitespace = true;
            XmlNode node           = doc.CreateNode(XmlNodeType.Element, 
                                                    "xmlencoder", null);
            node.InnerText         = text;
        //******************************************************************************
            return node.InnerXml;
        }

    }
}
