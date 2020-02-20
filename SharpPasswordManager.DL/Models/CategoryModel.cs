using System;
using System.Collections.Generic;

namespace SharpPasswordManager.DL.Models
{
    [Serializable]
    public class CategoryModel
    {
        public string Name { get; set; }
        public List<int> DataIndexes { get; set; }
    }
}
