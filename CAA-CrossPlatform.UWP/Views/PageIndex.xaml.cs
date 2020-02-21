﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CAA_CrossPlatform.UWP.Models;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageIndex
    {
        public PageIndex()
        {
            this.InitializeComponent();
        }

        //setup api
        static ApiHandler api = new ApiHandler();

        private void txtPassword_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                btnLogin_Click(sender, e);
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //login
            string res = await api.Login(txtUsername.Text, txtPassword.Password);

            //show error message
            if (!res.Contains("Welcome"))
            {
                await new MessageDialog(res).ShowAsync();
                return;
            }

            //set active username
            Environment.SetEnvironmentVariable("activeUser", txtUsername.Text);

            //redirect to events
            Frame.Navigate(typeof(PageEvent));
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            //register
            string res = await api.Register(txtUsername.Text, txtPassword.Password);

            //show error message
            if (!res.Contains("Welcome"))
            {
                await new MessageDialog(res).ShowAsync();
                return;
            }

            //set active username
            Environment.SetEnvironmentVariable("activeUser", txtUsername.Text);

            //redirect to events
            Frame.Navigate(typeof(PageEvent));
        }
    }
}