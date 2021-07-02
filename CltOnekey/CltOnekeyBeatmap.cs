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

        private CltOnekeyBeatmap() { }

        internal static List<CltOnekeyBeatmap> ConvertAllFrom(List<string> hashes)
        {
            List<CltOnekeyBeatmap> CltOnekeyBeatmaps = new List<CltOnekeyBeatmap>();
            var matched = MainWindow.Database.FindBeatmapsFromHashes(hashes);
            var misMatched = MainWindow.Database.FindMismatchedMaps(hashes, matched);
            foreach (var dbBeatmap in matched)
            {
                var CltOnekeyBeatmap = new CltOnekeyBeatmap()
                {
                    Hash = dbBeatmap.MD5Hash,
                    UnicodeTitle = dbBeatmap.TitleUnicode,
                    Title = dbBeatmap.Title,
                    BID = dbBeatmap.BeatmapId,
                    SID = dbBeatmap.BeatmapSetId,
                    Artist = dbBeatmap.Artist,
                    Difficult = dbBeatmap.Difficulty,
                    Mismatch = false
                };
                CltOnekeyBeatmaps.Add(CltOnekeyBeatmap);
            }
            foreach (var hash in misMatched)
            {
                var CltOnekeyBeatmap = new CltOnekeyBeatmap()
                {
                    Hash = hash,
                    Mismatch = true
                };
                CltOnekeyBeatmaps.Add(CltOnekeyBeatmap);
            }
            return CltOnekeyBeatmaps;
        }
    }
}
