using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Convert_to_WPF
{
    public class cButton : cBase
    {
    //**********************************************************************************
        public static bool findControl(string line)
        {
        //******************************************************************************
        // Does this string contain a definition for this control.
        //******************************************************************************
            if (line.Contains("new System.Windows.Forms.Button();")) return true;
        //******************************************************************************
            return false;
        }
    //**********************************************************************************
        public override string type
        {
            get { return "Button";        }
        }
    //**********************************************************************************
        public cButton(string line, string filename) : base(line, filename)
        {
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
            string o = "\t\t<Button ";
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
        //******************************************************************************
        // Close out control and write the line...
        //******************************************************************************
            o += "</Button>";
            w.WriteLine(o);
        }
    }

}
