using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    /// <summary>
    /// Provides a mechanism to generate random data.
    /// </summary>
    public interface IDataGenerator
    {
        string GenerateRandomDescription();
        string GenerateRandomLogin();
        string GenerateRandomPassword();
        string GenerateRandomPassword(int lenght);
        DateTime GenerateRandomDate();
    }
}
