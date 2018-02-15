using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
//**************************************************************************************
namespace Convert_to_WPF
{
    public class cForm : cBase
    {
    //**********************************************************************************
    // Events and properties specific to this control
    //**********************************************************************************
        public string    sNameSpace    = String.Empty;
        public string    designer_file = String.Empty;
        public ArrayList controls      = new ArrayList();
    //**********************************************************************************
        public override string type
        {
            get { return "Form";        }
        }
    //**********************************************************************************
        public override void Settings(string t)
        {
            int     index;
            string  search_text;
        //******************************************************************************
        // Parse settings specific to this control *i.e. Form)
        //******************************************************************************
            search_text = "namespace ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf('{', index) - index;
                string vText = t.Substring(index, len);
                sNameSpace = vText;
            }
        //******************************************************************************
            search_text = "this.Location = new System.Drawing.Point(";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                int x;
                int y;
                index = index + search_text.Length;
                int len = t.IndexOf(')', index) - index;
                string vText = t.Substring(index, len);
                string[] v = vText.Split(',');
                x = Convert.ToInt32(v[0]);
                y = Convert.ToInt32(v[1]);
                location = new Point(x, y);
            }
        //******************************************************************************
            search_text = "this.ClientSize = new System.Drawing.Size(";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                int x;
                int y;
                index = index + search_text.Length;
                int len = t.IndexOf(')', index) - index;
                string vText = t.Substring(index, len);
                string[] v = vText.Split(',');
                x = Convert.ToInt32(v[0]);
                y = Convert.ToInt32(v[1]);
                size = new Size(x, y);
            }
        //******************************************************************************
            search_text = "this.Text = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf("\";", index) - (index + 1);
                string vText = t.Substring(index + 1, len);
                text = vText;
            }
        //******************************************************************************
            search_text = "this.Name = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf("\";", index) - (index + 1);
                string vText = t.Substring(index + 1, len);
                name = vText;
            }
        }
    //**********************************************************************************
        public string convert(string line)
        {
            string r = line;
        //******************************************************************************
        // Walk through form's controls
        //******************************************************************************
            foreach (cBase b in controls)
            {
                if (b.convert(ref r)) break;    // did a conversion - return...
            }
        //******************************************************************************
            return r;
        }
    }
}
