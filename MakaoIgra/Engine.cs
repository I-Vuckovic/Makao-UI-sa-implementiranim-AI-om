using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIG.AV.Karte;
using Makao_v2._0;
using System.Threading;
using System.Windows.Forms;

namespace MakaoIgra
{
    public class Engine
    {
        private MainWindow view;

        private Spil spil;
        private Igra bot;
        private bool emptyDeck;
        private List<Karta> playerCards;
        private Boja trenutnaBoja;
        private int kazneneIndex;

        public Igra Bot { get => bot; set => bot = value; }
        public Spil Spil { get => spil; set => spil = value; }
        public List<Karta> PlayerCards { get => playerCards; set => playerCards = value; }
        public Boja TrenutnaBoja { get => trenutnaBoja; set => trenutnaBoja = value; }
        public int KazneneIndex { get => kazneneIndex; set => kazneneIndex = value; }
        public bool EmptyDeck { get => emptyDeck; set => emptyDeck = value; }
      

        public Engine(MainWindow view)
        {
            spil = new Spil(true);
            bot = new Igra();
            EmptyDeck = false;
            playerCards = new List<Karta>();
            this.view = view;
            kazneneIndex = 0;
        }

        public Karta GetCard()
        {

            Karta k = spil.Karte.Last();
            spil.Karte.Remove(spil.Karte.Last());
            view.DeckCount.Text = spil.Karte.Count.ToString();
            if (spil.Karte.Count == 0)
            {
                EmptyDeck = true;
                view.EmptyDeck();
            }
            return k;

        }

        public void SetHands()
        {
            List<Karta> hand = new List<Karta>();
            for (int i = 0; i < 6; i++)
            {
                hand.Add(spil.Karte.Last());
                spil.Karte.Remove(spil.Karte.Last());
            }
            bot.SetRuka(hand);

            hand.Clear();

            for (int i = 0; i < 6; i++)
            {
                hand.Add(spil.Karte.Last());
                spil.Karte.Remove(spil.Karte.Last());
            }
            playerCards.AddRange(hand);

        }

        void BotBuys(List<Karta> thrownCards, int n)
        {
            List<Karta> forBuying = new List<Karta>();

            if (!EmptyDeck)
                forBuying.Add(GetCard());

            bot.KupioKarte(forBuying);

            view.DrawBotCards();

            BotPlays(thrownCards, n);
        }

        public void BotPlays(List<Karta> thrownCards, int n)
        {
            bot.Bacenekarte(thrownCards, trenutnaBoja, n);

            bot.BeginBestMove();
            Wait(1000);
            //Thread.Sleep(1000);
            bot.EndBestMove();

            if (bot.BestMove.Tip != TipPoteza.KrajPoteza)
            {
                if (bot.BestMove.Tip == TipPoteza.KupiKartu)
                {
                    BotBuys(thrownCards, n);

                }
                else if (bot.BestMove.Tip == (TipPoteza.BacaKartu | TipPoteza.KupiKartu))
                {
                    BotBuys(thrownCards, n);
                }
                else if (bot.BestMove.Tip == TipPoteza.KupiKazneneKarte)
                {
                    List<Karta> forBuying = new List<Karta>();
                    for (int i = 0; i < kazneneIndex * 2; i++)
                    {
                        if (!EmptyDeck)
                            forBuying.Add(GetCard());

                    }
                    bot.KupioKarte(forBuying);

                    view.DrawBotCards();
                    //view.PlayerCanPlay = true; //Ne znam sto je ovo ostalo ovde

                    kazneneIndex = 0;

                    BotPlays(thrownCards, n);
                }
                else
                {
                    foreach (Karta k in bot.BestMove.Karte)
                    {
                        view.BotThrows(k);
                        Wait(500);
                    }

                    if (bot.BestMove.Karte.Last().Broj == "2" && bot.BestMove.Karte.Last().Boja == Boja.Tref)
                    {
                        kazneneIndex = 2;
                        view.KazneneIndexChanged();
                    }

                    if (bot.BestMove.Karte.Last().Broj == "7")
                    {
                        kazneneIndex += 1;
                        view.KazneneIndexChanged();

                    }

                    if (bot.BestMove.Karte.Last().Broj == "J")
                    {
                        trenutnaBoja = bot.BestMove.NovaBoja;
                        MessageBox.Show("Protivnik je promenio boju u " + bot.BestMove.NovaBoja.ToString(), "Obavestenje", MessageBoxButtons.OK);
                    }
                    else
                    {
                        trenutnaBoja = bot.BestMove.Karte.Last().Boja;
                    }

                    view.DrawBotCards();



                    view.PlayerCanPlay = true;
                    if (kazneneIndex == 0)
                        view.PlayerCanPlayCards();


                }
            }
            else
            {
                view.DrawBotCards();
                view.PlayerCanPlay = true;
                view.PlayerCanPlayCards();
            }


        }

        public void Wait(int min)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (min == 0 || min < 0) return;
            //Console.WriteLine("start wait timer");
            timer1.Interval = min;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                //Console.WriteLine("stop wait timer");
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }



    }
}
