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
        public BIMTotal total;

        public List<CustomMaterial> materials;

        public List<CustomElement> elements; // 包含了所有的element 和instance在其中
    }
}
