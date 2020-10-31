using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;

namespace TeardownSaveEditor.Scripts
{
    public class SaveFileManager
    {
        public class Tool
        {
            public string title { get; set; }
            public string name { get; set; }
            private bool _enabled;
            public bool enabled { get { return _enabled; } set { Instance.SetTool(name, value); _enabled = value; } }

            public Tool(string title, string name)
            {
                this.title = title;
                this.name = name;
                _enabled = Instance.HasToolEnabled(name);
            }
        }

        public class Valuable
        {
            public string title { get; set; }
            public string name;
            private bool _enabled;
            public bool enabled { get { return _enabled; } set { Instance.SetValuable(name, value); _enabled = value; } }

            public Valuable(string title, string name)
            {
                this.title = title;
                this.name = name;
                _enabled = Instance.HasValuableFound(name);
            }
        }

        public class Mission
        {
            public int missionNum;
            public string title { get; set; }
            public string name;
            private bool _enabled;
            public bool enabled { get { return _enabled; } set { Instance.SetMission(name, value); _enabled = value; } }

            public Mission(int missionOrderNum, string title, string name)
            {
                missionNum = missionOrderNum;
                this.title = title;
                this.name = name;
                _enabled = Instance.HasMission(name);
            }
        }

        public class Rank
        {
            public string title { get { return score >= 0 ? $"{name} (score {score})" : name; } }
            public string name;
            public int score;
            private bool _enabled = false;
            public bool enabled { get { return _enabled; } set { if (value) Instance.Score = score; _enabled = value; } }

            public Rank(int score, string name)
            {
                this.score = score;
                this.name = name;
            }
        }

        private static SaveFileManager instance;
        public static SaveFileManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SaveFileManager(); return instance;
            }
        }

        private readonly string saveFolderPath;
        private readonly string saveFilePath;
        private XmlDocument saveFileXml;
        private XmlElement savegameXml { get { return saveFileXml["registry"]["savegame"]; } }

        public bool fileLoaded = false;

        public int Cash
        {
            get
            {
                return int.Parse(savegameXml["cash"].GetAttribute("value"));
            }
            set
            {
                savegameXml["cash"].SetAttribute("value", value.ToString());
            }
        }

        public int Score
        {
            get
            {
                return int.Parse(savegameXml["hub"]["score"].GetAttribute("value"));
            }
            set
            {
                savegameXml["hub"]["score"].SetAttribute("value", value.ToString());
            }
        }


        public SaveFileManager()
        {
            instance = this;

            saveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Teardown/");
            saveFilePath = saveFolderPath + "savegame.xml";

            // Check path
            if (!File.Exists(saveFilePath))
            {
                MessageBox.Show($"Couldn't find \"{saveFilePath}\". There should be a folder named \"Teardown\" in the documents directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
                return;
            }

            string fileContent = File.ReadAllText(saveFilePath);
            // Prevent parsing errors
            if (fileContent.Contains("<reward>"))
            {
                int start = fileContent.IndexOf("<reward>");
                fileContent = fileContent.Remove(start, fileContent.IndexOf("</reward>") - start + "</reward>".Length);
            }

            saveFileXml = new XmlDocument();
            try
            {
                saveFileXml.LoadXml(fileContent);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
                return;
            }
            fileLoaded = true;
        }


        public bool HasToolUnlocked(string name)
        {
            return savegameXml["tool"][name] != null;
        }

        public bool HasToolEnabled(string name)
        {
            var tool = savegameXml["tool"][name];
            return tool != null && tool["enabled"].GetAttribute("value") == "1";
        }

        public void SetTool(string name, bool enabled)
        {
            string enabledStr = enabled ? "1" : "0";

            if (!HasToolUnlocked(name))
            {
                // Create tool
                XmlElement xmlTool = saveFileXml.CreateElement(name);
                XmlElement xmlEnabled = saveFileXml.CreateElement("enabled");
                xmlEnabled.SetAttribute("value", enabledStr);
                xmlTool.AppendChild(xmlEnabled);

                // Add to tool list
                savegameXml["tool"].AppendChild(xmlTool);
            }
            else
            {
                savegameXml["tool"][name]["enabled"].SetAttribute("value", enabledStr);
            }
        }


        public bool HasValuableFound(string name)
        {
            return savegameXml["valuable"][name] != null;
        }

        public void SetValuable(string name, bool found)
        {
            string foundStr = found ? "1" : "0";

            if (!HasValuableFound(name))
            {
                // Create valuable
                XmlElement xmlValuable = saveFileXml.CreateElement(name);
                xmlValuable.SetAttribute("value", foundStr);

                // Add to valuable list
                savegameXml["valuable"].AppendChild(xmlValuable);
            }
            else
            {
                savegameXml["valuable"][name].SetAttribute("value", foundStr);
            }
        }


        public bool HasMission(string name)
        {
            return savegameXml["mission"][name] != null;
        }

        public void SetMission(string name, bool enabled)
        {
            string enabledStr = enabled ? "1" : "0";

            if (!HasMission(name))
            {
                // Create tool
                XmlElement xmlMission = saveFileXml.CreateElement(name);
                XmlElement xmlScore = saveFileXml.CreateElement("score");
                XmlElement xmlTimeleft = saveFileXml.CreateElement("timeleft");
                XmlElement xmlMissionTime = saveFileXml.CreateElement("missiontime");
                xmlMission.SetAttribute("value", enabledStr);
                xmlScore.SetAttribute("value", "69");
                xmlTimeleft.SetAttribute("value", "-1.0");
                xmlMissionTime.SetAttribute("value", "420");
                xmlMission.AppendChild(xmlScore);
                xmlMission.AppendChild(xmlTimeleft);
                xmlMission.AppendChild(xmlMissionTime);

                // Add to tool list
                savegameXml["mission"].AppendChild(xmlMission);
            }
            else
            {
                savegameXml["mission"][name].SetAttribute("value", enabledStr);
            }
        }

        public void Save()
        {
            if (fileLoaded)
                saveFileXml.Save(saveFilePath);
        }
    }
}
