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
using System.Media;

namespace JeuFleurSae
{
    public partial class MainWindow : Window
    {
        public static DispatcherTimer minuterie;
        private bool enCooldownAttaqueRenforce = false;
        private DispatcherTimer timerCooldownAttaque;
        public static readonly int PAS_JOUEUR = 5;
        public static readonly int VIE_JOUEUR_MAX = 3;
        public static readonly int VIE_JOUEUR_MAX_NIVEAU_SIX = 6;
        public static readonly int VIE_JOUEUR_MINI = 0;
        public static readonly int VIE_BOSS_MAX = 100;
        public static readonly int VIE_BOSS_MINI = 0;
        public static readonly int VIE_BOSS_MOITIER = 50;
        public static readonly int VIE_BOSS_TRENTE = 30;
        public static readonly int DEGATS_EPEE = -5;
        public static readonly int DEGATS_PROJECTILE = -1;
        public static readonly int DEGATS_BOULE_DE_FEU = -2;
        public static readonly int DISPARITION = -100;
        public static readonly int VITESSE_PROJECTILE_BASE = 3;
        public static readonly int MARGE_COLLISION = 10;
        private static int nbProjectiles = 3;
        public static readonly int POSITION_JOUEUR_DEBUT_GAUCHE = 22;
        public static readonly int POSITION_JOUEUR_DEBUT_HAUT = 352;
        public static readonly int POSITION_BOSS_DEBUT_GAUCHE = 641;
        public static readonly int POSITION_BOSS_DEBUT_HAUT = 292;
        public static readonly int POSITION_COEUR_DEBUT_GAUCHE = 10;
        public static readonly int POSITION_COEUR_DEBUT_HAUT = 10;
        public static readonly int POSITION_FLEUR_DEBUT_GAUCHE = 11;
        public static readonly int POSITION_FLEUR_DEBUT_HAUT = 67;
        public static readonly int POSITION_VIE_BOSS_DEBUT_GAUCHE = 659;
        public static readonly int POSITION_VIE_BOSS_DEBUT_HAUT = 252;
        public static readonly int POSITION_LORE_DEBUT_HAUT = -454;
        public static readonly int POSITION_PAUSE_DEBUT_GAUCHE = 800;
        public static readonly int POSITION_JOUER_DEBUT_HAUT = -127;
        public static readonly int POSITION_JOUER_HAUT = 342;
        public static readonly int NIVEAU_MAX_BOSS = 6;
        public static readonly int CONSTANTE_POUR_DEBUG = 75;
        public static readonly int POSITION_ZERO = 0;
        public static readonly int LARGEUR_PROJECTILE = 25;
        public static readonly int HAUTEUR_PROJECTILE = 25;
        public static readonly int LARGEUR_PROJECTILE_JOUEUR = 44;
        public static readonly int HAUTEUR_PROJECTILE_JOUEUR = 25;
        public static readonly int LARGEUR_BOUCLIER_JOUEUR = 19;
        public static readonly int HAUTEUR_BOUCLIER_JOUEUR = 45;
        public static readonly int LARGEUR_COEUR_SIX = 320;
        public static readonly int LARGEUR_COEUR_TROIS = 160;
        public static readonly int COMPTEUR_FRAME = 0;
        public static readonly int SPRITE_ATTAQUE_MAX = 4;
        public static readonly int SPRITE_ATTAQUE_MINI = 1;
        public static readonly double VITESSE_CHANGEMENT_SPRITE = 0.5;
        public static readonly int SPRITE_INT_MIN = 0;
        public static readonly int SPRITE_INT_MAX = 7;
        public static readonly int NB_PROJECTILE_DEBUT = 3;
        public static readonly int REPOSITIONNEMENT_PROJECTILE_GAUCHE = 30;
        private static bool gauche;
        private static bool droite;
        private static bool lanceProjectile = false;
        private static bool clickAttaque = false;
        private bool projectileEnCours = false;
        private static Random alea;
        private bool saut = false;
        bool attaqueRenforce = false;
        private System.Windows.Vector vitesse;
        private double gravite = 0.3;
        private double hauteurSaut = -8;
        int vieJoueur = VIE_JOUEUR_MAX;
        int vieBoss = VIE_BOSS_MAX;
        private static Image[] lesProjectiles;
        private static Image projectileJoueur;
        private static Image bouclier;
        int NiveauFLeur = 0;
        int compteurVie = 3;
        double SpriteInt = 0;
        double SpriteIntGauche = 0;
        double SpriteAtkInt = 0;
        int niveauBoss = 1;
        bool toucher = false;
        private TimeSpan delayAttaque = TimeSpan.FromSeconds(0.6); // Temps entre les attaques
        private DateTime tempsDepuisAttaque = DateTime.MinValue;

        private int compteurFrameMouvement = 0; // Compteur pour ralentir les animations de mouvement
        private int compteurFrameAttaque = 0;  // Compteur pour ralentir les animations d'attaque
        private readonly int delayFrame = 5;  // Nombre de frames à attendre avant de changer l'image
        private static SoundPlayer sonGagne;
        private static MediaPlayer musique;
        public static double NiveauSon { get; set; }
        private bool peutDoubleSauter = false;
        private DispatcherTimer timerBouclier;
        private DispatcherTimer timerCooldownBouclier;
        private bool enCooldownBouclier = false;
        private Image bouclierActif = null;

