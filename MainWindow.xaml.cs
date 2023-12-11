using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp10_UDP_Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private string serverAddress = "127.0.0.1"; // IP адрес сервера
        private string name = "";
        private int port; // Порт сервера
        Random random;
        System.Threading.Thread receiveThread;
       
        public MainWindow()
        {
            InitializeComponent();
            udpClient = new UdpClient();
            random = new Random();
            port = random.Next(1024, 65535);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

            // Запускаем отдельный поток для прослушивания входящих сообщений
             receiveThread = new System.Threading.Thread(new System.Threading.ThreadStart(ReceiveMessages));
            receiveThread.Start();
            
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                // Получаем данные от сервера
                IPEndPoint remoteIp = null;
                byte[] receivedBytes = udpClient.Receive(ref remoteIp);

                // Преобразуем данные в строку и отображаем в окне чата
                string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
                Dispatcher.Invoke(() =>
                {
                    chatTextBox.AppendText($"Сервер: {receivedMessage}\n");
                });
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отправляем введенное сообщение на сервер
                string message = $" {name}: {inputTextBox.Text}";
                byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                udpClient.Send(sendBytes, sendBytes.Length, serverAddress, 1234);

                // Отображаем отправленное сообщение в окне чата
                Dispatcher.Invoke(() =>
                {
                    chatTextBox.AppendText($"Вы: {inputTextBox.Text}\n");
                    inputTextBox.Clear();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки сообщения: {ex.Message}!");
            }
        }

        private void nameButton_Click(object sender, RoutedEventArgs e)
        {
            name = nameTextBox.Text;
            SPName.IsEnabled = false;
            SPSend.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            receiveThread.Abort();
        }
    }

}
