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

namespace MakaoIgra
{
    public partial class ColorSelector : Form
    {
        Engine engine;

        public ColorSelector()
        {
            InitializeComponent();
        }

        public ColorSelector(Engine engine)
        {
            InitializeComponent();
            this.engine = engine;
            this.ControlBox = false;

        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            foreach (var control in this.Controls)
            {
                if (control != null && control.GetType() == typeof(RadioButton))
                {
                    RadioButton pom = control as RadioButton;
                    if (pom.Checked)
                        engine.TrenutnaBoja = (Boja)Enum.Parse(typeof(Boja), pom.Text); 
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
