using AutoDeskRevitCustomExport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core.Cache
{
    public interface ICache
    {
        /// <summary>
        /// 获取BIMIn 对象
        /// </summary>
        /// <returns></returns>
        BIMTotal GetTotal();

        /// <summary>
        /// 根据ID 返回字符串对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetData(int id);
    }
}
