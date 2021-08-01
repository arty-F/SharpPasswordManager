using SharpPasswordManager.BL.Handlers;
using SharpPasswordManager.BL.Security;

namespace SharpPasswordManager.Infrastructure.Injector
{
    /// <summary>
    /// Main DI app mechanism. Just simple singleton.
    /// </summary>
    public class Injector
    {
        /// <summary>
        /// Instance of injector.
        /// </summary>
        public static Injector Instance { get; private set; }

        /// <summary>
        /// Gets <seealso cref="ISecureHandler"/> injected realization.
        /// </summary>
        public ISecureHandler SecureHandler { get; private set; }

        /// <summary>
        /// Gets <seealso cref="ICryptographer"/> injected realization.
        /// </summary>
        public ICryptographer Cryptographer { get; private set; }

        /// <summary>
        /// Gets <seealso cref="IAuthenticator"/> injected realization.
        /// </summary>
        public IAuthenticator Authenticator { get; private set; }

        /// <summary>
        /// Gets <seealso cref="IAppSettingsHandler"/> injected realization.
        /// </summary>
        public IAppSettingsHandler AppSettingsHandler { get; private set; }

        static Injector()
        {
            Instance = new Injector();
        }

        private Injector()
        {
            InitDependencies();
        }

        private void InitDependencies()
        {
            SecureHandler = new SecureHandler();

            Cryptographer = new Cryptographer();
            
            Authenticator = new Authenticator(Cryptographer);

            AppSettingsHandler = new AppSettingsHandler();
        }
    }
}
