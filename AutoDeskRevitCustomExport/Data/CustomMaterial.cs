using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Data
{
    public class CustomMaterial : BaseCustomElement
    {
        public string name;
        /// <summary>
        /// 颜色,对应rgb值
        /// </summary>
        public List<int> color;

        public double glossiness;

        public double transparency;

        public double smoothness;
    }
}
