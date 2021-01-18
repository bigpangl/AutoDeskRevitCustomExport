using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AutoDeskRevitCustomExport.Core;
using AutoDeskRevitCustomExport.Core.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AutoDeskRevitCustomExport
{
    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class DemoLoad : IExternalCommand
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

            OpenFileDialog dilog = new OpenFileDialog()
            {
                Multiselect = false,//该值确定是否可以选择多个文件
                Title = "请选择文件",
                Filter = "所有文件(*.*)|*.*",
            };

            string path = null;

            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
            {
                path = dilog.FileName;
            }

            Debug.WriteLine("选择了路径：" + path); // 继续文件的导入

            //创建以视图ID命名的文件夹

            using (Transaction t = new Transaction(doc, "Create tessellated direct shape"))
            {
                t.Start();

                ICache baseCache = new LocalFileCache(path);

                //IBaseCache baseCache = new LocalFolderCache(path); // 文件夹载入
                CustomLoad baseLoad = new CustomLoad(baseCache, doc);
                baseLoad.LoadModel(Transform.Identity); // 此处可以用于传递初始的旋转平移变换
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
