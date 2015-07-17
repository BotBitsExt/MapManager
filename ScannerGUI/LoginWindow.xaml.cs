using System.Windows;
using PlayerIOClient;
using BotBits;
using BotBits.Events;
using MapManager;
using MapManager.Events;

namespace ScannerGUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private BotBitsClient client;

        public LoginWindow(BotBitsClient client)
        {
            InitializeComponent();

            this.client = client;

            emailBox.Text = Properties.Settings.Default.Email;
            passwordBox.Password = Properties.Settings.Default.Password;
            worldIdBox.Text = Properties.Settings.Default.WorldID;

            MapManagerExtension.LoadInto(client);
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
                await ConnectionManager.Of(client)
                    .EmailLoginAsync(email, password)
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
