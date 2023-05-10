using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOfLife
{
    /* What was changed:
     1.Instead of using switch-case statements, the ads are now stored in a List<AdInfo>. The LoadAdInfoList method creates the list
         and populates it with ad images and links.    
     2. The OnClick method has been updated to use the Link property of the current ad.
     3. The ChangeAds method has been updated to use the Image property of the current ad and update the imgNmb value.
     4. A new AdInfo class has been added to store the Image and Link properties for each ad.
    */
    class AdWindow : Window
    {
        private readonly DispatcherTimer adTimer;
        private int imgNmb; // the number of the image currently shown
        private List<AdInfo> adInfoList;
        Random rnd = new Random();

        public AdWindow(Window owner)
        {
            Owner = owner;
            Width = 350;
            Height = 100;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
            Title = "Support us by clicking the ads";
            Cursor = Cursors.Hand;
            ShowActivated = false;
            MouseDown += OnClick;

            imgNmb = rnd.Next(1, 3);
            adInfoList = LoadAdInfoList();

            ChangeAds(this, new EventArgs());

            // Run the timer that changes the ad's image 
            adTimer = new DispatcherTimer();
            adTimer.Interval = TimeSpan.FromSeconds(3);
            adTimer.Tick += ChangeAds;
            adTimer.Start();
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(adInfoList[imgNmb - 1].Link);
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            Unsubscribe();
            adTimer.Stop();
            adTimer.Tick -= ChangeAds;
            base.OnClosed(e);
        }

        public void Unsubscribe()
        {
            adTimer.Tick -= ChangeAds;
        }

        private List<AdInfo> LoadAdInfoList()
        {
            List<AdInfo> adInfoList = new List<AdInfo>();
            for (int i = 0; i < 3; i++)
            {
                var adImage = new WriteableBitmap(new BitmapImage(new Uri($"ad{i + 1}.jpg", UriKind.Relative)));
                adInfoList.Add(new AdInfo { Image = adImage, Link = "http://example.com" });
            }
            return adInfoList;
        }

        private void ChangeAds(object sender, EventArgs eventArgs)
        {
            ImageBrush myBrush = new ImageBrush
            {
                ImageSource = adInfoList[imgNmb - 1].Image
            };
            Background = myBrush;
            imgNmb = imgNmb % 3 + 1;
        }

        private class AdInfo
        {
            public WriteableBitmap Image { get; set; }
            public string Link { get; set; }
        }
    }
}