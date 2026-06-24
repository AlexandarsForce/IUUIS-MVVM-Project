using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {

        #region Properties and Commands

        public ObservableCollection<DERModel> DERModels
        {
            get; set;
        }
        private DERModel selectedDER;
        public DERModel SelectedDER
        {
            get => selectedDER;
            set
            {
                selectedDER = value;
                UpdatePositions(null);
            }
        }

        private readonly double graphC = 25.0;

        public MyICommand SelectCommand;
        public MyICommand SelectionChangedCommand { get; }


        private Point linePoint1;
        private Point linePoint2;
        private Point linePoint3;
        private Point linePoint4;
        private Point linePoint5;
        private Thickness marginPoint1;
        private Thickness marginPoint2;
        private Thickness marginPoint3;
        private Thickness marginPoint4;
        private Thickness marginPoint5;
        private SolidColorBrush circleColor1;
        private SolidColorBrush circleColor2;
        private SolidColorBrush circleColor3;
        private SolidColorBrush circleColor4;
        private SolidColorBrush circleColor5;
        private string circleText1;
        private string circleText2;
        private string circleText3;
        private string circleText4;
        private string circleText5;
        private string time1;
        private string time2;
        private string time3;
        private string time4;
        private string time5;

        private readonly Timer timer;

        private Point startPoint;
        public Point StartPoint { get => startPoint; set => SetProperty(ref startPoint, value); }
        
        public Point LinePoint1 { get => linePoint1; set => SetProperty(ref linePoint1, value); }
        public Point LinePoint2 { get => linePoint2; set => SetProperty(ref linePoint2, value); }
        public Point LinePoint3 { get => linePoint3; set => SetProperty(ref linePoint3, value); }
        public Point LinePoint4 { get => linePoint4; set => SetProperty(ref linePoint4, value); }
        public Point LinePoint5 { get => linePoint5; set => SetProperty(ref linePoint5, value); }
        public Thickness MarginPoint1 { get => marginPoint1; set => SetProperty(ref marginPoint1, value); }
        public Thickness MarginPoint2 { get => marginPoint2; set => SetProperty(ref marginPoint2, value); }
        public Thickness MarginPoint3 { get => marginPoint3; set => SetProperty(ref marginPoint3, value); }
        public Thickness MarginPoint4 { get => marginPoint4; set => SetProperty(ref marginPoint4, value); }
        public Thickness MarginPoint5 { get => marginPoint5; set => SetProperty(ref marginPoint5, value); }

        public SolidColorBrush CircleColor1 { get => circleColor1; set => SetProperty(ref circleColor1, value); }
        public SolidColorBrush CircleColor2 { get => circleColor2; set => SetProperty(ref circleColor2, value); }
        public SolidColorBrush CircleColor3 { get => circleColor3; set => SetProperty(ref circleColor3, value); }
        public SolidColorBrush CircleColor4 { get => circleColor4; set => SetProperty(ref circleColor4, value); }
        public SolidColorBrush CircleColor5 { get => circleColor5; set => SetProperty(ref circleColor5, value); }

        public string CircleText1 { get => circleText1; set => SetProperty(ref circleText1, value); }
        public string CircleText2 { get => circleText2; set => SetProperty(ref circleText2, value); }
        public string CircleText3 { get => circleText3; set => SetProperty(ref circleText3, value); }
        public string CircleText4 { get => circleText4; set => SetProperty(ref circleText4, value); }
        public string CircleText5 { get => circleText5; set => SetProperty(ref circleText5, value); }

        public string Time1 { get => time1; set => SetProperty(ref time1, value); }
        public string Time2 { get => time2; set => SetProperty(ref time2, value); }
        public string Time3 { get => time3; set => SetProperty(ref time3, value); }
        public string Time4 { get => time4; set => SetProperty(ref time4, value); }
        public string Time5 { get => time5; set => SetProperty(ref time5, value); }

        #endregion

        #region Constructor
        public MeasurementGraphViewModel()
        {
            this.DERModels = MainWindowViewModel.DERs;
            if (DERModels.Count != 0)
                SelectedDER = DERModels[0];

            StartPoint = new Point(45, 250);

            LinePoint1 = new Point(45, 250);
            LinePoint2 = new Point(95, 250);
            LinePoint3 = new Point(145, 250);
            LinePoint4 = new Point(195, 250);
            LinePoint5 = new Point(245, 250);

            MarginPoint1 = new Thickness(30, 235, 0, 0);
            MarginPoint2 = new Thickness(80, 235, 0, 0);
            MarginPoint3 = new Thickness(130, 235, 0, 0);
            MarginPoint4 = new Thickness(180, 235, 0, 0);
            MarginPoint5 = new Thickness(230, 235, 0, 0);

            CircleColor1 = new SolidColorBrush(Colors.Gray);
            CircleColor2 = new SolidColorBrush(Colors.Gray);
            CircleColor3 = new SolidColorBrush(Colors.Gray);
            CircleColor4 = new SolidColorBrush(Colors.Gray);
            CircleColor5 = new SolidColorBrush(Colors.Gray);

            CircleText1 = "X";
            CircleText2 = "X";
            CircleText3 = "X";
            CircleText4 = "X";
            CircleText5 = "X";

            Time1 = "00:00";
            Time2 = "00:00";
            Time3 = "00:00";
            Time4 = "00:00";
            Time5 = "00:00";

            timer = new Timer(UpdatePositions, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(200));
        }
        #endregion

        #region Update UI
        private void UpdatePositions(object state)
        {
            if (SelectedDER != null && Application.Current != null)
            {

                Application.Current.Dispatcher.Invoke(() =>
                {

                    const int heightOffset = 235;
                    if (SelectedDER.LastPowers.Count > 0)
                    {
                        LinePoint1 = new Point(LinePoint1.X, (int)Math.Round(250 - SelectedDER.LastPowers[0].Item2 * graphC));
                        StartPoint = LinePoint1;
                        MarginPoint1 = new Thickness(MarginPoint1.Left, (int)Math.Round(heightOffset - SelectedDER.LastPowers[0].Item2 * graphC), 0, 0);
                        CircleText1 = SelectedDER.LastPowers[0].Item2.ToString();

                        if (SelectedDER.LastPowers[0].Item2 < 1)
                        {
                            CircleColor1 = new SolidColorBrush(Colors.Orange);
                        }
                        else if (SelectedDER.LastPowers[0].Item2 > 5)
                        {
                            CircleColor1 = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            CircleColor1 = new SolidColorBrush(Colors.Green);
                        }
                        DateTime dateTime = SelectedDER.LastPowers[0].Item1;
                        Time1 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    else
                    {
                        LinePoint1 = new Point(45, 250);
                        StartPoint = LinePoint1;
                        MarginPoint1 = new Thickness(30, 235, 0, 0);
                        CircleColor1 = new SolidColorBrush(Colors.Gray);
                        CircleText1 = "X";
                        Time1 = "00:00";
                    }


                    if (SelectedDER.LastPowers.Count > 1)
                    {
                        LinePoint2 = new Point(LinePoint2.X, (int)Math.Round(250 - SelectedDER.LastPowers[1].Item2 * graphC));
                        MarginPoint2 = new Thickness(MarginPoint2.Left, (int)Math.Round(heightOffset - SelectedDER.LastPowers[1].Item2 * graphC), 0, 0);
                        CircleText2 = SelectedDER.LastPowers[1].Item2.ToString();

                        if (SelectedDER.LastPowers[1].Item2 < 1)
                        {
                            CircleColor2 = new SolidColorBrush(Colors.Orange);
                        }
                        else if (SelectedDER.LastPowers[1].Item2 > 5)
                        {
                            CircleColor2 = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            CircleColor2 = new SolidColorBrush(Colors.Green);
                        }
                        DateTime dateTime = SelectedDER.LastPowers[1].Item1;
                        Time2 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    else
                    {
                        LinePoint2 = new Point(95, 250);
                        MarginPoint2 = new Thickness(80, 235, 0, 0);
                        CircleColor2 = new SolidColorBrush(Colors.Gray);
                        CircleText2 = "X";
                        Time2 = "00:00";
                    }

                    if (SelectedDER.LastPowers.Count > 2)
                    {
                        LinePoint3 = new Point(LinePoint3.X, (int)Math.Round(250 - SelectedDER.LastPowers[2].Item2 * graphC));
                        MarginPoint3 = new Thickness(MarginPoint3.Left, (int)Math.Round(heightOffset - SelectedDER.LastPowers[2].Item2 * graphC), 0, 0);
                        CircleText3 = SelectedDER.LastPowers[2].Item2.ToString();

                        if (SelectedDER.LastPowers[2].Item2 < 1)
                        {
                            CircleColor3 = new SolidColorBrush(Colors.Orange);
                        }
                        else if (SelectedDER.LastPowers[2].Item2 > 5)
                        {
                            CircleColor3 = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            CircleColor3 = new SolidColorBrush(Colors.Green);
                        }
                        DateTime dateTime = SelectedDER.LastPowers[2].Item1;
                        Time3 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    else
                    {
                        LinePoint3 = new Point(145, 250);
                        MarginPoint3 = new Thickness(130, 235, 0, 0);
                        CircleColor3 = new SolidColorBrush(Colors.Gray);
                        CircleText3 = "X";
                        Time3 = "00:00";
                    }

                    if (SelectedDER.LastPowers.Count > 3)
                    {
                        LinePoint4 = new Point(LinePoint4.X, (int)Math.Round(250 - SelectedDER.LastPowers[3].Item2 * graphC));
                        MarginPoint4 = new Thickness(MarginPoint4.Left, (int)Math.Round(heightOffset - SelectedDER.LastPowers[3].Item2 * graphC), 0, 0);
                        CircleText4 = SelectedDER.LastPowers[3].Item2.ToString();

                        if (SelectedDER.LastPowers[3].Item2 < 1)
                        {
                            CircleColor4 = new SolidColorBrush(Colors.Orange);
                        }
                        else if (SelectedDER.LastPowers[3].Item2 > 5)
                        {
                            CircleColor4 = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            CircleColor4 = new SolidColorBrush(Colors.Green);
                        }
                        DateTime dateTime = SelectedDER.LastPowers[3].Item1;
                        Time4 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    else
                    {
                        LinePoint4 = new Point(195, 250);
                        MarginPoint4 = new Thickness(180, 235, 0, 0);
                        CircleColor4 = new SolidColorBrush(Colors.Gray);
                        CircleText4 = "X";
                        Time4 = "00:00";
                    }

                    if (SelectedDER.LastPowers.Count > 4)
                    {
                        LinePoint5 = new Point(LinePoint5.X, (int)Math.Round(250 - SelectedDER.LastPowers[4].Item2 * graphC));
                        MarginPoint5 = new Thickness(MarginPoint5.Left, (int)Math.Round(heightOffset - SelectedDER.LastPowers[4].Item2 * graphC), 0, 0);
                        CircleText5 = SelectedDER.LastPowers[4].Item2.ToString();

                        if (SelectedDER.LastPowers[4].Item2 < 1)
                        {
                            CircleColor5 = new SolidColorBrush(Colors.Orange);
                        }
                        else if (SelectedDER.LastPowers[4].Item2 > 5)
                        {
                            CircleColor5 = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            CircleColor5 = new SolidColorBrush(Colors.Green);
                        }
                        DateTime dateTime = SelectedDER.LastPowers[4].Item1;
                        Time5 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    else
                    {
                        LinePoint5 = new Point(245, 250);
                        MarginPoint5 = new Thickness(230, 235, 0, 0);
                        CircleColor5 = new SolidColorBrush(Colors.Gray);
                        CircleText5 = "X";
                        Time5 = "00:00";
                    }

                });

            }
            else if (Application.Current == null)
            {
                timer.Dispose();
            }
            else { }
        }

        #endregion
    }
}
