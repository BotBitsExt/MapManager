using System.Windows;
using BotBits;
using MapManager;
using MapManager.Events;

namespace ScannerGUI
{
    /// <summary>
    /// Interaction logic for ScannerWindow.xaml
    /// </summary>
    public partial class ScannerWindow : Window
    {
        private BotBitsClient client;

        public ScannerWindow(BotBitsClient client)
        {
            InitializeComponent();

            this.client = client;
            EventLoader.Of(client).Load(this);
        }

        [EventListener]
        private void OnScanResult(ScanResultEvent e)
        {
            EnableButtons(false);
            EnableSpecialButtons(true);
            ScanButton.IsEnabled = true;
            StatusText.Text = e.Message;
            WorldName.Text = "";
            ScanProgress.Text = e.AcceptedMapsCount + "/" + e.ScannedMapsCount;
        }

        [EventListener]
        private void OnMap(MapForReviewEvent e)
        {
            EnableButtons(true);
            StatusText.Text = "Scanning:";
            WorldName.Text = e.Name + " by " + e.Creators;
            ScanProgress.Text = e.MapNumber + "/" + e.TotalMaps;
        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            var target = ScanTargetBox.Text;
            if (target == "")
                return;

            ScanButton.IsEnabled = false;
            new ScanRequestEvent(target).RaiseIn(client);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            SendResult(ReviewResult.Accepted);
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            SendResult(ReviewResult.Rejected);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            SendResult(ReviewResult.Stopped);
        }

        private void SendResult(ReviewResult result)
        {
            EnableButtons(false);
            new MapReviewedEvent(result).RaiseIn(client);
        }

        private void EnableButtons(bool enable)
        {
            AcceptButton.IsEnabled = enable;
            RejectButton.IsEnabled = enable;
            StopButton.IsEnabled = enable;
            EnableSpecialButtons(enable);
        }

        private void EnableSpecialButtons(bool enable)
        {
            ClearEmptyButton.IsEnabled = enable;
            BuildBordersButton.IsEnabled = enable;
        }

        private async void ClearEmpty_Click(object sender, RoutedEventArgs e)
        {
            EnableSpecialButtons(false);
            await BlockChecker.Of(client).FinishChecksAsync();
            EnableSpecialButtons(true);
        }

        private async void BuildBorders_Click(object sender, RoutedEventArgs e)
        {
            EnableSpecialButtons(false);
            await BlockChecker.Of(client).FinishChecksAsync();
            EnableSpecialButtons(true);
        }
    }
}
