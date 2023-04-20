using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;



namespace Card_Game
{
    public sealed partial class MainPage : Page
    {
        private const int NumberOfCards = 13;  // Set the number of cards in the game
        private List<Card> cards;  // Create a list to hold the cards
        private Card firstSelectedCard;  // Create variables to hold the selected cards
        private Card secondSelectedCard;
        private int attempts;  // Create a variable to keep track of the number of attempts

        public MainPage()
        {
            InitializeComponent();
            InitializeGame(); // Initialize the game when the page is loaded
        }

        private async void InitializeGame()
        {
            cards = new List<Card>(); // Initialize the list of cards
            for (int i = 1; i <= NumberOfCards; i++) // Loop through each card image
            {
                var imageUri = new Uri($"ms-appx:///Assets/Cards/{i}.png"); // Create a URI for the image file
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(imageUri); // Load the image file

                // Create two separate BitmapImage objects for each card image
                using (var stream1 = await storageFile.OpenReadAsync())
                {
                    var bitmapImage1 = new BitmapImage();
                    await bitmapImage1.SetSourceAsync(stream1);
                    var cardImage1 = new Image { Source = bitmapImage1 };
                    cards.Add(new Card(i, cardImage1)); // Add the card to the list
                }

                using (var stream2 = await storageFile.OpenReadAsync())
                {
                    var bitmapImage2 = new BitmapImage();
                    await bitmapImage2.SetSourceAsync(stream2);
                    var cardImage2 = new Image { Source = bitmapImage2 };
                    cards.Add(new Card(i, cardImage2)); // Add the card to the list again (for a pair)
                }
            }

            NewGame(); // Start a new game
        }



        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            NewGame(); // Start a new game when the New Game button is clicked
        }

        private void NewGame()
        {
            ResetCardButtons();  // Reset the card buttons
            ShuffleCards();  // Shuffle the cards
            PlaceCardsOnTable();  // Place the cards on the table
            attempts = 0;  // Reset the number of attempts
            UpdateAttemptsText();  // Update the text showing the number of attempts
            firstSelectedCard = null;  // Reset the selected cards
            secondSelectedCard = null;
        }


        private void ResetCardButtons()
        {
            foreach (var card in cards)
            {
                if (card.Button != null)
                {
                    card.Button.Content = new Image { Source = new BitmapImage(new Uri("ms-appx:///Card_Game/Assets/Cards/back.png")) }; // Set the card button content to the back image
                    card.Button.Click += CardButton_Click; // Add a click event handler to the card button
                }
            }
        }


        private void ShuffleCards()
        {
            var random = new Random();
            cards = cards.OrderBy(_ => random.Next()).ToList(); // Shuffle the cards randomly
        }

        private void PlaceCardsOnTable()
        {
            CardGrid.Children.Clear(); // Clear the card grid

            int index = 0;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var card = cards[index];
                    var cardButton = new Button
                    {
                        Width = 100,
                        Height = 150,
                        Tag = card,
                        Content = new Image { Source = new BitmapImage(new Uri("ms-appx:///Assets/Cards/back.png")) }
                    };

                    card.Button = cardButton; // Store a reference to the button in the Card object
                    cardButton.Click += CardButton_Click;
                    Grid.SetRow(cardButton, row);
                    Grid.SetColumn(cardButton, col);
                    CardGrid.Children.Add(cardButton);
                    index++;
                }
            }
        }



        private async void CardButton_Click(object sender, RoutedEventArgs e)
        {
            var cardButton = (Button)sender;
            var card = (Card)cardButton.Tag;
            if (firstSelectedCard == null || secondSelectedCard == null)
            {
                if (firstSelectedCard != card) // Add this condition
                {
                    ((Button)card.Button).Content = card.Image;

                    if (firstSelectedCard == null)
                    {
                        firstSelectedCard = card;
                    }
                    else
                    {
                        secondSelectedCard = card;
                        attempts++;
                        UpdateAttemptsText();

                        if (firstSelectedCard.Id == secondSelectedCard.Id)
                        {
                            CardGrid.Children.Remove(firstSelectedCard.Button);
                            CardGrid.Children.Remove(secondSelectedCard.Button);
                            firstSelectedCard = null;
                            secondSelectedCard = null;
                        }
                    }
                }
            }
            else
            {
                var backImage = new BitmapImage(new Uri("ms-appx:///Assets/Cards/back.png"));

                ((Button)firstSelectedCard.Button).Content = new Image { Source = backImage };
                ((Button)secondSelectedCard.Button).Content = new Image { Source = backImage };
                firstSelectedCard = null;
                secondSelectedCard = null;
            }
        }




        private void ShowCardsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var card in cards)
            {
                if (card.Button != null)
                {
                    ((Button)card.Button).Content = card.Image;
                    ((Button)card.Button).Click -= CardButton_Click; // Disable clicking on the cards after showing them
                }
            }
        }



        private void UpdateAttemptsText()
        {
            AttemptsTextBlock.Text = $"Attempts: {attempts}";
        }

        private class Card
        {
            public int Id { get; }
            public Image Image { get; }
            public Button Button { get; set; }

            public Card(int id, Image image)
            {
                Id = id;
                Image = image;
            }
        }
    }
}



