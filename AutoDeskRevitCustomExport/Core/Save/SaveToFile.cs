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
    public class SaveToFile:ISave
    {
        private readonly string fileSaveTo;

        private readonly BIMTotal bims = null;
        private readonly CustomJson dataCache = null;

        public SaveToFile(string filePath)
        {
            this.fileSaveTo = filePath;

            this.bims = new BIMTotal()
            {
                eleids = new List<int>(),
                instances = new List<int>(),
                materials = new List<int>(),
            };
            this.dataCache = new CustomJson()
            {
                total = this.bims,
                materials = new List<CustomMaterial>(),
                elements = new List<CustomElement>(),
            };

        }

        public void AddInstanceElement(CustomElement element)
        {
            if (element.instances.Count > 0 || element.meshs.Count > 0)
            {
                this.bims.instances.Add(element.id);
                this.dataCache.elements.Add(element);
            }

        }

        public void AddElement(CustomElement element)
        {


            if (element.instances.Count > 0 || element.meshs.Count > 0)
            {
                this.bims.eleids.Add(element.id);
                this.dataCache.elements.Add(element);
            }
        }

        public void AddMaterial(CustomMaterial material)
        {
            if (!this.bims.materials.Contains(material.id))
            {
                this.bims.materials.Add(material.id);
                this.dataCache.materials.Add(material);
            }
        }

        private void JustSaveToFile(string data, string filename)
        {

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(data);
            }
        }

        public void Finish()
        {
            this.JustSaveToFile(JsonConvert.SerializeObject(this.dataCache), Path.Combine(this.fileSaveTo));

            Debug.WriteLine("导出到文件结束");
        }
    }
}
