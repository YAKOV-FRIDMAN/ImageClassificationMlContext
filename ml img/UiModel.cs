using DwImgApiML.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ml_img
{
    class UiModel : INotifyPropertyChanged
    {

        #region filds
        private bool _isVisibilityCamera;
        Camera camera = new Camera();
        private string _imageSorcr;
        private string prediction;
        private double progMan;
        private double progWoman;
        private bool _isVisibility;

        private BitmapImage bitmapImage;
        #endregion



        #region prop
        private bool _isVisibilityImage;

        public bool IsVisibilityImage
        {
            get { return _isVisibilityImage; }
            set
            {
                _isVisibilityImage = value;
                PropertyChangedFunc(nameof(IsVisibilityImage));

            }
        }

        public RelayCommand OpenImg { get; set; }

        public RelayCommand PhotoTaking { get; set; }

        public RelayCommand SetTrueFeedback { get; set; }
        public RelayCommand SetFalseFeedback { get; set; }
        public BitmapImage BitmapImage
        {
            get { return bitmapImage; }
            set
            {
                bitmapImage = value;
                PropertyChangedFunc(nameof(BitmapImage));
            }
        }

        public bool IsVisibilityCamera
        {
            get { return _isVisibilityCamera; }
            set
            {
                _isVisibilityCamera = value;
                PropertyChangedFunc(nameof(IsVisibilityCamera));
            }
        }

        public bool IsVisibility
        {
            get { return _isVisibility; }
            set
            {
                _isVisibility = value;
                PropertyChangedFunc(nameof(IsVisibility));
            }
        }
        private bool isVisibilityFeedback;

        public bool IsVisibilityFeedback
        {
            get { return isVisibilityFeedback; }
            set { isVisibilityFeedback = value; PropertyChangedFunc(nameof(IsVisibilityFeedback)); }
        }

        public string ImageSorc
        {
            get { return _imageSorcr; }
            set
            {
                _imageSorcr = value;
                PropertyChangedFunc(nameof(ImageSorc));
            }
        }

        public string Prediction
        {
            get => prediction;
            set
            {
                prediction = value;
                PropertyChangedFunc(nameof(Prediction));
            }
        }
        public double ProgMan
        {
            get => progMan;
            set
            {
                progMan = value;
                PropertyChangedFunc(nameof(ProgMan));
            }
        }
        public double ProgWoman
        {
            get => progWoman;
            set
            {
                progWoman = value;
                PropertyChangedFunc(nameof(ProgWoman));
            }
        }
        #endregion

        public UiModel()
        {
            OpenImg = new RelayCommand(openImage);
            PhotoTaking = new RelayCommand(PhotoTakingFunc);
            SetTrueFeedback = new RelayCommand(OnSetTrueFeedback);
            SetFalseFeedback = new RelayCommand(OnSetFalseFeedback);
            IsVisibilityImage = true;
        }

        private void OnSetFalseFeedback(object obj)
        {
            IsVisibility = true;
            ConsumeModel.SetFeedback(new ModelInput { ImageSource = ImageSorc, Label = Prediction == "man" ? "woman" : "man" });
            IsVisibility = false;
        }

        private void OnSetTrueFeedback(object obj)
        {
            IsVisibility = true;
            ConsumeModel.SetFeedback(new ModelInput { ImageSource = ImageSorc, Label = Prediction == "man" ? "man" : "woman" });
            IsVisibility = false;
        }

        private void PhotoTakingFunc(object obj)
        {
            IsVisibilityFeedback = false;
            if (IsVisibilityCamera == false)
            {
                ImageSorc = null;
                IsVisibilityCamera = true;
                IsVisibilityImage = false;
                BitmapImage = camera.BitmapImage;
                camera.UpBitmap += (o, e) =>
                {

                    BitmapImage = camera.BitmapImage;

                };
                camera.Start();
            }
            else
            {
                IsVisibility = true;
                camera.SaveImageInStop();
                IsVisibilityImage = true;
                IsVisibilityCamera = false;
                ImageSorc = camera.ImagePath;
                Task.Run(() =>
                {
                    ModelInput sampleData = new ModelInput()
                    {
                        ImageSource = ImageSorc,
                    };
                    var predictionResult = ConsumeModel.Predict(sampleData);
                    Prediction = predictionResult.Prediction;
                    ProgMan = predictionResult.Score[0];
                    ProgWoman = predictionResult.Score[1];
                    IsVisibility = false;
                    IsVisibilityFeedback = true;
                });
            }
        }

        private void openImage(object obj)
        {
            IsVisibilityFeedback = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Task.Run(() =>
                {
                    IsVisibility = true;
                    ImageSorc = openFileDialog.FileName;
                });

                Task.Run(() =>
                {
                    ModelInput sampleData = new ModelInput()
                    {
                        ImageSource = openFileDialog.FileName,
                    };
                    var predictionResult = ConsumeModel.Predict(sampleData);
                    Prediction = predictionResult.Prediction;
                    ProgMan = predictionResult.Score[0];
                    ProgWoman = predictionResult.Score[1];
                    IsVisibility = false;
                    IsVisibilityFeedback = true;
                });

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void PropertyChangedFunc(string propNmae)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propNmae));
        }
    }
}
