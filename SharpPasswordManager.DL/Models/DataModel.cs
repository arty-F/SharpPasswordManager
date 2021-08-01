using System;

namespace SharpPasswordManager.DL.Models
{
    /// <summary>
    /// Class that represents item of user data.
    /// </summary>
    [Serializable]
    public class DataModel
    {
        /// <summary>
        /// Url of current data item.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Login of current data item.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Date this model was created.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Password of current data item.
        /// </summary>
        public string Password { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
            {
                DataModel m = (DataModel)obj;
                return (Url == m.Url) && (Login == m.Login) && (Date == m.Date) && (Password == m.Password);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                int hashMultiple = 104;

                if (Url != null)
                    hash = (hash * hashMultiple) ^ Url.GetHashCode();
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
