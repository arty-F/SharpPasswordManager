using System.Threading.Tasks;

namespace SharpPasswordManager.BL.Security
{
    /// <summary>
    /// Provides a mechanism to autentication.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Returns the result of an attempt of autenticate.
        /// </summary>
        /// <param name="password">Entered password.</param>
        Task<bool> Authenticate(string password, string encryptedPassword);

        /// <summary>
        /// Change key of inner cryptographer instance.
        /// </summary>
        /// <param name="newKey">New key.</param>
        void ChangeKey(string newKey);
    }
}
