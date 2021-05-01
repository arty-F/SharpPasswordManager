using System.Threading.Tasks;

namespace SharpPasswordManager.BL.Interfaces
{
    /// <summary>
    /// Provides a mechanism to autentication.
    /// </summary>
    public interface IAuthenticator
    {
        Task<bool> Autenticate(string password, string encryptedPassword);
        void ChangeKey(string newKey);
    }
}
