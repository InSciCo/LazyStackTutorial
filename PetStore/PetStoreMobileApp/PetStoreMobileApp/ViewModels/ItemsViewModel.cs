using PetStoreMobileApp.Models;
using PetStoreMobileApp.Views;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

using PetStoreClientSDK;
using PetStoreSchema.Models;

namespace PetStoreMobileApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Pet _selectedItem;
        public ObservableCollection<Tag> PetTags { get; }
        public ObservableCollection<Category> PetCategories { get; }

        public ObservableCollection<Pet> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Pet> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Pet>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Pet>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            petStore = App.PetStore;

        }

        PetStore petStore;
        ObservableCollection<Tag> petTags;
        ObservableCollection<Category> petCategories;

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (petTags == null)
                    petTags = new ObservableCollection<Tag>(await petStore.GetPetTagsAsync());

                if (petCategories == null)
                    petCategories = new ObservableCollection<Category>(await petStore.GetPetCategoriesAsync());

                Items.Clear();
                var statuses = Enum.GetValues(typeof(PetStatus)).Cast<PetStatus>().ToList();

                var items = await petStore.FindPetsByStatusAsync(new List<PetStatus> { PetStatus.Available });
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Pet SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Pet item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}