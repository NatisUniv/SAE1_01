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
        public static readonly int VIE_JOUEUR_MINI = 0;
        public static readonly int VIE_BOSS_MAX = 5;
        public static readonly int VIE_BOSS_MINI = 0;
        public static readonly int DEGATS_EPEE = -5;
        public static readonly int DEGATS_PROJECTILE = -1;
        public static readonly int DISPARITION_BOSS = -100;
        public static readonly int VITESSE_PROJECTILE = 3;
        public static readonly int MARGE_COLLISION = 10;
        private static int nbProjectiles = 3;
        public static readonly int POSITION_JOUEUR_DEBUT_GAUCHE = 22;
        public static readonly int POSITION_JOUEUR_DEBUT_HAUT = 352;
        public static readonly int NIVEAU_MAX_BOSS = 6;
        private static bool gauche;
        private static bool droite;
        private static bool clickAttaque = false;
        private static Random alea;
        private bool saut = false;
        private System.Windows.Vector vitesse;
        private double gravite = 0.3;
        private double hauteurSaut = -8;
        int vieJoueur = VIE_JOUEUR_MAX;
        int vieBoss = VIE_BOSS_MAX;
        private static Image[] lesProjectiles;
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


        public MainWindow()
        {
            InitializeComponent();
        }

        public void Lancement()
        {
            InitTimer();
            alea = new Random();
            ImageBrush ibFond = new ImageBrush();
            BitmapImage bmiFond = new BitmapImage(new Uri("pack://application:,,,/img/Fond_niveaux/fond_niveau_1.png"));
            ibFond.ImageSource = bmiFond;
            zone.Background = ibFond;
            Canvas.SetTop(joueur, POSITION_JOUEUR_DEBUT_HAUT);
            Canvas.SetLeft(joueur, POSITION_JOUEUR_DEBUT_GAUCHE);
            Canvas.SetTop(boss, 292);
            Canvas.SetLeft(boss, 641);
            Canvas.SetTop(rectCoeur, 10);
            Canvas.SetLeft(rectCoeur, 10);
            Canvas.SetTop(fleur, 67);
            Canvas.SetLeft(fleur, 11);
            Canvas.SetTop(labVieBoss, 252);
            Canvas.SetLeft(labVieBoss, 659);
            InitProjectiles();

        }
        // Logique principale du jeu, appelée chaque frame
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
                    saut = false; // Arrête le saut
                    vitesse = new System.Windows.Vector(0, 0); // Arrête la vitesse verticale
                    Canvas.SetTop(joueur, solHaut - joueur.Height); // Positionne le joueur juste sur le sol
                }
            }
            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                DeplaceProjectile(lesProjectiles[i], boss);
            }

            for (int i = 0; i < lesProjectiles.Length; i++)
            {
                DetecterCollisionJoueur(lesProjectiles[i]);
            }
            VerifierCollision();
            if (compteurVie == VIE_JOUEUR_MINI)
            {
                reset();

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
            Console.WriteLine(e.Key);
            double PauseHaut = Canvas.GetLeft(labPause);
            if (e.Key == Key.Q)
            {
                gauche = true;
            }
            else if (e.Key == Key.D)
            {
                droite = true;
            }

            if (e.Key == Key.Space && !saut)
            {
                saut = true;
                vitesse = new System.Windows.Vector(0, hauteurSaut);
            }
            if (e.Key == Key.P)
            {
                if (minuterie.IsEnabled)
                {
                    minuterie.Stop();
                    Canvas.SetLeft(labPause, 0);
                }
                else
                {
                    minuterie.Start();
                    Canvas.SetLeft(labPause, 800);
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = false;
                ImageBrush ib = new ImageBrush();
                BitmapImage bmi = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arret.png"));
                ib.ImageSource = bmi;
                joueur.Fill = ib;

            }
            else if (e.Key == Key.D)
            {
                droite = false;

                ImageBrush ib = new ImageBrush();
                BitmapImage bmi = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_perso/arret.png"));
                ib.ImageSource = bmi;
                joueur.Fill = ib;
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


            if (joueurDroit > bossGauche + MARGE_COLLISION && joueurGauche < bossDroite && joueurBas > bossHaut && joueurHaut < bossBas)
            {

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

                    vieBoss += DEGATS_EPEE;
                    this.labVieBoss.Content = vieBoss;
                    if (vieBoss < 55)
                    {
                        labVieBoss.Foreground = Brushes.Orange;
                    }
                    if (vieBoss < 30)
                    {
                        labVieBoss.Foreground = Brushes.Yellow;
                    }
                    if (vieBoss <= VIE_BOSS_MINI)
                    {

                        if (niveauBoss <= NIVEAU_MAX_BOSS - 1)
                        {
                            niveauBoss++;

                            ImageBrush ibBoss = new ImageBrush();
                            BitmapImage bmiBoss = new BitmapImage(new Uri("pack://application:,,,/img/Boss/boss" + (niveauBoss) + ".png"));
                            ibBoss.ImageSource = bmiBoss;
                            boss.Fill = ibBoss;
                            ImageBrush ibFond = new ImageBrush();
                            BitmapImage bmiFond = new BitmapImage(new Uri("pack://application:,,,/img/Fond_niveaux/fond_niveau_" + (niveauBoss) + ".png"));
                            ibFond.ImageSource = bmiFond;
                            zone.Background = ibFond;
                            if (niveauBoss < NIVEAU_MAX_BOSS)
                            {
                                for (int i = 0; i < lesProjectiles.Length; i++)
                                {
                                    lesProjectiles[i].Source = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_" + (niveauBoss) + ".png"));
                                    Canvas.SetTop(lesProjectiles[i], alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                                    Canvas.SetLeft(lesProjectiles[i], Canvas.GetLeft(boss) - 30);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < lesProjectiles.Length; i++)
                                {
                                    zone.Children.Remove(lesProjectiles[i]);
                                }
                                nbProjectiles = 5;
                                lesProjectiles = new Image[nbProjectiles];
                                for (int i = 0; i < lesProjectiles.Length; i++)
                                {
                                    lesProjectiles[i] = new Image();
                                    lesProjectiles[i].Width = 25;
                                    lesProjectiles[i].Height = 25;
                                    lesProjectiles[i].Source = new BitmapImage(new Uri("pack://application:,,,/img/Sprite_Projectile/Projectile_" + (niveauBoss) + ".png"));
                                    zone.Children.Add(lesProjectiles[i]);
                                    Canvas.SetTop(lesProjectiles[i], alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                                    Canvas.SetLeft(lesProjectiles[i], Canvas.GetLeft(boss) - 30);
                                }
                            }
                            vieBoss = VIE_BOSS_MAX;
                            this.labVieBoss.Content = vieBoss;
                            labVieBoss.Foreground = Brushes.Red;
                            Canvas.SetLeft(joueur, POSITION_JOUEUR_DEBUT_GAUCHE);
                            Canvas.SetTop(joueur, POSITION_JOUEUR_DEBUT_HAUT);
                            MessageBox.Show("Bien Joué,  vous avez vaincu le boss " + (niveauBoss - 1) + "/6", "Félicitation", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        if (niveauBoss == NIVEAU_MAX_BOSS && vieBoss == VIE_BOSS_MINI)
                        {
                            Canvas.SetTop(boss, DISPARITION_BOSS);
                            Canvas.SetTop(labVieBoss, DISPARITION_BOSS);
                            MessageBox.Show("Bien Joué, vous avez tuer le boss final", "Victoire", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        NiveauFLeur++;
                        ImageBrush ibFleur = new ImageBrush();
                        BitmapImage bmiFleur = new BitmapImage(new Uri("pack://application:,,,/img/Fleur/fleur" + (NiveauFLeur) + ".png"));
                        ibFleur.ImageSource = bmiFleur;
                        fleur.Fill = ibFleur;

                    }

                }
            }

        }

        public void animationAttaque()
        {
            compteurFrameAttaque++;
            if (compteurFrameAttaque >= delayFrame)
            {
                compteurFrameAttaque = 0; // Réinitialiser le compteur
                SpriteAtkInt += 1;

                if (SpriteAtkInt > 4)
                {
                    SpriteAtkInt = 1; // Réinitialiser l'index du sprite
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
                lesProjectiles[i].Width = 25;
                lesProjectiles[i].Height = 25;
                lesProjectiles[i].Source = imgProjectile;
                zone.Children.Add(lesProjectiles[i]);
                Canvas.SetLeft(lesProjectiles[i], bossGauche - lesProjectiles[i].Width);
                int projectilePositionY = alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height));
                Canvas.SetTop(lesProjectiles[i], projectilePositionY);
            }
        }

        private void DetecterCollisionJoueur(Image imgProjectile)
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
                if (compteurVie > VIE_JOUEUR_MINI)
                {
                    changementCoeur();
                }
                // Réinitialiser la position du projectile après qu'il ait touché le joueur
                for (int i = 0; i < lesProjectiles.Length; i++)
                {
                    Canvas.SetTop(imgProjectile, alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));
                    Canvas.SetLeft(imgProjectile, Canvas.GetLeft(boss) - 30);
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
            double newPosition = Canvas.GetLeft(imgProjectile) - VITESSE_PROJECTILE;  // Déplacement constant vers la gauche
            Canvas.SetLeft(imgProjectile, newPosition);

            System.Drawing.Rectangle rImgProjectile = new System.Drawing.Rectangle((int)Canvas.GetLeft(imgProjectile), (int)Canvas.GetTop(imgProjectile),
                (int)imgProjectile.Width, (int)imgProjectile.Height);

            // Vérifier la collision avec le joueur
            if (joueurDroit > projectileGauche + 10 && joueurGauche < projectileDroit && joueurBas > projectileHaut && joueurHaut < projectileBas)
            {

                toucher = true;
            }

            // Si le projectile sort de l'écran ou touche le joueur, le réinitialiser
            if (newPosition < 0 || toucher)
            {

                for (int i = 0; i < lesProjectiles.Length; i++)
                {
                    Canvas.SetTop(imgProjectile, alea.Next((int)bossHaut, (int)(bossBas - lesProjectiles[i].Height)));  // Repositionner sous le boss
                    Canvas.SetLeft(imgProjectile, Canvas.GetLeft(boss) - 30);
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
                compteurFrameMouvement = 0; // Réinitialiser le compteur
                SpriteInt += 0.5;

                if (SpriteInt >= 7)
                {
                    SpriteInt = 0; // Réinitialiser l'index du sprite
                }

                spriteCourse(SpriteInt);
            }
        }

        public void animationCourseGauche()
        {
            compteurFrameMouvement++;
            if (compteurFrameMouvement >= delayFrame)
            {
                compteurFrameMouvement = 0; // Réinitialiser le compteur
                SpriteIntGauche += 0.5;

                if (SpriteIntGauche > 7)
                {
                    SpriteIntGauche = 0; // Réinitialiser l'index du sprite
                }

                spriteCourseGauche(SpriteIntGauche);
            }
        }

        public void reset()
        {
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
            nbProjectiles = 3;
            labVieBoss.Foreground = Brushes.Red;
            vieBoss = VIE_BOSS_MAX;
            this.labVieBoss.Content = vieBoss;
            niveauBoss = 1;
            Canvas.SetTop(joueur, POSITION_JOUEUR_DEBUT_HAUT);
            Canvas.SetLeft(joueur, POSITION_JOUEUR_DEBUT_GAUCHE);
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
        }

    }
}