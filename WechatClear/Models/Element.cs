using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WechatClear.Models
{
    public class Element
    {
        public Element()
        {
            Children = new List<Element>();
        }

        public string Name { get; set; }
        public string AutomationId { get; set; }
        public ControlType ControlType { get; set; }
        public List<Element> Children { get; set; }
    }
}
