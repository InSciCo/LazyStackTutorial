using PetStoreMobileApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace PetStoreMobileApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}