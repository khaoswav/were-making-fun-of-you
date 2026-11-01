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
            ["host_avoids_interaction_alt"] = "You and that button had a moment. Then you ghosted it.",
            ["host_wrong_door_3"] = "Third attempt. I admire the confidence. The door remains deeply unimpressed.",
            ["host_wrong_door_4"] = "At this point, the door is considering filing a restraining order.",
            ["host_locker_hide_prompt"] = "You squeeze inside. Press E to leave. Try not to breathe dramatically.",
            ["host_locker_peek"] = "Found you. I was going to knock, but this felt funnier.",
            ["host_locker_exit"] = "Back into the world. The locker says it wants a raise.",
            ["host_door_wave"] = "The door waved at you. Management has been informed.",
            ["host_fake_jumpscare"] = "AAAAAAAA—",
            ["host_fake_jumpscare_punchline"] = "You flinched. The cameras got that in 4K.",
            ["host_fake_scare_repeat"] = "The button already got you once. It has no encore contract.",
            ["host_safe_room_welcome"] = "Welcome to the safe room. Finally, a sensible decision.",
            ["host_safe_room_door_closed"] = "Closing the door? Sensible. Suspiciously sensible.",
            ["host_safe_room_trap"] = "You thought this room was safe. That's adorable.",
            ["host_monster_enraged"] = "It tripped. You laughed. It heard you. That was a poor life choice.",
            ["host_real_jumpscare"] = "SHOW'S OVER. The audience would like its heartbeat back."
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
            ["host_avoids_interaction_alt"] = "O düğmeyle aranızda bir şeyler oluyordu. Sonra mesajlarına dönmedin.",
            ["host_wrong_door_3"] = "Üçüncü deneme. Özgüvenine saygı duyuyorum. Kapıysa hiç etkilenmedi.",
            ["host_wrong_door_4"] = "Bu saatten sonra kapı senin hakkında uzaklaştırma kararı çıkartacak.",
            ["host_locker_hide_prompt"] = "İçeri sığındın. Çıkmak için E'ye bas. Dramatik nefes almamaya çalış.",
            ["host_locker_peek"] = "Buldum seni. Kapıyı çalacaktım ama bu daha komik geldi.",
            ["host_locker_exit"] = "Tekrar dünyaya döndün. Dolap zam istemeye başladı.",
            ["host_door_wave"] = "Kapı sana el salladı. Yönetim bilgilendirildi.",
            ["host_fake_jumpscare"] = "AAAAAA—",
            ["host_fake_jumpscare_punchline"] = "İrkindin. Kameralar bunu 4K kaydetti.",
            ["host_fake_scare_repeat"] = "Bu düğme seni bir kez korkuttu. Tekrar gösterim anlaşması yok.",
            ["host_safe_room_welcome"] = "Güvenli odaya hoş geldin. Sonunda mantıklı bir karar.",
            ["host_safe_room_door_closed"] = "Kapıyı kapatıyorsun. Mantıklı. Şüpheli derecede mantıklı.",
            ["host_safe_room_trap"] = "Buranın güvenli olduğunu sandın. Çok tatlı.",
            ["host_monster_enraged"] = "Yere kapaklandı. Güldün. Seni duydu. Hayatının kötü kararlarından biriydi.",
            ["host_real_jumpscare"] = "GÖSTERİ BİTTİ. Seyirci kalp atışlarını geri istiyor."
        };

        public static string Get(GameLanguage language, string key)
        {
            var table = language == GameLanguage.Turkish ? Turkish : English;
            return table.TryGetValue(key, out var value) ? value : key;
        }
    }
}