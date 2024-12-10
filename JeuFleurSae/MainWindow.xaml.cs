using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JeuFleurSae
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static DispatcherTimer minuterie;

        public MainWindow()
        {
            InitializeComponent();
            InitTimer();
        }

        public void Jeu(object? sender, System.EventArgs e)
        {

            double joueurTop = Canvas.GetTop(Joueur);
            double joueurBottom = joueurTop + Joueur.Height;
            double solTop = Canvas.GetTop(Sol);



        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) - 5);
            }
            else if (e.Key == Key.Right)
            {
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + 5);
            }

        }

        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            minuterie.Start();
        }
    }
}