using Azure.Communication.Calling.WindowsClient;
using Microsoft.UI.Xaml;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Dotnet8
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const string authToken = "<Azure Communication Services auth token>";

        private CallClient callClient;
        private CallTokenRefreshOptions callTokenRefreshOptions;
        private CallAgent callAgent;
        private CommunicationCall call = null;

        private LocalOutgoingAudioStream micStream;
        private LocalOutgoingVideoStream cameraStream;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.callClient = new CallClient(new CallClientOptions()
                {
                    Diagnostics = new CallDiagnosticsOptions()
                    {
                        AppName = "CallingQuickstart",
                        AppVersion = "1.0",
                        Tags = new[] { "Calling", "ACS", "Windows" }
                    }
                });

                // Set up local video stream using the first camera enumerated
                var deviceManager = await this.callClient.GetDeviceManagerAsync();
                var camera = deviceManager?.Cameras?.FirstOrDefault();
                var mic = deviceManager?.Microphones?.FirstOrDefault();
                micStream = new LocalOutgoingAudioStream();

                callTokenRefreshOptions = new CallTokenRefreshOptions(false);
                callTokenRefreshOptions.TokenRefreshRequested += CallTokenRefreshOptions_TokenRefreshRequested; ;

                var tokenCredential = new CallTokenCredential(authToken, callTokenRefreshOptions);

                var callAgentOptions = new CallAgentOptions()
                {
                    DisplayName = "Contoso",
                    //https://github.com/lukes/ISO-3166-Countries-with-Regional-Codes/blob/master/all/all.csv
                    EmergencyCallOptions = new EmergencyCallOptions() { CountryCode = "840" }
                };


                try
                {
                    this.callAgent = await this.callClient.CreateCallAgentAsync(tokenCredential, callAgentOptions);
                    //await this.callAgent.RegisterForPushNotificationAsync(await this.RegisterWNS());
                    this.callAgent.CallsUpdated += CallAgent_CallsUpdated; ;
                    this.callAgent.IncomingCallReceived += CallAgent_IncomingCallReceived; ;

                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2147024809)
                    {
                        // E_INVALIDARG
                        // Handle possible invalid token
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void CallAgent_IncomingCallReceived(object sender, IncomingCallReceivedEventArgs e)
        {
        }

        private void CallAgent_CallsUpdated(object sender, CallsUpdatedEventArgs e)
        {
        }

        private void CallTokenRefreshOptions_TokenRefreshRequested(object sender, CallTokenRefreshRequestedEventArgs e)
        {
        }
    }
}
