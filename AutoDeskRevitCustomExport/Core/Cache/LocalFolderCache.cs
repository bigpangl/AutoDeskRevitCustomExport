using AutoDeskRevitCustomExport.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core.Cache
{
    /// <summary>
    /// 从文件夹中载入
    /// </summary>
    public class LocalFolderCache : ICache
    {
        private readonly string modelPath = null;
        public LocalFolderCache(string folder)
        {
            this.modelPath = folder;
        }
        private string ReadFromFile(string fileName)
        {
            string data = null;

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    data = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return data;
        }

        public BIMTotal GetTotal()
        {
            BIMTotal bims = null;
            string filePath = Path.Combine(this.modelPath, "TOTAL");
            string data = this.ReadFromFile(filePath);
            if (null != data)
            {
                try
                {
                    bims = JsonConvert.DeserializeObject<BIMTotal>(data);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            return bims;
        }

        public string GetData(int id)
        {
            string filePath = Path.Combine(this.modelPath, id.ToString());
            string data = this.ReadFromFile(filePath);
            return data;
        }

        public CustomElement GetElement(int id)
        {
            string data = this.GetData(id);
            CustomElement ele = null;
            if (data != null)
            {
                try
                {
                    ele = JsonConvert.DeserializeObject<CustomElement>(data);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            return ele;
        }
        public CustomMaterial GetMaterial(int id)
        {
            string data = this.GetData(id);
            CustomMaterial material = null;
            if (data != null)
            {
                try
                {
                    material = JsonConvert.DeserializeObject<CustomMaterial>(data);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            return material;
        }
    }
}
