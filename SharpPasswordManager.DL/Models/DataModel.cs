using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.DL.Models
{
    [Serializable]
    public class DataModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public DateTime Date { get; set; }
        public string Password { get; set; }
    }
}
