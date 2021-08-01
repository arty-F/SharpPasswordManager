namespace SharpPasswordManager.BL.Handlers
{
    /// <summary>
    /// Provides mechanism to manage app settings.
    /// </summary>
    public interface IAppSettingsHandler
    {
        /// <summary>
        /// Returns is there a key in app settings.
        /// </summary>
        /// <param name="key">Key of settings.</param>
        bool AlreadyExist(string key);

        /// <summary>
        /// Write value to key setting. If key doesn't exist, add that.
        /// </summary>
        /// <param name="key">Key of app setting.</param>
        /// <param name="value">Value of app setting.</param>
        void Write(string key, string value);

        /// <summary>
        /// Get value of recieved key.
        /// </summary>
        /// <param name="key">Key of app setting.</param>
        string GetByKey(string key);

        /// <summary>
        /// Delete settings key.
        /// </summary>
        /// <param name="key">Key of app setting.</param>
        void Delete(string key);
    }
}
