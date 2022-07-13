using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Classes
{
    public class CardStack
    {
        public List<Card> Cards { get; set; }

        public CardStack()
        {
            this.Cards = new List<Card>();
        }
        /// <summary>
        /// Creates all possible Cards (Only Basic Cards)
        /// </summary>
        public void createAllCards()
        {
            /*
             * Every Card twice except for number 0
             */
            for(int i = 0; i < 4; i++)  //4 Colors
            {
                for (int k = 0; k < 2; k++)
                {
                    for (int j = 1; j <= 9; j++) //Numbers
                    {
                        this.Cards.Add(new Card(j, i));
                    }
                }
                this.Cards.Add(new Card(0, i));      
            }
        }

        /// <summary>
        /// Returns a random Card and deletes it from Stack
        /// </summary>
        /// <returns>Random Card</returns>
        public Card getRandomCard()
        {
            int rnd = Globals.randomNumber.Next(0, this.Cards.Count() - 1);
            Card ret = Cards[rnd];
            Cards.Remove(Cards[rnd]);
            return ret;
        }

        /// <summary>
        /// Adds Card to Stack
        /// </summary>
        /// <param name="add">Card to add to the Stack</param>
        public void AddCard(Card add)
        {
            Cards.Add(add);
        }

        /// <summary>
        /// Removes specific Card from Stack, if it exists in Stack
        /// </summary>
        /// <param name="rem">Card to remove from Stack</param>
        /// <returns>Card which has been removed, or null if card didn't exist in Stack</returns>
        public Card RemoveCard(Card rem)
        {
            for(int i = 0; i < Cards.Count(); i++)
            {
                if ((Cards[i].number == rem.number) && (Cards[i].color == rem.color))
                {
                    Card c = Cards[i];
                    Cards.RemoveAt(i);
                    return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Counts the Cards in Stack
        /// </summary>
        /// <returns>Number of Cards in Stack</returns>
        public int Count()
        {
            return Cards.Count();
        }

        /// <summary>
        /// Returns Card at specific index of Stack
        /// </summary>
        /// <param name="index">Index of Card that should be returned</param>
        /// <returns>Card at index 'index' (null if index out of range)</returns>
        public Card returnCard(int index)
        {
            if (index < Cards.Count)
                return Cards[index];
            return null;
        }
    }
    
    public class Card
    {
        public int number;
        public int color;

        public Card(int number, int color)
        {
            this.number = number;
            this.color = color;
        }
    }
}
