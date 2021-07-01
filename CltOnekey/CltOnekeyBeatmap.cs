using Newtonsoft.Json;
using OsuParsers.Database.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CltOnekey
{
    public class CltOnekeyBeatmap
    {
        public string Type = "CltOnekey Collection Beatmap Profile";
        public string Hash { get; set; }
        public string UnicodeTitle { get; set; }
        public string Title { get; set; }
        public int BID { get; set; }
        public int SID { get; set; }
        public string Artist { get; set; }
        public string Difficult { get; private set; }
        [JsonIgnore]
        public bool Mismatch { get; set; }

        public static List<CltOnekeyBeatmap> ConvertFromDbBeatmaps(List<DbBeatmap> dbBeatmaps)
        {
            List<CltOnekeyBeatmap> CltOnekeyBeatmaps = new List<CltOnekeyBeatmap>();
            foreach (var dbBeatmap in dbBeatmaps)
            {
                var CltOnekeyBeatmap = new CltOnekeyBeatmap(dbBeatmap.MD5Hash, false)
                {
                    UnicodeTitle = dbBeatmap.TitleUnicode,
                    Title = dbBeatmap.Title,
                    BID = dbBeatmap.BeatmapId,
                    SID = dbBeatmap.BeatmapSetId,
                    Artist = dbBeatmap.Artist,
                    Difficult = dbBeatmap.Difficulty
                };
                CltOnekeyBeatmaps.Add(CltOnekeyBeatmap);
            }
            return CltOnekeyBeatmaps;
        }

        public CltOnekeyBeatmap(string hash, bool mismatch)
        {
            Hash = hash;
            Mismatch = mismatch;
        }
    }
}
