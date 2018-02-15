using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

namespace Convert_to_WPF
{
    public class attribute
    {
        public string name     = String.Empty;
        public string value    = String.Empty;
    }
    
    public class cfgNode
    {
        public ArrayList children   = new ArrayList(); 
        public ArrayList attributes = new ArrayList(); 
        public string    text       = String.Empty;
        public string    name       = String.Empty;
        
        public cfgNode(string n)
        {
            name = n;
        }
    
        public int parse(XmlTextReader r)
        {
            bool parse_doc = !r.IsEmptyElement;

            if (r.HasAttributes) 
            { 
                for (int i = 0; i < r.AttributeCount; i++) 
                { 
                    r.MoveToAttribute(i);
                    attribute a = new attribute();
                    a.name  = r.Name;
                    a.value = r.Value;
                    attributes.Add(a);
                } 
            }
            while (parse_doc) 
            { 
                parse_doc = r.Read();
                switch (r.NodeType) 
                { 
                    case XmlNodeType.Element:
                        cfgNode n = new cfgNode(r.Name); 
                        n.parse(r);
                        children.Add(n);
                        break; 
                 // 
                 //you can handle other cases here 
                 // 
      
                    case XmlNodeType.EndElement: 
                        parse_doc = false;
                        break;
                     case XmlNodeType.Text: 
                        text = r.Value;
                        break; 
                } 
            }

            return children.Count;
        }

    }
}
