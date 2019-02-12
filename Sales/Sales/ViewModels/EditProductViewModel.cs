namespace Sales.ViewModels
{
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Helpers;
    using Sales.Services;
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class EditProductViewModel : BaseViewModels
    {
        #region Atributes
        private Product product;
        private bool isRunning;
        private bool isEnable;
        private ApiService apiService;
        private ImageSource imageSource;
        private MediaFile file;
        #endregion

        #region properties
        public Product Product
        {
            get { return this.product; }
            set { this.SetValue(ref this.product, value); }
        }
        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value); }
        }
        public bool IsEnable
        {
            get { return this.isEnable; }
            set { this.SetValue(ref this.isEnable, value); }
        }
        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set { this.SetValue(ref this.imageSource, value); }
        }
        #endregion

        #region Constructors
        public EditProductViewModel(Product product)
        {
            this.product = product;
            this.apiService = new ApiService();
            this.isEnable = true;
            this.ImageSource = product.ImageFullPath;
        }

        //public EditProductViewModel()
        //{
        //}
        #endregion

        #region Commands
        public ICommand DeleteCommand {
            get { return new RelayCommand(Delete); }
        }

        private async void Delete()
        {
            var answer = await Application.Current.MainPage.DisplayAlert(Languages.Confirm, Languages.DeleteConfirmation, Languages.Yes, Languages.Not);
            if (!answer)
            {
                return;
            }
            this.IsRunning = true;
            this.isEnable = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.isEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Delete<Product>(url, prefix, controller, this.product.ProductId, Settings.TokenType, Settings.AccessToken);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }
            var productViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productViewModel.MyProducts.Where(p => p.ProductId == this.product.ProductId).FirstOrDefault();
            if (deleteProduct != null)
            {
                productViewModel.MyProducts.Remove(deleteProduct);
            }
            productViewModel.RefreshList();
            this.IsRunning = false;
            this.isEnable = true;
            await App.Navigator.PopAsync();
        }

        public ICommand SaveCommand
        {
            get { return new RelayCommand(Save); }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Product.Description))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.DescriptionError, Languages.Accept);
                return;
            }
            if (this.Product.Price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.DescriptionError, Languages.Accept);
                return;
            }
            this.IsRunning = true;
            this.IsEnable = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.isRunning = false;
                this.IsEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
                this.Product.ImageArray = imageArray;
            }
            

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Put(url, prefix, controller, this.product,this.product.ProductId, Settings.TokenType, Settings.AccessToken);

            if (!response.IsSuccess)
            {
                this.isRunning = false;
                this.IsEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            var newProduct = (Product)response.Result;
            var productViewModel = ProductsViewModel.GetInstance();

            var oldProduct = productViewModel.MyProducts.Where(p => p.ProductId == this.product.ProductId).FirstOrDefault();
            if (oldProduct!=null)
            {
                productViewModel.MyProducts.Remove(oldProduct);
            }

            productViewModel.MyProducts.Add(newProduct);
            productViewModel.RefreshList();

            this.isRunning = false;
            this.IsEnable = true;
            await App.Navigator.PopAsync();
        }

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }

        private async void ChangeImage()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                var source = await Application.Current.MainPage.DisplayActionSheet(
                    Languages.ImageSource,
                    Languages.Cancel,
                    null,
                    Languages.FromGallery,
                    Languages.NewPicture);

                if (source == Languages.Cancel)
                {
                    this.file = null; return;
                }

                if (source == Languages.NewPicture)
                {
                    this.file = await CrossMedia.Current.TakePhotoAsync(
                        new StoreCameraMediaOptions
                        {
                            Directory = "Sample",
                            Name = "test.jpg",
                            PhotoSize = PhotoSize.Small,
                        }
                        );
                }
                else
                {
                    this.file = await CrossMedia.Current.PickPhotoAsync();
                }

                if (this.file != null)
                {
                    this.ImageSource = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        return stream;
                    });
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, e.ToString(), Languages.Accept);
                return;
            }
        }

        #endregion

    }
}
