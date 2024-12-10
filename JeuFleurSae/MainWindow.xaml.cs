using System.Numerics;
using System.Security.Policy;
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
        public static DispatcherTimer minuterie;
        public static readonly int PAS_JOUEUR = 5;
        private static bool gauche;
        private static bool droite;
        public MainWindow()
        {
            InitializeComponent();
            InitTimer();
        }

        public void Jeu(object? sender, System.EventArgs e)
        {
            double nouveauJoueurGauche = Canvas.GetLeft(Joueur);
            double joueurTop = Canvas.GetTop(Joueur);
            double joueurBottom = joueurTop + Joueur.Height;
            double solTop = Canvas.GetTop(Sol);

            if (gauche == true && !droite)
            {
                nouveauJoueurGauche = Canvas.GetLeft(Joueur) - PAS_JOUEUR;
            }
            else if (droite == true && !gauche)
            {
                nouveauJoueurGauche = Canvas.GetLeft(Joueur) + PAS_JOUEUR;
            }
            if (nouveauJoueurGauche <= Zone.ActualWidth - Joueur.Width && nouveauJoueurGauche >= 0)
                Canvas.SetLeft(Joueur, nouveauJoueurGauche);

        }

        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            minuterie.Start();
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                gauche = true;
            }
            else if (e.Key == Key.Right)
            {
                droite = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                gauche = false;
            }
            else if (e.Key == Key.Right)
            {
                droite = false;
            }
        }
    }
}