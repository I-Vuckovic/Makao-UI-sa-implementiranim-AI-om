using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIG.AV.Karte;

namespace MakaoIgra
{
    public class Card : Button
    {
        MainWindow view;

        private Boja boja;
        private string broj;
        private bool avaliableForPlay;

        public Boja Boja { get => boja; set => boja = value; }
        public string Broj { get => broj; set => broj = value; }
        public bool AvaliableForPlay { get => avaliableForPlay; set => avaliableForPlay = value; }

        public Card(MainWindow view)
        {
            this.view = view;
        }

        public Card(Boja boja, string broj, MainWindow view)
        {
            this.boja = boja;
            this.broj = broj;
            this.view = view;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.MouseOverBackColor = BackColor;
            this.MouseLeave += OnMouseLeave;
            ChosePicture();
            FlatAppearance.BorderSize = 3;
            this.Click += PlayCard;
        }

        public void PlayCard(object sender, EventArgs e)
        {
            if (avaliableForPlay && view.PlayerCanPlay)
            {
                view.PlayerPlays(this.boja, this.broj, new List<Karta>());
            }
            
        }

        void ChosePicture()
        {
            BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(PictureName(broj, boja));
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        string PictureName(string broj, Boja boja)
        {
            string s = string.Empty;
            if (int.TryParse(broj, out int p))
            {
                s += "_";
            }
            s += broj;

            switch (boja)
            {
                case (Boja.Herz):
                    s += "H";
                    break;
                case (Boja.Tref):
                    s += "C";
                    break;
                case (Boja.Pik):
                    s += "S";
                    break;
                case (Boja.Karo):
                    s += "D";
                    break;
            }

            return s;
        }

        private void OnMouseHover(object sender, EventArgs e)
        {
            FlatAppearance.BorderColor = Color.Red;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            FlatAppearance.BorderColor = Color.Black;
        }

        public void SetForPlay()
        {
            this.MouseHover += OnMouseHover;
            AvaliableForPlay = true;
        }

        public void RemoveForPlay()
        {
            this.MouseHover -= OnMouseHover;
            AvaliableForPlay = false;
        }

        public new void Resize(int height, int width, bool cardOverFlow, int i, int count)
        {
            this.Height = height - 2;
            if (!cardOverFlow)
            {
                this.Location = new Point(i * width/6, 0);
                this.Width = width / 6;
            }
            else
            {
                this.Location = new Point(i * width / count, 0);
                this.Width = width / count;
            }


        }



    }
}
