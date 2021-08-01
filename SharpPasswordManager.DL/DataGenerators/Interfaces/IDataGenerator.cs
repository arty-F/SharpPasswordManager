namespace SharpPasswordManager.DL.DataGenerators
{
    /// <summary>
    /// Provides a mechanism to generate random data.
    /// </summary>
    public interface IDataGenerator
    {
        /// <summary>
        /// Generate random string that simulated data url.
        /// </summary>
        string GenerateRandomUrl();

        /// <summary>
        /// Generate random string that simulated user login.
        /// </summary>
        /// <returns>Random generated string.</returns>
        string GenerateRandomLogin();

        /// <summary>
        /// Generate random string that simulated password.
        /// </summary>
        string GenerateRandomPassword();

        /// <summary>
        /// Generate random string that simulated password of fixes length.
        /// </summary>
        /// <param name="lenght">Password length.</param>
        string GenerateRandomPassword(int lenght);

        /// <summary>
        /// Generate random datetime in string format.
        /// </summary>
        string GenerateRandomDate();
    }
}
