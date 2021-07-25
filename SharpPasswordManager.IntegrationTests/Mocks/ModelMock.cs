using System;

namespace SharpPasswordManager.IntegrationTests.Mocks
{
    public class ModelMock
    {
        public string Login { get; set; }
        public DateTime Date { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var objModel = (ModelMock)obj;

            return Login == objModel.Login && Date == objModel.Date;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
