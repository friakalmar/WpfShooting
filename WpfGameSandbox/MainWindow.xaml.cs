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
        Random random = new Random();
        DispatcherTimer Timer = new DispatcherTimer();

        bool goLeft, goRight, goUp, goDown, isVisible;

        int ShipTop = 290;
        int ShipLeft = 350;

        int ParcelTop = 0;
        int ParcelLeft = 200;

        public MainWindow()
        {
            InitializeComponent();

            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Start();
            LifeBar.Width = 10;
        }

        private void WpfGameSandbox_KeyDown(object sender, KeyEventArgs e)
        {
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
                goLeft = false; goRight = false; goDown = false; goUp = false;
                isVisible = !isVisible;
                Ship.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (LifeBar.Width < 200)
            {
                LifeBar.Width += 1;
            }
            
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


            ParcelTop += 4;
            if (ParcelTop > 400)
            {
                ParcelLeft = random.Next(0, 730);
                ParcelTop = -50;
            }
            Canvas.SetTop(Parcel, ParcelTop);
            Canvas.SetLeft(Parcel, ParcelLeft);


        }
    }
}
