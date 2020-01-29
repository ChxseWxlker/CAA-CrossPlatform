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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class EventExcel
    {
        List<Event> visibleEvents = new List<Event>();

        public EventExcel()
        {
            this.InitializeComponent();
            this.Loaded += EventExcel_Loaded;
        }

        private void EventExcel_Loaded(object sender, RoutedEventArgs e)
        {
            //get all events
            List<Event> events = Json.Read("event.json");

            //create list of visible events
            foreach (Event ev in events)
                if (ev.hidden == false)
                {
                    lstEvents.Items.Add(ev.name);
                    visibleEvents.Add(ev);
                }
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Questions));
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            List<Event> events = await Excel.Load();
            string eventsStr = "";
            foreach (Event ev in events)
            {
                eventsStr += $"{ev.id} {ev.name} {ev.location} {ev.startDate} {ev.endDate} {ev.game}\n";
            }

            await new MessageDialog(eventsStr).ShowAsync();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //validation
            if (lstEvents.SelectedItems.Count == 0)
            {
                await new MessageDialog("No events selected, please choose one").ShowAsync();
                return;
            }

            //create list of selected events
            List<Event> selectedEvents = new List<Event>();

            //add to events
            foreach (string evStr in lstEvents.SelectedItems)
                foreach (Event ev in visibleEvents)
                    if (evStr == ev.name)
                        selectedEvents.Add(ev);

            //save to excel spreadsheet
            Excel.Save(selectedEvents);
        }

        private void chkAllEvents_Checked(object sender, RoutedEventArgs e)
        {
            lstEvents.SelectAll();
        }

        private void chkAllEvents_Unchecked(object sender, RoutedEventArgs e)
        {
            lstEvents.SelectedIndex = -1;
        }
    }
}