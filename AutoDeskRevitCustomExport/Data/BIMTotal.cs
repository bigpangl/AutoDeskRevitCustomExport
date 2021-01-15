using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Data
{
    /// <summary>
    /// 汇总信息,导出文件夹时指向TOTAL 文件,导出单个文件时也会存在此数据对象
    /// </summary>
    public class BIMTotal
    {
        /// <summary>
        /// 导出部分,存在哪些ids
        /// </summary>
        public List<int> eleids;

        /// <summary>
        /// 导出部分,有哪些可复用实例,同样以ID 存在
        /// </summary>
        public List<int> instances;

        /// <summary>
        /// 导出部分有哪些材质信息,同样以ID 存在
        /// </summary>
        public List<int> materials;

    }
}
