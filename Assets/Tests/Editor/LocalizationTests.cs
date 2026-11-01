using NUnit.Framework;

namespace WereMakingFunOfYou.Tests.Editor
{
    public sealed class LocalizationTests
    {
        private static readonly string[] RequiredKeys =
        {
            "objective_intro",
            "host_intro",
            "interaction_hint",
            "door_locked",
            "generic_interact",
            "host_wrong_door",
            "host_wrong_door_alt",
            "host_idle",
            "host_idle_alt",
            "host_sprinting",
            "host_sprinting_alt",
            "host_avoids_interaction",
            "host_avoids_interaction_alt",
            "host_wrong_door_3",
            "host_wrong_door_4",
            "host_locker_hide_prompt",
            "host_locker_peek",
            "host_locker_exit",
            "host_door_wave",
            "host_fake_jumpscare",
            "host_fake_jumpscare_punchline",
            "host_fake_scare_repeat",
            "host_safe_room_welcome",
            "host_safe_room_door_closed",
            "host_safe_room_trap",
            "host_monster_enraged",
            "host_real_jumpscare"
        };

        [TestCase(GameLanguage.English)]
        [TestCase(GameLanguage.Turkish)]
        public void EveryRequiredKeyResolvesToLocalizedText(GameLanguage language)
        {
            foreach (string key in RequiredKeys)
            {
                string value = LocalizedText.Get(language, key);
                Assert.That(value, Is.Not.Null.And.Not.Empty, "Missing value for " + key);
                Assert.That(value, Is.Not.EqualTo(key), "Unresolved localization key: " + key);
            }
        }
    }
}
