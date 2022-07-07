using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Uno.Classes
{
    public class Visual_Card
    {
        public Label card;

        public Visual_Card(Card data)
        {
            var colors = new[] {
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Yellow
            };
            card = new Label();
            card.Width = 30;
            card.Height = 70;
            card.FontSize = 20;
            card.Visibility = System.Windows.Visibility.Visible;
            card.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            card.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            card.Background = colors[data.color];
            card.Content = data.number.ToString();
        }
    }
}
