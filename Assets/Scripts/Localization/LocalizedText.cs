using System.Collections.Generic;

namespace WereMakingFunOfYou
{
    public static class LocalizedText
    {
        private static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            ["objective_intro"] = "OBJECTIVE: Explore the studio. Find a way backstage.",
            ["host_intro"] = "Welcome, superstar! Try not to embarrass yourself. The cameras are already rolling.",
            ["interaction_hint"] = "[E] INTERACT",
            ["door_locked"] = "HOST: That door is locked. Impressive work. You found the one door that doesn't work.",
            ["generic_interact"] = "HOST: You touched it. Historic moment. Someone get this player a trophy."
        };

        private static readonly Dictionary<string, string> Turkish = new Dictionary<string, string>
        {
            ["objective_intro"] = "GÖREV: Stüdyoyu keşfet. Sahne arkasına geçmenin yolunu bul.",
            ["host_intro"] = "Hoş geldin süperstar! Kendini rezil etmemeye çalış. Kameralar çoktan kayıtta.",
            ["interaction_hint"] = "[E] ETKİLEŞİM",
            ["door_locked"] = "SUNUCU: Bu kapı kilitli. Müthiş iş. Çalışmayan tek kapıyı bulmayı başardın.",
            ["generic_interact"] = "SUNUCU: Dokundun. Tarihi an. Birisi bu oyuncuya kupa getirsin."
        };

        public static string Get(GameLanguage language, string key)
        {
            var table = language == GameLanguage.Turkish ? Turkish : English;
            return table.TryGetValue(key, out var value) ? value : key;
        }
    }
}
