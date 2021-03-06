﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class EventsEdit : Page
    {
        //create event object
        Event gEvent;

        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public EventsEdit()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get event from previous page
            gEvent = (Event)e.Parameter;

            //get all games
            List<Game> games = Json.Read("game.json");

            //populate listbox
            foreach (Game game in games)
                if (game.hidden == false)
                {
                    QuizCmb.Items.Add(game.title);
                    visibleGames.Add(game);
                }

            //populate info
            EventTxt.Text = gEvent.name;
            LocationTxt.Text = gEvent.location;
            StartDateDtp.SelectedDate = gEvent.startDate;
            EndDateDtp.SelectedDate = gEvent.endDate;
            foreach (Game game in visibleGames)
                if (game.id == gEvent.game)
                    QuizCmb.SelectedIndex = visibleGames.IndexOf(game);
            MemberOnlyChk.IsChecked = gEvent.memberOnly;
            trackGuestChk.IsChecked = gEvent.trackGuestNum;
            trackAdultChk.IsChecked = gEvent.trackAdultNum;
            trackChildChk.IsChecked = gEvent.trackChildNum;
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Questions));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            //get list of events
            List<Event> events = Json.Read("event.json");

            //validation
            if (EventTxt.Text == "")
            {
                EventNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EventTxt.Style = (Style)Application.Current.Resources["TxtValidationFailedTemplate"];
                await new MessageDialog("Please enter an event name").ShowAsync();
                return;
            }

            foreach (Event ev in events)
                //validate name
                if (ev.name.ToLower().Trim() == EventTxt.Text.ToLower().Trim() && ev.hidden == true)
                {
                    EventNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    EventTxt.Style = (Style)Application.Current.Resources["TxtValidationFailedTemplate"];
                    await new MessageDialog("That event already exists, please enter a different name").ShowAsync();
                    return;
                }

            //set object properties
            try
            {
                gEvent.name = EventTxt.Text;
                gEvent.location = LocationTxt.Text;
                gEvent.startDate = Convert.ToDateTime(StartDateDtp.SelectedDate.ToString());
                gEvent.endDate = Convert.ToDateTime(EndDateDtp.SelectedDate.ToString());
                gEvent.game = visibleGames[QuizCmb.SelectedIndex].id;
                gEvent.memberOnly = MemberOnlyChk.IsChecked ?? false;
                gEvent.trackGuestNum = trackGuestChk.IsChecked ?? false;
                gEvent.trackAdultNum = trackAdultChk.IsChecked ?? false;
                gEvent.trackChildNum = trackChildChk.IsChecked ?? false;
            }
            catch
            {
                StartDateDtp.Style = (Style)Application.Current.Resources["DateValidationFailedTemplate"];
                EndDateDtp.Style = (Style)Application.Current.Resources["DateValidationFailedTemplate"];
                EndDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                StartDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                await new MessageDialog("Please select a start and end date").ShowAsync();
                return;
            }
            //update json file
            Json.Edit(gEvent, "event.json");

            //navigate back to events
            Frame.Navigate(typeof(Events));
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }
    }
}
