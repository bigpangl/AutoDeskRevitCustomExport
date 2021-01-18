using Autodesk.Revit.DB;
using AutoDeskRevitCustomExport.Core.Cache;
using AutoDeskRevitCustomExport.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core
{
    public class CustomLoad
    {
        private readonly ICache baseCache = null;
        private readonly Document doc;

        public CustomLoad(ICache baseCache, Document doc)
        {
            this.baseCache = baseCache;
            this.doc = doc;
        }

        /// <summary>
        /// 载入单个面
        /// </summary>
        /// <param name="mesh"></param>
        public TessellatedShapeBuilder LoadSingleMesh(CustomMesh mesh, Transform transfrom)
        {

            IList<GeometryObject> data_back = new List<GeometryObject>();

            TessellatedShapeBuilder builder = new TessellatedShapeBuilder();

            builder.OpenConnectedFaceSet(true); // 允许开口,此处的位置可能是开口的


            // 处理点
            List<XYZ> points = new List<XYZ>();
            for (int i = 0; i < mesh.vertices.Count; i += 3)
            {
                XYZ mid = new XYZ(mesh.vertices[i], mesh.vertices[i + 1], mesh.vertices[i + 2]);

                points.Add(transfrom.OfPoint(mid));
            }
            // 处理三角形
            for (int j = 0; j < mesh.angles.Count; j += 3)
            {
                try
                {
                    List<XYZ> faceTmp = new List<XYZ>() {
                            points[mesh.angles[j]],
                            points[mesh.angles[j+1]],
                            points[mesh.angles[j+2]],
                        };
                    builder.AddFace(new TessellatedFace(faceTmp, ElementId.InvalidElementId));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            try
            {
                builder.CloseConnectedFaceSet();
                builder.Build();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return builder;
        }

        /// <summary>
        /// 载入所有的mesh信息
        /// </summary>
        /// <param name="meshs"></param>
        /// <param name="transfrom"></param>
        /// <returns></returns>
        public IList<TessellatedShapeBuilder> LoadMesh(List<CustomMesh> meshs, Transform transfrom)
        {
            IList<TessellatedShapeBuilder> builders = new List<TessellatedShapeBuilder>();
            foreach (CustomMesh mesh in meshs)
            {
                builders.Add(this.LoadSingleMesh(mesh, transfrom));
            }
            return builders;
        }

        //通过堆栈循环载入复用部分
        public IList<GeometryObject> LoadSingleElement(int eleid, Transform transfrom)
        {
            IList<GeometryObject> dataBack = new List<GeometryObject>();

            CustomElement ele = this.baseCache.GetElement(eleid);

            if (ele != null)
            {
                IList<TessellatedShapeBuilder> builders = LoadMesh(ele.meshs, transfrom);

                foreach (TessellatedShapeBuilder builder in builders)
                {
                    TessellatedShapeBuilderResult result = builder.GetBuildResult();
                    IList<GeometryObject> geoemtriesMid = result.GetGeometricalObjects();
                    foreach (GeometryObject obj in geoemtriesMid)
                    {
                        dataBack.Add(obj); // 将复用部分添加进去
                    }

                }
                Transform changeMid = transfrom;
                XYZ basex = new XYZ(1, 0, 0);
                XYZ basey = new XYZ(0, 1, 0);
                XYZ basez = new XYZ(0, 0, 1);

                foreach (CustomInstance instance in ele.instances)
                {

                    List<double> rotate = instance.rotate;

                    XYZ location = new XYZ(rotate[3], rotate[7], rotate[11]);

                    double x = Math.Atan2(rotate[9], rotate[10]);
                    double y = Math.Atan2(-rotate[8], Math.Sqrt(rotate[9] * rotate[9] + rotate[10] * rotate[10]));
                    double z = Math.Atan2(rotate[4], rotate[0]);
                    changeMid = changeMid.Multiply(Transform.CreateTranslation(location)); // 平移后
                    // 欧拉角
                    changeMid = changeMid.Multiply(Transform.CreateRotation(basez, z));
                    changeMid = changeMid.Multiply(Transform.CreateRotation(basex, x));
                    changeMid = changeMid.Multiply(Transform.CreateRotation(basey, y));

                    IList<GeometryObject> dataMid = LoadSingleElement(instance.id, changeMid);
                    foreach (GeometryObject geobj in dataMid)
                    {
                        dataBack.Add(geobj);
                    }
                }
            }
            return dataBack;
        }

        public List<DirectShape> LoadModel(Transform transfrom)
        {
            List<DirectShape> dataBack = new List<DirectShape>();

            BIMTotal bimin = this.baseCache.GetTotal();
            Debug.Assert(bimin != null, "BIMIn 数据获取为空");

            /*
             *
             * 尝试用一个字典缓存所有的变量类型,避免再次读取数据
             */

            foreach (int eleid in bimin.eleids)
            {
                CustomElement ele = this.baseCache.GetElement(eleid);

                IList<GeometryObject> datasUse = this.LoadSingleElement(eleid, transfrom);

                if (datasUse.Count > 0)
                {
                    DirectShape dsLifting = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                    dsLifting.AppendShape(datasUse);
                    if (ele.name != null)
                    {
                        dsLifting.SetName(ele.name);
                    }
                    dataBack.Add(dsLifting);
                }
            }

            return dataBack;
        }
    }
}
