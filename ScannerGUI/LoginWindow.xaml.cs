using System.Windows;
using PlayerIOClient;
using BotBits;
using MapManager;

namespace ScannerGUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private BotBitsClient client;

        public LoginWindow(BotBitsClient client)
        {
            InitializeComponent();

            this.client = client;

            emailBox.Text = Properties.Settings.Default.Email;
            passwordBox.Password = Properties.Settings.Default.Password;
            worldIdBox.Text = Properties.Settings.Default.WorldID;

            MapManagerExtension.LoadInto(client, 22, 11);
        }

        private async void OnLogin(object sender, RoutedEventArgs e)
        {
            var email = emailBox.Text;
            var password = passwordBox.Password;
            var worldId = worldIdBox.Text;

            if (email == "" || password == "" || worldId == null)
            {
                loginButton.Content = "Error. Enter all information!";
                return;
            }

            Properties.Settings.Default.Email = email;
            Properties.Settings.Default.Password = password;
            Properties.Settings.Default.WorldID = worldId;

            loginButton.IsEnabled = false;
            loginButton.Content = "Connecting...";

            try
            {
                await Login.Of(client)
                    .WithEmailAsync(email, password)
                    .CreateJoinRoomAsync(worldId);

                loginButton.Content = "Connected";
                Properties.Settings.Default.Save();
            }
            catch (PlayerIOError)
            {
                loginButton.IsEnabled = true;
                loginButton.Content = "Error. Press to try again...";
            }
        }
    }
}
