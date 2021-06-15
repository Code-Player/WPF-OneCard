using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
        bool player = true;
        List<string> shuffledCards = new();
        int player1Hand;
        int player2Hand;
        bool computerCardVisible = false;
        int drawStack = 1;
        int drawStackTrunk = 0;

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
            for (int i = 0; i < 4; i++)
                Play1_DeckList.Children.Add(
                    CardBase(shuffledCards[i], computerCardVisible)//false
                );

            // player 2
            for (int i = 4; i < 9; i++)
                Play2_DeckList.Children.Add(
                    CardBase(shuffledCards[i], true)
                );

            LastCardBase.Child = CardBase(shuffledCards[10], true);
            indexCount = 11;
            RemainCardDisplay.Text = $"{index.Count - indexCount}";
            lastCard = shuffledCards[10];

            CardDraw(null, null);
        }

        private Border CardBase(string info, bool display)
        {
            var card = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Background = Brushes.White,
                Margin = new Thickness(5),
                Width = 70,
                Height = 100,
            };

            var cardBody = new Grid
            {
                Width = 70,
                Height = 100,
            };

            var cardSuit = new TextBlock
            {
                Margin = new Thickness(5),
                Text = info.Split("/")[0],
            };

            var cardSuitReverse = new TextBlock
            {
                Margin = new Thickness(0, 0, -3, -7),
                Text = info.Split("/")[0],
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
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

            cardBody.Children.Add(cardSuit);
            cardBody.Children.Add(cardSuitReverse);
            cardBody.Children.Add(cardNumber);

            if (display == false)
            {
                cardSuit.Visibility = Visibility.Collapsed;
                cardSuitReverse.Visibility = Visibility.Collapsed;
                cardNumber.Visibility = Visibility.Collapsed;

                card.Background = Brushes.LightGray;
                card.BorderBrush = Brushes.Black;
            }

            card.MouseLeftButtonDown += CardSelect;

            card.Child = cardBody;
            return card;
        }

        void CardSelect(object sender, MouseButtonEventArgs e)
        {
            bool playerTrunk = player;
            if (sender is Border border)
            {
                if (border.Child is Grid grid)
                {
                    if (grid.Children[2] is TextBlock textOfNumber)
                        if (grid.Children[0] is TextBlock textOfSuit)
                        {
                            // Select Card Info
                            if (textOfNumber.Text == "A" || textOfNumber.Text == "2")
                            {
                                border.Visibility = Visibility.Collapsed;
                                LastCardBase.Child = CardBase($"{textOfSuit.Text}/{textOfNumber.Text}", true);
                                lastCard = $"{textOfSuit.Text}/{textOfNumber.Text}";

                                if (lastCard.Split("/")[1] == "A")
                                    drawStack += 2;
                                else if (lastCard.Split("/")[1] == "2")
                                    drawStack++;

                                ChangeTurn();
                            }
                            else if (textOfSuit.Text == lastCard.Split("/")[0] || textOfNumber.Text == lastCard.Split("/")[1])
                            {
                                if (drawStack == 1)
                                {
                                    // Check Card Suit and Number
                                    border.Visibility = Visibility.Collapsed;
                                    LastCardBase.Child = CardBase($"{textOfSuit.Text}/{textOfNumber.Text}", true);
                                    lastCard = $"{textOfSuit.Text}/{textOfNumber.Text}";

                                    if (lastCard.Split("/")[1] == "A")
                                        drawStack += 2;
                                    else if (lastCard.Split("/")[1] == "2")
                                        drawStack++;

                                    // Jump
                                    if (!(textOfNumber.Text == "J" || textOfNumber.Text == "K"))
                                        ChangeTurn();
                                }
                                else
                                {
                                    CardDraw(null,null);
                                }


                                Debug.WriteLine("sel " + textOfSuit.Text + textOfNumber.Text);
                            }
                        }
                }
            }

            if (drawStack > 1)
                Alarm.Text = drawStack.ToString();

            // Stupid AI(AKA. WUT)
            if (player == true)
                ComputerAction();

            WinLose();
        }

        private void CardDraw(object sender, MouseButtonEventArgs e)
        {
            WinLose();
            if ((index.Count - indexCount) == 1)
            {
                RemainCardBase.Opacity = 0.5;
            }
            else if ((index.Count - indexCount) == 0)
                return;

            ChangeTurn();
            for (int i = 1; i <= drawStack; i++)
            {
                if (!player)
                {
                    Play1_DeckList.Children.Add(
                        CardBase(shuffledCards[indexCount++], computerCardVisible)
                    );
                }
                else
                {
                    Play2_DeckList.Children.Add(
                        CardBase(shuffledCards[indexCount++], true)
                    );
                }
            }
            if (drawStack > 1)
                drawStack = 1;

            if (player == true)
                ComputerAction();

            RemainCardDisplay.Text = $"{index.Count - indexCount}";
            Debug.WriteLine("draw");
        }

        private void ChangeTurn()
        {
            player = !player;
            if (player == false)
            {
                Play1_DeckList.IsEnabled = false;
                Play2_DeckList.IsEnabled = true;
                Play1_DeckList.Opacity = 0.7;
                Play2_DeckList.Opacity = 1;
            }
            else
            {
                Play1_DeckList.IsEnabled = true;
                Play2_DeckList.IsEnabled = false;
                Play1_DeckList.Opacity = 1;
                Play2_DeckList.Opacity = 0.7;
            }

            Debug.WriteLine("as " + drawStack);
        }

        private void ComputerAction()
        {
            bool playerTrunk = player;
            drawStackTrunk = 0;
            foreach (Border element in Play1_DeckList.Children)
            {
                if (element.Visibility == Visibility.Visible)
                {
                    if (element.Child is Grid grid)
                    {
                        if (grid.Children[2] is TextBlock textOfNumber)
                            if (grid.Children[0] is TextBlock textOfSuit)
                            {
                                // Select Card Info
                                if (textOfSuit.Text == lastCard.Split("/")[0] || textOfNumber.Text == lastCard.Split("/")[1])
                                {
                                    if (drawStack == 1)
                                    {
                                        element.Visibility = Visibility.Collapsed;
                                        LastCardBase.Child = CardBase($"{textOfSuit.Text}/{textOfNumber.Text}", true);
                                        lastCard = $"{textOfSuit.Text}/{textOfNumber.Text}";

                                        // Jump
                                        if (!(textOfNumber.Text == "J" || textOfNumber.Text == "K"))
                                        {
                                            player = playerTrunk;
                                        }
                                    }
                                    else if(drawStack > 1 && (textOfNumber.Text == "A" || textOfNumber.Text == "2"))
                                    {
                                        element.Visibility = Visibility.Collapsed;
                                        LastCardBase.Child = CardBase($"{textOfSuit.Text}/{textOfNumber.Text}", true);
                                        lastCard = $"{textOfSuit.Text}/{textOfNumber.Text}";

                                        ChangeTurn();
                                    }
                                    else
                                    {
                                        CardDraw(null,null);
                                        return;
                                    }


                                    CardSelect(element, null);
                                    return;
                                }
                            }
                    }
                }
            }
            CardDraw(null, null);
        }

        private void WinLose()
        {
            player1Hand = 0;
            player2Hand = 0;


            // 이게 안됨 ㅇㅇ;
            if (Convert.ToInt32(RemainCardDisplay.Text) == 0)
            {
                if (player1Hand > player2Hand)
                    (FindResource("WIN_Event") as Storyboard)?.Begin();
                else if (player1Hand < player2Hand)
                    (FindResource("LOSE_Event") as Storyboard)?.Begin();
            }

            foreach (Border a in Play1_DeckList.Children)
                if (a.Visibility == Visibility.Visible)
                    player1Hand++;

            if (player1Hand == 0)
                (FindResource("LOSE_Event") as Storyboard)?.Begin();

            foreach (Border a in Play2_DeckList.Children)
                if (a.Visibility == Visibility.Visible)
                    player2Hand++;

            if (player2Hand == 0)
                (FindResource("WIN_Event") as Storyboard)?.Begin();
        }
    }
}
