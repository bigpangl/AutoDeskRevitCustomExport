using AutoDeskRevitCustomExport.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core.Save
{
    public class SaveToFolder
    {
        private readonly string folderSaveTo;
        private readonly BIMTotal bims = null;

        /// <summary>
        /// 该类初始化方法
        /// </summary>
        /// <param name="folder"></param>
        public SaveToFolder(string folder)
        {
            this.folderSaveTo = folder;
            Directory.CreateDirectory(folder);
            this.bims = new BIMTotal()
            {
                eleids = new List<int>(),
                instances = new List<int>(),
                materials = new List<int>(),
            };
        }

        private void SaveToFile(string data, string filename)
        {

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(data);
            }
        }

        public void AddInstanceElement(CustomElement element)
        {
            if (element.instances.Count > 0 || element.meshs.Count > 0)
            {
                this.bims.instances.Add(element.id);
                this.SaveToFile(JsonConvert.SerializeObject(element), Path.Combine(this.folderSaveTo, element.id.ToString()));
            }

        }

        public void AddElement(CustomElement element)
        {


            if (element.instances.Count > 0 || element.meshs.Count > 0)
            {
                this.bims.eleids.Add(element.id);
                this.SaveToFile(JsonConvert.SerializeObject(element), Path.Combine(this.folderSaveTo, element.id.ToString()));
            }
        }

        public void AddMaterial(CustomMaterial material)
        {
            if (!this.bims.materials.Contains(material.id))
            {
                this.bims.materials.Add(material.id);
                this.SaveToFile(JsonConvert.SerializeObject(material), Path.Combine(this.folderSaveTo, material.id.ToString()));
            }
        }

        public void Finish()
        {
            this.SaveToFile(JsonConvert.SerializeObject(this.bims), Path.Combine(this.folderSaveTo, "TOTAL"));
            Debug.WriteLine("导出到文件结束");
        }
    }
}
