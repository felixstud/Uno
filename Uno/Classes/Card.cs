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

        public Card getRandomCard()
        {
            int rnd = Globals.randomNumber.Next(0, this.Cards.Count() - 1);
            Card ret = Cards[rnd];
            Cards.Remove(Cards[rnd]);
            return ret;
        }

        public void AddCard(Card add)
        {
            Cards.Add(add);
        }

        public Card RemoveCard(Card rem)
        {
            for(int i = 0; i < Cards.Count() - 1; i++)
            {
                if ((Cards[i].number == rem.number) && Cards[i].color == rem.color)
                {
                    Cards.RemoveAt(i);
                    return Cards[i];
                }
            }
            return null;
        }

        public int getCounter()
        {
            return Cards.Count();
        }

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
