using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickChat.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Тестовые данные
            ChatsList.Items.Add(new { Name = "Общий чат" });
            MessagesList.Items.Add(new { Text = "Привет! Добро пожаловать в QuickChat!" });
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageBox.Text))
            {
                MessagesList.Items.Add(new { Text = MessageBox.Text });
                MessageBox.Clear();
            }
        }
    }
}