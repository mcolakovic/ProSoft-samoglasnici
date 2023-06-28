using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KorisnickiInterfejs
{
    public partial class FrmKlijent : Form
    {
        private List<char> lista = new List<char>();
        public FrmKlijent()
        {
            InitializeComponent();
            try
            {
                Communication.Instance.Connect();
                InitializeListener();
            }
            catch (Exception)
            {
                MessageBox.Show("Server nije startovan");
                Environment.Exit(0);
            }
        }

        private void InitializeListener()
        {
            try
            {
                Thread porukaNit = new Thread(CitajPoruke);
                porukaNit.IsBackground = true;
                porukaNit.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CitajPoruke()
        {
            try
            {
                while (true)
                {
                    Poruka poruka = Communication.Instance.ReadMessage<Poruka>();
                    switch (poruka.Operations)
                    {
                        case Operations.Igra:
                            if (poruka.IsSuccessful)
                            {
                                lista.Add(((Info)poruka.PorukaObject).Slovo);
                                Invoke(new Action(() => txtKarakteri.Text += ((Info)poruka.PorukaObject).Slovo.ToString() + " "));
                                Invoke(new Action(() => txtCount.Text = lista.Count().ToString()));
                            }
                            break;
                        case Operations.ZavrsiIgru:
                            Invoke(new Action(() => btnBrisi.Enabled = false));
                            Invoke(new Action(() => lblInfo.Text = "Svi karakteri su obrisani"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void btnBrisi_Click(object sender, EventArgs e)
        {
            try
            {
                Poruka poruka = new Poruka
                {
                    Operations = Operations.Igra
                };
                Communication.Instance.SendMessage(poruka);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
