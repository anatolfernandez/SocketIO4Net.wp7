using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SocketIOClient;
using SocketIOClient.Messages;

namespace TestApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly Client _client;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _client = new Client("http://10.30.200.81:1337");

            _client.On("open", message =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Chat.Text += "Connection opened\n";
                });
            });

            _client.On("echo", message =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Chat.Text += message.Json.Args[0] + "\n";
                });
            });

        }

        private void connect(object sender, GestureEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((o) => _client.Connect());
        }

        private void send(object sender, GestureEventArgs e)
        {
            _client.Emit("echo", Input.Text);
        }
    }
}