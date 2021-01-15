using Autodesk.Revit.DB;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core
{
    /// <summary>
    /// 通过ICP 算法,计算revit 中transform 对应的旋转矩阵
    /// </summary>
    public class ICP
    {
        private readonly IList<XYZ> now = null;
        private readonly IList<XYZ> old = null;

        private readonly bool errorStauts = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="now">现在的坐标系</param>
        /// <param name="old">曾经的坐标系</param>
        public ICP(IList<XYZ> now, IList<XYZ> old)
        {

            if (now.Count() == old.Count() && now.Count() == 4)
            {
                this.now = now;
                this.old = old;
            }
            else
            {
                this.errorStauts = true;
                Debug.WriteLine("两个点集长度不一样");
            }

        }

        /// <summary>
        /// 获取一个3X3 的旋转矩阵,未考虑缩放,仅考虑旋转变化
        /// </summary>
        /// <returns></returns>
        public Matrix<double> GetH()
        {
            if (this.errorStauts == true)
            {
                return null;
            }
            else
            {
                var ma = Matrix<double>.Build;

                Matrix<double> finalH = ma.Dense(3, 3, 0);

                for (int i = 0; i < this.now.Count(); i += 1)
                {
                    Matrix<double> tmpa = ma.Dense(1, 3);
                    tmpa[0, 0] = this.now[i].X;
                    tmpa[0, 1] = this.now[i].Y;
                    tmpa[0, 2] = this.now[i].Z;
                    Matrix<double> tmpb = ma.Dense(3, 1);
                    tmpb[0, 0] = this.old[i].X;
                    tmpb[1, 0] = this.old[i].Y;
                    tmpb[2, 0] = this.old[i].Z;
                    Matrix<double> mid = tmpb * tmpa;
                    finalH += mid;
                }
                return finalH;
            }
        }
    }
}