        public MainWindow()
        {
            InitializeComponent();
        }
        public void preLancement()
        {
            Canvas.SetLeft(lore, POSITION_ZERO);
            Canvas.SetTop(lore, POSITION_ZERO);
            Canvas.SetLeft(butJouer, zone.ActualWidth/2);
            Canvas.SetTop(butJouer, POSITION_JOUER_HAUT);
        }
        public void Lancement()
        {
            InitTimer();
            InitCooldownTimer();
            alea = new Random();
            ImageBrush ibFond = new ImageBrush();
            BitmapImage bmiFond = new BitmapImage(new Uri("pack://application:,,,/img/Fond_niveaux/fond_niveau_1.png"));
            ibFond.ImageSource = bmiFond;
            zone.Background = ibFond;
            Canvas.SetTop(joueur, POSITION_JOUEUR_DEBUT_HAUT);
            Canvas.SetLeft(joueur, POSITION_JOUEUR_DEBUT_GAUCHE);
            Canvas.SetTop(boss, POSITION_BOSS_DEBUT_HAUT);
            Canvas.SetLeft(boss, POSITION_BOSS_DEBUT_GAUCHE);
            Canvas.SetTop(rectCoeur, POSITION_COEUR_DEBUT_HAUT);
            Canvas.SetLeft(rectCoeur, POSITION_COEUR_DEBUT_GAUCHE);
            Canvas.SetTop(fleur, POSITION_FLEUR_DEBUT_HAUT);
            Canvas.SetLeft(fleur, POSITION_FLEUR_DEBUT_GAUCHE);
            Canvas.SetTop(labVieBoss, POSITION_VIE_BOSS_DEBUT_HAUT);
            Canvas.SetLeft(labVieBoss, POSITION_VIE_BOSS_DEBUT_GAUCHE);
            Canvas.SetLeft(lore, POSITION_ZERO);
            Canvas.SetTop(lore, POSITION_LORE_DEBUT_HAUT);
            Canvas.SetLeft(butJouer, zone.ActualWidth / 2);
            Canvas.SetTop(butJouer, POSITION_JOUER_DEBUT_HAUT);
            InitProjectiles();
            InitSon();
            InitMusique();
            InitBouclierTimers();
        }

