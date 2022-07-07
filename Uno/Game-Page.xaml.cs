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
using Uno.Classes;

namespace Uno
{
    /// <summary>
    /// Interaktionslogik für Game_Page.xaml
    /// </summary>
    public partial class Game_Page : Page
    {
        public Game_Page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.initializeLabels();
            if (Globals.network.server != null)
                this.serverStartGame();
            else
                this.ClientStartGame();
        }

        List<Label> CardLabels;
        CardStack AllCards;

        private void initializeLabels()
        {
            CardLabels = new List<Label>();
            CardLabels.Add(c1);
            CardLabels.Add(c2);
            CardLabels.Add(c3);
            CardLabels.Add(c4);
            CardLabels.Add(c5);
            CardLabels.Add(c6);
            CardLabels.Add(c7);
            CardLabels.Add(c8);
            CardLabels.Add(c9);
            CardLabels.Add(c10);
            CardLabels.Add(c11);
            CardLabels.Add(c12);
            CardLabels.Add(c13);
            CardLabels.Add(c14);
            CardLabels.Add(c15);
            foreach (Label L in CardLabels)
            {
                L.Visibility = Visibility.Hidden;
                L.Width = 50;
                L.Height = 90;
                L.HorizontalContentAlignment = HorizontalAlignment.Center;
                L.VerticalContentAlignment = VerticalAlignment.Center;
                L.FontSize = 20;
                L.Margin = new Thickness(1);
            }
        }

        private void serverStartGame()
        {
            this.AllCards = new CardStack();
            this.AllCards.createAllCards();
            this.GetInitialCards();
            this.ShowOwnCards();
        }

        private void ClientStartGame()
        {
            //Get 7 Cards from Server and add to own local Cardstack
            //Implement Card reception (Client RxData)
        }

        private void GetInitialCards()
        {
            for(int j = 0; j < Globals.MaxPlayers; j++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Globals.Players[j].CardStack.AddCard(AllCards.getRandomCard());
                    if (j != 0)  //Client Player
                        Globals.network.server.Send(Globals.Players[j].ip_port, "!card!" + Globals.Players[j].CardStack.returnCard(i).color.ToString() + Globals.Players[j].CardStack.returnCard(i).number.ToString());
                }
            }
        }

        private void ShowOwnCards()
        {
            var lcolor = new[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Yellow };
            int i = 0;
            foreach (Card c in Globals.Players[0].CardStack.Cards)
            {
                CardLabels[i].Content = c.number.ToString();
                CardLabels[i].Background = lcolor[c.color];
                CardLabels[i].Visibility = Visibility.Visible;
                i++;
            }
        }

        private void onCardClick(object sender, MouseButtonEventArgs e)
        {
            foreach (Label L in CardLabels)
            {
                if (sender.Equals(L))
                    L.Content = "YES";
            }
        }
    }
}
