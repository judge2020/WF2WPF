using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Convert_to_WPF
{
    public class cDataGridView : cBase
    {
        public ArrayList items                      = new ArrayList();

        public static bool findControl(string line)
        {
            if (line.Contains("new System.Windows.Forms.DataGridView();")) return true;
            return false;
        }
        public override string type
        {
            get { return "DataGridView";        }
        }
        public cDataGridView(string line, string filename) : base(line, filename)
        {
        }
        public override void Settings(string t)
        {
            base.Settings(t);
        }
        public override void xaml(StreamWriter w, Size pSize)
        {
            string o = "\t\t<Grid ";
            o += xamlAttributes();
            o += margins(pSize);
            if (items.Count == 0)
                o += "/>";
            else
            {
                o += ">\n";
                o += "\t\t</Grid>";
            }
            w.WriteLine(o);
        }
    }
}
