using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Convert_to_WPF
{
    public class cLabel : cBase
    {
    //**********************************************************************************
        public static bool findControl(string line)
        {
        //******************************************************************************
        // Does this string contain a definition for this control.
        //******************************************************************************
            if (line.Contains("new System.Windows.Forms.Label();")) return true;
        //******************************************************************************
            return false;
        }
    //**********************************************************************************
        public override string type
        {
            get { return "Label";        }
        }
    //**********************************************************************************
        public cLabel(string line, string filename) : base(line, filename)
        {
        }
    //**********************************************************************************
        public override bool convert(ref string line)
        {
            if (line.Contains(name + ".Text"))
            {
                line = line.Replace(name + ".Text", name + ".Content");
                return true;
            }
            return false;
        }
    //**********************************************************************************
        public override void Settings(string t)
        {
        //******************************************************************************
        // Parse our base settings - nothing unique needed for this control.
        //******************************************************************************
            base.Settings(t);
        }
    //**********************************************************************************
        public override void xaml(StreamWriter w, Size pSize)
        {
        //******************************************************************************
        // Note: There is a significant difference between WPF and Form for labels.
        //       Need to adjust font settings for label text to show up...
        //******************************************************************************
            fontfamily         = "Microsoft Sans Serif";
            fontsize           = "7.8";
            string o = "\t\t<Label ";
        //******************************************************************************
        // Write base XAML attributes and margins...
        //******************************************************************************
            o += xamlAttributes();
            o += margins(pSize);
            o += ">";
        //******************************************************************************
        // Add any text
        //******************************************************************************
            o += XAML.encodeText(text);
            o += "</Label>";
        //******************************************************************************
        // Close out control and write the line...
        //******************************************************************************
            w.WriteLine(o);
        }
    }
}
