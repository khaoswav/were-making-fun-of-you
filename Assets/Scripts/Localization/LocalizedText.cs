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
            ["generic_interact"] = "HOST: You touched it. Historic moment. Someone get this player a trophy.",
            ["host_wrong_door"] = "You tried that door again? Commitment is admirable. Pattern recognition, less so.",
            ["host_wrong_door_alt"] = "The door hasn't changed its mind. I checked. Twice.",
            ["host_idle"] = "Still here? I was starting to think the pause menu had more personality.",
            ["host_idle_alt"] = "Take your time. The building isn't getting any smarter while you stand there.",
            ["host_sprinting"] = "Look at you go! Running from your problems. Very relatable.",
            ["host_sprinting_alt"] = "Careful! At this speed you might accidentally make progress.",
            ["host_avoids_interaction"] = "You looked right at it, then walked away. Even the furniture feels rejected.",
            ["host_avoids_interaction_alt"] = "You and that button had a moment. Then you ghosted it."
        };

        private static readonly Dictionary<string, string> Turkish = new Dictionary<string, string>
        {
            ["objective_intro"] = "GÖREV: Stüdyoyu keşfet. Sahne arkasına geçmenin yolunu bul.",
            ["host_intro"] = "Hoş geldin süperstar! Kendini rezil etmemeye çalış. Kameralar çoktan kayıtta.",
            ["interaction_hint"] = "[E] ETKİLEŞİM",
            ["door_locked"] = "SUNUCU: Bu kapı kilitli. Müthiş iş. Çalışmayan tek kapıyı bulmayı başardın.",
            ["generic_interact"] = "SUNUCU: Dokundun. Tarihi an. Birisi bu oyuncuya kupa getirsin.",
            ["host_wrong_door"] = "Yine aynı kapıyı mı denedin? Azmine hayranım. Algılarına biraz daha az.",
            ["host_wrong_door_alt"] = "Kapı fikrini değiştirmedi. Kontrol ettim. İki kere.",
            ["host_idle"] = "Hâlâ burada mısın? Duraklatma menüsünün daha çok kişiliği var sanmıştım.",
            ["host_idle_alt"] = "Acele etme. Sen durunca bina daha akıllı hâle gelmiyor.",
            ["host_sprinting"] = "Şuna bak, koşuyor! Sorunlarından kaçmak. Gayet tanıdık bir hareket.",
            ["host_sprinting_alt"] = "Dikkat et! Bu hızla yanlışlıkla ilerleme kaydedebilirsin.",
            ["host_avoids_interaction"] = "Tam ona baktın, sonra çekip gittin. Eşyalar bile reddedilmiş hissediyor.",
            ["host_avoids_interaction_alt"] = "O düğmeyle aranızda bir şeyler oluyordu. Sonra mesajlarına dönmedin."
        };

        public static string Get(GameLanguage language, string key)
        {
            var table = language == GameLanguage.Turkish ? Turkish : English;
            return table.TryGetValue(key, out var value) ? value : key;
        }
    }
}
