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

        /// <summary>
        /// 多个mesh,此部分保留,用于在其他端构建数据
        /// </summary>
        public List<CustomMesh> meshs;

        /// <summary>
        /// 几何体复用部分,给加载优化做数据准备
        /// </summary>
        public List<CustomInstance> instances;
    }
}
