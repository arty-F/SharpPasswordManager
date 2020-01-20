using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    public interface IDataGenerator
    {
        string GenerateRandomDescription();
        string GenerateRandomLogin();
        string GenerateRandomPassword();
        string GenerateRandomPassword(int lenght);
        DateTime GenerateRandomDate();
    }
}
