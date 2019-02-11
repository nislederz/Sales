namespace Sales.ViewModels
{
    using System;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Common.Models;
    using Services;
    using Xamarin.Forms;

    public class AddProductViewModel:BaseViewModels
    {
        #region Atributes
        private bool isRunning;
        private bool isEnable;
        private ApiService apiService;
        private ImageSource imageSource;
        private MediaFile file;
        #endregion

        #region Properties
        public string Description { get; set; }
        public string Price { get; set; }
        public string Remarks { get; set; }
        public bool IsRunning {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value); }
        }
        public bool IsEnable
        {
            get { return this.isEnable; }
            set { this.SetValue(ref this.isEnable, value); }
        }
        public ImageSource ImageSource {
            get { return this.imageSource; }
            set { this.SetValue(ref this.imageSource, value); }
        }

        #endregion

        #region Constructor
        public AddProductViewModel() {
            this.apiService = new ApiService();
            this.isEnable = true;
            this.ImageSource = "noproduct";
        }
        #endregion

        #region Commands
        public ICommand SaveCommand {
            get { return new RelayCommand(Save); }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Description)) {
                await Application.Current.MainPage.DisplayAlert(Languages.Error,Languages.DescriptionError,Languages.Accept);
                return;
            }
            if (string.IsNullOrEmpty(this.Price))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.DescriptionError, Languages.Accept);
                return;
            }
            var price = decimal.Parse(this.Price);
            if (price<0)
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
            }

            var product = new Product
            {
                Description = this.Description,
                Price = price,
                Remarks=this.Remarks,
                ImageArray = imageArray,
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Post(url, prefix, controller, product);

            if (!response.IsSuccess) {
                this.isRunning = false;
                this.IsEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            var newProduct = (Product)response.Result;
            var productViewModel = ProductsViewModel.GetInstance();
            productViewModel.MyProducts.Add(newProduct);
            productViewModel.RefreshList();

            this.isRunning = false;
            this.IsEnable = true;
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public ICommand ChangeImageCommand {
            get {
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
