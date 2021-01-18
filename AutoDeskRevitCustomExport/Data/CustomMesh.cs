using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Data
{
    /// <summary>
    /// 定义自定义导出的Mesh
    /// </summary>
    public class CustomMesh
    {
        /// <summary>
        /// 对应材质ID
        /// </summary>
        public int material;

        /// <summary>
        /// 此mesh 具有的顶点信息
        /// </summary>
        public List<double> vertices;

        /// <summary>
        /// 此mesh 具有的三角形片段,对应vertices 下标索引
        /// </summary>
        public List<int> angles;

        /// <summary>
        /// 对应每个三角形的法向量
        /// </summary>
        public List<double> normals;
    }
}
