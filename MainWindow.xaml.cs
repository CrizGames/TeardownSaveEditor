using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TeardownSaveEditor.Controls;
using TeardownSaveEditor.Scripts;

namespace TeardownSaveEditor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SaveFileManager save;

        // TODO: Get tools from game.lua
        private ObservableCollection<SaveFileManager.Tool> tools = new ObservableCollection<SaveFileManager.Tool>();

        // TODO: Get mission from missions.lua
        private ObservableCollection<SaveFileManager.Mission> missions = new ObservableCollection<SaveFileManager.Mission>();

        // TODO: Get ranks from game.lua
        private ObservableCollection<SaveFileManager.Rank> ranks = new ObservableCollection<SaveFileManager.Rank>();


        public MainWindow()
        {
            InitializeComponent();

            save = SaveFileManager.Instance;

            if (save.fileLoaded)
                Start();
        }

        private void Start()
        {
            cashText.SetText(save.Cash.ToString());

            InitTools();

            InitMissions();

            InitRanks();
        }

        private void InitTools()
        {
            tools.Add(new SaveFileManager.Tool("Sledge (is always enabled)", "sledge"));
            tools.Add(new SaveFileManager.Tool("Spray can (is always enabled)", "spraycan"));
            tools.Add(new SaveFileManager.Tool("Fire extinguisher (is always enabled)", "extinguisher"));
            tools.Add(new SaveFileManager.Tool("Blowtorch", "blowtorch"));
            tools.Add(new SaveFileManager.Tool("Shotgun", "shotgun"));
            tools.Add(new SaveFileManager.Tool("Plank", "plank"));
            tools.Add(new SaveFileManager.Tool("Pipe bomb", "pipebomb"));
            tools.Add(new SaveFileManager.Tool("Gun", "gun"));
            tools.Add(new SaveFileManager.Tool("Bomb", "bomb"));
            tools.Add(new SaveFileManager.Tool("Rocket launcher", "launcher"));

            if (tools.All(x => x.enabled))
                toolsAllCheckBox.IsChecked = true;

            toolsList.ItemsSource = tools;
        }

        private void InitMissions()
        {
            // TODO: Order list chronogical or categorical
            missions.Add(new SaveFileManager.Mission(01, "Evertides Mall - The old building problem", "mall_intro"));
            missions.Add(new SaveFileManager.Mission(02, "Lee Chemicals - The Lee Computers", "lee_computers"));
            missions.Add(new SaveFileManager.Mission(03, "Lee Chemicals - The login devices", "lee_login"));
            missions.Add(new SaveFileManager.Mission(06, "Lee Chemicals - Heavy lifting", "lee_safe"));
            missions.Add(new SaveFileManager.Mission(07, "Lee Chemicals - The tower", "lee_tower"));
            missions.Add(new SaveFileManager.Mission(13, "Lee Chemicals - Power outage", "lee_powerplant"));
            missions.Add(new SaveFileManager.Mission(90, "Lee Chemicals - Flooding", "lee_flooding"));
            missions.Add(new SaveFileManager.Mission(04, "West Point Marina - Making space", "marina_demolish"));
            missions.Add(new SaveFileManager.Mission(05, "West Point Marina - The GPS devices", "marina_gps"));
            missions.Add(new SaveFileManager.Mission(05, "West Point Marina - Classic cars", "marina_cars"));
            missions.Add(new SaveFileManager.Mission(91, "West Point Marina - Tool up", "marina_tools"));
            missions.Add(new SaveFileManager.Mission(09, "West Point Marina - Art return", "marina_art_back"));
            missions.Add(new SaveFileManager.Mission(06, "Villa Gordon - The car wash", "mansion_pool"));
            missions.Add(new SaveFileManager.Mission(08, "Villa Gordon - Fine arts", "mansion_art"));
            missions.Add(new SaveFileManager.Mission(10, "Villa Gordon - Insurance fraud", "mansion_fraud"));
            missions.Add(new SaveFileManager.Mission(12, "Villa Gordon - A wet affair", "mansion_safe"));
            missions.Add(new SaveFileManager.Mission(92, "Villa Gordon - The speed deal", "mansion_race"));
            missions.Add(new SaveFileManager.Mission(11, "Hollowrock Island - The BlueTide Computers", "caveisland_computers"));
            missions.Add(new SaveFileManager.Mission(14, "Hollowrock Island - Motivational reminder", "caveisland_propane"));
            missions.Add(new SaveFileManager.Mission(15, "Hollowrock Island - An assortment of dishes", "caveisland_dishes"));
            missions.Add(new SaveFileManager.Mission(16, "Frustrum - The chase", "frustrum_chase"));

            if (missions.All(x => x.enabled))
                missionsAllCheckBox.IsChecked = true;

            missionsList.ItemsSource = missions;
        }

        private void InitRanks()
        {
            ranks.Add(new SaveFileManager.Rank(0, "Demolisher"));
            ranks.Add(new SaveFileManager.Rank(5, "Amateur"));
            ranks.Add(new SaveFileManager.Rank(10, "Novice"));
            ranks.Add(new SaveFileManager.Rank(15, "Trespasser"));
            ranks.Add(new SaveFileManager.Rank(20, "Breaker"));
            ranks.Add(new SaveFileManager.Rank(30, "Crook"));
            ranks.Add(new SaveFileManager.Rank(40, "Wrecker"));
            ranks.Add(new SaveFileManager.Rank(50, "Ballistic"));
            ranks.Add(new SaveFileManager.Rank(60, "Crackerjack"));
            ranks.Add(new SaveFileManager.Rank(70, "Professional"));
            ranks.Add(new SaveFileManager.Rank(80, "Expert"));
            ranks.Add(new SaveFileManager.Rank(90, "Midnighter"));
            ranks.Add(new SaveFileManager.Rank(100, "Virtuoso"));
            ranks.Add(new SaveFileManager.Rank(-1, "Custom"));

            int score = save.Score;
            int rIdx = 0;
            for (int i = 0; i < ranks.Count; i++)
            {
                if (ranks[i].score > score)
                    break;
                rIdx = i;
            }
            ranks[rIdx].enabled = true;

            ranksList.ItemsSource = ranks;

            customScoreText.SetText(score.ToString());

            OnRankChanged();
        }

        private void Stop(object sender, CancelEventArgs e)
        {
            save.Save();
        }

        #region Cash
        private void MaxCashButton_Click(object sender, RoutedEventArgs e)
        {
            cashText.SetText(int.MaxValue.ToString());
            save.Cash = int.MaxValue;
        }

        private void CashText_OnTextEntered(object sender, TextField.TextEventArgs e)
        {
            save.Cash = int.Parse(e.text);
        }
        #endregion

        #region Tools
        private void ToolsAllUnchecked(object sender, RoutedEventArgs e)
        {
            EnableAllTools(false);
        }

        private void ToolsAllChecked(object sender, RoutedEventArgs e)
        {
            EnableAllTools(true);
        }

        private void EnableAllTools(bool enabled)
        {
            for (int i = 0; i < tools.Count; i++)
            {
                tools[i].enabled = enabled;
            }

            // Refresh view
            toolsList.ItemsSource = null;
            toolsList.ItemsSource = tools;
        }
        #endregion

        #region Missions
        private void MissionsAllUnchecked(object sender, RoutedEventArgs e)
        {
            EnableAllMission(false);
        }

        private void MissionsAllChecked(object sender, RoutedEventArgs e)
        {
            EnableAllMission(true);
        }

        private void EnableAllMission(bool enabled)
        {
            for (int i = 0; i < missions.Count; i++)
            {
                missions[i].enabled = enabled;
            }

            // Refresh view
            missionsList.ItemsSource = null;
            missionsList.ItemsSource = missions;
        }
        #endregion

        #region Ranks / Score
        private void rank_Checked(object sender, RoutedEventArgs e)
        {
            OnRankChanged();
        }

        private void rank_Unchecked(object sender, RoutedEventArgs e)
        {
            OnRankChanged();
        }

        private void OnRankChanged()
        {
            customScoreText.Visibility = ranks.Last().enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void customScoreText_OnTextEntered(object sender, TextField.TextEventArgs e)
        {
            save.Score = int.Parse(e.text);
        }
        #endregion
    }
}
