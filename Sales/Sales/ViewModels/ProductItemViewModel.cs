namespace Sales.ViewModels
{
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Windows.Input;
    using Services;
    using Xamarin.Forms;
    using Helpers;
    using System.Linq;
    using Views;

    public class ProductItemViewModel:Product
    {
        #region Atributes
        private ApiService apiService;
        #endregion

        #region Constructor
        public ProductItemViewModel() {
            this.apiService=new ApiService();
        }
        #endregion

        #region Commands
        public ICommand EditProductCommand
        {
            get
            {
                return new RelayCommand(EditProduct);
            }
        }

        private async void EditProduct()
        {
            MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);
            await Application.Current.MainPage.Navigation.PushAsync(new EditProductPage());
        }

        public ICommand DeleteProductCommand {
            get
            {
                return new RelayCommand(DeleteProduct);
            }
        }

        private async void DeleteProduct()
        {
            var answer = await Application.Current.MainPage.DisplayAlert(Languages.Confirm, Languages.DeleteConfirmation,Languages.Yes,Languages.Not);
            if (!answer) {
                return;
            }
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Delete<Product>(url, prefix, controller, this.ProductId);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }
            var productViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productViewModel.Products.Where(p => p.ProductId == this.ProductId).FirstOrDefault();
            if (deleteProduct != null) {
                productViewModel.Products.Remove(deleteProduct);
            }
        }
        #endregion
    }
}
