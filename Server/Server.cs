using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Common;

namespace Server
{
    public class Server
    {
        private Socket serverSocket;
        private bool isRunning = false;
        private List<ClientHandler> clients = new List<ClientHandler>();
        public List<ClientHandler> Clients { get => clients; }
        private List<char> slova = new List<char>();
        public List<char> Slova { get => slova; set => slova = value; }
        private static Object zakljucaj = new Object();

        public void Start()
        {
            if (!isRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
                serverSocket.Listen(5);
                isRunning = true;
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                serverSocket.Dispose();
                serverSocket = null;
                isRunning = false;
            }
        }

        public void HandleClients()
        {
            try
            {
                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();
                    ClientHandler handler = new ClientHandler(clientSocket, slova);
                    clients.Add(handler);
                    handler.OdjavljenKorisnik += Handler_OdjavljenKorisnik;
                    handler.ObrisiSlovo += Handler_ObrisiSlovo;
                    Thread klijentskaNit = new Thread(handler.HandleRequests);
                    klijentskaNit.IsBackground = true;
                    klijentskaNit.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Handler_ObrisiSlovo(object sender, EventArgs e)
        {
            char? odg = ObrisiSlovoIzListe();

            if (odg != null)
            {
                Poruka poruka = new Poruka
                {
                    IsSuccessful = true,
                    Operations = Operations.Igra,
                    PorukaObject = new Info
                    {
                        Slovo = (char)odg
                    }
                };
                ((ClientHandler)sender).Helper.Send(poruka);
            }
            else
            {
                Poruka poruka = new Poruka
                {
                    IsSuccessful = true,
                    Operations = Operations.ZavrsiIgru
                };
                ((ClientHandler)sender).Helper.Send(poruka);
            }
        }

        private char? ObrisiSlovoIzListe()
        {
            char s;
            if (slova.Count != 0)
            {
                lock (zakljucaj)
                {
                    s = slova[0];
                    slova.RemoveAt(0);
                }
                return s;
            }
            return null;
        }

        private void Handler_OdjavljenKorisnik(object sender, EventArgs e)
        {
            clients.Remove((ClientHandler)sender);
        }
    }
}
