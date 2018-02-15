using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
//**************************************************************************************
namespace Convert_to_WPF
{
    public class cCheckbox : cBase
    {
    //**********************************************************************************
    // Events and properties specific to this control
    //**********************************************************************************
        public string    checkEvent         = String.Empty;
        public bool      IsChecked          = false;
        public bool      IsThreeState       = false;
    //**********************************************************************************
        public static bool findControl(string line)
        {
        //******************************************************************************
        // Does this string contain a definition for this control.
        //******************************************************************************
            if (line.Contains("new System.Windows.Forms.CheckBox();")) return true;
        //******************************************************************************
            return false;
        }
    //**********************************************************************************
        public override string type
        {
            get { return "Checkbox";        }
        }
    //**********************************************************************************
        public cCheckbox(string line, string filename) : base(line, filename)
        {
        }
    //**********************************************************************************
        public override void Settings(string t)
        {
        //******************************************************************************
        // Parse our base settings
        //******************************************************************************
            base.Settings(t);
        //******************************************************************************
        // Parse settings specific to this control
        //******************************************************************************
            int     index;
            string  search_text;
        //******************************************************************************
            search_text = "this." + name + ".Checked = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                IsChecked = Convert.ToBoolean(vText);
            }
        //******************************************************************************
            search_text = "this." + name + ".ThreeState = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                IsThreeState = Convert.ToBoolean(vText);
            }
        //******************************************************************************
            checkEvent = base.parseEvent(name, "CheckedChanged", t);
        }
    //**********************************************************************************
        public override void xaml(StreamWriter w, Size pSize)
        {
            string o = "\t\t<CheckBox ";
        //******************************************************************************
        // Write base XAML attributes and margins...
        //******************************************************************************
            o += xamlAttributes();
            o += margins(pSize);
        //******************************************************************************
        // Write specific control settings...
        //******************************************************************************
            if (IsChecked)
                o += "IsChecked=\"True\" ";
            if (IsThreeState)
                o += "IsThreeState=\"True\" ";

            if (checkEvent != String.Empty)
                o += "Checked=\"" + checkEvent + "\" ";
            o += ">";
        //******************************************************************************
        // Add any text
        //******************************************************************************
            o += XAML.encodeText(text);
        //******************************************************************************
        // Close out control and write the line...
        //******************************************************************************
            o += "</CheckBox>";
            w.WriteLine(o);
        }
    }
}
