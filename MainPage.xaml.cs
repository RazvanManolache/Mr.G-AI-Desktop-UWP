using Microsoft.UI.Xaml.Controls;
using MrG.AI.Client.Data;
using MrG.AI.Client.Data.Action;
using MrG.AI.Client.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MrG.Desktop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel
        {
            get
            {
                return this.DataContext as MainPageViewModel;
            }
        }
        public MainPage()
        {
            this.InitializeComponent(); 
            this._viewModel.PropertyChanged += _viewModel_PropertyChanged;
            SocketHelper.OnConnectedEvent += OnServerConnected;
            SocketHelper.OnDisconnectedEvent += OnServerDisonnected;
            SocketHelper.OnActionsUpdated += OnActionsUpdated;
            UpdateView();
        }

        private async void OnActionsUpdated(List<AI.Client.Data.Action.ActionApi> obj)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _viewModel.Actions.Clear();
                foreach (var item in obj)
                {
                    _viewModel.Actions.Add(item);
                }
            });

            
        }

        private async void OnServerDisonnected(Server server)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ConnectionStatusLabel.Text = "Disconnected";

            });


        }

        private async void OnServerConnected(Server server)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ConnectionStatusLabel.Text = "Connected";

            });
        }

        private void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(_viewModel.Server))
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            Server.Instance = this._viewModel.Server;
            if (this._viewModel.Server != null)
            {   
                MainTabPanel.SelectedItem = ActionsTab;
            }
            else
            {
                MainTabPanel.SelectedItem = SetupTab;
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            var testUrl = Server.GetTestUrl(_viewModel.IP);
            var res = await Server.MakeHttpRequest(testUrl);
            _viewModel.IPChecked = !string.IsNullOrWhiteSpace(res);
         
        }

        private void ConnectButton_Click(Object sender, RoutedEventArgs e)
        {
            _viewModel.Server = new Server(AI.Client.Enum.ServerType.Local, "Local", _viewModel.IP);
        }

        private void LoginButton_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void LogoutButton_Click(Object sender, RoutedEventArgs e)
        {
            _viewModel.Server = null;
        }

        
        Dictionary<ActionParameter, FrameworkElement> SelectedActionFields = new Dictionary<ActionParameter, FrameworkElement>();

       

        private void ActionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var action =  ((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem as ActionApi;
            ImplementAction(action);


        }
        

        private void ImplementAction(ActionApi action)
        {
            ActionFormGrid.Children.Clear();
            SelectedActionFields.Clear();
            ActionFormGrid.RowDefinitions.Clear();

            _viewModel.SelectedAction = action;
            var i = 0;

            var nameBlock = new TextBlock();
            ActionFormGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            nameBlock.Text = action.Name;
            nameBlock.HorizontalAlignment = HorizontalAlignment.Center;
            nameBlock.VerticalAlignment = VerticalAlignment.Center;
            nameBlock.FontSize = 24;
            ActionFormGrid.Children.Add(nameBlock);
            Grid.SetRow(nameBlock, i);
            Grid.SetColumn(nameBlock, 0);
            Grid.SetColumnSpan(nameBlock, 2);
            i++;

            if (!string.IsNullOrWhiteSpace(action.Description))
            {
                ActionFormGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var desc = new TextBlock();
                desc.Text = action.Description;
                desc.HorizontalAlignment = HorizontalAlignment.Left;
                desc.VerticalAlignment = VerticalAlignment.Center;
                ActionFormGrid.Children.Add(desc);
                Grid.SetRow(desc, i);
                Grid.SetColumn(desc, 0);
                Grid.SetColumnSpan(desc, 2);
                i++;
            }

            foreach (var param in action.Parameters)
            {
                ActionFormGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var label = new TextBlock();
                label.Text = param.Name;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Center;
                ActionFormGrid.Children.Add(label);
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);

                FrameworkElement field = null;
                switch (param.Type)
                {
                    case "STRING":
                        var tb = new TextBox();
                        tb.Text = param.DefaultValue;
                        field = tb;
                        break;

                    case "INT":
                        var nb = new NumberBox();
                        nb.Value = int.Parse(param.DefaultValue);
                        field = nb;
                        break;


                    case "FLOAT":
                        var fb = new NumberBox();
                        fb.Value = double.Parse(param.DefaultValue);
                        field = fb;
                        break;
                    case "BOOLEAN":
                        var cb = new CheckBox();
                        cb.IsChecked = bool.Parse(param.DefaultValue);
                        field = cb;
                        break;
                    case "SELECT":
                        var cb2 = new ComboBox();
                        foreach (var val in param.Collection.Values)
                        {
                            cb2.Items.Add(val.Value);
                        }
                        cb2.SelectedValue = param.DefaultValue;
                        field = cb2;
                        break;

                    case "IMAGE":
                        var grid = new Grid();
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        var ib = new Button();
                        ib.HorizontalAlignment = HorizontalAlignment.Stretch;
                        ib.Margin = new Thickness(5);
                        grid.Children.Add(ib);
                        Grid.SetRow(ib, 0);
                        Grid.SetColumn(ib, 0);
                        ib.Content = "Select Image";
                        if (!string.IsNullOrWhiteSpace(param.DefaultValue))
                        {
                            try
                            {
                                var bytes = Convert.FromBase64String(param.DefaultValue);
                                var stream = new MemoryStream(bytes);
                                var bitmap = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                                bitmap.SetSource(stream.AsRandomAccessStream());
                                var img = new Image();
                                img.Source = bitmap;
                                grid.Children.Add(img);
                                Grid.SetRow(img, 1);
                                Grid.SetColumn(img, 0);
                            }
                            catch(Exception ex)
                            {
                                // do nothing
                            }
                        }
                        ib.Click += async (s, ev) =>
                        {
                            var picker = new Windows.Storage.Pickers.FileOpenPicker();
                            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                            picker.FileTypeFilter.Add(".png");
                            picker.FileTypeFilter.Add(".jpg");
                            picker.FileTypeFilter.Add(".jpeg");
                            param.Value = param.DefaultValue;
                           
                            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

                            if (file != null)
                            {
                                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                                var bitmap = new Windows.UI.Xaml.Media.Imaging.BitmapImage();

                                param.Value = Convert.ToBase64String(Encoding.ASCII.GetBytes(stream.ToString()));
                                bitmap.SetSource(stream);

                                grid.Children.Where(a => a is Image).ToList().ForEach(a => grid.Children.Remove(a));

                                var img = new Image();
                                img.MaxHeight = bitmap.PixelHeight;
                                img.MaxWidth = bitmap.PixelWidth;
                                img.Source = bitmap;
                                grid.Children.Add(img);
                                Grid.SetRow(img, 1);
                                Grid.SetColumn(img, 0);
                            }
                        };
                        field = grid;
                        break;

                    default:
                        var dtb = new TextBox();
                        dtb.Text = param.Type;
                        field = dtb;
                        break;

                }


                field.Margin = new Thickness(10);
                field.HorizontalAlignment = HorizontalAlignment.Stretch;
                field.VerticalAlignment = VerticalAlignment.Center;

                ActionFormGrid.Children.Add(field);
                SelectedActionFields.Add(param, field);
                Grid.SetRow(field, i);
                Grid.SetColumn(field, 1);
                i++;

            }
        }

        private void ExecuteAction(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedAction == null) return;
            var values = new Dictionary<string, object>();
            foreach (var field in SelectedActionFields)
            {
                switch (field.Key.Type)
                {
                    case "STRING":
                        values.Add(field.Key.Name, ((TextBox)field.Value).Text);
                        break;
                    case "INT":
                        values.Add(field.Key.Name, ((NumberBox)field.Value).Value);
                        break;
                    case "FLOAT":
                        values.Add(field.Key.Name, ((NumberBox)field.Value).Value);
                        break;
                    case "BOOLEAN":
                        values.Add(field.Key.Name, ((CheckBox)field.Value).IsChecked);
                        break;
                    case "SELECT":
                        values.Add(field.Key.Name, ((ComboBox)field.Value).SelectedValue);
                        break;
                    case "IMAGE":
                        values.Add(field.Key.Name, field.Key.Value);
                        break;
                    default:
                        values.Add(field.Key.Name, ((TextBox)field.Value).Text);
                        break;
                }
            }
            SocketHelper.ExecuteApi(_viewModel.SelectedAction, values);
        }

        private void ResetAction(object sender, RoutedEventArgs e)
        {
            ImplementAction(_viewModel.SelectedAction);
        }
    }
}
