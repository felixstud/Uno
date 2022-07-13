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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Uno
{
    /// <summary>
    /// Interaktionslogik für Game_Page.xaml
    /// </summary>
    public partial class Game_Page : Page
    {
        List<Label> CardLabels;
        List<Label> NameLabels;
        List<Label> CountLabels;

        public Game_Page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.initializeLabels();
            this.init();
        }

        /// <summary>
        /// Initialization for GamePage
        /// </summary>
        private async void init()
        {
            GameClient.Events.CardReceived += Events_CardReceived;
            GameClient.Events.EnemyPlayerNameReceived += Events_EnemyPlayerNameReceived;
            GameClient.Events.MoveReceived += Events_MoveReceived;
            GameClient.Events.WinnerReceived += Events_WinnerReceived;

            while (!GameClient.client.IsConnected) ;
            GameClient.RequestServer("?card?" + Globals.initialCards.ToString());
            while (GameClient.myCards.Count() < Globals.initialCards) ;
            await Task.Delay(500);
            GameClient.RequestServer("?midcard?");
            while (MiddleStack.Content == "") ;
            await Task.Delay(300);
            GameClient.RequestServer("?EnemyNames?");
            while (NameLabels[Globals.MaxPlayers - 2].Content != null) ;
            ShowOwnCards();
            while (CardLabels[6].Content == null) ;
            //GameClient.RequestServer("?move?"); //Crashes the Game ?!?!?
        }

        /// <summary>
        /// Fired when one Player wins the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_WinnerReceived(object? sender, GameClient.Events.WinEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.name.Equals(GameClient.myName))
                    this.NavigationService.Navigate(new Winner());
                else
                    this.NavigationService.Navigate(new Loser(e.name));
            }));
        }

        /// <summary>
        /// Fired when a Player ended his move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_MoveReceived(object? sender, GameClient.Events.MoveEventArgs e)
        {
            if(e.Playername.Equals(GameClient.myName))
            {
                this.MarkActivePlayer(GameClient.myName);
                this.Move(true);
            }
            else
            {
                this.Move(false);
                this.MarkActivePlayer(e.Playername);
            }
        }

        /// <summary>
        /// Fired when Client receives a Players name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_EnemyPlayerNameReceived(object? sender, GameClient.Events.EnemyNameReceivedEventArgs e)
        {
            if(e.Playernumber < 0)
            {
                for(int i = 0; i < (Globals.MaxPlayers - 2); i++)
                {
                    NameLabels[i].Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (NameLabels[i].Content.Equals(e.PlayerName))
                            e.Playernumber = i;
                    }));
                }
            }

            if (e.Playernumber < 0 || e.Playernumber > Globals.MaxPlayers - 2)
                return;

            if (NameLabels[e.Playernumber] != null)
            {
                NameLabels[e.Playernumber].Dispatcher.BeginInvoke(new Action(() =>
                    {NameLabels[e.Playernumber].Content = e.PlayerName;}));
            }
            if (CountLabels[e.Playernumber] != null)
            {
                CountLabels[e.Playernumber].Dispatcher.BeginInvoke(new Action(() =>
                    { CountLabels[e.Playernumber].Content = e.nCards.ToString(); }));
            }
        }

        /// <summary>
        /// Fired when Client received a Card from Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_CardReceived(object? sender, GameClient.Events.CardReceivedEventArgs e)
        {
            if (e.midcard == true)
            {
                ShowCard(MiddleStack, e.Card);
                return;
            }
            else if (e.remove == true)
                GameClient.myCards.RemoveCard(e.Card);
            else
                GameClient.myCards.AddCard(e.Card);
            ShowOwnCards();
        }

        /// <summary>
        /// Activates or deactivates all Card Labels, and Buttons
        /// </summary>
        /// <param name="activate"></param>
        private void Move(bool activate)
        {
            foreach (Label L in CardLabels)
            {
                L.Dispatcher.BeginInvoke(new Action(() => {L.IsEnabled = activate;}));
            }
            btn_newCard.Dispatcher.BeginInvoke(new Action(() =>{ btn_newCard.IsEnabled = activate; }));
            btn_SayUno.Dispatcher.BeginInvoke(new Action(() => { btn_SayUno.IsEnabled = activate; }));
        }

        /// <summary>
        /// Marks the Player with a specific Name
        /// </summary>
        /// <param name="Name">Players name to get marked</param>
        private void MarkActivePlayer(string Name)
        {
            foreach(Label L in NameLabels)
            {
                L.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (L.Content.Equals(Name))
                        L.Background = Brushes.LightGreen;
                    else
                        L.Background = Brushes.Transparent;
                }));
            }
        }

        /// <summary>
        /// Initializes all Labels
        /// </summary>
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
                L.Width = 48;
                L.Height = 90;
                L.HorizontalContentAlignment = HorizontalAlignment.Center;
                L.VerticalContentAlignment = VerticalAlignment.Center;
                L.FontSize = 50;
                L.Margin = new Thickness(1);
            }

            NameLabels = new List<Label>();
            NameLabels.Add(lab_Player1_Name);
            NameLabels.Add(lab_Player2_Name);
            NameLabels.Add(lab_Player3_Name);

            CountLabels = new List<Label>();
            CountLabels.Add(lab_Player1_Number);
            CountLabels.Add(lab_Player2_Number);
            CountLabels.Add(lab_Player3_Number);
        }

        /// <summary>
        /// Shows every Card which are existing in own cards
        /// </summary>
        private void ShowOwnCards()
        {
            var lcolor = new[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Yellow };
            for(int i = 0; i < CardLabels.Count(); i++)
            {
                if(i < GameClient.myCards.Count())
                {
                    ShowCard(CardLabels[i], GameClient.myCards.Cards[i]);
                }
                else
                {
                    HideCard(CardLabels[i]);
                }
            }
        }

        /// <summary>
        /// Hides a specific Label and resets its content and Background
        /// </summary>
        /// <param name="L">Label to hide</param>
        private void HideCard(Label L)
        {
            L.Dispatcher.BeginInvoke(new Action(() =>
            {
                L.Content = "";
                L.Background = Brushes.White;
                L.Visibility = Visibility.Hidden;
            }));
        }

        /// <summary>
        /// Shows specific Card on an specific Label
        /// </summary>
        /// <param name="L">Label that the Card be displayed at</param>
        /// <param name="C">Card to be displayed</param>
        private void ShowCard(Label L, Card C)
        {
            var lcolor = new[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Yellow };
            L.Dispatcher.BeginInvoke(new Action(() =>
            {
                L.Content = C.number.ToString();
                L.Background = lcolor[C.color];
                L.Visibility = Visibility.Visible;

            }));
        }

        /// <summary>
        /// Fired when a card is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCardClick(object sender, MouseButtonEventArgs e)
        {
            this.Move(false);
            foreach (Label L in CardLabels)
            {
                if (sender.Equals(L))
                {
                    Card c = LabelToCard(L);
                    GameClient.RequestServer("!card!" + c.number.ToString() + c.color.ToString());
                }
            }
        }//ToDo

        /// <summary>
        /// Converts a CardLabel to an actual Card
        /// </summary>
        /// <param name="L">Label to convert</param>
        /// <returns></returns>
        private Card LabelToCard(Label L)
        {
            var lcolor = new[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Yellow };
            for (int i = 0; i < 4; i++)
            {
                if (L.Background == lcolor[i])
                {
                    int n = Int32.Parse(L.Content.ToString());
                    return new Card(n, i);
                }
            }
            return null;
        }

        /// <summary>
        /// Fired when Button "new Card" is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_newCard_Click(object sender, RoutedEventArgs e)
        {
            btn_newCard.Dispatcher.BeginInvoke(new Action(() => { btn_newCard.IsEnabled = false; }));
            GameClient.RequestServer("?card?");
        }

        /// <summary>
        /// Fired when "Exit" Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Sure you want to Exit?", "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                GameServer.Stop();
                GameClient.Stop();
                this.NavigationService.Navigate(new Start_Screen());
            }
        }
    }
}
