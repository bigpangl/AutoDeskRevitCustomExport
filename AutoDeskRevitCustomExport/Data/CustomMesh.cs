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
        // 一个mesh 具有的材质特点
        public int material;

        // 此mesh 具有的顶点信息
        public List<double> vertices;

        // 此mesh 具有的三角形片段,对应vertices 下标索引
        public List<int> angles;

        // 对应每个三角形的法向量
        public List<double> normals;
    }
}
