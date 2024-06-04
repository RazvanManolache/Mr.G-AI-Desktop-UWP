using MrG.AI.Client.Database;
using MrG.AI.Client.Database.Data;
using MrG.AI.Client.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MrG.Desktop
{
    public sealed partial class RequestList : UserControl
    {
        RequestListVM viewModel;
        public RequestList()
        {
            this.InitializeComponent();
            viewModel = this.DataContext as RequestListVM;
            this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
            LoadMoreClicked(null, null);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.SortOrder) || e.PropertyName == nameof(viewModel.SearchText))
            {
                viewModel.Requests.Clear();
                LoadMoreClicked(null, null);
            }
        }

      
        private async void LoadMoreClicked(object sender, EventArgs e)
        {
            var cnt = viewModel.Requests.Count;
            var result = await DatabaseHelper.Instance.GetAll<Request>(a=>a.ActionName.Contains(viewModel.SearchText), cnt, cnt + 10, viewModel.SortOrder == "Desc");
            foreach (var item in result.Item1)
                viewModel.Requests.Add(item);
            if (result.Item2 > viewModel.Requests.Count)
                viewModel.LoadMoreVisible = true;
            else
                viewModel.LoadMoreVisible = false;

        }

        private void OnPressedItem(object sender, EventArgs e)
        {
            //var obj = (RequestDetailsList)sender;


            //var item = (Request)obj.BindingContext;

            //Shell.Current.GoToAsync("/request_details", new ShellNavigationQueryParameters()
            //{
            //    { nameof(Request), item }
            //});

        }
    }
}
