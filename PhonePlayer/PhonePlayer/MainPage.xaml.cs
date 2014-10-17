using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using PhonePlayer.Resources;

namespace PhonePlayer
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
//            BuildApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
/*        private void BuildApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonText;
            ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        } */

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PageNowPlaying.xaml?select=new", UriKind.Relative));
        }

        private void ButtonRandom_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PageNowPlaying.xaml?select=random", UriKind.Relative));
        }

        private void ButtonElectronic_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PageNowPlaying.xaml?genre=electronic", UriKind.Relative));
        }
    }
}