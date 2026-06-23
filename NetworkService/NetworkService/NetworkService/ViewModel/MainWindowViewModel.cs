using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public static MainWindowViewModel Instance { get; private set; }
        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> CloseWindowCommand { get; private set; }
        public static ObservableCollection<DERModel> DERs { get; set; }

        public NetworkEntitiesViewModel networkEntitiesViewModel;
        public NetworkDisplayViewModel networkDisplayViewModel;
        public MeasurementGraphViewModel measurementGraphViewModel;
        public AddEntityViewModel addEntityViewModel;
        private BindableBase currentViewModel;
        private NotificationManager notificationManager;

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom

            Instance = this;
            notificationManager = new NotificationManager();
            
            NavCommand = new MyICommand<string>(OnNav);
            CloseWindowCommand = new MyICommand<Window>(CloseWindow);

            DERs = new ObservableCollection<DERModel>
            {
                new DERModel("Entity_0", new DERModelType(DERType.SOLAR_PANEL)),
                new DERModel("Entity_1", new DERModelType(DERType.WIND_TURBINE)),
                new DERModel("Entity_2", new DERModelType(DERType.WIND_TURBINE)),
                new DERModel("Entity_3", new DERModelType(DERType.SOLAR_PANEL)),
                new DERModel("Entity_4", new DERModelType(DERType.SOLAR_PANEL)),
                new DERModel("Entity_5", new DERModelType(DERType.WIND_TURBINE)),
            };
            networkEntitiesViewModel = new NetworkEntitiesViewModel();
            networkDisplayViewModel = new NetworkDisplayViewModel();
            measurementGraphViewModel = new MeasurementGraphViewModel();
            addEntityViewModel = new AddEntityViewModel();
            currentViewModel = networkEntitiesViewModel;
        }


        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "NetworkEntities":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "NetworkDisplay":
                    CurrentViewModel = networkDisplayViewModel;
                    break;
                case "MeasurementGraph":
                    CurrentViewModel = measurementGraphViewModel;
                    break;
                default:
                    break;
            }
        }

        public void ShowToastNotification(NotificationContent notificationContent)
        {
            notificationManager.Show(notificationContent, "WindowNotificationArea");
        }

        private void CloseWindow(Window MainWindow)
        {
            MainWindow.Close();
        }


        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(DERs.Count.ToString());
                            stream.Write(data, 0, data.Length);

                            if (File.Exists("Log.txt"))
                            {
                                File.WriteAllText("Log.txt", String.Empty);
                            }
                            else
                            {
                                File.Create("Log.txt");
                            }
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            if (DERs.Count > 0)
                            {
                                var splited = incomming.Split(':');
                                DateTime dt = DateTime.Now;
                                using (StreamWriter sw = File.AppendText("Log.txt"))
                                    sw.WriteLine(dt + "; " + splited[0] + ", " + splited[1]);

                                int id = Int32.Parse(splited[0].Split('_')[1]);
                                DERs[id].Power = Int32.Parse(splited[1]);
                            }
                            
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
    }
}
