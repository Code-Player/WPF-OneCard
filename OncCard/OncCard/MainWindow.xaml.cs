using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace OncCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] suit = { "♠", "♥", "♦", "♣" };
        string[] number = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        List<string> index = new();
        int indexCount = 0;
        string lastCard;
        bool turn;
        List<string> shuffledCards = new();

        private Random rand = new();

        public MainWindow()
        {
            InitializeComponent();

            // card set
            for (int suitIndex = 0; suitIndex < suit.Length; suitIndex++)
                for (int numberIndex = 0; numberIndex < number.Length; numberIndex++)
                {
                        index.Add($"{suit[suitIndex]}/{number[numberIndex]}");
                }

            // card shuffle
            shuffledCards = index.OrderBy(a => rand.Next()).ToList();

            // card debug check
            foreach (var i in shuffledCards)
                Debug.WriteLine(i);

            // player 1
            for (int i = 0; i < 5; i++)
                Play1_DeckList.Children.Add(
                    CardBase(shuffledCards[i], false)
                );

            // player 2
            for (int i = 5; i < 10; i++)
                Play2_DeckList.Children.Add(
                    CardBase(shuffledCards[i], true)
                );

            LastCardBase.Child = CardBase(shuffledCards[10], true);
            indexCount = 11;
            RemainCardDisplay.Text = $"{index.Count - indexCount}";
            lastCard = shuffledCards[10];
        }

        private Border CardBase(string info, bool display)
        {
            var card = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness=new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Background = Brushes.White,
                Margin=new Thickness(5),
                Width = 70,
                Height=100,
            };

            var cardBody = new Grid
            {
                Width=70,
                Height=100,
            };

            var cardSuit = new TextBlock
            {
                Margin = new Thickness(5),
                Text = info.Split("/")[0],
            };

            var cardSuitReverse = new TextBlock
            {
                Margin = new Thickness(0,0,-3,-7),
                Text = info.Split("/")[0],
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment=HorizontalAlignment.Right,
            };

            var cardNumber = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = info.Split("/")[1],
                FontWeight = FontWeights.Bold,
            };

            // Suit angle 180 degrees rotate
            RotateTransform rotate = new RotateTransform(180);
            cardSuitReverse.RenderTransform = rotate;

            if (info.Split("/")[0] == "♥" || info.Split("/")[0] == "♦")
            {
                cardSuit.Foreground = Brushes.Red;
                cardSuitReverse.Foreground = Brushes.Red;
                cardNumber.Foreground = Brushes.Red;
                card.BorderBrush = Brushes.Red;
            }

            if (display == true)
            {
                cardBody.Children.Add(cardSuit);
                cardBody.Children.Add(cardSuitReverse);
                cardBody.Children.Add(cardNumber);
            }
            else
            {
                card.Background = Brushes.LightGray;
                card.BorderBrush = Brushes.Black;
            }

            card.MouseLeftButtonDown += CardSelect;
            
            void CardSelect(object sender, MouseButtonEventArgs e)
            {
                if (sender is Border border)
                {
                    if(border.Child is Grid grid)
                    {
                        if (grid.Children[2] is TextBlock textOfNumber)
                            if (grid.Children[0] is TextBlock textOfSuit)
                            {
                                // Select Card Info
                                LastCardBase.Child = CardBase($"{textOfSuit.Text}/{textOfNumber.Text}", true);
                            }
                        //Debug.WriteLine($"{textOfSuit.Text}{textOfNumber.Text}");
                    }
                }

                if(sender is Border _border)
                {
                    if(_border.Parent is WrapPanel wrap)
                    {
                    }
                }
            }

            card.Child = cardBody;
            return card;
        }

        private void CardDraw(object sender, MouseButtonEventArgs e)
        {
            if ((index.Count - indexCount) == 1)
            {
                RemainCardBase.Opacity = 0.5;
            }
            else if ((index.Count - indexCount) == 0)
                return;

            Play2_DeckList.Children.Add(
                    CardBase(shuffledCards[indexCount++], true)
            );

            RemainCardDisplay.Text = $"{index.Count - indexCount}";
        }
    }
}
