using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NESEmulatorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            var engine = new Engine();
            var myCanvas = new Canvas
            {
                Background = Brushes.LightSteelBlue,
                Height = 240,
                Width = 256
            };

            engine.Initialise(myCanvas);
            Content = myCanvas;
        }
    }
}