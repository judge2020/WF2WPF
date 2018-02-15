using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace Convert_to_WPF
{
//**************************************************************************************
    public class Project : cfgNode
    {
    //**********************************************************************************
    // Supported projects...
    //**********************************************************************************
        public enum pType
        {
            CS,
            VB,
            Unknown,
            Error
        }
    //**********************************************************************************
        public ArrayList files        = new ArrayList();
        public ArrayList forms        = new ArrayList();
        public string    ns           = String.Empty;
        public string    default_form = String.Empty;
        public pType     type         = pType.Unknown;
        public string    status       = String.Empty;
    //**********************************************************************************
        public Project(string name) : base(name)
        {
        }
    //**********************************************************************************
        public bool addSourceFile(string filename)
        {
            pType    file_type;
            FileInfo fi = new FileInfo(filename);
        //******************************************************************************
        // Determine file type
        //******************************************************************************
            switch (fi.Extension.ToUpper())
            {
                case ".CS":
                    file_type = pType.CS;
                    break;
                case ".VB":
                    file_type = pType.VB;
                    break;
                default:
                    type   = pType.Error;
                    status = "Invalid file type: " + fi.Extension;
                    return false;
            }
        //******************************************************************************
        // Match this file type with existing - we don't support mixed projects
        //******************************************************************************
            if (type == pType.Unknown) type = file_type;
            else
            {
                if (type != file_type) 
                {
                    status = "Mixed source file types - not supported";
                    return false;
                }
            }
        //******************************************************************************
        // Everything's okay - add the file and return
        //******************************************************************************
            files.Add(filename);
            return true;
        }
    //**********************************************************************************
        public bool isFormResx(string file)
        {
            bool isResx = false;
        //******************************************************************************
        // Compare this file to our form files...
        //******************************************************************************
            foreach (cForm f in forms)
            {
                string fr = formFile(f.definition_file, ".resx");
            //**************************************************************************
                if (fr == file)
                {
                    isResx = true;  // It's one of ours...
                    break;
                }
            }
        //******************************************************************************
            return isResx;
        }
    //**********************************************************************************
        public bool isForm(string file)
        {
            bool isForm = false;
        //******************************************************************************
        // Compare this file to our form files...
        //******************************************************************************
            foreach (cForm f in forms)
            {
                if (f.definition_file == file)
                {
                    isForm = true;  // It's one of ours
                    break;
                }
            }
        //******************************************************************************
            return isForm;
        }
    //**********************************************************************************
        public static string formFile(string form_file, string suffix)
        {
            string form = form_file;
            int    i    = form.LastIndexOf('.');
        //******************************************************************************
        // Swap extension of this file name.
        //******************************************************************************
            form = form.Substring(0, i);
        //******************************************************************************
            return form + suffix;
        }
    }
}
