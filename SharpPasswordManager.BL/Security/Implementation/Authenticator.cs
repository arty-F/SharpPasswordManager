using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SharpPasswordManager.BL.Security
{
    /*----------------------------------------------------------------------------
     * Result of Autenticate method will be true when the method parameter 
     <password> and <data> recived by constructor will be equal after <data> 
     encryption by <cryptographer> if it used. Key for <cryptographer> assigned by 
     converted method parameter <password> to byte array.

     * Exceptions:

            ArgumentNullException - When recieved <password> or <encryptedPassword>
                                    value in <autenticate> method is null.
            
            NullReferenceException - When try run <ChangeKey> method with null
                                     <cryptographer> value.
    ----------------------------------------------------------------------------*/
    /// <summary>
    /// This class used for autenticate user by entered password.
    /// </summary>
    public class Authenticator : IAuthenticator
    {
        const int authDelay = 500;
        const int authDelayRange = 100;
        Random rng = new Random();

        ICryptographer cryptographer;

        /// <summary>
        /// Create a new class instance.
        /// </summary>
        /// <param name="data">Encrypted data for check autentication.</param>
        /// <param name="cryptographer">Cryptographer for decrypting data.</param>
        public Authenticator(ICryptographer cryptographer = null)
        {
            this.cryptographer = cryptographer;
        }

        /// <summary>
        /// Returns the result of an attempt of autenticate.
        /// </summary>
        /// <param name="password">Entered password.</param>
        /// <returns></returns>
        public async Task<bool> Authenticate(string password, string encryptedPassword)
        {
            if (password == null || encryptedPassword == null)
                throw new ArgumentNullException();

            await Task.Delay(rng.Next(authDelay - authDelayRange, authDelay + authDelayRange));

            if (cryptographer != null)
            {
                cryptographer.ChangeKey(password);

                try
                {
                    return cryptographer.Decrypt(encryptedPassword) == password;
                }
                catch (CryptographicException)
                {
                    return false;
                }
            }
            else
                return encryptedPassword == password;
        }

        /// <summary>
        /// Change key of inner cryptographer instance.
        /// </summary>
        /// <param name="newKey">New key.</param>
        public void ChangeKey(string newKey)
        {
            if (cryptographer != null)
                cryptographer.ChangeKey(newKey);
            else
                throw new NullReferenceException(nameof(cryptographer));
        }
    }
}
