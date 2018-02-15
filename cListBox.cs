using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Convert_to_WPF
{
    public class cListBox : cBase
    {
    //**********************************************************************************
    // Events and properties specific to this control
    //**********************************************************************************
        public ArrayList items                      = new ArrayList();
        public string    selectionMode              = String.Empty;
        public string    selectedindexchangedEvent  = String.Empty;
    //**********************************************************************************
        public static bool findControl(string line)
        {
        //******************************************************************************
        // Does this string contain a definition for this control.
        //******************************************************************************
            if (line.Contains("new System.Windows.Forms.ListBox();")) return true;
        //******************************************************************************
            return false;
        }
    //**********************************************************************************
        public override string type
        {
            get { return "ListBox";        }
        }
    //**********************************************************************************
        public cListBox(string line, string filename) : base(line, filename)
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
            search_text = "this." + name + ".Items.AddRange(new object[] {";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(");", index) - (index + 2); // skip trailing "))"
                string vText = t.Substring(index, len);
                string[] v = vText.Split(',');
                foreach (string s in v)
                    items.Add(s.Replace("\"","").Trim());
            }
        //******************************************************************************
            search_text = "this." + name + ".SelectionMode = System.Windows.Forms.SelectionMode.";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(";", index) - index;
                string vText = t.Substring(index + 1, len);
                selectionMode = vText;
            }
        //******************************************************************************
            selectedindexchangedEvent = base.parseEvent(name, "SelectedIndexChanged", t);
        }
    //**********************************************************************************
        public override void xaml(StreamWriter w, Size pSize)
        {
            string o = "\t\t<ListBox ";
        //******************************************************************************
        // Write base XAML attributes and margins...
        //******************************************************************************
            o += xamlAttributes();
            o += margins(pSize);
        //******************************************************************************
        // Write specific control settings...
        //******************************************************************************
            if (selectionMode != String.Empty)
                o += "SelectionMode=\"" + selectionMode + "\" ";
            if (selectedindexchangedEvent != String.Empty)
                o += "SelectionChanged=\"" + selectedindexchangedEvent + "\" ";
        //******************************************************************************
        // Control contains child items - if there are none just close
        //******************************************************************************
            if (items.Count == 0)
                o += "/>";
            else
            {
                o += ">\n";     // terminate XML statement
            //**************************************************************************
            // Add child items
            //**************************************************************************
                foreach (string item in items)
                {
                    o += "\t\t\t<ListBoxItem>";
                    o += XAML.encodeText(item);
                    o += "</ListBoxItem>";
                    o += "\n";
                }
            //**************************************************************************
            // Close control
            //**************************************************************************
                o += "\t\t</ListBox>";
            }
        //******************************************************************************
            w.WriteLine(o);
        }
    }
}
