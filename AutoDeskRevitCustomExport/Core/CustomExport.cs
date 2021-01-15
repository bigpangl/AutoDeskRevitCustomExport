using Autodesk.Revit.DB;
using AutoDeskRevitCustomExport.Core.Save;
using AutoDeskRevitCustomExport.Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core
{
    public class CustomExport : IExportContext
    {
        private readonly ISave saver = null;
        private readonly Document doc = null;

        private readonly Stack<Transform> transformationStack = null;
        private readonly Stack<CustomElement> singleGeometry = null; // 导出的数据格式element
        private readonly IList<int> instanceIDs = null;
        private CustomMaterial material = null;

        public CustomExport(ISave saverin, Document doc)
        {
            this.saver = saverin;
            this.doc = doc;
            this.transformationStack = new Stack<Transform>();
            this.singleGeometry = new Stack<CustomElement>();
            this.instanceIDs = new List<int>();
        }

        /// <summary>
        /// 获取当前几何体信息
        /// </summary>
        private CustomElement CurrentSolid
        {
            get
            {
                return this.singleGeometry.Peek();
            }
        }

        /// <summary>
        /// 转换过程
        /// </summary>
        private Transform CurrentTransform
        {
            get
            {
                return this.transformationStack.Peek();
            }
        }

        /// <summary>
        /// 工具函数 用于抽取一个面的三角形网格和
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private CustomMesh GetFace(PolymeshTopology node)
        {
            // 此处对应的是一个mesh 的数据，点，三角，法向量，等等

            CustomMesh mesh = new CustomMesh();

            try
            {
                DistributionOfNormals distrib = node.DistributionOfNormals;

                List<int> trangleFace = new List<int>();

                IList<XYZ> vertices_in = node.GetPoints();// 一个element 的几何体的所有顶点
                List<double> vertices = new List<double>();
                foreach (XYZ pointxyz in vertices_in)
                {
                    vertices.Add(pointxyz.X);
                    vertices.Add(pointxyz.Y);
                    vertices.Add(pointxyz.Z);
                }

                //法向量部分数据，方法来自建筑编码器的示例
                List<double> normals = new List<double>();

                int iFacet = 0;
                foreach (PolymeshFacet triangle in node.GetFacets())
                {
                    XYZ normal;
                    if (DistributionOfNormals.OnePerFace == distrib)
                    {
                        normal = node.GetNormal(0);
                    }
                    else if (DistributionOfNormals.OnEachFacet == distrib)
                    {
                        normal = node.GetNormal(iFacet++);
                    }
                    else
                    {
                        normal = node.GetNormal(triangle.V1) + node.GetNormal(triangle.V2) + node.GetNormal(triangle.V3);
                        normal /= 3.0;
                    }

                    trangleFace.Add(triangle.V1);
                    trangleFace.Add(triangle.V2);
                    trangleFace.Add(triangle.V3);

                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);
                }

                if (vertices.Count > 0)
                {
                    mesh.vertices = vertices;
                    mesh.angles = trangleFace;
                    mesh.normals = normals;
                    mesh.material = -1;
                }
                else
                {
                    mesh = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("获取面的网格数据失败：" + e.ToString());
                mesh = null;
            }
            return mesh;
        }


        // Revit API 固定

        public bool Start()
        {
            try
            {
                this.transformationStack.Push(Transform.Identity);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }

        public void Finish()
        {
            this.saver.Finish();
        }

        public bool IsCanceled()
        {
            return false;
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            try
            {
                CustomElement ele = new CustomElement
                {
                    id = elementId.IntegerValue,
                    meshs = new List<CustomMesh>(),
                    instances = new List<CustomInstance>(),
                    name = doc.GetElement(elementId)?.Name,
                }; //初始化element 导出,导出部分的instance 是ele，element 是ele

                this.singleGeometry.Push(ele);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {

            try
            {
                CustomElement ele = this.singleGeometry.Pop();
                Debug.Assert(ele.id == elementId.IntegerValue);
                this.saver.AddElement(ele);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Proceed;// continue
        }

        public void OnFaceEnd(FaceNode node)
        {

        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            try
            {
                ElementId symbolid = node.GetSymbolId();
                // 无论是否跳过,始终添加

                this.transformationStack.Push(CurrentTransform.Multiply(node.GetTransform()));
                this.singleGeometry.Push(new CustomElement()
                {
                    id = symbolid.IntegerValue,
                    meshs = new List<CustomMesh>(),
                    instances = new List<CustomInstance>()
                }); // 始终添加

                try
                {
                    if (this.instanceIDs.Contains(symbolid.IntegerValue))
                    {
                        return RenderNodeAction.Skip; // 跳过 跳过后会执行end，不执行中间部分
                    }
                    else
                    {
                        return RenderNodeAction.Proceed; // 继续，会执行中间部分，最后执行end
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return RenderNodeAction.Skip;
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine(e);
                return RenderNodeAction.Skip;
            }


        }

        public void OnInstanceEnd(InstanceNode node)
        {
            ElementId symbolid = node.GetSymbolId();

            CustomElement ele = this.singleGeometry.Pop(); // instance 的数据

            if (this.instanceIDs.Contains(node.GetSymbolId().IntegerValue) == false) // 没有包含，进行了处理
            {
                this.instanceIDs.Add(symbolid.IntegerValue);
                Debug.Assert(ele.id == symbolid.IntegerValue);
                this.saver.AddInstanceElement(ele);
            }

            /*
             * 此部分逻辑,无论是否执行,都需要处理
             */

            CustomElement solidlast = CurrentSolid;

            try
            {
                CustomInstance instanceTMP = new CustomInstance() { };

                instanceTMP.id = symbolid.IntegerValue;
                IList<XYZ> A = new List<XYZ>() { XYZ.BasisX, XYZ.BasisY, XYZ.BasisZ, XYZ.Zero }; // 世界坐标
                IList<XYZ> B = new List<XYZ>() { CurrentTransform.BasisX, CurrentTransform.BasisY, CurrentTransform.BasisZ, CurrentTransform.Origin }; // 实例的世界坐标
                ICP icp = new ICP(A, B);
                Matrix<double> mpH = icp.GetH();
                var svd = mpH.Svd(true);
                Matrix<double> tmpR = svd.VT.Transpose() * svd.U.Transpose(); // 旋转矩阵
                tmpR = tmpR.Transpose();

                // 包括旋转和平移的，旋转矩阵,未包括缩放
                instanceTMP.rotate = new List<double>() {
                        tmpR[0, 0], tmpR[0, 1], tmpR[0, 2], CurrentTransform.Origin.X, tmpR[1, 0], tmpR[1, 1], tmpR[1, 2], CurrentTransform.Origin.Y, tmpR[2, 0], tmpR[2, 1], tmpR[2, 2], CurrentTransform.Origin.Z, 0, 0, 0, 1
                };

                solidlast.instances.Add(instanceTMP);
            }
            catch (Exception e)
            {
                Debug.WriteLine("添加实例引用时发生失败，不会添加到实体的引用：" + e.ToString());
            }

            try
            {

                this.transformationStack.Pop(); // 弹出添加的旋转变换,需要最后再弹出这个变换关系

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }



        }

        public void OnLight(LightNode node)
        {
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            // 是否要去除掉link 的处理？是个问题
            try
            {
                this.transformationStack.Push(CurrentTransform.Multiply(node.GetTransform()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {

            try
            {
                this.transformationStack.Pop();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        public void OnMaterial(MaterialNode node)
        {
            try
            {
                Autodesk.Revit.DB.Element revitEle = this.doc.GetElement(node.MaterialId);
                this.material = new CustomMaterial()
                {
                    id = node.MaterialId.IntegerValue,
                    name = this.doc.GetElement(node.MaterialId)?.Name,
                    color = new List<int>() {
                        node.Color.Red,node.Color.Green,node.Color.Blue
                    },
                    glossiness = node.Glossiness,
                    transparency = node.Transparency,
                    smoothness = node.Smoothness
                };

                this.saver.AddMaterial(this.material);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        public void OnPolymesh(PolymeshTopology node)
        {
            try
            {
                CustomMesh mesh = this.GetFace(node);

                if (mesh != null)
                {
                    CurrentSolid.meshs.Add(mesh);
                    mesh.material = this.material.id;
                }
                else
                {
                    Debug.WriteLine("未从PolymeshTopolopy 中获取到mesh data");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void OnRPC(RPCNode node)
        {
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {

        }
    }
}
