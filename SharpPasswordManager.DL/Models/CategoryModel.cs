using System;
using System.Collections.Generic;

namespace SharpPasswordManager.DL.Models
{
    /// <summary>
    /// Model that represents users data category (work data, market passwords, e.t.c.)
    /// </summary>
    [Serializable]
    public class CategoryModel
    {
        /// <summary>
        /// Name of the category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indexes of that category items in general storage.
        /// </summary>
        public List<int> DataIndexes { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
            {
                CategoryModel m = (CategoryModel)obj;
                return (Name == m.Name) && (DataIndexes == m.DataIndexes);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 25;
                int hashMultiple = 512;

                if (Name != null)
                    hash = (hash * hashMultiple) ^ Name.GetHashCode();

                if (DataIndexes != null && DataIndexes.Count > 0)
                {
                    foreach (var item in DataIndexes)
                        hash = (hash * hashMultiple) ^ item.GetHashCode();
                }
                return hash;
            }
        }
    }
}
