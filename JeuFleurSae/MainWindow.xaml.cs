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

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace JeuFleurSae
{
    public partial class MainWindow : Window
    {
        public static DispatcherTimer minuterie;
        public static readonly int PAS_JOUEUR = 5;
        public static readonly int MAX_VIE_JOUEUR = 3;
        public static readonly int MAX_VIE_BOSS = 100;
        public static readonly int DEGATS_EPEE = -5;
        private static bool gauche;
        private static bool droite;
        private bool saute = false;
        private Point debutSaut;
        private System.Windows.Vector vitesse;
        private double gravite = 0.3;
        private double sautHauteur = -5;
        int vieJoueur = MAX_VIE_JOUEUR;
        int vieBoss = MAX_VIE_BOSS;


        public MainWindow()
        {
            InitializeComponent();
            InitTimer();
            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;

        }


        // Logique principale du jeu, appelée chaque frame
        public void Jeu(object? sender, EventArgs e)
        {
            double nouveauJoueurGauche = Canvas.GetLeft(Joueur);
            double joueurTop = Canvas.GetTop(Joueur);
            double joueurBottom = joueurTop + Joueur.Height;  // Position du bas du joueur
            double solTop = Canvas.GetTop(Sol);  // Position du sol
            
            // Déplacement horizontal
            if (gauche && !droite)
            {
                nouveauJoueurGauche = Canvas.GetLeft(Joueur) - PAS_JOUEUR;
            }
            else if (droite && !gauche)
            {
                nouveauJoueurGauche = Canvas.GetLeft(Joueur) + PAS_JOUEUR;
            }

            // Vérifiez si le personnage ne dépasse pas les bords de la zone
            if (nouveauJoueurGauche <= Zone.ActualWidth - Joueur.Width && nouveauJoueurGauche >= 0)
            {
                Canvas.SetLeft(Joueur, nouveauJoueurGauche);
            }

            // Appliquer la gravité et mettre à jour la position verticale du joueur
            if (saute)
            {
                vitesse.Y += gravite; // Applique la gravité au personnage

                // Mettre à jour la position en fonction de la vitesse
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + vitesse.X); // Déplacement horizontal
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) + vitesse.Y); // Déplacement vertical

                // Vérification de la collision avec le sol
                if (Canvas.GetTop(Joueur) + Joueur.Height >= solTop)
                {
                    saute = false; // Arrête le saut
                    vitesse = new System.Windows.Vector(0, 0); // Arrête la vitesse verticale
                    Canvas.SetTop(Joueur, solTop - Joueur.Height); // Positionne le joueur juste sur le sol
                }
            }
            VerifierCollision();
        }

        // Initialisation du Timer
        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            minuterie.Tick += Jeu;
            minuterie.Start();
        }

        // Gestion des appuis de touches pour les déplacements horizontaux
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = true; // Déplacer vers la gauche
            }
            else if (e.Key == Key.D)
            {
                droite = true; // Déplacer vers la droite
            }

            // Si la touche espace est pressée, le personnage saute
            if (e.Key == Key.Space && !saute)
            {
                debutSaut = new Point(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur));
                saute = true;
                vitesse = new System.Windows.Vector(0, sautHauteur); // Saut vers le haut
            }
            
        }

        // Gestion des relâchements de touches
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = false;
            }
            else if (e.Key == Key.D)
            {
                droite = false;
            }
        }

        private void VerifierCollision()
        {
            double joueurLeft = Canvas.GetLeft(Joueur);
            double joueurTop = Canvas.GetTop(Joueur);
            double joueurRight = joueurLeft + Joueur.Width;
            double joueurBottom = joueurTop + Joueur.Height;
            double bossLeft = Canvas.GetLeft(Boss);
            double bossTop = Canvas.GetTop(Boss);
            double bossRight = bossLeft + Boss.Width;
            double bossBottom = bossTop + Boss.Height;

            if (joueurRight > bossLeft+10 && joueurLeft < bossRight && joueurBottom > bossTop && joueurTop < bossBottom)
            {
                
                Canvas.SetTop(Joueur, joueurTop);
                Canvas.SetLeft(Joueur, bossLeft - Joueur.Width);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double joueurLeft = Canvas.GetLeft(Joueur);
            double joueurTop = Canvas.GetTop(Joueur);
            double joueurRight = joueurLeft + Joueur.Width;
            double joueurBottom = joueurTop + Joueur.Height;
            double bossLeft = Canvas.GetLeft(Boss);
            double bossTop = Canvas.GetTop(Boss);
            double bossRight = bossLeft + Boss.Width;
            double bossBottom = bossTop + Boss.Height;

            if (joueurRight > bossLeft -10 && joueurLeft < bossRight && joueurBottom > bossTop)
            {
                vieBoss += DEGATS_EPEE;
                this.LabVieBoss.Content = vieBoss;
                if (vieBoss == 0)
                    MessageBox.Show("Bien Joué, vous avez tuer le Boss", "Victoire", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}