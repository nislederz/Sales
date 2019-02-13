namespace Sales.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Services;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModels
    {
        #region attributes
        private string filter;
        private ApiService apiService;
        private DataService dataservice;
        private bool isRefreshing;
        private ObservableCollection<ProductItemViewModel> products;
        #endregion

        #region properties
        public List<Product> MyProducts {get;set;}
        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { this.SetValue(ref this.isRefreshing, value); }
        }
        public ObservableCollection<ProductItemViewModel> Products
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value); }
        }
        public string Filter
        {
            get { return this.filter; }
            set
            {
                this.filter = value;
                this.RefreshList();
                this.IsRefreshing = false;
            }
        }
        #endregion

        #region constructor
        public ProductsViewModel()
        {
            instance = this;
            this.apiService = new ApiService();
            this.dataservice = new DataService();
            this.LoadProducts();
        }
        #endregion

        #region Singleton
        private static ProductsViewModel instance;
        public static ProductsViewModel GetInstance() {

            if (instance==null)
            {
                return new ProductsViewModel();
            }
            return instance;
        }
        #endregion

        #region Metodos
        private async void LoadProducts()
        {
            this.isRefreshing = true;
            var connection = await this.apiService.CheckConnection();

            if (connection.IsSuccess)
            {
                var answer = await this.LoadProductsFromAPI();
                if (answer)
                {
                    this.SaveProductstoDB();
                }
            }
            else
            {
                await this.LoadProductsFromDB();
            }
            if (MyProducts==null || this.MyProducts.Count==0)
            {
                this.isRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.NoProductsMessage, Languages.Accept);
                return;
            }
            
            this.RefreshList();
            this.IsRefreshing = false;
        }

        private async Task LoadProductsFromDB()
        {
            this.MyProducts = await this.dataservice.GetAllProducts();
        }

        private async Task SaveProductstoDB()
        {
            await this.dataservice.DeleteAllProducts();
            dataservice.Insert(this.MyProducts);
        }

        private async Task<bool> LoadProductsFromAPI()
        {
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.GetList<Product>(url, prefix, controller, Settings.TokenType, Settings.AccessToken);
            if (!response.IsSuccess)
            {
                return false;
            }
            this.MyProducts = (List<Product>)response.Result;
            return true;
        }

        public void RefreshList()
        {
            if (string.IsNullOrEmpty(this.Filter))
            {
                var myListProductItemViewModel = MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,
                });
                this.Products = new ObservableCollection<ProductItemViewModel>(myListProductItemViewModel.OrderBy(p => p.Description));
            }
            else
            {
                var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,
                }).Where(p=>p.Description.ToLower().Contains(this.Filter.ToLower())).ToList();
                this.Products = new ObservableCollection<ProductItemViewModel>(myListProductItemViewModel.OrderBy(p => p.Description));
            }
                 
        }


        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }
        }
        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(RefreshList);
            }
        }
        #endregion



    }

}
