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
using static System.Formats.Asn1.AsnWriter;

namespace JeuFleurSae
{
    public partial class MainWindow : Window
    {
        public static DispatcherTimer minuterie;
        public static readonly int PAS_JOUEUR = 5;
        public static readonly int VIE_JOUEUR_MAX = 3;
        public static readonly int VIE_BOSS_MAX = 100;
        public static readonly int DEGATS_EPEE = -5;
        public static readonly int DEGATS_PROJECTILE = -1;
        public static readonly int DISPARITION_BOSS = -100;
        public static readonly int VITESSE_PROJECTILE = 3;
        private static int nbProjectiles = 5;
        private static bool gauche;
        private static bool droite;
        private static Random alea;
        private bool saut = false;
        private Point debutSaut;
        private System.Windows.Vector vitesse;
        private double gravite = 0.3;
        private double hauteurSaut = -5;
        int vieJoueur = VIE_JOUEUR_MAX;
        int vieBoss = VIE_BOSS_MAX;
        private static Image[] lesProjectiles;


        public MainWindow()
        {
            InitializeComponent();
            InitTimer();
            InitProjectiles();
            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;
            alea = new Random();

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
            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                DeplaceProjectile(lesProjectiles[i], boss);
            }

            // Détection des collisions des projectiles avec le joueur
            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                DetecterCollisionJoueur(lesProjectiles[i]);
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
                }
                    
            }
        }
        private void InitProjectiles()
        {

            BitmapImage imgProjectile = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_Terre.png"));
            lesProjectiles = new Image[nbProjectiles];

            // Assurez-vous que le boss est déjà affiché avant d'initialiser les projectiles.
            double bossGauche = Canvas.GetLeft(boss);
            double bossHaut = Canvas.GetTop(boss);
            double bossBas = bossHaut + boss.Height;

            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                lesProjectiles[i] = new Image();
                lesProjectiles[i].Width = 25;   // Taille du projectile
                lesProjectiles[i].Height = 25;  // Taille du projectile
                lesProjectiles[i].Source = imgProjectile;

                // Ajoutez le projectile à la zone de jeu
                zone.Children.Add(lesProjectiles[i]);

                // Appliquer la position au projectile
                Canvas.SetLeft(lesProjectiles[i], bossGauche - lesProjectiles[i].Width);
                Canvas.SetTop(lesProjectiles[i], alea.Next((int)bossHaut, (int)bossBas - (int)lesProjectiles[i].Height));
            }
        }

        private void DetecterCollisionJoueur(Image imgProjectile)
        {
            double projectileGauche = Canvas.GetLeft(imgProjectile);
            double projectileHaut = Canvas.GetTop(imgProjectile);
            double projectileDroit = projectileGauche + imgProjectile.Width;
            double projectileBas = projectileHaut + imgProjectile.Height;

            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;

            // Vérifier si le projectile touche le joueur
            if (projectileDroit > joueurGauche && projectileGauche < joueurDroit &&
                projectileBas > joueurHaut && projectileHaut < joueurBas)
            {
                // Le projectile touche le joueur
                vieJoueur += DEGATS_PROJECTILE;  // Les projectiles infligent des dégâts au joueur

                // Mettre à jour la vie du joueur en chanceant les images du coeur
                

                // Si la vie du joueur atteint 0 ou moins, le jeu est terminé
                if (vieJoueur <= 0)
                {
                    MessageBox.Show("Game Over, vous avez perdu !", "Perte", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Vous pouvez ajouter ici du code pour réinitialiser le jeu ou arrêter l'exécution.
                }

                // Réinitialiser la position du projectile après qu'il ait touché le joueur
                Canvas.SetLeft(imgProjectile, Canvas.GetLeft(boss) - 30);  // Repositionner à gauche du boss
                Canvas.SetTop(imgProjectile, Canvas.GetTop(boss) + 20);  // Repositionner sous le boss
            }
        }

        private void DeplaceProjectile(Image imgProjectile, System.Windows.Shapes.Rectangle joueur)
        {
            bool toucher = false;
            double projectileGauche = Canvas.GetLeft(imgProjectile);
            double projectileHaut = Canvas.GetTop(imgProjectile);
            double projectileDroit = projectileGauche + imgProjectile.Width;
            double projectileBas = projectileHaut + imgProjectile.Height;

            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;

            // Déplacer le projectile vers la gauche
            double newPosition = Canvas.GetLeft(imgProjectile) - VITESSE_PROJECTILE;  // Déplacement constant vers la gauche
            Canvas.SetLeft(imgProjectile, newPosition);

            System.Drawing.Rectangle rImgProjectile = new System.Drawing.Rectangle((int)Canvas.GetLeft(imgProjectile), (int)Canvas.GetTop(imgProjectile),
                (int)imgProjectile.Width, (int)imgProjectile.Height);

            // Vérifier la collision avec le Père Noël
            if (joueurDroit > projectileGauche + 10 && joueurGauche < projectileDroit && joueurBas > projectileHaut && joueurHaut < projectileBas)
            {

                toucher = true;
            }

            if (toucher)
            {
                MessageBox.Show("toucher", "Perte", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            // Si le projectile sort de l'écran ou touche le Père Noël, le réinitialiser
            if (newPosition < 0 || toucher)
            {
                Canvas.SetLeft(imgProjectile, Canvas.GetLeft(boss) - 30);  // Repositionner à gauche du boss
                Canvas.SetTop(imgProjectile, Canvas.GetTop(boss) + 20);  // Repositionner sous le boss
            }
        }
    }
}