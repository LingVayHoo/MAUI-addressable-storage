using Newtonsoft.Json;
using System;

namespace ADSCrossPlatform.Code.Models
{
    public class StoredSettingsJson
    {
        public string Username { get; set; } = string.Empty;
        public string MemoID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string MoveToStore310 { get; set; } = string.Empty;
        public string MoveFromStore310 { get; set; } = string.Empty;
        public string MoveToStore390 { get; set; } = string.Empty;
        public string MoveFromStore390 { get; set; } = string.Empty;
        public Dictionary<string, string>? Storages { get; set; }
    }

    public class StoredSettings
    {
        private readonly string _filePath;
        public string Username { get; set; } = string.Empty;
        public string MemoID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string MoveToStore310 { get; set; } = string.Empty;
        public string MoveFromStore310 { get; set; } = string.Empty;
        public string MoveToStore390 { get; set; } = string.Empty;
        public string MoveFromStore390 { get; set; } = string.Empty;
        public Dictionary<string, string>? Storages { get; set; }

        public StoredSettings()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _filePath = Path.Combine(path, "json.json");
            LoadData();
        }

        public void SaveData()
        {
            StoredSettingsJson storedSettingsJson = new StoredSettingsJson()
            {
                //Token = this.Token,
                Username = this.Username,
                MemoID = this.MemoID,
                MoveToStore310 = this.MoveToStore310,
                MoveFromStore310 = this.MoveFromStore310,
                MoveToStore390 = this.MoveToStore390,
                MoveFromStore390 = this.MoveFromStore390,
                Storages = this.Storages,
                Password = this.Password,
            };

            var json = JsonConvert.SerializeObject(storedSettingsJson, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }        

        private void LoadData()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    StoredSettingsJson data = JsonConvert.DeserializeObject<StoredSettingsJson>(json);
                    //Token = data?.Token ?? string.Empty;
                    Username = data?.Username ?? string.Empty;
                    MemoID = data?.MemoID ?? string.Empty;
                    MoveToStore310 = data?.MoveToStore310 ?? string.Empty;
                    MoveFromStore310 = data?.MoveFromStore310 ?? string.Empty;
                    MoveToStore390 = data?.MoveToStore390 ?? string.Empty;
                    MoveFromStore390 = data?.MoveFromStore390 ?? string.Empty;
                    Storages = data?.Storages;
                    Password = data?.Password ?? string.Empty;
                }
                catch (Exception ex)
                {

                    File.Delete(_filePath);
                    Username = string.Empty;
                    MemoID = string.Empty;
                    MoveToStore310 = string.Empty;
                    MoveFromStore310 = string.Empty;
                    MoveToStore390 = string.Empty;
                    MoveFromStore390 = string.Empty;
                }
                
            }
            else
            {
                Username = string.Empty;
                MemoID = string.Empty;
                MoveToStore310 = string.Empty;
                MoveFromStore310 = string.Empty;
                MoveToStore390 = string.Empty;
                MoveFromStore390 = string.Empty;
            }
        }
    }
}

