using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Data
{
    /// <summary>
    /// 定义导出element 所具备的数据格式,element 和instance 都统一这种格式
    /// </summary>
    public class CustomElement : BaseCustomElement
    {
        /// <summary>
        /// name
        /// </summary>
        public string name;

        public List<CustomMesh> meshs;

        public List<CustomInstance> instances;
    }
}
