using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AutoDeskRevitCustomExport.Core;
using AutoDeskRevitCustomExport.Core.Save;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AutoDeskRevitCustomExport
{
    /// <summary>
    /// 此处是一个示例,用于在revit 插件中指向,然后导出数据到指定文件
    /// </summary>
    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class DemoExport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            if (null == uidoc)
            {
                message = "请打开项目文件";
                return Result.Failed;
            }
            View3D view3d = null;
            try
            {
                view3d = (View3D)doc.ActiveView;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            if (null == view3d)
            {
                message = "请先切换至三维视图再进行相关操作";
                return Result.Failed;
            }
            else
            {
                FolderBrowserDialog dilog = new FolderBrowserDialog
                {
                    Description = "请选择文件夹"
                };

                string path = null;

                if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
                {
                    path = dilog.SelectedPath;
                }

                Debug.WriteLine("选择了路径：" + path);

                //创建以视图ID命名的文件夹

                string outputFile = Path.Combine(path, view3d.Id.ToString()); // 文件输出路径

                ISave saver = new SaveToFile(outputFile); // 导出器 数据合并导以json 格式出到一个文件

                //IBaseSave saver = new SaveToFolder(outputFile); // 导出器以多个文件导出到指定文件夹，针对大项目可以使用
                CustomExport baseExport = new CustomExport(saver, doc);

                using (CustomExporter exporter = new CustomExporter(doc, baseExport))
                {
                    exporter.IncludeGeometricObjects = true;
                    exporter.ShouldStopOnError = false;
                    exporter.Export(view3d);
                }

            }

            return Result.Succeeded;
        }
    }
}
