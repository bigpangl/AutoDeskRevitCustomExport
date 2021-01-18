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
    public class LocalFileCache:ICache
    {
        private readonly string filePath = null;
        private readonly CustomJson jsonModel = null;
        private readonly Dictionary<int, CustomElement> elementsCache = null;
        private readonly Dictionary<int, CustomMaterial> materialCache = null;

        public LocalFileCache(string filePath)
        {
            this.filePath = filePath;
            this.elementsCache = new Dictionary<int, CustomElement>();
            this.materialCache = new Dictionary<int, CustomMaterial>();

            try
            {
                string data = this.ReadFromFile(this.filePath);
                this.jsonModel = JsonConvert.DeserializeObject<CustomJson>(data);
                foreach (CustomElement element in this.jsonModel.elements)
                {
                    this.elementsCache[element.id] = element;
                }
                foreach (CustomMaterial material in this.jsonModel.materials)
                {
                    this.materialCache[material.id] = material;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

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
            return this.jsonModel?.total;

        }

        public CustomMaterial GetMaterial(int id)
        {
            this.materialCache.TryGetValue(id, out CustomMaterial material);
            return material;
        }

        public CustomElement GetElement(int id)
        {

            this.elementsCache.TryGetValue(id, out CustomElement ele);

            return ele;
        }
    }

}
