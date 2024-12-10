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
        private static bool gauche;
        private static bool droite;
        private bool isJumping = false;
        private Point startPos;
        private Point vertex;
        private Point endPos;
        private System.Windows.Vector velocity;
        private double gravity = 0.3;
        private double jumpHeight = -5; // Hauteur du saut (valeur négative pour aller vers le haut)
        private double jumpSpeed = 0.9; // La vitesse de gravité (à ajuster pour plus ou moins de gravité)
        private DispatcherTimer gameTimer;

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
            if (isJumping)
            {
                velocity.Y += gravity; // Applique la gravité au personnage

                // Mettre à jour la position en fonction de la vitesse
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + velocity.X); // Déplacement horizontal
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) + velocity.Y); // Déplacement vertical

                // Vérification de la collision avec le sol
                if (Canvas.GetTop(Joueur) + Joueur.Height >= solTop)
                {
                    isJumping = false; // Arrête le saut
                    velocity = new System.Windows.Vector(0, 0); // Arrête la vitesse verticale
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
            if (e.Key == Key.Space && !isJumping)
            {
                startPos = new Point(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur));
                isJumping = true;
                velocity = new System.Windows.Vector(0, jumpHeight); // Saut vers le haut
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

        // Déclenché au clic souris


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
    }
}