using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
//**************************************************************************************
namespace Convert_to_WPF
{
    public class cTextBox : cBase
    {
    //**********************************************************************************
    // Events and properties specific to this control
    //**********************************************************************************
        public bool      IsReadOnly          = false;
        public bool      AcceptsReturn       = false;
        public bool      RighttoLeft         = false;
        public bool      WordWrap            = true;
        public string    CharacterCasing     = String.Empty;
    //**********************************************************************************
        public static bool findControl(string line)
        {
        //******************************************************************************
        // Does this string contain a definition for this control.
        //******************************************************************************
            if (line.Contains("new System.Windows.Forms.TextBox();")) return true;
        //******************************************************************************
            return false;
        }
    //**********************************************************************************
        public override string type
        {
            get { return "TextBox";        }
        }
    //**********************************************************************************
        public cTextBox(string line, string filename) : base(line, filename)
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
            search_text = "this." + name + ".ReadOnly = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                IsReadOnly = Convert.ToBoolean(vText);
            }
        //******************************************************************************
            search_text = "this." + name + ".AcceptsReturn = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                AcceptsReturn = Convert.ToBoolean(vText);
            }
        //******************************************************************************
            search_text = "this." + name + ".CharacterCasing = System.Windows.Forms.CharacterCasing.";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(";", index) - index;
                string vText = t.Substring(index + 1, len);
                CharacterCasing = vText;
            }
        //******************************************************************************
            search_text = "this." + name + ".RightToLeft = System.Windows.Forms.RightToLeft.";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(";", index) - index;
                string vText = t.Substring(index + 1, len);
                if (vText == "Yes") RighttoLeft = true;
            }
        //******************************************************************************
            search_text = "this." + name + ".WordWrap = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                WordWrap = Convert.ToBoolean(vText);
            }
        }
    //**********************************************************************************
        public override void xaml(StreamWriter w, Size pSize)
        {
            string o = "\t\t<TextBox ";
        //******************************************************************************
        // Write base XAML attributes and margins...
        //******************************************************************************
            o += xamlAttributes();
            o += margins(pSize);
        //******************************************************************************
        // Write specific control settings...
        //******************************************************************************
            if (IsReadOnly)
                o += "IsReadOnly=\"True\" ";
            if (AcceptsReturn)
                o += "AcceptsReturn=\"True\" ";
            if (WordWrap)
                o += "TextWrapping=\"Wrap\" ";

            if (RighttoLeft)
                o += "FlowDirection=\"RightToLeft\" ";

            if (CharacterCasing != String.Empty)
                o += "CharacterCasing=\"" + CharacterCasing + "\" ";
            
            o += ">";
        //******************************************************************************
        // Add any text
        //******************************************************************************
            o += XAML.encodeText(text);
        //******************************************************************************
        // Close out control and write the line...
        //******************************************************************************
            o += "</TextBox>";
            w.WriteLine(o);
        }
    }
}
