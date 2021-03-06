﻿using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using Windows.Storage;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace AdvancedWallpaperManager.Models.Settings
{
    public class ActiveLockscreenThemeHistorySetting : ApplicationSettingBase<List<string>>
    {
        public const int MAX_ITEMS = 5;
        /// <summary>
        /// In order to modify the list you must assign it to a temporary variable and re-assign to Value when complete
        /// </summary>
        public override List<string> Value
        {
            get { return JsonConvert.DeserializeObject<List<string>>((string)Container.Values[Key]); }
            set
            {
                // Limit the number of items on the Recent List
                while (value.Count > MAX_ITEMS) value.RemoveAt(MAX_ITEMS);

                Container.Values[Key] = JsonConvert.SerializeObject(value);
                RaisePropertyChanged(nameof(Value));
            }
        }

        public ActiveLockscreenThemeHistorySetting()
            :base(StorageTask.LocalSettings, "ActiveLockscreenThemeHistory", new List<string>())
        {
        }

        public void Add(string str)
        {
            // Save the Value to a Temp variable then Enqueue the item
            var tmpValue = Value;
            tmpValue.Insert(0, str); 

            // Restrict to the MAX_ITEMS
            while (tmpValue.Count > MAX_ITEMS)
                tmpValue.RemoveAt(MAX_ITEMS);

            // Reupdate the Value
            Value = tmpValue;
        }
    }
}
