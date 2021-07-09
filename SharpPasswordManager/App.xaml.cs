using SharpPasswordManager.Helpers;
using System.Windows;

namespace SharpPasswordManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly private string passwordKey = "Password";
        readonly private string firstLoadView = "Views/FirstLoadView.xaml";
        readonly private string followingLoadView = "Views/PasswordCheckView.xaml";

        /// <summary>
        /// Determines which window to open when app run.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            AppSettingsHelper appSetting = new AppSettingsHelper();

            // If settings[key] already defined set startup uri to default password check view
            if (appSetting.AlreadyExist(passwordKey))
                StartupUri = new System.Uri(followingLoadView, System.UriKind.Relative);
            // Else set startup uri to view which allows to assign password
            else
                StartupUri = new System.Uri(firstLoadView, System.UriKind.Relative);
        }
    }
}
