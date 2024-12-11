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

namespace JeuFleurSae
{
    public partial class MainWindow : Window
    {
        public static DispatcherTimer minuterie;
        public static readonly int PAS_JOUEUR = 5;
        public static readonly int VIE_JOUEUR_MAX = 3;
        public static readonly int VIE_BOSS_MAX = 100;
        public static readonly int DEGATS_EPEE = -5;
        public static readonly int DISPARITION_BOSS = -100;
        private static bool gauche;
        private static bool droite;
        private bool saut = false;
        private Point debutSaut;
        private System.Windows.Vector vitesse;
        private double gravite = 0.3;
        private double hauteurSaut = -5;
        int vieJoueur = VIE_JOUEUR_MAX;
        int vieBoss = VIE_BOSS_MAX;
        int compteurBoss = 0;

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
            double nouveauXJoueur = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurBas = joueurHaut + joueur.Height;  // Position du bas du joueur
            double solHaut = Canvas.GetTop(sol);  // Position du sol
            
            // Déplacement horizontal
            if (gauche && !droite)
            {
                nouveauXJoueur = Canvas.GetLeft(joueur) - PAS_JOUEUR;
            }
            else if (droite && !gauche)
            {
                nouveauXJoueur = Canvas.GetLeft(joueur) + PAS_JOUEUR;
            }

            // Vérifiez si le personnage ne dépasse pas les bords de la zone
            if (nouveauXJoueur <= zone.ActualWidth - joueur.Width && nouveauXJoueur >= 0)
            {
                Canvas.SetLeft(joueur, nouveauXJoueur);
            }

            // Appliquer la gravité et mettre à jour la position verticale du joueur
            if (saut)
            {
                vitesse.Y += gravite; // Applique la gravité au personnage

                // Mettre à jour la position en fonction de la vitesse
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesse.X); // Déplacement horizontal
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) + vitesse.Y); // Déplacement vertical

                // Vérification de la collision avec le sol
                if (Canvas.GetTop(joueur) + joueur.Height >= solHaut)
                {
                    saut = false; // Arrête le saut
                    vitesse = new System.Windows.Vector(0, 0); // Arrête la vitesse verticale
                    Canvas.SetTop(joueur, solHaut - joueur.Height); // Positionne le joueur juste sur le sol
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
            if (e.Key == Key.Space && !saut)
            {
                debutSaut = new Point(Canvas.GetLeft(joueur), Canvas.GetTop(joueur));
                saut = true;
                vitesse = new System.Windows.Vector(0, hauteurSaut); // Saut vers le haut
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
            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;
            double bossGauche = Canvas.GetLeft(boss);
            double bossHaut = Canvas.GetTop(boss);
            double bossDroite = bossGauche + boss.Width;
            double bossBas = bossHaut + boss.Height;
            int compteurVie = 0;

            if (joueurDroit > bossGauche + 10 && joueurGauche < bossDroite && joueurBas > bossHaut && joueurHaut < bossBas)
            {
                
                Canvas.SetTop(joueur, joueurHaut);
                Canvas.SetLeft(joueur, bossGauche - joueur.Width);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;
            double bossGauche = Canvas.GetLeft(boss);
            double bossHaut = Canvas.GetTop(boss);
            double bossDroite = bossGauche + boss.Width;
            double bossBas = bossHaut + boss.Height;

            if (joueurDroit > bossGauche -10 && joueurGauche < bossDroite && joueurBas > bossHaut)
            {
                
                vieBoss += DEGATS_EPEE;
                this.labVieBoss.Content = vieBoss;
                if (vieBoss == 0)
                {
                    Canvas.SetTop(boss, DISPARITION_BOSS);
                    Canvas.SetTop(labVieBoss, DISPARITION_BOSS);
                    MessageBox.Show("Bien Joué, vous avez tuer le boss", "Victoire", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (compteurBoss <= 5)
                    {
                        compteurBoss++;
                        ImageBrush ib = new ImageBrush();
                        BitmapImage bmi = new BitmapImage(new Uri("pack://application:,,,/img/Fleur/fleur" + (compteurBoss) + ".png"));
                        ib.ImageSource = bmi;
                        fleur.Fill = ib;
                    }
                    
                }
                    
            }
        }
    }
}