using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace SharpPasswordManager.BL
{
    /*----------------------------------------------------------------------
     * Result of Autenticate method will be true when the method parameter 
     <password> and <data> recived by constructor will be equal after <data> 
     encryption by <cryptographer> if it used. Key for <cryptographer> 
     assigned by converted method parameter <password> to byte array.
    ----------------------------------------------------------------------*/
    /// <summary>
    /// This class used for autenticate user by entered password.
    /// </summary>
    public class Autenticator : IAuthenticator
    {
        string data;
        ICryptographer cryptographer;

        /// <summary>
        /// Create a new class instance.
        /// </summary>
        /// <param name="data">Encrypted data for check autentication.</param>
        /// <param name="cryptographer">Cryptographer for decrypting data.</param>
        public Autenticator(string data, ICryptographer cryptographer = null)
        {
            this.data = data;
            this.cryptographer = cryptographer;
        }

        /// <summary>
        /// Returns the result of an attempt of autenticate.
        /// </summary>
        /// <param name="password">Entered password.</param>
        /// <returns></returns>
        public bool Autenticate(string password)
        {
            if (cryptographer != null)
            {
                byte[] convertedPassword = Convert.FromBase64String(password);
                cryptographer.ChangeKey(convertedPassword);

                return cryptographer.Decrypt(data) == password;
            }
            else
                return data == password;
        }
    }
}
