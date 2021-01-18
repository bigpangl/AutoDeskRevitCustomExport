using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Data
{
    /// <summary>
    /// 当导出数据为单个文件时,使用此数据对象
    /// </summary>
    public class CustomJson
    {
        /// <summary>
        /// 预览信息
        /// </summary>
        public BIMTotal total;

        /// <summary>
        /// 实际的每个材质信息
        /// </summary>
        public List<CustomMaterial> materials;


        /// <summary>
        /// 实际的每个元素信息,element和instance 未明确区分
        /// </summary>
        public List<CustomElement> elements; // 包含了所有的element 和instance在其中
    }
}
