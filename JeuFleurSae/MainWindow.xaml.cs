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
        public const double GRAVITE = 1.0;
        public const double FORCE_SAUT = 20.0;
        public static bool gauche;
        public static bool droite;
        public bool auSol = true;
        public double joueurVitesseY = 0;

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
            
            if (joueurBottom >= solTop)
            {
                auSol = true;
                joueurVitesseY = 0; 
                Canvas.SetTop(Joueur, solTop - Joueur.Height);
            }
            else
            {
                auSol = false;
                joueurVitesseY += GRAVITE;
                Canvas.SetTop(Joueur, joueurTop + joueurVitesseY);
            }

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
            else if (e.Key == Key.Space && auSol)
            {
                joueurVitesseY = -FORCE_SAUT; 
                auSol = false;
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