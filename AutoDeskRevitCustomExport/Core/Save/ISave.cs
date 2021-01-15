using AutoDeskRevitCustomExport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDeskRevitCustomExport.Core.Save
{
    public interface ISave
    {
        void AddInstanceElement(CustomElement ele);
        void AddElement(CustomElement ele);
        void AddMaterial(CustomMaterial material);

        void Finish();
    }
}
