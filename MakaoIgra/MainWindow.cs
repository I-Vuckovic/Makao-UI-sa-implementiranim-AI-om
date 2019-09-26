using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIG.AV.Karte;
using System.Threading;

namespace MakaoIgra
{
    public partial class MainWindow : Form
    {
        private bool endGame;
        private Label deckCount;
        private Panel playerCardPanel;
        private Panel botCardPanel;
        private Card deck;
        private Card table;
        private Engine engine;
        private bool playerCanPlay;
        private bool playerBought;
        private bool played8;

        public Panel PlayerCardPanel { get => playerCardPanel; set => playerCardPanel = value; }
        public Panel BotCardPanel { get => botCardPanel; set => botCardPanel = value; }
        public bool PlayerCanPlay { get => playerCanPlay; set => playerCanPlay = value; }
        public Card Table { get => table; set => table = value; }
        public Label DeckCount { get => deckCount; set => deckCount = value; }

        public MainWindow()
        {

            BotCardPanel = new Panel();
            PlayerCardPanel = new Panel();

            InitializeComponent();

            DrawPanels();

            NewGame();

        }

        void NewGame()
        {
            endGame = false;
            played8 = false;
            playerCanPlay = true;
            playerBought = false;

            playerCardPanel.Controls.Clear();
            botCardPanel.Controls.Clear();
            this.Controls.Remove(DeckCount);
            this.Controls.Remove(deck);
            this.Controls.Remove(Table);
            engine = new Engine(this);

            DeckCount = new Label();

            deck = new Card(this);
            deck.Click += BuyCard;
            deck.Click -= deck.PlayCard;

            Karta k = engine.GetCard();
            engine.TrenutnaBoja = k.Boja;
            Table = new Card(k.Boja, k.Broj, this);
            List<Karta> bacene = new List<Karta>();
            bacene.Add(k);
            engine.Bot.Bacenekarte(bacene, Boja.Unknown, 6);

            engine.SetHands();


            DrawBotCards();
            DrawPlayerCards();
            DrawDeck();
            DrawLabel();
            DrawTable();

            PlayerCanPlayCards();

        }

        private void DrawLabel()
        {
            this.Controls.Add(DeckCount);

            DeckCount.Text = engine.Spil.Karte.Count.ToString();
            DeckCount.Font = new Font(FontFamily.GenericSerif, this.Size.Height / 20);
            DeckCount.Height = this.Height / 15;
            DeckCount.Width = this.Width / 10;
            DeckCount.Location = new Point(deck.Location.X + deck.Size.Width + 20, deck.Location.Y + deck.Size.Height / 2 - DeckCount.Height / 2);

        }


        private void BuyCard(object sender, EventArgs e)
        {
            if (engine.KazneneIndex == 0)
            {
                if (!playerBought && !played8)
                {
                    if (!engine.EmptyDeck)
                        engine.PlayerCards.Add(engine.GetCard());
                    playerBought = true;
                    DrawPlayerCards();
                    PlayerCanPlayCards();
                }
                else
                {
                    PlayerCanPlay = false;
                    engine.BotPlays(new List<Karta>(), engine.PlayerCards.Count);
                    playerBought = false;
                    played8 = false;
                }
            }
            else
            {
                for (int i = 0; i < engine.KazneneIndex * 2; i++)
                {
                    if (!engine.EmptyDeck)
                        engine.PlayerCards.Add(engine.GetCard());
                }
                engine.KazneneIndex = 0;
                DrawPlayerCards();
                PlayerCanPlayCards();
            }
        }

        private void MainWindow_ClientSizeChanged(object sender, EventArgs e)
        {
            if (playerCardPanel != null && botCardPanel != null && deck != null && Table != null && DeckCount != null)
            {
                DrawPanels();

                for (int i = 0; i < playerCardPanel.Controls.Count; i++)
                {
                    Card p = (Card)playerCardPanel.Controls[i];
                    p.Resize(playerCardPanel.Height, playerCardPanel.Width, playerCardPanel.Controls.Count > 6, i, playerCardPanel.Controls.Count);
                }

                for (int i = 0; i < botCardPanel.Controls.Count; i++)
                {
                    Card p = (Card)botCardPanel.Controls[i];
                    p.Resize(BotCardPanel.Height, BotCardPanel.Width, engine.Bot.ruka.Count > 6, i, engine.Bot.ruka.Count);
                }

                DrawDeck();

                DrawLabel();

                DrawTable();
            }
        }

        void DrawDeck()
        {
            deck.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("gray_back");
            deck.BackgroundImageLayout = ImageLayout.Stretch;

            this.Controls.Add(deck);

            deck.Location = new Point(this.ClientSize.Width * 2 / 3, (this.ClientSize.Height + menuStrip.Height) * 1 / 3 + 9);
            deck.Height = (this.ClientSize.Height - menuStrip.Height) / 3;
            deck.Width = this.ClientSize.Width / 6;

        }

        void DrawPanels()
        {

            this.Controls.Add(PlayerCardPanel);
            PlayerCardPanel.Location = new Point(2, (this.ClientSize.Height + menuStrip.Height) * 2 / 3 - 8);
            PlayerCardPanel.Height = (this.ClientSize.Height - menuStrip.Height) / 3;
            PlayerCardPanel.Width = this.Width - 25;
            PlayerCardPanel.BorderStyle = BorderStyle.FixedSingle;

            this.Controls.Add(BotCardPanel);
            BotCardPanel.Location = new Point(2, 2 + menuStrip.Height);
            BotCardPanel.Height = (this.ClientSize.Height - menuStrip.Height) / 3;
            BotCardPanel.Width = this.Width - 25;
            BotCardPanel.BorderStyle = BorderStyle.FixedSingle;


        }

