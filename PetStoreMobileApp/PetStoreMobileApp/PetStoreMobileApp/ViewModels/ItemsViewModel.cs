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
        public ObservableCollection<Tag> PetTags { get; }
        public ObservableCollection<Category> PetCategories { get; }

        public ObservableCollection<Pet> Items { get; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Pet>();
            petStore = App.PetStore;
        }

        PetStore petStore;
        ObservableCollection<Tag> petTags;
        ObservableCollection<Category> petCategories;

        public async Task LoadItemsAsync()
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

    }
}