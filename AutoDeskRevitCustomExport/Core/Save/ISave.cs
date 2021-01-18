using AutoDeskRevitCustomExport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core.Save
{
    /// <summary>
    /// 定制save 时的接口,目前完成导出的文件,导出到文件夹,预留接口类供自定义导出到网络API或者其他
    /// </summary>
    public interface ISave
    {
        /// <summary>
        /// 添加一个instance 性质的element
        /// </summary>
        /// <param name="ele"></param>
        void AddInstanceElement(CustomElement ele);
        /// <summary>
        /// 添加一个element
        /// </summary>
        /// <param name="ele"></param>
        void AddElement(CustomElement ele);

        /// <summary>
        /// 添加一个材质
        /// </summary>
        /// <param name="material"></param>
        void AddMaterial(CustomMaterial material);
        /// <summary>
        /// 结束导出
        /// </summary>
        void Finish();
    }
}
