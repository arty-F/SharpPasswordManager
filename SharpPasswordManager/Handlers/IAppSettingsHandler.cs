namespace SharpPasswordManager.Handlers
{
    /// <summary>
    /// Provides mechanism to manage app settings.
    /// </summary>
    public interface IAppSettingsHandler
    {
        bool AlreadyExist(string key);
        void Write(string key, string value);
        string GetByKey(string key);
        void Delete(string key);
    }
}
