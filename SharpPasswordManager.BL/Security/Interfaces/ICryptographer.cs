namespace SharpPasswordManager.BL.Security
{
    /// <summary>
    /// Provides a mechanism to data encryption/decryption.
    /// </summary>
    public interface ICryptographer
    {
        /// <summary>
        /// Return encrypted string.
        /// </summary>
        /// <param name="data">String for encryption.</param>
        string Encypt(string data);

        /// <summary>
        /// Return decrypted string.
        /// </summary>
        /// <param name="data">Ecrypted string.</param>
        string Decrypt(string data);

        /// <summary>
        /// Change key for crypto alghoritm and makes it suitable length.
        /// </summary>
        /// <param name="newKey">New key.</param>
        void ChangeKey(string newKey);
    }
}
