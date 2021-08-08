using System.Collections.Generic;

namespace SharpPasswordManager.IntegrationTests.Mocks
{
    public class CategoryMock
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
                CategoryMock m = (CategoryMock)obj;
                return (Name == m.Name) && (DataIndexes == m.DataIndexes);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
