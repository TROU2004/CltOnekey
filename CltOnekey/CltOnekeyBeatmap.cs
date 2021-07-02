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
        public string Difficulty { get; set; }

        public static List<CltOnekeyBeatmap> ConvertFromDbBeatmaps(List<DbBeatmap> dbBeatmaps)
        {
            List<CltOnekeyBeatmap> CltOnekeyBeatmaps = new List<CltOnekeyBeatmap>();
            foreach (var dbBeatmap in dbBeatmaps)
            {
                var CltOnekeyBeatmap = new CltOnekeyBeatmap()
                {
                    Hash = dbBeatmap.MD5Hash,
                    UnicodeTitle = dbBeatmap.TitleUnicode,
                    Title = dbBeatmap.Title,
                    BID = dbBeatmap.BeatmapId,
                    SID = dbBeatmap.BeatmapSetId,
                    Artist = dbBeatmap.Artist,
                    Difficulty = dbBeatmap.Difficulty
                };
                if (dbBeatmap.TitleUnicode == "") dbBeatmap.TitleUnicode = dbBeatmap.Title;
                CltOnekeyBeatmaps.Add(CltOnekeyBeatmap);
            }
            return CltOnekeyBeatmaps;
        }
    }
}
