using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {

        #region Properties

        private DERModel selectedDER;
        public DERModel SelectedDER
        {
            get => selectedDER;
            set
            {
                SetProperty(ref selectedDER, value);
            }
        }

        //dragging item info
        private DERModel draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;

        public static ObservableCollection<DERModel> DERModels { get; set; }
        public static ObservableCollection<DERGroup> DERGroups { get; set; }
        public static Dictionary<int, DERModel> AddedToGrid { get; set; } = new Dictionary<int, DERModel>();

        public static Dictionary<string, Line> Lines { get; set; } = new Dictionary<string, Line>();
        public ObservableCollection<Line> LinesToDisplay { get; set; }

        public static ObservableCollection<Canvas> CanvasCollection { get; set; } = new ObservableCollection<Canvas>();
        public static ObservableCollection<DERModel> DERInfo { get; set; } = new ObservableCollection<DERModel>();
        public static ObservableCollection<Brush> BorderBrushCollection { get; set; } = new ObservableCollection<Brush>();

        #endregion

        #region Commands

        public MyICommand<string> DropDEROnCanvasCommand { get; set; }
        public MyICommand<string> MouseLeftButtonDownCommand { get; set; }
        public MyICommand MouseLeftButtonUpCommand { get; set; }
        public MyICommand<string> FreeCanvas { get; set; }
        public MyICommand<object> SelectionChangedCommand { get; set; }
        public MyICommand<string> ClearCanvasCommand { get; set; }


        #endregion

        #region Initialization
        public NetworkDisplayViewModel()
        {
            LinesToDisplay = new ObservableCollection<Line>();
            InitializeCollections();
            InitializeGroups();

            DrawExistingLines();

            DropDEROnCanvasCommand = new MyICommand<string>(OnDrop);
            MouseLeftButtonDownCommand = new MyICommand<string>(OnLeftMouseButtonDown);
            MouseLeftButtonUpCommand = new MyICommand(OnLeftMouseButtonUp);
            FreeCanvas = new MyICommand<string>(ResetCanvas);
            SelectionChangedCommand = new MyICommand<object>(OnSelectionChanged);
            ClearCanvasCommand = new MyICommand<string>(ResetCanvas);


        }

        public static void InitializeCollections()
        {
            if (CanvasCollection.Count == 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    Canvas canvas = new Canvas();
                    if (AddedToGrid.ContainsKey(i))
                    {
                        string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AddedToGrid[i].Type.ImagePath);
                        var logo = new BitmapImage(new Uri(filePath, UriKind.Relative));
                        canvas.Background = new ImageBrush(logo);
                        DERInfo.Add(AddedToGrid[i]);
                        canvas.Resources["taken"] = true;
                        canvas.Resources["data"] = AddedToGrid[i];
                    }
                    else
                    {
                        canvas.Background = Brushes.Transparent;
                        DERInfo.Add(null);
                    }

                    canvas.AllowDrop = true;
                    CanvasCollection.Add(canvas);
                    BorderBrushCollection.Add(Brushes.Transparent);
                }
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    if (AddedToGrid.ContainsKey(i))
                    {
                        var logo = new BitmapImage(new Uri(AddedToGrid[i].Type.ImagePath, UriKind.Relative));
                        CanvasCollection[i].Background = new ImageBrush(logo);
                        CanvasCollection[i].Resources["taken"] = true;
                        CanvasCollection[i].Resources["data"] = AddedToGrid[i];
                        DERInfo[i] = AddedToGrid[i];
                    }
                    else
                    {
                        CanvasCollection[i].Background = Brushes.Transparent;
                        if (CanvasCollection[i].Resources.Contains("taken"))
                            CanvasCollection[i].Resources.Remove("taken");
                        if (CanvasCollection[i].Resources.Contains("data"))
                            CanvasCollection[i].Resources.Remove("data");
                        BorderBrushCollection[i] = Brushes.Transparent;
                        DERInfo[i] = null;

                        List<int> connections = FindAllConnections(i);
                        if (connections.Count > 0)
                        {
                            foreach (int connectedTo in connections)
                            {
                                int source = Math.Min(i, connectedTo);
                                int destination = Math.Max(i, connectedTo);
                                DeleteLine(source, destination);
                            }
                        }

                    }
                }
            }
            foreach (DERModel d in AddedToGrid.Values)
                RemoveFromGroup(d);
        }
        public static void InitializeGroups()
        {

            DERModels = MainWindowViewModel.DERs;

            DERGroups = DERGroups ?? new ObservableCollection<DERGroup>
            {
                new DERGroup(DERType.SOLAR_PANEL),
                new DERGroup(DERType.WIND_TURBINE),
            };
            DERGroups[0].DERModels.Clear();
            DERGroups[1].DERModels.Clear();

            foreach (var derModel in DERModels)
            {
                foreach (var category in DERGroups)
                {
                    if (category.Name.Equals(derModel.Type.Name) && !AddedToGrid.ContainsValue(derModel))
                    {
                        category.DERModels.Add(derModel);
                    }
                }
            }
        }

        #endregion

        #region Line Drawing
        public void DrawExistingLines()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                LinesToDisplay.Clear();

                var linesArray = Lines.Values.ToArray();

                for (int i = 0; i < linesArray.Length; i++)
                {
                    try
                    {
                        LinesToDisplay.Add(linesArray[i]);
                    }
                    catch (ArgumentOutOfRangeException ex) { }

                }

            });

        }

        #endregion

        #region Line Methods
        private Line CreateNewLine(int sourceIndex, int destinationIndex)
        {
            Line newLine = new Line
            {
                X1 = ConvertToAbsoluteX(sourceIndex),
                Y1 = ConvertToAbsoluteY(sourceIndex),
                X2 = ConvertToAbsoluteX(destinationIndex),
                Y2 = ConvertToAbsoluteY(destinationIndex),
                Stroke = Brushes.Orange,
                StrokeThickness = 3,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            return newLine;
        }
        private double ConvertToAbsoluteY(int index)
        {
            index /= 3;

            return Math.Round(index * 86.4285 + 30.0);
        }
        private double ConvertToAbsoluteX(int index)
        {
            index %= 3;

            return Math.Round(index * 113.33 + 60.0);
        }
        public static List<int> FindAllConnections(int index)
        {
            return Lines.Keys.Select(c =>
            {
                var parts = c.Split(',');
                int index1 = int.Parse(parts[0]);
                int index2 = int.Parse(parts[1]);
                return index == index1 ? index2 : index == index2 ? index1 : (int?)null;
            })
            .Where(connectedIndex => connectedIndex.HasValue)
            .Select(connectedIndex => connectedIndex.Value)
            .ToList();

        }
        public static void DeleteLine(int index1, int index2)
        {
            string key = $"{index1},{index2}";
            if (!Lines.Remove(key))
            {
                key = $"{index2},{index1}";
                Lines.Remove(key);
            }
        }
        private int IsLineAlreadyDrawn(int sourceIndex, int destinationIndex)
        {
            return Lines.Keys.Cast<string>().Any(c =>
            {
                var parts = c.Split(',');
                int index1 = int.Parse(parts[0]);
                int index2 = int.Parse(parts[1]);
                return sourceIndex == index1 && destinationIndex == index2;
            }) ? 1 : 0;
        }

        #endregion

        #region Drag & Drop and Canvas Logic

        private void OnLeftMouseButtonUp()
        {
            dragging = false;
            draggedItem = null;
            draggingSourceIndex = -1;
        }
        private void OnSelectionChanged(object parameter)
        {
            if (!dragging && parameter != null)
            {
                dragging = true;
                draggedItem = SelectedDER;
                if (draggedItem != null)
                    DragDrop.DoDragDrop((ListView)parameter, draggedItem, DragDropEffects.Move);
            }
        }
        private void OnLeftMouseButtonDown(string indexString)
        {
            int index = int.Parse(indexString);
            if (!dragging && CanvasCollection[index].Resources.Contains("taken"))
            {
                dragging = true;
                draggedItem = CanvasCollection[index].Resources["data"] as DERModel;
                draggingSourceIndex = index;

                if (draggedItem != null)
                    DragDrop.DoDragDrop(CanvasCollection[index], draggedItem, DragDropEffects.Move);

            }
        }
        private void OnDrop(string indexString)
        {

            int index = int.Parse(indexString);

            if (CanvasCollection[index].Resources.Contains("data"))
            {
                if (draggedItem != null && (CanvasCollection[index].Resources["data"] as DERModel).Id == draggedItem.Id)
                {
                    draggedItem = null;
                    draggingSourceIndex = -1;
                    dragging = false;
                    return;
                }
            }



            if (draggedItem != null && !CanvasCollection[index].Resources.Contains("taken"))
            {
                SaveState();
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, draggedItem.Type.ImagePath);
                var logo = new BitmapImage(new Uri(filePath, UriKind.Relative));
                CanvasCollection[index].Background = new ImageBrush(logo);
                CanvasCollection[index].Resources.Add("taken", true);
                CanvasCollection[index].Resources.Add("data", draggedItem);
                AddedToGrid.Add(index, draggedItem);
                BorderBrushCollection[index] = draggedItem.State == DERState.NORMAL ? Brushes.Green : Brushes.Red;
                DERInfo[index] = draggedItem;

                if (draggingSourceIndex != -1)
                {
                    ResetCanvas(draggingSourceIndex.ToString());

                    List<int> connections = FindAllConnections(draggingSourceIndex);

                    if (connections.Count != 0)
                    {
                        foreach (int connectedTo in connections)
                        {
                            int source = Math.Min(draggingSourceIndex, connectedTo);
                            int destination = Math.Max(draggingSourceIndex, connectedTo);
                            DeleteLine(source, destination);

                            source = Math.Min(index, connectedTo);
                            destination = Math.Max(index, connectedTo);
                            Lines.Add($"{source},{destination}", CreateNewLine(source, destination));
                        }
                        DrawExistingLines();
                    }
                }

                RemoveFromGroup(draggedItem);
                draggingSourceIndex = -1;
                draggedItem = null;
                dragging = false;

            }
            else if (draggedItem != null && CanvasCollection[index].Resources.Contains("taken"))
            {
                if (IsLineAlreadyDrawn(draggingSourceIndex, index) == 0)
                {
                    SaveState();

                    int source = Math.Min(draggingSourceIndex, index);
                    int destination = Math.Max(draggingSourceIndex, index);
                    if (!Lines.ContainsKey($"{source},{destination}"))
                    {
                        Lines.Add($"{source},{destination}", CreateNewLine(source, destination));
                        DrawExistingLines();
                    }
                }

                draggingSourceIndex = -1;
                draggedItem = null;
                dragging = false;
            }
        }
        private void ResetCanvas(string indexString)
        {

            int index = int.Parse(indexString);

            if (!CanvasCollection[index].Resources.Contains("taken"))
            {
                ToastNotify.RaiseToast("Warning", "Cannot perform removal on the empty field", Notification.Wpf.NotificationType.Warning);
                return;

            }
            if (draggingSourceIndex == -1)
            {
                SaveState();

                List<int> connections = FindAllConnections(index);

                foreach (int connectedTo in connections)
                {
                    if (connectedTo > index)
                    {
                        DeleteLine(index, connectedTo);
                    }
                    else
                    {
                        DeleteLine(connectedTo, index);
                    }
                }
                DrawExistingLines();
            }

            DERModel removedDER = CanvasCollection[index].Resources["data"] as DERModel;
            AddedToGrid.Remove(index);
            AddToGroup(removedDER);

            CanvasCollection[index].Background = Brushes.Transparent;
            CanvasCollection[index].Resources.Remove("taken");
            CanvasCollection[index].Resources.Remove("data");

            BorderBrushCollection[index] = Brushes.Transparent;
            DERInfo[index] = null;

        }

        #endregion

        #region Adding/Removing From Categories
        private void AddToGroup(DERModel derModel)
        {
            foreach (DERGroup d in DERGroups)
            {
                if (d.Name.Equals(derModel.Type.Name))
                {
                    d.DERModels.Add(derModel);
                    break;
                }
            }
        }

        private static void RemoveFromGroup(DERModel derModel)
        {
            foreach (var group in DERGroups)
            {
                if (group.DERModels.Contains(derModel))
                {
                    group.DERModels.Remove(derModel);
                    break;
                }
            }
        }

        #endregion

        #region UI Updates
        public static void UpdateEntitiesOnCanvas()
        {
            if (CanvasCollection != null)
            {
                if (CanvasCollection.Count != 0)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (CanvasCollection[i].Resources.Contains("taken"))
                        {
                            if (AddedToGrid.TryGetValue(i, out DERModel derModel))
                            {
                                BorderBrushCollection[i] = derModel.State == DERState.NORMAL ? Brushes.Green : Brushes.Red;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Undo Logic

        public static void SaveState()
        {

            Dictionary<int, DERModel> entityState = new Dictionary<int, DERModel>();
            foreach (var entry in AddedToGrid)
            {
                entityState.Add(entry.Key, entry.Value);
            }
            Dictionary<string, Line> lineState = new Dictionary<string, Line>();
            foreach (var entry in Lines)
            {
                lineState.Add(entry.Key, entry.Value);
            }

            List<object> state = new List<object>() { entityState, lineState };
            //MainWindowViewModel.UndoStack.Push(
             //   new SaveState<CommandType, object>(CommandType.CanvasManipulation, state));

        }

        #endregion
    }
}
