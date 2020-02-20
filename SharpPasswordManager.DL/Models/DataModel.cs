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
    }
}
