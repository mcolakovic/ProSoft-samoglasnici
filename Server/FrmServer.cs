using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class FrmServer : Form
    {
        Server server = new Server();
        public FrmServer()
        {
            InitializeComponent();
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            char s;
            if (char.TryParse(txtKarakter.Text, out s) && Validacija())
            {
                if (isSamoglasnik(s))
                {
                    if (server.Slova.Where(x => x == s).Count() < 2)
                    {
                        server.Slova.Insert(0, s);
                    }
                    else
                    {
                        MessageBox.Show("Vec postoje dva karaktera " + s.ToString());
                    }
                }
                else
                {
                    server.Slova.Insert(0, s);
                }
            }
            txtKarakteri.Text = "";
            foreach (char c in server.Slova)
            {
                txtKarakteri.Text += c.ToString() + " ";
            }
            txtKarakter.Text = "";
        }

        private bool isSamoglasnik(char s)
        {
            if (s == 'a' || s == 'e' || s == 'i' || s == 'o' || s == 'u')
                return true;
            return false;
        }

        private bool Validacija()
        {
            if (Regex.IsMatch(txtKarakter.Text, @"^[a-z]{1}"))
                return true;
            return false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (server.Slova.Count >= 4)
                {
                    server.Start();
                    Thread serverskaNit = new Thread(server.HandleClients);
                    serverskaNit.IsBackground = true;
                    serverskaNit.Start();
                    lblStatus.Text = "Server je pokrenut";
                    btnStart.Visible = false;
                    btnStop.Visible = true;
                    btnDodaj.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            btnStart.Visible = true;
            btnStop.Visible = false;
            btnDodaj.Visible = true;
            lblStatus.Text = "Server nije pokrenut";
        }
    }
}
