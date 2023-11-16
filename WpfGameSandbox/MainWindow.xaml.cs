using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace WpfGameSandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double ShipAngle = 0;
        Random random = new Random();
        DispatcherTimer Timer = new DispatcherTimer();

        bool goLeft, goRight, goUp, goDown, isVisible;

        int ShipTop = 290;
        int ShipLeft = 350;

        int ParcelTop = 0;
        int ParcelLeft { get; set; } = 200;

        //Shot grafik
        BitmapImage shotTexture = new BitmapImage(new Uri("pack://application:,,,/Images/shot.png"));

        BitmapImage enemyTexture = new BitmapImage(new Uri("pack://application:,,,/Images/Enemy.png"));

        //Skapar en tom lista som ska innehålla alla skott!
        List<Image> Shots = new List<Image>();

        List<Image> Enemies = new List<Image>();


        int EnemyDelay = 10;

        int PlayerLife = 3;
        protected int PlayerEnergi = 100;

        int Points = 0;

        public MainWindow()
        {
            //programstart

            InitializeComponent();

            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = TimeSpan.FromMilliseconds(15);
            Timer.Start();
            LifeBar.Width = PlayerEnergi;
        }

        private void WpfGameSandbox_KeyDown(object sender, KeyEventArgs e)
        {

            // Lyssna av tangentbordet!

            if(e.Key==Key.P)
            {
                ShipAngle += 10;
                Ship.RenderTransform = new RotateTransform(ShipAngle,Ship.Width/2,Ship.Height/2);
            }


            if (e.Key == Key.Left)
            {
                goLeft = true; goRight = false;
                goUp = false; goDown = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = true; goLeft = false;
                goUp = false; goDown = false;
            }
            if (e.Key == Key.Down)
            {
                goLeft = false; goRight = false; goUp = false;
                goDown = true;
            }
            if (e.Key == Key.Up)
            {
                goLeft = false; goRight = false; goDown = false;
                goUp = true;
            }
            if (e.Key == Key.Space)
            {
                //Skapa skott
                CreateShot();
            }
            if(e.Key == Key.X) {
                goLeft = false; goRight = false; goDown = false; goUp = false;
                isVisible = !isVisible;
                Ship.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateShots();
            MoveShip(); 
            MoveParcel();
            CreateEnemies();
            MoveEnemies();
            DrawScore();
        }

        private void DrawScore()
        {
            Life.Text = "Life: " + PlayerLife;
            Score.Text = "Score: " + Points;
        }
        private void MoveParcel()
        {
            //Flytta paketet och kolla om man träffar den
            ParcelTop += 5;
            Rect player = new Rect(Canvas.GetLeft(Ship), Canvas.GetTop(Ship), Ship.Width, Ship.Height);
            Rect parcel = new Rect(ParcelLeft, ParcelTop, Parcel.Width, Parcel.Height);

            //Om paketet åkt för långt
            if (ParcelTop > 400)
            {
                ResetParcel();
              
            } 

            //Rita ut paketet
            Canvas.SetTop(Parcel, ParcelTop);
            Canvas.SetLeft(Parcel, ParcelLeft);

            //Koll om krock med spelare
            if (player.IntersectsWith(parcel))
            {
                if (PlayerEnergi <= 180)
                {
                    PlayerEnergi += 20;
                }
                if (PlayerEnergi >= 200)
                {
                    PlayerEnergi = 20;
                    PlayerLife++;

                }

                LifeBar.Width = PlayerEnergi;
                ResetParcel();
            }

            DrawScore();
        }

        void ResetParcel()
        {
            ParcelLeft = random.Next(0, 730);
            ParcelTop = -50;
        }

        private void CreateEnemies()
        {
            EnemyDelay--;
            if(EnemyDelay < 0)
            {
                EnemyDelay = 30;
                Image img = new Image() { Width=50,Height=50};
                Canvas.SetLeft(img, random.Next(10,500));
                Canvas.SetTop(img,0);
                img.Source = enemyTexture;
                GameCanvas.Children.Add(img);
                Enemies.Add(img);
            }
        }
        private void MoveEnemies()
        {
            Rect player = new Rect(Canvas.GetLeft(Ship),Canvas.GetTop(Ship),Ship.Width,Ship.Height);
            foreach(var enemy in Enemies)
            {
                
                Canvas.SetTop(enemy,Canvas.GetTop(enemy)+5);
                //Canvas.SetLeft(enemy,Canvas.GetLeft(enemy)+random.Next(-8,8));

                Rect EnemyRect = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.Width,enemy.Height);

                //Koll kolliossion spelare
                if (EnemyRect.IntersectsWith(player))
                {
                    PlayerLife--;
                    enemy.Visibility = Visibility.Hidden;
                    GameCanvas.Children.Remove(enemy);
                }

                foreach(var shot in Shots)
                {
                    Rect ShotRect = new Rect(Canvas.GetLeft(shot), Canvas.GetTop(shot), shot.Width,shot.Height);

                      if(ShotRect.IntersectsWith(EnemyRect)) {
                          Points++;
                        enemy.Visibility = Visibility.Hidden;
                        GameCanvas.Children.Remove(enemy);

                        shot.Visibility = Visibility.Hidden;
                        GameCanvas.Children.Remove(shot);
                    }
                }
            }
            //Radera alla osynliga bilder från minnet!!

            Shots.RemoveAll(x => x.Visibility == Visibility.Hidden);
            Enemies.RemoveAll(x => x.Visibility == Visibility.Hidden);
        }
        private void CreateShot()
        {
            Image img = new Image() { Height=20,Width=8};
            Canvas.SetLeft(img, Canvas.GetLeft(Ship) + 20);
            Canvas.SetTop(img, Canvas.GetTop(Ship));
            img.Source = shotTexture;
            GameCanvas.Children.Add(img);
            Shots.Add(img);
        }
        private void UpdateShots()
        {
            foreach (var shot in Shots)
            {
                var top = Canvas.GetTop(shot);
                Canvas.SetTop(shot, top - 5);
                if (top <= 0)
                {
                    GameCanvas.Children.Remove(shot);
                }
            }
            Shots.RemoveAll(x => Canvas.GetTop(x) < -10);
            NrOfShots.Text = "Antal: " + Shots.Count;
        }
        private void MoveShip()
        {
            if (goLeft && ShipLeft > 0)
            {
                Canvas.SetLeft(Ship, ShipLeft -= 3);
            }
            else if (goRight && ShipLeft < 732)
            {
                Canvas.SetLeft(Ship, ShipLeft += 3);
            }
            if (goUp && ShipTop > 0)
            {
                Canvas.SetTop(Ship, ShipTop -= 3);
            }
            if (goDown)
            {
                Canvas.SetTop(Ship, ShipTop += 3);
            }
        }

    }
}
