using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole
{
    /// <summary>
    /// Простой способ хранить настройки в файле в XML.
    /// </summary>
    public class Settings
    {
        public Settings()
        {
            LoadSetting();
        }

        [Serializable()]
        public sealed class InternalSettings
        {
            public ushort restTcpPort  = 0;
            public ushort debugTcpPort = 0;
            public string posgresConnectionString = String.Empty;
        }

        public ushort restTcpPort
        {
            get => internalSettings.restTcpPort;
            set => internalSettings.restTcpPort = value;
        }
        public ushort debugTcpPort
        {
            get => internalSettings.debugTcpPort;
            set => internalSettings.debugTcpPort = value;
        }

        public string posgresConnectionString
        {
            get => internalSettings.posgresConnectionString;
            set => internalSettings.posgresConnectionString = value;
        }

        public bool anySettingEmpty
        {
            get => restTcpPort == 0 || debugTcpPort == 0 || posgresConnectionString == String.Empty;
        }

        private string settingFileName = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "settings.xml";

        private InternalSettings internalSettings = new InternalSettings();

        public void LoadSetting()
        {
            lock (settingFileName)
            {
                if (System.IO.File.Exists(settingFileName))
                {
                    System.IO.StreamReader srReader = System.IO.File.OpenText(settingFileName);
                    Type tType = internalSettings.GetType();
                    System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                    object oData = xsSerializer.Deserialize(srReader);
                    internalSettings = (InternalSettings)oData;
                    srReader.Close();
                }
            }
        }

        public void saveSetting()
        {
            lock (settingFileName)
            {
                System.IO.StreamWriter swWriter = System.IO.File.CreateText(settingFileName);
                Type tType = internalSettings.GetType();
                if (tType.IsSerializable)
                {
                    System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                    xsSerializer.Serialize(swWriter, internalSettings);
                }
                swWriter.Close();
            }
        }
    }
}