        void DrawTable()
        {
            this.Controls.Add(Table);

            Table.Location = new Point(this.ClientSize.Width * 1 / 3, (this.ClientSize.Height + menuStrip.Height) * 1 / 3 + 9);
            Table.Height = (this.ClientSize.Height - menuStrip.Height) / 3;
            Table.Width = this.ClientSize.Width / 6;
        }

        public void DrawBotCards()
        {
            botCardPanel.Controls.Clear();

            for (int i = 0; i < engine.Bot.ruka.Count; i++)
            {
                Card k = new Card(this);
                k.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject("gray_back");
                k.BackgroundImageLayout = ImageLayout.Stretch;
                k.Enabled = false;

                BotCardPanel.Controls.Add(k);
                k.Resize(BotCardPanel.Height, BotCardPanel.Width, engine.Bot.ruka.Count > 6, i, engine.Bot.ruka.Count);
            }
        }

        public void DrawPlayerCards()
        {
            playerCardPanel.Controls.Clear();

            for (int i = 0; i < engine.PlayerCards.Count; i++)
            {
                Card k = new Card(engine.PlayerCards[i].Boja, engine.PlayerCards[i].Broj, this);

                playerCardPanel.Controls.Add(k);
                k.Resize(PlayerCardPanel.Height, PlayerCardPanel.Width, engine.PlayerCards.Count > 6, i, engine.PlayerCards.Count);
            }
        }

        public void PlayerCanPlayCards()
        {
            int i = 0;

            foreach (Card c in playerCardPanel.Controls)
            {
                c.RemoveForPlay();
                if (c.Broj == Table.Broj || c.Boja == engine.TrenutnaBoja || c.Broj == "J")
                {
                    c.SetForPlay();
                    i++;
                }
            }

            if (i == 0 && endGame && PlayerCanPlay)
            {
                FinalResults();
            }
        }

        public void PlayerPlays(Boja boja, string broj, List<Karta> playedCards)
        {
            List<Karta> currentlyPlayed = new List<Karta>();
            currentlyPlayed.AddRange(playedCards);

            this.Controls.Remove(Table);
            Karta k = new Karta() { Boja = boja, Broj = broj };
            currentlyPlayed.Add(k);

            if (broj == "J")
            {
                ColorSelector frm = new ColorSelector(engine);
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);

            }
            else
            {
                engine.TrenutnaBoja = k.Boja;
            }
            Table = new Card(k.Boja, k.Broj, this);
            DrawTable();

            engine.PlayerCards.RemoveAll(x => (x.Broj == broj && x.Boja == boja));
            DrawPlayerCards();


            if (engine.PlayerCards.Count > 0)
            {
                if (broj == "8")
                    played8 = true;
                else
                    played8 = false;

                if (broj != "A" && broj != "8")
                {
                    if (broj == "7")
                        engine.KazneneIndex += 1;

                    if (broj == "2" && boja == Boja.Tref)
                        engine.KazneneIndex += 2;

                    PlayerCanPlay = false;
                    engine.BotPlays(currentlyPlayed, engine.PlayerCards.Count);
                    playerBought = false;
                }

                if (engine.Bot.ruka.Count == 0)
                {
                    if (engine.Bot.BestMove.Karte.Last().Broj != "7")
                    {
                        Lose();
                    }
                    else if (!PlayerCanStopLose())
                    {
                        Lose();
                    }
                }

                if (engine.KazneneIndex == 0 && engine.Bot.ruka.Count != 0)
                    PlayerCanPlayCards();
            }
            else
            {
                if (broj != "7" && broj != "A")
                {
                    Win();
                }
                else
                {
                    if (broj == "A")
                    {

                    }
                    else if (broj == "7")
                    {

                        PlayerCanPlay = false;
                        engine.BotPlays(currentlyPlayed, engine.PlayerCards.Count);
                        playerBought = false;
                        if (engine.KazneneIndex == 0)
                        {
                            Win();
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        bool PlayerCanStopLose()
        {
            foreach (Card c in playerCardPanel.Controls)
            {
                if (c.Broj == "7")
                    return true;
            }

            return false;
        }

        void Lose()
        {
            MessageBox.Show("Izgubili ste...", "Obavestenje", MessageBoxButtons.OK);
            foreach (Card c in PlayerCardPanel.Controls)
                c.RemoveForPlay();
            deck.Click -= BuyCard;
        }

        void Win()
        {
            MessageBox.Show("POBEDILI STE!!!", "Obavestenje", MessageBoxButtons.OK);
            foreach (Card c in PlayerCardPanel.Controls)
                c.RemoveForPlay();
            deck.Click -= BuyCard;
        }

        public void BotThrows(Karta k)
        {
            this.Controls.Remove(Table);
            Table = new Card(k.Boja, k.Broj, this);
            DrawTable();
            Thread.Sleep(500);
        }

        public void KazneneIndexChanged()
        {
            foreach (Card c in playerCardPanel.Controls)
                c.RemoveForPlay();

            if (table.Broj == "2" && table.Boja == Boja.Tref)
            {

            }
            else
            {
                foreach (Card c in playerCardPanel.Controls)
                {
                    if (c.Broj == "7")
                        c.SetForPlay();
                }
            }
        }

        public void EmptyDeck()
        {
            this.Controls.Remove(deck);
            endGame = true;
        }

        void FinalResults()
        {
            MessageBox.Show("Kraj, recicemo da ko u ovom trenutku ima vise karata da je izgubio, ili je nereseno.", "Obavestneje", MessageBoxButtons.OK);
        }

        private void newGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rules frm = new Rules();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }
    }
}
