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
        /// <summary>
        /// 对应材质中的glossiness
        /// </summary>
        public double glossiness;

        /// <summary>
        /// 对应材质中transparency
        /// </summary>
        public double transparency;

        /// <summary>
        /// 对应材质中smoothness
        /// </summary>
        public double smoothness;
    }
}
