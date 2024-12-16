using System.Text;
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
using static System.Formats.Asn1.AsnWriter;

namespace MasterMind2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string[] codeColors = { "Red", "Orange", "Yellow", "Green", "White", "Blue" };
        private string[] generatedCode = new string[4];
        private DispatcherTimer timer = new DispatcherTimer();
        private int currentAttempt = 0;

        private int timeLeft = 60; // Aantal seconden per poging


        private int correctPosition = 0;
        private int correctColorWrongPosition = 0;
        private int incorrectColor = 0;
        private int totalScore = 100;
        private int remainingAttempts = 10;
        private string name = string.Empty;
        private string[] highscores = new string[15];
        private int highscoreCount = 0;
        private List<string> playerNames = new List<string>();
        private int currentPlayerIndex = 0;






        public MainWindow()
        {
            InitializeComponent();
            GenerateRandomCode();
            NewTitle();
            this.KeyDown += MainWindow_keyDown;
            Name = StartGame();
            ResetGame();
            Title = $"MasterMind - Welkom {Name}";
            playerNames.Clear();
            currentPlayerIndex = 0;
            highscoreCount = 0;


            //  timer.Tick += StartCountDown; 
            // timer.Interval = new TimeSpan(0, 0, 1); 
            // timer.Start();
        }

        /// <summary>
        /// Stopt de aftellingstimer en behandelt het scenario waarin de tijd op is.
        /// Deze methode wordt aangeroepen wanneer de speler niet binnen de tijd een poging doet.
        /// De beurt van de speler wordt verloren verklaard, en de volgende poging begint.
        /// </summary>



        private void MainWindow_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12 && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ToggleDebug();
            }
        }
        /// <summary>
        /// Start een aftellingstimer voor de huidige poging van de speler.
        /// De timer begint bij een vastgestelde tijd (bijv. 10 seconden) 
        /// en telt per seconde af totdat de tijd op is.
        /// Indien de tijd op is, wordt de methode <see cref="StopCountDown"/> aangeroepen.
        /// </summary>

        private void StartCountDown()
        {
            timeLeft = 10;
            UpdateTitleWithTime();

            // Voorkom dubbele eventhandlers
            timer.Tick -= Timer_Tick;
            timer.Tick += Timer_Tick;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();


            //timeLabel.Content = $"{DateTime.Now.ToLongTimeString()}";
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft--;

            // Update de titel terwijl de tijd aftelt
            UpdateTitleWithTime();

            if (timeLeft <= 0)
            {
                stopCountDown();
            }
        }


        private void UpdateTitleWithTime()
        {
            Title = $"MasterMind - Poging {currentAttempt} ";
            timerLabel.Content = $"\n Tijd over : \n" +
                $"{timeLeft} seconden! ";
        }


        private bool isDebugMode = false;



        /// <summary>
        /// Schakelt de debugmodus in of uit.
        /// In debugmodus wordt een TextBox weergegeven waarin de gegenereerde code wordt getoond.
        /// De debugmodus kan worden geactiveerd of gedeactiveerd met de sneltoets CTRL + F12.
        /// </summary>

        private void ToggleDebug()
        {
            isDebugMode = !isDebugMode;
            debugCodeTextBox.Visibility = isDebugMode ? Visibility.Visible : Visibility.Collapsed;

            if (isDebugMode)
            {
                debugCodeTextBox.Text = string.Join(" , ", generatedCode);
            }
        }




        private void GenerateRandomCode()
        {
            Random random = new Random();
            generatedCode = new string[4];

            for (int i = 0; i < 4; i++)
            {
                generatedCode[i] = codeColors[random.Next(codeColors.Length)];
            }

            StartCountDown();


        }
        private void NewTitle()
        {

            //Title = "MasterMind  /  Code: ( " + string.Join(" , ", generatedCode)+" )";
            Title = $"MasterMind - Poging {currentAttempt}";
        }

        private Brush BrushColor(string colorName)

        {
            switch (colorName)
            {
                case "Red":
                    return Brushes.Red;
                case "Orange":
                    return Brushes.Orange;
                case "Yellow":
                    return Brushes.Yellow;
                case "Green":
                    return Brushes.Green;
                case "White":
                    return Brushes.White;
                case "Blue":
                    return Brushes.Blue;
                default:
                    return Brushes.Transparent;


            }
        }

        private void color1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (color1.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is string colorName)
            {
                color1Label.Background = BrushColor(colorName);

            }
        }

        private void color2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (color2.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is string colorName)
            {
                color2Label.Background = BrushColor(colorName);
            }

        }

        private void color3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (color3.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is string colorName)
            {
                color3Label.Background = BrushColor(colorName);
            }
        }

        private void color4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (color4.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is string colorName)
            {
                color4Label.Background = BrushColor(colorName);
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            currentAttempt++;
            this.Title = $"MasterMind ({string.Join(",", generatedCode)}),Poging: " + currentAttempt;
            timer.Start();


            if (currentAttempt >= remainingAttempts)
            {
                EndRound("failed");
                return;
            }

            if (correctPosition == 4)
            {
                EndRound("won");
            }
            else if (currentAttempt >= remainingAttempts)
            {
                EndRound("failed");
            }
            else
            {
                StartCountDown();
            }

            string guess1 = (color1.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            string guess2 = (color2.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            string guess3 = (color3.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            string guess4 = (color4.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;

            CheckGuesses(guess1, guess2, guess3, guess4);
            currentAttempt++;
            NewTitle();

            StartCountDown();
            CheckForWin();

        }
        private void EndRound(string result)
        {
            timer.Stop();

            string message;
            string currentPlayer = playerNames[currentPlayerIndex];


            string nextPlayer = GetNextPlayer();

            if (result == "won")
            {
                message = $"Gefeliciteerd {playerNames[currentPlayerIndex]}! Je hebt de code gekraakt!\n Nu is {nextPlayer} aan de beurt.";
                AddHighScore(playerNames[currentPlayerIndex], currentAttempt, totalScore);
            }
            else
            {
                message = $"Helaas {playerNames[currentPlayerIndex]}! Je hebt de code niet gekraakt.\nDe juiste code was: {string.Join(", ", generatedCode)}\n Nu is {nextPlayer} aan de beurt." + $"Spel verloren - {playerNames[currentPlayerIndex]}";

            }
            MessageBox.Show(message, "Ronde Eindigt", MessageBoxButton.OK, MessageBoxImage.Information);
            MoveToNextPlayer();
            UpdateScoreLabel();

        }
        private void stopCountDown()
        {
            timer.Stop();

            if (currentAttempt >= remainingAttempts)
            {
                AddHighScore(name, currentAttempt, totalScore);

                string nextPlayer = GetNextPlayer();

                var failed = MessageBox.Show($"You failed! De correcte code was: {string.Join(", ", generatedCode)} " + $"De volgende speler is: {nextPlayer}. " + "Bekijk highscores?",
                                 "FAILED",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question);


                if (failed == MessageBoxResult.Yes)
                {
                    ShowHighScores();
                    StartGame();

                }
                else
                {
                    App.Current.Shutdown();
                }
                return;


            }
            currentAttempt++;
            NewTitle();
            StartCountDown();
        }
        private string GetNextPlayer()
        {
            int nextPlayerIndex = (currentPlayerIndex + 1) % playerNames.Count;
            return playerNames[nextPlayerIndex];


        }
        private void MoveToNextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % playerNames.Count;

            if (currentPlayerIndex == 0)
            {
                MessageBox.Show("Alle spelers hebben gespeeld. Bekijk de highscores!", "Spel Eindigt", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowHighScores();
                Application.Current.Shutdown();
                return;
            }
            Title = $"MasterMind - {playerNames[currentPlayerIndex]}";
            UpdateScoreLabel();
            ResetGame();
        }

        private void ResetGame()
        {
            currentAttempt = 0;
            totalScore = 0;
            correctPosition = 0;
            correctColorWrongPosition = 0;
            incorrectColor = 0;
            feedbackOverviewPanel.Children.Clear();
            color1.SelectedItem = null;
            color2.SelectedItem = null;
            color3.SelectedItem = null;
            color4.SelectedItem = null;

            scoreLabel.Content = string.Empty;
            GenerateRandomCode();
            NewTitle();
            StartCountDown();
            UpdateScoreLabel();



        }



        private void CheckGuesses(string guess1, string guess2, string guess3, string guess4)
        {
            List<string?> guesses = new List<string?> { guess1, guess2, guess3, guess4 };

            string?[] copy = (string?[])generatedCode.Clone();
            correctPosition = 0;
            correctColorWrongPosition = 0;
            incorrectColor = 0;

            ClearBorder();

            StackPanel feedbackRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };



            for (int i = 0; i < guesses.Count; i++)
            {
                Border chosenColorCircle = new Border
                {
                    Width = 20,
                    Height = 20,
                    CornerRadius = new CornerRadius(5),
                    Background = BrushColor(guesses[i] ?? "Transparent"),
                    BorderThickness = new Thickness(3),
                    Margin = new Thickness(5)
                };

                if (guesses[i] == copy[i] && guesses[i] != null && copy[i] != null)
                {
                    GetLabel(i).BorderBrush = Brushes.DarkRed;
                    GetLabel(i).BorderThickness = new Thickness(2);
                    chosenColorCircle.BorderBrush = Brushes.DarkRed;

                    copy[i] = null;
                    guesses[i] = null;
                    correctPosition++;
                }
                else if (guesses[i] != null && copy.Contains(guesses[i]))
                {

                    GetLabel(i).BorderBrush = Brushes.Wheat;
                    GetLabel(i).BorderThickness = new Thickness(2);
                    chosenColorCircle.BorderBrush = Brushes.White;

                    correctColorWrongPosition++;
                    var index = Array.IndexOf(copy, guesses[i]);
                    copy[index] = null;


                }
                else
                {
                    incorrectColor++;
                }
                feedbackRow.Children.Add(chosenColorCircle);
            }
            totalScore += (correctPosition * 10) + (correctColorWrongPosition * 5) + (incorrectColor * -2);

            feedbackOverviewPanel.Children.Add(feedbackRow);

        }
        private void UpdateScoreLabel()
        {
            scoreLabel.Content = $"Speler: {playerNames[currentPlayerIndex]} \n" +
                $"| Score: {totalScore} | \n " +
                $"| Poging: {currentAttempt}/{remainingAttempts} |";
        }
        private void CheckForWin()
        {
            if (correctPosition == 4)
            {
                timer.Stop();
                AddHighScore(name, currentAttempt, totalScore);

                string nextPlayer = GetNextPlayer();

                var result = MessageBox.Show($"Code is geraakt in {currentAttempt} pogingen.\n  De volgende speler is: {nextPlayer}.\\n\" +\r\n            \"Wil je nog eens spelen?",
                                             "WINNER",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Information);


                if (result == MessageBoxResult.Yes)
                {
                    ResetGame();
                    StartGame();
                }
                else if (result == MessageBoxResult.No)
                {


                    ShowHighScores();
                }
                else
                {
                    Application.Current.Shutdown();
                }

            }
        }




        private Label? GetLabel(int Index)
        {
            switch (Index)
            {
                case 0: return color1Label.Child as Label;
                case 1: return color2Label.Child as Label;
                case 2: return color3Label.Child as Label;
                case 3: return color4Label.Child as Label;
                default: return null;
            }


        }
        private void ClearBorder()
        {
            color1Label.BorderBrush = Brushes.Transparent;
            color2Label.BorderBrush = Brushes.Transparent;
            color3Label.BorderBrush = Brushes.Transparent;
            color4Label.BorderBrush = Brushes.Transparent;
        }


        private void AantalPoging_Click(object sender, RoutedEventArgs e)
        {
            {
                MessageBox.Show($"Aantal pogingen: {currentAttempt}");
            }
        }
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MenuItem_HighScore_Click(object sender, RoutedEventArgs e)
        {
            ShowHighScores();
        }
        private void MenuItem_NewGame_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
            StartGame();
        }





        private string StartGame()
        {
            do
            {

                string name = Microsoft.VisualBasic.Interaction.InputBox("Wat is uw naam?", "Speler toevoegen", " ");

                if (string.IsNullOrEmpty(name))
                {

                    Application.Current.Shutdown();
                    return "";
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Gelieve een geldige naam in te voeren.", "Ongeldige invoer", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    playerNames.Add(name);

                }
                playerNames.Add(name.Trim());


            } while (MessageBox.Show("Wilt u nog een speler toevoegen?", "Nieuwe speler?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);

            if (playerNames.Count == 0)
            {
                MessageBox.Show("U moet minstens één speler toevoegen om het spel te starten.", "Geen spelers", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown(); // applicatie sluiten af als er geen spelers zijn
                return "";
            }
            string playerList = string.Join(", ", playerNames);
            MessageBox.Show($"Deelnemende spelers: {playerList}", "Spel Starten", MessageBoxButton.OK, MessageBoxImage.Information);


            remainingAttempts = AskMaxAttempts();
            totalScore = 100;
            currentAttempt = 0;

            ResetGame();
            UpdateScoreLabel();

            Title = $"MasterMind - Welkom {playerNames[0]}";
            return playerNames[0];


        }

        private void AddHighScore(string Name, int attempts, int score)
        {
            if (highscoreCount < highscores.Length)
            {
                highscores[highscoreCount] = $"{name} - {attempts} pogingen - {score}/100";
                highscoreCount++;
            }
            else
            {

                highscores[highscoreCount - 1] = $"{name} - {attempts} pogingen - {score}/100";
            }

            highscores = highscores
                 .Where(h => IsValidHighScore(h))
                   .OrderByDescending(h => GetScoreFromHighScore(h))
                   .ThenBy(h => GetAttemptsFromHighScore(h))
                    .Concat(Enumerable.Repeat(string.Empty, highscores.Length))
                    .Take(highscores.Length)
                   .ToArray();

        }
        private int GetScoreFromHighScore(string highscore)
        {
            string scorePart = highscore.Split('-')[2].Split('/')[0].Trim();
            return int.TryParse(scorePart, out int score) ? score : 0;
        }



        private int GetAttemptsFromHighScore(string highscore)
        {
            string attemptsPart = highscore.Split('-')[1].Split(' ')[0].Trim();
            return int.TryParse(attemptsPart, out int attempts) ? attempts : 0;

        }


        private void ShowHighScores()
        {
            string highScoreDisplay = string.Join("\n", highscores.Where(h => !string.IsNullOrWhiteSpace(h) && IsValidHighScore(h)));

            if (string.IsNullOrWhiteSpace(highScoreDisplay))
            {
                highScoreDisplay = "Er zijn nog geen highscores.";
            }

            MessageBox.Show(highScoreDisplay, "Highscores", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private bool IsValidHighScore(string highscore)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(highscore))
                    return false;

                var parts = highscore.Split('-');
                if (parts.Length < 3)
                    return false;


                GetScoreFromHighScore(highscore);
                GetAttemptsFromHighScore(highscore);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private int AskMaxAttempts()
        {
            int maxAttempts = 0;

            while (true)
            {

                string input = Microsoft.VisualBasic.Interaction.InputBox("Hoeveel pogingen wil je toestaan? (tussen 3 en 20)", "Maximaal aantal pogingen", "10");

                if (string.IsNullOrWhiteSpace(input))
                {

                    Application.Current.Shutdown();
                    return 0;
                }


                if (int.TryParse(input, out maxAttempts))
                {

                    if (maxAttempts >= 3 && maxAttempts <= 20)
                    {
                        return maxAttempts;
                    }
                }


                MessageBox.Show("Gelieve een geldig getal tussen 3 en 20 in te voeren.", "Ongeldige invoer", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CheckGameEnd()
        {
            if (currentAttempt >= remainingAttempts)
            {
                MessageBox.Show($"Je hebt het niet gehaald binnen {remainingAttempts} pogingen.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                stopCountDown();
            }
        }

        private void KoopHint_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
            "Wil je een hint kopen?\n\n" +
                "1. Een juiste kleur (kost 15 punten).\n" +
                "2. Een juiste kleur op de juiste plaats (kost 25 punten).\n\n" +
                "Klik op 'Yes' voor een juiste kleur.\n" +
                "Klik op 'No' voor een juiste kleur op de juiste plaats.",
                "Koop een Hint",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question
            );
            if (result == MessageBoxResult.Yes)
            {
               
                if (UpdateScore(-15)) 
                {
                    GeefHintJuisteKleur();
                }
            }
            else if (result == MessageBoxResult.No)
            {
                
                if (UpdateScore(-25)) 
                {
                    GeefHintJuisteKleurEnPlaats();
                }
            }
            else
            {
                MessageBox.Show("Hint geannuleerd.", "Annulering", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        private bool UpdateScore(int strafpunten)
        {
            int huidigeScore = int.Parse(scoreLabel.Content.ToString());

            if (huidigeScore + strafpunten < 0)
            {
                MessageBox.Show("Niet genoeg punten om deze hint te kopen!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            huidigeScore += strafpunten;
            scoreLabel.Content = huidigeScore.ToString();
            return true;
        }

        private void GeefHintJuisteKleur()
        {
            Random random = new Random();
            int hintIndex = random.Next(0, generatedCode.Length); 
            string kleur = generatedCode[hintIndex];
            MessageBox.Show($"Hint: Eén van de juiste kleuren is '{kleur}'.", "Hint - Juiste Kleur", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GeefHintJuisteKleurEnPlaats()
        {
            Random random = new Random();
            int hintIndex = random.Next(0, generatedCode.Length); 
            string kleur = generatedCode[hintIndex];
            MessageBox.Show($"Hint: De kleur '{kleur}' zit op positie {hintIndex + 1}.", "Hint - Kleur & Positie", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
