namespace SharpPasswordManager.Infrastructure.Injector
{
    /// <summary>
    /// Main DI app mechanism. Just simple singleton.
    /// </summary>
    public class Injector
    {
        public static Injector Instance { get; private set; }

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

        }
    }
}
