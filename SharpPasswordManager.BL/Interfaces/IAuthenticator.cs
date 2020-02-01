using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    /// <summary>
    /// Provides a mechanism to autentication.
    /// </summary>
    interface IAuthenticator
    {
        bool Autenticate(string password);
    }
}
