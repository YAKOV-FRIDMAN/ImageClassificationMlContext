﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Diagnostics;

namespace ml_img
{
    class Camera : INotifyPropertyChanged
    {
        public event EventHandler UpBitmap;
        private string imagePath;
        private BitmapImage bitmapImage;
        public BitmapImage BitmapImage
        {
            get { return bitmapImage; }
            set
            {
                bitmapImage = value;
                PropertyChangedFunc(nameof(BitmapImage));
            }
        }
        public string ImagePath
        {
            get { return imagePath; }
            set
            {

                imagePath = value;
                PropertyChangedFunc(nameof(ImagePath));
            }
        }




        VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LoaclWebCamsCollection;
        System.Drawing.Image img;
        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var clone = new Bitmap(eventArgs.Frame))
                {
                    // img = clone;  
                    // Bitmap to System.Drawing.Image
                    img = (System.Drawing.Image)clone.Clone();

                    //  img = (Bitmap)eventArgs.Frame.Clone();

                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);

                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();

                    bi.Freeze();
                    Task.Run(() =>
                    {
                        BitmapImage = bi;
                        UpBitmap?.Invoke(this, new EventArgs());
                    });
                }
                //LocalWebCam.Stop();
            }
            catch (Exception ex)
            {
            }
        }


        public void Start()
        {
            LoaclWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            LocalWebCam = new VideoCaptureDevice(LoaclWebCamsCollection[0].MonikerString);
            Debug.WriteLine("------------------start------------------------");
            foreach (var item in LoaclWebCamsCollection)
            {
                Debug.WriteLine(item);
            }
            Debug.WriteLine(LoaclWebCamsCollection[0].MonikerString);
            // Debug.WriteLine(LoaclWebCamsCollection[1].MonikerString);
            Debug.WriteLine("------------------stop------------------------");
            LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

            LocalWebCam.Start();

        }

        public void SaveImageInStop()
        {
            lock (img)
            {
                // save the image here
                LocalWebCam.SignalToStop();
                Task.Delay(1000);
                string fileName = System.IO.Path.GetTempFileName();
                System.IO.File.Create(fileName).Close();
                using (var clone = new Bitmap(img))
                {
                    clone.Save(fileName, ImageFormat.Png);
                }

                // img.Save(fileName, ImageFormat.Png);
                ImagePath = fileName;
               /// LocalWebCam.SignalToStop();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void PropertyChangedFunc(string propNmae)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propNmae));
        }

    }
}
