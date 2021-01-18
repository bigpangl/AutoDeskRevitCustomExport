using AutoDeskRevitCustomExport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core.Cache
{
    /// <summary>
    /// 定义一个读取关键数据的接口,用于在不同场景,如单文件，文件夹，或者网络API 下进行数据获取
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 获取BIMIn 对象
        /// </summary>
        /// <returns></returns>
        BIMTotal GetTotal();

        ///// <summary>
        ///// 根据ID 返回字符串对象
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //string GetData(int id);

        /// <summary>
        /// 根据ID 获取材质信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CustomMaterial GetMaterial(int id);

        /// <summary>
        /// 根据ID 获取某个元素实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CustomElement GetElement(int id);

    }
}