        public void Jeu(object? sender, EventArgs e)
        {
            double nouveauXJoueur = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurBas = joueurHaut + joueur.Height;  // Position du bas du joueur
            double solHaut = Canvas.GetTop(sol);  // Position du sol
            alea = new Random();
            // Déplacement horizontal
            if (gauche && !droite)
            {
                nouveauXJoueur = Canvas.GetLeft(joueur) - PAS_JOUEUR;
                if (!saut)
                {
                    animationCourseGauche();
                }
            }
            else if (droite && !gauche)
            {
                nouveauXJoueur = Canvas.GetLeft(joueur) + PAS_JOUEUR;
                if (!saut)
                {
                    animationCourse();
                }
            }

            if (clickAttaque == true)
            {
                animationAttaque();

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
                    vitesse = new System.Windows.Vector(0, 0); // Arrête la vitesse verticale
                    Canvas.SetTop(joueur, solHaut - joueur.Height); // Positionne le joueur juste sur le sol
                    saut = false;
                    peutDoubleSauter = false; // Réinitialise la capacité de double saut
                }

            }
            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                DeplaceProjectile(lesProjectiles[i], boss);
            }

            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                DetecterCollisionJoueurProjectile(lesProjectiles[i]);
            }
            DetecterCollisionJoueurBoss();
            if (compteurVie == VIE_JOUEUR_MINI)
            {
                reset();

            }
            if (projectileEnCours)
            {

                Console.WriteLine("Boule de feu");
                double joueurGauche = Canvas.GetLeft(joueur);
                double joueurDroit = joueurGauche + joueur.Width;
                double bossGauche = Canvas.GetLeft(boss);
                double bossHaut = Canvas.GetTop(boss);
                double bossDroite = bossGauche + boss.Width;
                double bossBas = bossHaut + boss.Height;
                BitmapImage imgProjectileJoueur = (BitmapImage)projectileJoueur.Source;
                double projectileJoueurGauche = Canvas.GetLeft(projectileJoueur);
                double projectileJoueurHaut = Canvas.GetTop(projectileJoueur);
                double projectileJoueurDroit = projectileJoueurGauche + projectileJoueur.Width;
                double projectileJoueurBas = projectileJoueurHaut + projectileJoueur.Height;

                bool projectileToucheBoss = projectileJoueurDroit > bossGauche && projectileJoueurGauche < bossDroite && projectileJoueurBas > bossHaut && projectileJoueurHaut < bossBas;
                bool projectileToucheMur = projectileJoueurDroit > zone.ActualWidth;
                Console.WriteLine($"Projectile - Gauche: {projectileJoueurGauche}, Droite: {projectileJoueurDroit}");
                Console.WriteLine($"Boss - Gauche: {bossGauche}, Droite: {bossDroite}");
                Console.WriteLine($"Collision: {projectileToucheBoss}");

                if (!projectileToucheBoss && !projectileToucheMur)
                    Canvas.SetLeft(projectileJoueur, projectileJoueurGauche + MARGE_COLLISION);
                else
                {
                    if (projectileToucheBoss)
                    {
                        vieBoss += DEGATS_BOULE_DE_FEU;
                        this.labVieBoss.Content = vieBoss;
                        zone.Children.Remove(projectileJoueur);
                        projectileJoueur = null;
                        projectileEnCours = false;
                        ChangementBoss();
                    }
                    if (projectileToucheMur)
                    {
                        Console.WriteLine("Rentrer");
                        zone.Children.Remove(projectileJoueur);
                        projectileEnCours = false;
                    }
                }
            }
            if (bouclierActif != null)
            {
                double joueurGauche = Canvas.GetLeft(joueur);
                double joueurDroit = joueurGauche + joueur.Width;

                Canvas.SetLeft(bouclierActif, joueurDroit + bouclierActif.Width);
                Canvas.SetTop(bouclierActif, joueurHaut - 20);

                // Détecter les collisions avec les projectiles
                DetecterCollisionBouclierProjectiles();
            }
        }

        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            minuterie.Start();
        }

        private void InitCooldownTimer()
        {
            timerCooldownAttaque = new DispatcherTimer();
            timerCooldownAttaque.Interval = TimeSpan.FromSeconds(5);
            timerCooldownAttaque.Tick += FinCooldownAttaque;
        }

        private void FinCooldownAttaque(object sender, EventArgs e)
        {
            enCooldownAttaqueRenforce = false;
            timerCooldownAttaque.Stop();
            Console.WriteLine("Attaque renforcée disponible !");

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key);
            double PauseHaut = Canvas.GetLeft(labPause);
            if (e.Key == Key.Q)
            {
                gauche = true;
                Arret();
            }
            else if (e.Key == Key.D)
            {
                droite = true;
                Arret();
            }

            if (e.Key == Key.Space)
            {
                if (!saut)
                {
                    // Premier saut
                    saut = true;
                    vitesse = new System.Windows.Vector(0, hauteurSaut);
                    if (niveauBoss >= 5)
                        peutDoubleSauter = true; // Permet le double saut
                }
                else if (peutDoubleSauter)
                {
                    // Double saut
                    vitesse = new System.Windows.Vector(0, hauteurSaut);
                    peutDoubleSauter = false; // Empêche les sauts supplémentaires
                }
            }
            if (e.Key == Key.P)
            {
                if (minuterie.IsEnabled)
                {
                    minuterie.Stop();
                    Canvas.SetLeft(labPause, POSITION_ZERO);
                }
                else
                {
                    minuterie.Start();
                    Canvas.SetLeft(labPause, POSITION_PAUSE_DEBUT_GAUCHE);
                }
            }

            if (e.Key == Key.A)
            {
                Pouvoir();
            }
            if (e.Key == Key.E)
            {
                Bouclier(bouclier);
            }
            if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad6)
            {
                double joueurGauche = Canvas.GetLeft(joueur);
                double joueurHaut = Canvas.GetTop(joueur);
                double joueurDroit = joueurGauche + joueur.Width;
                double joueurBas = joueurHaut + joueur.Height;
                double bossGauche = Canvas.GetLeft(boss);
                double bossHaut = Canvas.GetTop(boss);
                double bossDroite = bossGauche + boss.Width;
                double bossBas = bossHaut + boss.Height;

                niveauBoss = (int)e.Key - CONSTANTE_POUR_DEBUG;
                NiveauFLeur = niveauBoss;
                if (niveauBoss <= NIVEAU_MAX_BOSS - 1)
                {
                    Canvas.SetTop(joueur, POSITION_JOUEUR_DEBUT_HAUT);
                    Canvas.SetLeft(joueur, POSITION_JOUEUR_DEBUT_GAUCHE);
                    Canvas.SetTop(boss, POSITION_BOSS_DEBUT_HAUT);
                    Canvas.SetLeft(boss, POSITION_BOSS_DEBUT_GAUCHE);
                    Canvas.SetTop(labVieBoss, POSITION_VIE_BOSS_DEBUT_HAUT);
                    Canvas.SetLeft(labVieBoss, POSITION_VIE_BOSS_DEBUT_GAUCHE);
                    niveauBoss++;
                    ImageBrush ibBoss = new ImageBrush();
                    BitmapImage bmiBoss = new BitmapImage(new Uri("pack://application:,,,/img/Boss/boss" + (niveauBoss) + ".png"));
                    ibBoss.ImageSource = bmiBoss;
                    boss.Fill = ibBoss;
                    ImageBrush ibFond = new ImageBrush();
                    BitmapImage bmiFond = new BitmapImage(new Uri("pack://application:,,,/img/Fond_niveaux/fond_niveau_" + (niveauBoss) + ".png"));
                    ibFond.ImageSource = bmiFond;
                    zone.Background = ibFond;
                    for (int i = 0; i < lesProjectiles.Length; i++)
                    {
                        zone.Children.Remove(lesProjectiles[i]);
                    }
                }
                else
                {
                    nbProjectiles = 5;
                }
                lesProjectiles = new Image[nbProjectiles];
                for (int i = 0; i < lesProjectiles.Length; i++)
                {
                    lesProjectiles[i] = new Image();
                    lesProjectiles[i].Width = LARGEUR_PROJECTILE;
                    lesProjectiles[i].Height = HAUTEUR_PROJECTILE;
                    lesProjectiles[i].Source = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_" + (niveauBoss) + ".png"));
                    zone.Children.Add(lesProjectiles[i]);
                    Canvas.SetTop(lesProjectiles[i], alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                    Canvas.SetLeft(lesProjectiles[i], Canvas.GetLeft(boss) - REPOSITIONNEMENT_PROJECTILE_GAUCHE);
                }
            }
            if (e.Key == Key.C)
            {
                vieBoss = VIE_BOSS_MINI + 1;
            }
            ImageBrush ibFleur = new ImageBrush();
            BitmapImage bmiFleur = new BitmapImage(new Uri("pack://application:,,,/img/Fleur/fleur" + (NiveauFLeur) + ".png"));
            ibFleur.ImageSource = bmiFleur;
            fleur.Fill = ibFleur;
        }

        public void Pouvoir()
        {
            if (NiveauFLeur < 1)
            {
                Console.WriteLine("L'attaque renforcée n'est pas encore disponible ! Pouvoir verrouillé");
                return;
            }

            if (enCooldownAttaqueRenforce)
            {
                Console.WriteLine("Attaque renforcée en cooldown. Veuillez patienter.");
                return;
            }
            attaqueRenforce = true;
            enCooldownAttaqueRenforce = true;
            Console.WriteLine("Attaque renforcée activée !");
            timerCooldownAttaque.Start();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = false;
                ImageBrush ibArret = new ImageBrush();
                BitmapImage bmiArret = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arret.png"));
                ibArret.ImageSource = bmiArret;
                joueur.Fill = ibArret;

            }
            else if (e.Key == Key.D)
            {
                droite = false;
            }
        }

        private void DetecterCollisionJoueurBoss()
        {
            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;
            double bossGauche = Canvas.GetLeft(boss);
            double bossHaut = Canvas.GetTop(boss);
            double bossDroite = bossGauche + boss.Width;
            double bossBas = bossHaut + boss.Height;


            if (joueurDroit > bossGauche + MARGE_COLLISION && joueurGauche < bossDroite && joueurBas > bossHaut && joueurHaut < bossBas)    //Detect si le joueur touche le boss
            {
                Console.WriteLine("Toucher par le boss");
                Canvas.SetTop(joueur, joueurHaut);
                Canvas.SetLeft(joueur, bossGauche - joueur.Width);
                if (compteurVie > VIE_JOUEUR_MINI)
                {
                    changementCoeur();
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(e.ChangedButton);
            if (e.ChangedButton == MouseButton.Left)
            {
                DateTime currentTime = DateTime.Now;

                // Vérifie si le délai entre les attaques est écoulé
                if (currentTime - tempsDepuisAttaque >= delayAttaque)
                {
                    // Met à jour le temps de la dernière attaque
                    tempsDepuisAttaque = currentTime;

                    double joueurGauche = Canvas.GetLeft(joueur);
                    double joueurHaut = Canvas.GetTop(joueur);
                    double joueurDroit = joueurGauche + joueur.Width;
                    double joueurBas = joueurHaut + joueur.Height;
                    double bossGauche = Canvas.GetLeft(boss);
                    double bossHaut = Canvas.GetTop(boss);
                    double bossDroite = bossGauche + boss.Width;
                    double bossBas = bossHaut + boss.Height;

                    clickAttaque = true;

                    if (joueurDroit > bossGauche - MARGE_COLLISION && joueurGauche < bossDroite && joueurBas > bossHaut)
                    {

                        int degats = DEGATS_EPEE;
                        if (attaqueRenforce)
                            degats += DEGATS_EPEE;
                        attaqueRenforce = false;
                        vieBoss += degats;
                        this.labVieBoss.Content = vieBoss;
                        degats = DEGATS_EPEE;
                        ChangementBoss();

                    }
                }
            }
            if (e.ChangedButton == MouseButton.Right && !projectileEnCours)
            {
                if (NiveauFLeur < 3)
                {
                    Console.WriteLine("La boule de feu n'est pas encore disponible ! Pouvoir verrouillé");
                    return;
                }
                projectileEnCours = true;
                double joueurGauche = Canvas.GetLeft(joueur);
                double joueurDroit = joueurGauche + joueur.Width;
                double joueurHaut = Canvas.GetTop(joueur);
                double joueurBas = joueurHaut + joueur.Height;
                BitmapImage imgProjectileJoueur = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_joueur.png"));
                projectileJoueur = new Image();
                projectileJoueur.Width = LARGEUR_PROJECTILE_JOUEUR;
                projectileJoueur.Height = HAUTEUR_PROJECTILE_JOUEUR;
                projectileJoueur.Source = imgProjectileJoueur;
                zone.Children.Add(projectileJoueur);
                Canvas.SetLeft(projectileJoueur, joueurDroit);
                Canvas.SetTop(projectileJoueur, joueurBas - (joueur.Height / 2));
            }
            e.Handled = true;


        }


        public void animationAttaque()
        {
            compteurFrameAttaque++;
            if (compteurFrameAttaque >= delayFrame)
            {
                compteurFrameAttaque = COMPTEUR_FRAME; // Réinitialiser le compteur
                SpriteAtkInt ++;

                if (SpriteAtkInt > SPRITE_ATTAQUE_MAX)
                {
                    SpriteAtkInt = SPRITE_ATTAQUE_MINI; // Réinitialiser l'index du sprite
                }

                spriteAttaque(SpriteAtkInt);
            }
        }


        private void spriteAttaque(double i)
        {
            ImageBrush ibAtk = new ImageBrush();
            BitmapImage bmiAtk = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arret.png"));
            switch (i)
            {
                case 1:
                    bmiAtk = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/atk1.png"));
                    break;
                case 2:
                    bmiAtk = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/atk2.png"));
                    break;
                case 3:
                    bmiAtk = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/atk3.png"));
                    break;
                case 4:
                    bmiAtk = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arret.png"));
                    clickAttaque = false;
                    break;

            }
            ibAtk.ImageSource = bmiAtk;
            joueur.Fill = ibAtk;
        }
        private void InitProjectiles()
        {

            BitmapImage imgProjectile = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_1.png"));
            lesProjectiles = new Image[nbProjectiles];
            double bossGauche = Canvas.GetLeft(boss);
            double bossHaut = Canvas.GetTop(boss);
            double bossBas = bossHaut + boss.Height;

            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                lesProjectiles[i] = new Image();
                lesProjectiles[i].Width = LARGEUR_PROJECTILE;
                lesProjectiles[i].Height = HAUTEUR_PROJECTILE;
                lesProjectiles[i].Source = imgProjectile;
                zone.Children.Add(lesProjectiles[i]);
                Canvas.SetLeft(lesProjectiles[i], bossGauche - lesProjectiles[i].Width);
                int projectilePositionY = alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height));
                Canvas.SetTop(lesProjectiles[i], projectilePositionY);
            }
        }

        private void DetecterCollisionJoueurProjectile(Image imgProjectile)
        {
            double projectileGauche = Canvas.GetLeft(imgProjectile);
            double projectileHaut = Canvas.GetTop(imgProjectile);
            double projectileDroit = projectileGauche + imgProjectile.Width;
            double projectileBas = projectileHaut + imgProjectile.Height;
            double bossHaut = Canvas.GetTop(boss);
            double bossBas = bossHaut + boss.Height;
            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;


            if (projectileDroit > joueurGauche && projectileGauche < joueurDroit && projectileBas > joueurHaut && projectileHaut < joueurBas)   // Vérifier si le projectile touche le joueur
            {
                Console.WriteLine("Toucher par le projectile");
                if (compteurVie > VIE_JOUEUR_MINI)
                {
                    changementCoeur();
                }
                // Réinitialiser la position du projectile après qu'il ait touché le joueur
                for (int i = 0; i < lesProjectiles.Length; i++)
                {
                    Canvas.SetTop(imgProjectile, alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                    Canvas.SetLeft(imgProjectile, Canvas.GetLeft(boss) - REPOSITIONNEMENT_PROJECTILE_GAUCHE);
                }
            }
        }

        private void DeplaceProjectile(Image imgProjectile, System.Windows.Shapes.Rectangle joueur)
        {

            double projectileGauche = Canvas.GetLeft(imgProjectile);
            double projectileHaut = Canvas.GetTop(imgProjectile);
            double projectileDroit = projectileGauche + imgProjectile.Width;
            double projectileBas = projectileHaut + imgProjectile.Height;
            double bossHaut = Canvas.GetTop(boss);
            double bossBas = bossHaut + boss.Height;
            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;

            // Déplacer le projectile vers la gauche
            double newPosition = Canvas.GetLeft(imgProjectile) - (VITESSE_PROJECTILE_BASE + niveauBoss);  // Déplacement constant vers la gauche
            Canvas.SetLeft(imgProjectile, newPosition);

            System.Drawing.Rectangle rImgProjectile = new System.Drawing.Rectangle((int)Canvas.GetLeft(imgProjectile), (int)Canvas.GetTop(imgProjectile),
                (int)imgProjectile.Width, (int)imgProjectile.Height);

            // Vérifier la collision avec le joueur
            if (joueurDroit > projectileGauche + MARGE_COLLISION && joueurGauche < projectileDroit && joueurBas > projectileHaut && joueurHaut < projectileBas)
            {
                toucher = true;
            }

            // Si le projectile sort de l'écran ou touche le joueur, le réinitialiser
            if (newPosition < 0 || toucher)
            {

                for (int i = 0; i < lesProjectiles.Length; i++)
                {
                    Canvas.SetTop(imgProjectile, alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));  // Repositionner sous le boss
                    Canvas.SetLeft(imgProjectile, Canvas.GetLeft(boss) - REPOSITIONNEMENT_PROJECTILE_GAUCHE);
                }
            }
        }
        public void spriteCourse(double i)
        {
            ImageBrush ibMouv = new ImageBrush();
            BitmapImage bmiMouv = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arret.png"));
            switch (i)
            {
                case 1:
                    bmiMouv = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouv1.png"));
                    break;
                case 2:
                    bmiMouv = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouv2.png"));
                    break;
                case 3:
                    bmiMouv = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouv3.png"));
                    break;
                case 4:
                    bmiMouv = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouv4.png"));
                    break;
                case 5:
                    bmiMouv = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouv5.png"));
                    break;
                case 6:
                    bmiMouv = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouv6.png"));
                    break;
            }
            ibMouv.ImageSource = bmiMouv;
            joueur.Fill = ibMouv;
        }
        public void spriteCourseGauche(double i)
        {
            ImageBrush ibMouvGauche = new ImageBrush();
            BitmapImage bmiMouvGauche = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arretgauche.png"));
            switch (i)
            {
                case 1:
                    bmiMouvGauche = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouvgauche1.png"));
                    break;
                case 2:
                    bmiMouvGauche = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouvgauche2.png"));
                    break;
                case 3:
                    bmiMouvGauche = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouvgauche3.png"));
                    break;
                case 4:
                    bmiMouvGauche = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouvgauche4.png"));
                    break;
                case 5:
                    bmiMouvGauche = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouvgauche5.png"));
                    break;
                case 6:
                    bmiMouvGauche = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/mouvgauche6.png"));
                    break;
            }
            ibMouvGauche.ImageSource = bmiMouvGauche;
            joueur.Fill = ibMouvGauche;
        }
        public void animationCourse()
        {
            compteurFrameMouvement++;
            if (compteurFrameMouvement >= delayFrame)
            {
                compteurFrameMouvement = COMPTEUR_FRAME; // Réinitialiser le compteur
                SpriteInt += VITESSE_CHANGEMENT_SPRITE;

                if (SpriteInt >= SPRITE_INT_MAX)
                {
                    SpriteInt = SPRITE_INT_MIN; // Réinitialiser l'index du sprite
                }

                spriteCourse(SpriteInt);
            }
        }

        public void animationCourseGauche()
        {
            compteurFrameMouvement++;
            if (compteurFrameMouvement >= delayFrame)
            {
                compteurFrameMouvement = COMPTEUR_FRAME; // Réinitialiser le compteur
                SpriteIntGauche += VITESSE_CHANGEMENT_SPRITE;

                if (SpriteIntGauche > SPRITE_INT_MAX)
                {
                    SpriteIntGauche = SPRITE_INT_MIN; // Réinitialiser l'index du sprite
                }

                spriteCourseGauche(SpriteIntGauche);
            }
        }

        public void reset()
        {

            gauche = false;
            droite = false;
            Console.WriteLine("Reset");
            ImageBrush ibBoss = new ImageBrush();
            BitmapImage bmiBoss = new BitmapImage(new Uri("pack://application:,,,/img/Boss/boss1.png"));
            ibBoss.ImageSource = bmiBoss;
            boss.Fill = ibBoss;
            ImageBrush ibFond = new ImageBrush();
            BitmapImage bmiFond = new BitmapImage(new Uri("pack://application:,,,/img/Fond_niveaux/fond_niveau_1.png"));
            ibFond.ImageSource = bmiFond;
            zone.Background = ibFond;
            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                zone.Children.Remove(lesProjectiles[i]);
            }
            InitProjectiles();
            nbProjectiles = NB_PROJECTILE_DEBUT;
            labVieBoss.Foreground = Brushes.Red;
            vieBoss = VIE_BOSS_MAX;
            this.labVieBoss.Content = vieBoss;
            niveauBoss = 1;
            Canvas.SetTop(joueur, POSITION_JOUEUR_DEBUT_HAUT);
            Canvas.SetLeft(joueur, POSITION_JOUEUR_DEBUT_GAUCHE);
            saut = false;
            ImageBrush ibVie = new ImageBrush();
            BitmapImage bmiVie = new BitmapImage(new Uri("pack://application:,,,/img/Coeur/coeur3.png"));
            ibVie.ImageSource = bmiVie;
            rectCoeur.Fill = ibVie;
            NiveauFLeur = 0;
            ImageBrush ibFleur = new ImageBrush();
            BitmapImage bmiFleur = new BitmapImage(new Uri("pack://application:,,,/img/Fleur/fleur0.png"));
            ibFleur.ImageSource = bmiFleur;
            fleur.Fill = ibFleur;
            compteurVie = VIE_JOUEUR_MAX;
            MessageBox.Show("Vous êtes mort retour au premier boss", "Félicitation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void changementCoeur()
        {
            compteurVie--;
            ImageBrush ibVie = new ImageBrush();
            BitmapImage bmiVie = new BitmapImage(new Uri("pack://application:,,,/img/Coeur/coeur" + compteurVie + ".png"));
            ibVie.ImageSource = bmiVie;
            rectCoeur.Fill = ibVie;
            if (compteurVie < 4)
            {
                rectCoeur.Width = LARGEUR_COEUR_TROIS;
            }

        }
        private void InitSon()
        {
            sonGagne = new SoundPlayer(Application.GetResourceStream(
                new Uri("pack://application:,,,/sons/victoire.wav")).Stream);
        }
        private void MenuAudio_Click(object sender, RoutedEventArgs e)
        {
            Parametre dialog = new Parametre();
            bool? result = dialog.ShowDialog();
            if (result == true)
                MainWindow.NiveauSon = dialog.sliderSon.Value;
        }
        private void InitMusique()
        {
            musique = new MediaPlayer();
            musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "sons/Musique.mp3"));
            musique.MediaEnded += RelanceMusique;
            musique.Volume = NiveauSon;
            musique.Play();
        }
        private void RelanceMusique(object? sender, EventArgs e)
        {
            musique.Position = TimeSpan.Zero;
            musique.Play();
        }
        private void Arret()
        {
            ImageBrush ibArret = new ImageBrush();
            BitmapImage bmiArret = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arret.png"));
            ibArret.ImageSource = bmiArret;
            joueur.Fill = ibArret;
        }
        private void Bouclier(Image bouclier)
        {
            if (NiveauFLeur < 2)
            {
                Console.WriteLine("Le bouclier n'est pas encore disponible ! Pouvoir verrouillé");
                return;
            }
            // Vérifier si le bouclier est en cooldown
            if (enCooldownBouclier)
            {
                Console.WriteLine("Bouclier en cooldown. Veuillez patienter.");
                return;
            }

            // Vérifier si un bouclier est déjà actif
            if (bouclierActif != null)
            {
                return;
            }

            double joueurGauche = Canvas.GetLeft(joueur);
            double joueurHaut = Canvas.GetTop(joueur);
            double joueurDroit = joueurGauche + joueur.Width;
            double joueurBas = joueurHaut + joueur.Height;

            BitmapImage imgBouclier = new BitmapImage(new Uri("pack://application:,,,/img/Pouvoir/Pouvoir2.png"));

            bouclierActif = new Image();
            bouclierActif.Width = LARGEUR_BOUCLIER_JOUEUR;
            bouclierActif.Height = HAUTEUR_BOUCLIER_JOUEUR;
            bouclierActif.Source = imgBouclier;

            zone.Children.Add(bouclierActif);

            // Démarrer le timer du bouclier
            timerBouclier.Start();
        }
        private void InitBouclierTimers()
        {
            timerBouclier = new DispatcherTimer();
            timerBouclier.Interval = TimeSpan.FromSeconds(3);
            timerBouclier.Tick += FinBouclier;

            timerCooldownBouclier = new DispatcherTimer();
            timerCooldownBouclier.Interval = TimeSpan.FromSeconds(5);
            timerCooldownBouclier.Tick += FinCooldownBouclier;
        }
        private void FinBouclier(object sender, EventArgs e)
        {
            // Supprimer le bouclier de la zone
            if (bouclierActif != null)
            {
                zone.Children.Remove(bouclierActif);
                bouclierActif = null;
            }

            // Arrêter le timer du bouclier
            timerBouclier.Stop();

            // Démarrer le cooldown
            timerCooldownBouclier.Start();
            enCooldownBouclier = true;
        }
        private void FinCooldownBouclier(object sender, EventArgs e)
        {
            // Fin du cooldown
            enCooldownBouclier = false;
            timerCooldownBouclier.Stop();
            Console.WriteLine("Bouclier à nouveau disponible !");
        }

        private void DetecterCollisionBouclierProjectiles()
        {
            if (bouclierActif == null) return;

            // Position et dimensions du bouclier
            double bouclierGauche = Canvas.GetLeft(bouclierActif);
            double bouclierHaut = Canvas.GetTop(bouclierActif);
            double bouclierDroit = bouclierGauche + bouclierActif.Width;
            double bouclierBas = bouclierHaut + bouclierActif.Height;

            // Vérifier la collision avec chaque projectile
            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                // Position et dimensions du projectile
                double projectileGauche = Canvas.GetLeft(lesProjectiles[i]);
                double projectileHaut = Canvas.GetTop(lesProjectiles[i]);
                double projectileDroit = projectileGauche + lesProjectiles[i].Width;
                double projectileBas = projectileHaut + lesProjectiles[i].Height;

                // Vérifier s'il y a collision
                bool collision =
                    projectileDroit > bouclierGauche &&
                    projectileGauche < bouclierDroit &&
                    projectileBas > bouclierHaut &&
                    projectileHaut < bouclierBas;

                if (collision)
                {
                    // Réinitialiser la position du projectile
                    double bossHaut = Canvas.GetTop(boss);
                    double bossBas = bossHaut + boss.Height;

                    Canvas.SetTop(lesProjectiles[i], alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                    Canvas.SetLeft(lesProjectiles[i], Canvas.GetLeft(boss) - REPOSITIONNEMENT_PROJECTILE_GAUCHE);

                    Console.WriteLine("Projectile bloqué par le bouclier !");
                }
            }
        }
        private void ChangementBoss()
        {
            double bossHaut = Canvas.GetTop(boss);
            double bossBas = bossHaut + boss.Height;

            if (vieBoss <= VIE_BOSS_MOITIER)
            {
                labVieBoss.Foreground = Brushes.Orange;
            }
            if (vieBoss < VIE_BOSS_TRENTE)
            {
                labVieBoss.Foreground = Brushes.Yellow;
            }
            if (vieBoss <= VIE_BOSS_MINI)
            {

                if (niveauBoss <= NIVEAU_MAX_BOSS - 1)
                {
                    gauche = false;
                    droite = false;
                    niveauBoss++;
                    ImageBrush ibBoss = new ImageBrush();
                    BitmapImage bmiBoss = new BitmapImage(new Uri("pack://application:,,,/img/Boss/boss" + (niveauBoss) + ".png"));
                    ibBoss.ImageSource = bmiBoss;
                    boss.Fill = ibBoss;
                    ImageBrush ibFond = new ImageBrush();
                    BitmapImage bmiFond = new BitmapImage(new Uri("pack://application:,,,/img/Fond_niveaux/fond_niveau_" + (niveauBoss) + ".png"));
                    ibFond.ImageSource = bmiFond;
                    zone.Background = ibFond;
                    for (int i = 0; i < lesProjectiles.Length; i++)
                    {
                        zone.Children.Remove(lesProjectiles[i]);
                    }
                    if (niveauBoss < NIVEAU_MAX_BOSS)
                    {
                        nbProjectiles = NB_PROJECTILE_DEBUT;
                        lesProjectiles = new Image[nbProjectiles];
                        for (int i = 0; i < lesProjectiles.Length; i++)
                        {
                            lesProjectiles[i] = new Image();
                            lesProjectiles[i].Width = LARGEUR_PROJECTILE;
                            lesProjectiles[i].Height = HAUTEUR_PROJECTILE;
                            lesProjectiles[i].Source = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_" + (niveauBoss) + ".png"));
                            zone.Children.Add(lesProjectiles[i]);
                            Canvas.SetTop(lesProjectiles[i], alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                            Canvas.SetLeft(lesProjectiles[i], Canvas.GetLeft(boss) - REPOSITIONNEMENT_PROJECTILE_GAUCHE);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lesProjectiles.Length; i++)
                        {
                            zone.Children.Remove(lesProjectiles[i]);
                        }
                        nbProjectiles = NB_PROJECTILE_DEBUT + 2;
                        lesProjectiles = new Image[nbProjectiles];
                        for (int i = 0; i < lesProjectiles.Length; i++)
                        {
                            lesProjectiles[i] = new Image();
                            lesProjectiles[i].Width = LARGEUR_PROJECTILE;
                            lesProjectiles[i].Height = HAUTEUR_PROJECTILE;
                            lesProjectiles[i].Source = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_" + (niveauBoss) + ".png"));
                            zone.Children.Add(lesProjectiles[i]);
                            Canvas.SetTop(lesProjectiles[i], alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                            Canvas.SetLeft(lesProjectiles[i], Canvas.GetLeft(boss) - REPOSITIONNEMENT_PROJECTILE_GAUCHE);
                        }
                    }

                    vieBoss = VIE_BOSS_MAX;
                    this.labVieBoss.Content = vieBoss;
                    labVieBoss.Foreground = Brushes.Red;
                    Canvas.SetLeft(joueur, POSITION_JOUEUR_DEBUT_GAUCHE);
                    Canvas.SetTop(joueur, POSITION_JOUEUR_DEBUT_HAUT);
                    saut = false;
                    sonGagne.Play();
                    minuterie.Stop();
                    switch (niveauBoss)
                    {
                        case 2:
                            MessageBox.Show("Bien Joué,  vous avez vaincu le boss " + (niveauBoss - 1) + "/6 ! \nVous pouvez maintenant renforcer votre prochaine attaque toutes les 5 secondes en appuyant sur A", "Félicitation", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case 3:
                            MessageBox.Show("Bien Joué,  vous avez vaincu le boss " + (niveauBoss - 1) + "/6 ! \nVous pouvez maintenant faire apparaitre un bouclier toutes les 5 secondes en appuyant sur E", "Félicitation", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case 4:
                            MessageBox.Show("Bien Joué,  vous avez vaincu le boss " + (niveauBoss - 1) + "/6 ! \nVous pouvez maintenant lancer une boule de feu en appuyant sur clic droit", "Félicitation", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case 5:
                            MessageBox.Show("Bien Joué,  vous avez vaincu le boss " + (niveauBoss - 1) + "/6 ! \nVous pouvez maintenant faire des doubles sauts", "Félicitation", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case 6:
                            MessageBox.Show("Bien Joué,  vous avez vaincu le boss " + (niveauBoss - 1) + "/6 ! \nVotre vie a été restauré et doublé pour affronter le Boss final", "Félicitation", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                    }
                    minuterie.Start();
                }
                if (niveauBoss > 5)
                {
                    rectCoeur.Width = LARGEUR_COEUR_SIX;
                    ImageBrush ibVie = new ImageBrush();
                    BitmapImage bmiVie = new BitmapImage(new Uri("pack://application:,,,/img/Coeur/coeur6.png"));
                    ibVie.ImageSource = bmiVie;
                    rectCoeur.Fill = ibVie;
                    compteurVie = VIE_JOUEUR_MAX_NIVEAU_SIX;
                }
                if (niveauBoss == NIVEAU_MAX_BOSS && vieBoss <= VIE_BOSS_MINI)
                {
                    gauche = false;
                    droite = false;
                    Canvas.SetTop(boss, DISPARITION);
                    Canvas.SetTop(labVieBoss, DISPARITION);
                    Canvas.SetTop(joueur, DISPARITION * 2);
                    Canvas.SetTop(rectCoeur, DISPARITION);
                    Canvas.SetTop(fleur, DISPARITION);
                    for (int i = 0; i < lesProjectiles.Length; i++)
                    {
                        zone.Children.Remove(lesProjectiles[i]);
                    }
                    MessageBox.Show("Bien Joué, vous avez tuer le boss final", "Victoire", MessageBoxButton.OK, MessageBoxImage.Information);
                    Canvas.SetTop(labFin, POSITION_ZERO);
                    Canvas.SetLeft(labFin, POSITION_ZERO);
                }

                NiveauFLeur++;
                ImageBrush ibFleur = new ImageBrush();
                BitmapImage bmiFleur = new BitmapImage(new Uri("pack://application:,,,/img/Fleur/fleur" + (NiveauFLeur) + ".png"));
                ibFleur.ImageSource = bmiFleur;
                fleur.Fill = ibFleur;

            }
        }

        private void butJouer_Click(object sender, RoutedEventArgs e)
        {
            butJouer.IsEnabled = false;
            Console.WriteLine("jouer");
            Lancement();
        }
    }
}