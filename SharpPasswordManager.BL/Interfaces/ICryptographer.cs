namespace SharpPasswordManager.BL.Interfaces
{
    /// <summary>
    /// Provides a mechanism to data encryption/decryption.
    /// </summary>
    public interface ICryptographer
    {
        string Encypt(string data);
        string Decrypt(string data);
        void ChangeKey(string newKey);
    }
}
