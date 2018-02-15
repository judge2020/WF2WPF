using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Convert_to_WPF
{
    public class cBase
    {
    //**********************************************************************************
    // Default properties - applies to all controls
    //**********************************************************************************
        public string    definition_file    = String.Empty;
        public string    name               = String.Empty;
        public Point     location           = new Point();
        public Size      size               = new Size();
        public int       tabindex           = -1;
        public string    text               = String.Empty;
        public string    fontfamily         = String.Empty;
        public string    fontsize           = String.Empty;
        public bool      visible            = true;
        public string    foreground         = String.Empty;
        public string    background         = String.Empty;
        public bool      IsTabStop          = true;
        public bool      IsEnabled          = true;
        public string    textAlign          = String.Empty;
    //**********************************************************************************
    // Default events - applies to all controls
    //**********************************************************************************
        public string    clickEvent         = String.Empty;
        public string    textchangedEvent   = String.Empty;
        public string    keydownEvent       = String.Empty;
        public string    keyupEvent         = String.Empty;
        public string    doubleclickEvent   = String.Empty;
    //**********************************************************************************
        public virtual string type
        {
            get { return "Base";        }
        }
    //**********************************************************************************
        public cBase()
        {
        }
    //**********************************************************************************
        public cBase(string line, string filename)
        {
            int    nstart = line.IndexOf('.') + 1;
            int    nlen   = line.IndexOf('=') - nstart - 1;
            string bname  = line.Substring(nstart, nlen);
            name            = bname;
            definition_file = filename;
        }
    //**********************************************************************************
    // Scan source code file (in a string) for our properties and events...
    //   Note: not easy to parse properties - there are too many varations.  So each
    //         property is parsed inline...
    //**********************************************************************************
        public virtual void Settings(string t)
        {
            int     index;
            string  search_text;
        //******************************************************************************
        // Location property
        //******************************************************************************
            search_text = "this." + name + ".Location = new System.Drawing.Point(";
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
        // Size property
        //******************************************************************************
            search_text = "this." + name + ".Size = new System.Drawing.Size(";
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
        // TabIndex property
        //******************************************************************************
            search_text = "this." + name + ".TabIndex = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                tabindex = Convert.ToInt32(vText);
            }
        //******************************************************************************
        // TabStop property
        //******************************************************************************
            search_text = "this." + name + ".TabStop = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                IsTabStop = Convert.ToBoolean(vText);
            }
        //******************************************************************************
        // Enabled property
        //******************************************************************************
            search_text = "this." + name + ".Enabled = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                IsEnabled = Convert.ToBoolean(vText);
            }
        //******************************************************************************
        // Visible property
        //******************************************************************************
            search_text = "this." + name + ".Visible = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                visible = Convert.ToBoolean(vText);
            }
        //******************************************************************************
        // ForeColor property
        //******************************************************************************
            search_text = "this." + name + ".ForeColor = System.Drawing.Color.";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                foreground = vText;
            }
        //******************************************************************************
        // BackColor property
        //******************************************************************************
            search_text = "this." + name + ".BackColor = System.Drawing.Color.";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(';', index) - index;
                string vText = t.Substring(index, len);
                background = vText;
            }
        //******************************************************************************
        // Text property
        //******************************************************************************
            search_text = "this." + name + ".Text = ";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf("\";", index) - (index + 1);
                string vText = t.Substring(index + 1, len);
                text = vText;
            }
        //******************************************************************************
        // TextAlign property
        //******************************************************************************
            search_text = "this." + name + ".TextAlign = System.Windows.Forms.HorizontalAlignment.";
            index = t.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = t.IndexOf(";", index) - index;
                string vText = t.Substring(index + 1, len);
                textAlign = vText;
            }
        //******************************************************************************
        // Events - much easier to parse...
        //******************************************************************************
            clickEvent          = parseEvent(name, "Click", t);
            textchangedEvent    = parseEvent(name, "TextChanged", t);
            keydownEvent        = parseEvent(name, "KeyDown", t);
            keyupEvent          = parseEvent(name, "KeyUp", t);
            doubleclickEvent    = parseEvent(name, "DoubleClick", t);
        }
    //**********************************************************************************
        protected string parseEvent(string name, string field, string source)
        {
            string search_text;
            int    index;
            string results = String.Empty;
        //******************************************************************************
        // Search for allocating statement for this event
        //******************************************************************************
            search_text = "this." + name + "." + field + " += new System.EventHandler(this.";
            index = source.IndexOf(search_text);
            if (index != -1)
            {
                index = index + search_text.Length;
                int len = source.IndexOf(')', index) - index;
                string vText = source.Substring(index, len);
                results = vText;
            }
        //******************************************************************************
            return results;
        }
    //**********************************************************************************
    // Virtual function - this function allows the application an easier way to write
    // out XAML definitions.  Each subClass implements this function.
    //**********************************************************************************
        public virtual void xaml(StreamWriter w, Size pSize)
        {
        }
    //**********************************************************************************
    // Virtual function - this function replaces any differences between Windows Forms
    // and WPF methods and properties.
    //**********************************************************************************
        public virtual bool convert(ref string line)
        {
            return false;
        }
    //**********************************************************************************
        public string xamlAttributes()
        {
            string o = String.Empty;
        //******************************************************************************
        // Generate standard XAML attributes based on the retrieved settings...
        //******************************************************************************
            o += "Name=\"" + name + "\" ";
            o += "Height=\"" + size.Height + "\" ";
            o += "Width=\"" + size.Width + "\" ";
            if (fontsize != String.Empty)
                o += "FontSize=\"" + fontsize + "\" ";
            if (fontfamily != String.Empty)
                o += "FontFamily=\"" + fontfamily + "\" ";

            if (textAlign != String.Empty)
                o += "TextAlignment=\"" + textAlign + "\" ";
            
            if (tabindex != -1)
                o += "TabIndex=\"" + tabindex.ToString() + "\" ";
            if (visible == false)
                o += "Visibility=\"Hidden\" ";
            if (IsEnabled == false)
                o += "IsEnabled=\"False\" ";
            if (IsTabStop == false)
                o += "IsTabStop=\"False\" ";
            if (foreground != String.Empty)
                o += "Foreground=\"" + foreground + "\" ";
            if (background != String.Empty)
                o += "Background=\"" + background + "\" ";

            if (clickEvent != String.Empty)
                o += "Click=\"" + clickEvent + "\" ";
            if (textchangedEvent != String.Empty)
                o += "TextChanged=\"" + textchangedEvent + "\" ";
            if (keydownEvent != String.Empty)
                o += "KeyDown=\"" + keydownEvent + "\" ";
            if (keyupEvent != String.Empty)
                o += "KeyUp=\"" + keyupEvent + "\" ";
            if (doubleclickEvent != String.Empty)
                o += "MouseDoubleClick=\"" + doubleclickEvent + "\" ";
        //******************************************************************************
            return o;
        }
    //**********************************************************************************
    // Calculate margins versus location and size - the routine is the tough part.
    // We need to determine the center point and convert the XAML settings to margins.
    // Basically controls over the half way point are referenced (margins) from that
    // quadrant.
    // Example:
    //   PanelSize - 100, 100
    //   Control location - 40,40 - would have margins from left and top
    //   Control location - 55,40 - would have margins from right and top
    //   Control location - 40,55 - would have margins from left and bottom
    //
    // Note: the calculations below are not always exact.  Sometimes the control will
    // overlap.  This effect is because some control sizes are slightly different
    // between WPF and Forms.
    //**********************************************************************************
        public string margins(Size panelSize)
        {
            int     x_from_left     = 0;
            int     y_from_top      = 0;
            int     x_from_right    = 0;
            int     y_from_bottom   = 0;
            string  horz            = "Left";
            string  vert            = "Top";
            string  o               = String.Empty;
        //******************************************************************************
        // Determine the X center point and reference direction
        //******************************************************************************
            if (location.X < panelSize.Width / 2)
                x_from_left = location.X;
            else 
            {
                x_from_right = (panelSize.Width - location.X) - size.Width;
                horz = "Right";
            }
        //******************************************************************************
        // Determine the Y center point and reference direction
        //******************************************************************************
            if (location.Y < panelSize.Height / 2)
                y_from_top = location.Y;
            else 
            {
                y_from_bottom = (panelSize.Height - location.Y) - size.Height;
                vert = "Bottom";
            }
        //******************************************************************************
        // Create margin string
        //******************************************************************************
            o += "Margin=\"" + x_from_left + "," + y_from_top + "," +
                                   + x_from_right + "," + y_from_bottom + "\" ";
            o += "HorizontalAlignment=\"" + horz + "\" ";
            o += "VerticalAlignment=\"" + vert + "\" ";
        //******************************************************************************
            return o;
        }
    }
}
