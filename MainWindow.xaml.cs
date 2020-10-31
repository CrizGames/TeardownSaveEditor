using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TeardownSaveEditor.Scripts;

namespace TeardownSaveEditor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SaveFileManager save;


        public MainWindow()
        {
            InitializeComponent();

            save = SaveFileManager.Instance;

            Start();
        }

        private void Start()
        {
            cashText.SetText(save.Cash.ToString());

            // TODO: Read tools from game.lua
            ObservableCollection<SaveFileManager.Tool> tools = new ObservableCollection<SaveFileManager.Tool>
            {
                new SaveFileManager.Tool("Sledge (is always enabled)", "sledge"),
                new SaveFileManager.Tool("Spray can (is always enabled)", "spraycan"),
                new SaveFileManager.Tool("Fire extinguisher (is always enabled)", "extinguisher"),
                new SaveFileManager.Tool("Blowtorch", "blowtorch"),
                new SaveFileManager.Tool("Shotgun", "shotgun"),
                new SaveFileManager.Tool("Plank", "plank"),
                new SaveFileManager.Tool("Pipe bomb", "pipebomb"),
                new SaveFileManager.Tool("Gun", "gun"),
                new SaveFileManager.Tool("Bomb", "bomb"),
                new SaveFileManager.Tool("Rocket launcher", "launcher"),
            };

            toolsList.ItemsSource = tools;
        }

        private void Stop(object sender, System.ComponentModel.CancelEventArgs e)
        {
            save.Save();
        }

        private void MaxCashButton_Click(object sender, RoutedEventArgs e)
        {
            cashText.SetText(int.MaxValue.ToString());
            save.Cash = int.MaxValue;
        }

        private void CashText_OnTextEntered(object sender, string e)
        {
            save.Cash = int.Parse(e);
        }
    }
}
