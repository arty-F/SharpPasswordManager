using System;

namespace SharpPasswordManager.DL.Models
{
    [Serializable]
    public class DataModel
    {
        public string Description { get; set; }
        public string Login { get; set; }
        public string Date { get; set; }
        public string Password { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
            {
                DataModel m = (DataModel)obj;
                return (Description == m.Description) && (Login == m.Login) && (Date == m.Date) && (Password == m.Password);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                int hashMultiple = 104;

                if (Description != null)
                    hash = (hash * hashMultiple) ^ Description.GetHashCode();
                if (Login != null)
                    hash = (hash * hashMultiple) ^ Login.GetHashCode();
                if (Date != null)
                    hash = (hash * hashMultiple) ^ Date.GetHashCode();
                if (Password != null)
                    hash = (hash * hashMultiple) ^ Password.GetHashCode();

                return hash;
            }
        }
    }
}
