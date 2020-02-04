namespace SharpPasswordManager.BL.Interfaces
{
    /// <summary>
    /// Provides a mechanism to autentication.
    /// </summary>
    public interface IAuthenticator
    {
        bool Autenticate(string password, string encryptedPassword);
        void ChangeKey(string newKey);
    }
}
