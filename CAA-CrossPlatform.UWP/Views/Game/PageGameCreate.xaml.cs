﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public sealed partial class PageGameCreate : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //list of questions
        static List<Question> visibleQuestions = new List<Question>();

        public PageGameCreate()
        {
            this.InitializeComponent();
            this.Loaded += PageGameCreate_Loaded;
        }

        private async void PageGameCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get question list
            List<Question> questions = await Connection.Get("Question");
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lstQuestions.Items.Add(q.name);
                    visibleQuestions.Add(q);
                }
        }

        static dynamic selections = null;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            selections = e.Parameter;

            base.OnNavigatedTo(e);
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEvent));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGame));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestion));
        }

        private async void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = await Connection.Get("Game");

            //validation
            if (QuizTxt.Text == "")
            {
                QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                QuizTxt.Style = (Style)Application.Current.Resources["TxtValidationFailedTemplate"];
                await new MessageDialog("Please enter a quiz name").ShowAsync();
                return;
            }

            foreach (Game g in games)
            {
                //validate name
                if (g.name.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == false)
                {
                    QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    QuizTxt.Style = (Style)Application.Current.Resources["TxtValidationFailedTemplate"];
                    await new MessageDialog("That quiz already exists, please enter a different name").ShowAsync();
                    return;
                }

                //unhide game if user chooses
                else if (g.name.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That quiz is hidden, would you like to re-activate it?");
                    msg.Commands.Add(new UICommand("Yes") { Id = 1 });
                    msg.Commands.Add(new UICommand("No") { Id = 0 });
                    msg.CancelCommandIndex = 0;
                    var choice = await msg.ShowAsync();

                    //re-activate game
                    if ((int)choice.Id == 1)
                    {
                        g.hidden = false;
                        Connection.Update(g);
                        Frame.Navigate(Frame.BackStack.Last().SourcePageType, selections);
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //setup game object
            Game game = new Game();
            game.name = QuizTxt.Text;

            //save to database
            game.Id = await Connection.Insert(game);

            //create questions
            foreach (Question q in visibleQuestions)
            {
                //question is selected
                if (lstQuestions.SelectedItems.Contains(q.name))
                {
                    //create game question
                    GameQuestion gq = new GameQuestion();
                    gq.GameID = game.Id;
                    gq.QuestionID = q.Id;

                    //save to database
                    gq.Id = await Connection.Insert(gq);
                }
            }

            //navigate back
            Frame.Navigate(Frame.BackStack.Last().SourcePageType, selections);
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(Frame.BackStack.Last().SourcePageType, selections);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            lstQuestions.Items.Clear();
            foreach (Question q in visibleQuestions)
                if (q.name.ToLower().Trim().Contains(TxtSearch.Text.ToLower().Trim()))
                    lstQuestions.Items.Add(q.name);
        }

        private void TxtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(TxtSearch.Text == "Search")
                TxtSearch.Text = "";
        }

        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //prompt user
            ContentDialog logoutDialog = new ContentDialog
            {
                Title = "Logout?",
                Content = "You will be redirected to the home page and locked out until you log back in. Are you sure you want to logout?",
                PrimaryButtonText = "Logout",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult logoutRes = await logoutDialog.ShowAsync();
        }

        private void btnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //get menu button
            Button btn = (Button)sender;

            //event
            if (btn.Content.ToString().Contains("Event"))
                Frame.Navigate(typeof(PageEvent));

            //game
            else if (btn.Content.ToString().Contains("Game"))
                Frame.Navigate(typeof(PageGame));

            //question
            else if (btn.Content.ToString().Contains("Question"))
                Frame.Navigate(typeof(PageQuestion));
        }
    }
}
