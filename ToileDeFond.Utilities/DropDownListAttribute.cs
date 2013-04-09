using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToileDeFond.Utilities
{
    public class DropDownListAttribute : UIHintAttribute
    {
        public DropDownListAttribute()
            : base("_DropDownList")
        {
        }

        public DropDownListAttribute(string presentationLayer)
            : base("_DropDownList", presentationLayer)
        {
        }

        public DropDownListAttribute(string presentationLayer, params object[] controlParameters)
            : base("_DropDownList", presentationLayer, controlParameters)
        {
        }
    }
}
