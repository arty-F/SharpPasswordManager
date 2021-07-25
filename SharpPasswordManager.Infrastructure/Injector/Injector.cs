using SharpPasswordManager.BL.Handlers;
using SharpPasswordManager.BL.Security;

namespace SharpPasswordManager.Infrastructure.Injector
{
    /// <summary>
    /// Main DI app mechanism. Just simple singleton.
    /// </summary>
    public class Injector
    {
        public static Injector Instance { get; private set; }

        public ISecureHandler SecureHandler;

        public ICryptographer Cryptographer;

        public IAuthenticator Autheticator;

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
            
            Autheticator = new Authenticator(Cryptographer);
        }
    }
}
