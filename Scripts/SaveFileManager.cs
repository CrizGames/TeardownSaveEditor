using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        public SaveFileManager()
        {
            instance = this;

            saveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Teardown/");
            saveFilePath = saveFolderPath + "savegame.xml";

            saveFileXml = new XmlDocument();
            saveFileXml.Load(saveFilePath);
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

        public void Save()
        {
            saveFileXml.Save(saveFilePath);
        }
    }
}
