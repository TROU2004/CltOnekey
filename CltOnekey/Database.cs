using Newtonsoft.Json;
using OsuParsers.Database;
using OsuParsers.Database.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CltOnekey
{
    public class Database
    {
        public OsuDatabase OsuDatabase { get; }
        public string GamePath { get; set; }
        public CollectionDatabase CollectionDatabase { get; set; }

        public Database(OsuDatabase osuDatabase, string gamePath)
        {
            OsuDatabase = osuDatabase;
            GamePath = gamePath;
        }

        public List<DbBeatmap> FindBeatmapsFromHashes(List<string> hashes)
        {
            return OsuDatabase.Beatmaps.FindAll(map => hashes.Contains(map.MD5Hash));
        }

        public List<Collection> BuildCollections(string path)
        {
            List<Collection> collections = new List<Collection>();
            foreach (var item in Directory.GetDirectories(path))
            {
                Collection collection = new Collection();
                collection.Name = Path.GetDirectoryName(item);
                foreach (var map in Directory.GetFiles(item, "*.json"))
                {
                    CltOnekeyBeatmap CltOnekeyBeatmap = JsonConvert.DeserializeObject<CltOnekeyBeatmap>(File.ReadAllText(map));
                    collection.MD5Hashes.Add(CltOnekeyBeatmap.Hash);
                }
                collection.Count = collection.MD5Hashes.Count;
                collections.Add(collection);
            }
            return collections;
        }
    }
}
