using BepInEx.Configuration;

namespace CleaningCompany.Misc
{
    public class PluginConfig
    {
        readonly ConfigFile configFile;

        // active bodies
        public bool SPIDER { get; set; }
        public bool THUMPER { get; set; }
        public bool NUTCRACKER { get; set; }
        public bool CENTIPEDE { get; set; }
        public bool HOARDER { get; set; }
        public bool BRACKEN { get; set; }
        public bool MOUTHDOG { get; set; }
        public bool BABOON { get; set; }
        public bool COILHEAD { get; set; }
        public bool GHOSTGIRL { get; set; }
        public bool FORESTKEEPER { get; set; }
        public bool JESTER { get; set; }
        public bool HYGRODORE { get; set; }
        public bool SPORELIZARD { get; set; }
        public bool MANTICOIL { get; set; }
        public bool RADMECH { get; set; }
        public bool TULIPSNAKE { get; set; }
        public bool MODDEDENEMY { get; set; }

        // two-handed bodies
        public bool SPIDER_TWOHANDED { get; set; }
        public bool THUMPER_TWOHANDED { get; set; }
        public bool NUTCRACKER_TWOHANDED { get; set; }
        public bool CENTIPEDE_TWOHANDED { get; set; }
        public bool HOARDER_TWOHANDED { get; set; }
        public bool BRACKEN_TWOHANDED { get; set; }
        public bool MOUTHDOG_TWOHANDED { get; set; }
        public bool BABOON_TWOHANDED { get; set; }
        public bool COILHEAD_TWOHANDED { get; set; }
        public bool GHOSTGIRL_TWOHANDED { get; set; }
        public bool FORESTKEEPER_TWOHANDED { get; set; }
        public bool JESTER_TWOHANDED { get; set; }
        public bool HYGRODORE_TWOHANDED { get; set; }
        public bool SPORELIZARD_TWOHANDED { get; set; }
        public bool MANTICOIL_TWOHANDED { get; set; }
        public bool RADMECH_TWOHANDED { get; set; }
        public bool TULIPSNAKE_TWOHANDED { get; set; }
        public bool MODDEDENEMYPOWERLEVEL1_TWOHANDED { get; set; }
        public bool MODDEDENEMYPOWERLEVEL2_TWOHANDED { get; set; }
        public bool MODDEDENEMYPOWERLEVEL3_TWOHANDED { get; set; }

        // bodyspawn min values
        public int SPIDER_MIN { get; set; }
        public int THUMPER_MIN { get; set; }
        public int NUTCRACKER_MIN { get; set; }
        public int CENTIPEDE_MIN { get; set; }
        public int HOARDER_MIN { get; set; }
        public int BRACKEN_MIN { get; set; }
        public int MOUTHDOG_MIN { get; set; }
        public int BABOON_MIN { get; set; }
        public int COILHEAD_MIN { get; set; }
        public int GHOSTGIRL_MIN { get; set; }
        public int FORESTKEEPER_MIN { get; set; }
        public int JESTER_MIN { get; set; }
        public int HYGRODORE_MIN { get; set; }
        public int SPORELIZARD_MIN { get; set; }
        public int MANTICOIL_MIN { get; set; }
        public int RADMECH_MIN { get; set; }
        public int TULIPSNAKE_MIN { get; set; }
        public int MODDEDENEMYPOWERLEVEL1_MIN { get; set; }
        public int MODDEDENEMYPOWERLEVEL2_MIN { get; set; }
        public int MODDEDENEMYPOWERLEVEL3_MIN { get; set; }

        // bodyspawn max values
        public int NUTCRACKER_MAX { get; set; }
        public int SPIDER_MAX { get; set; }
        public int THUMPER_MAX { get; set; }
        public int CENTIPEDE_MAX { get; set; }
        public int HOARDER_MAX { get; set; }
        public int BRACKEN_MAX { get; set; }
        public int MOUTHDOG_MAX { get; set; }
        public int BABOON_MAX { get; set; }
        public int COILHEAD_MAX { get; set; }
        public int GHOSTGIRL_MAX { get; set; }
        public int FORESTKEEPER_MAX { get; set; }
        public int JESTER_MAX { get; set; }
        public int HYGRODORE_MAX { get; set; }
        public int SPORELIZARD_MAX { get; set; }
        public int MANTICOIL_MAX { get; set; }
        public int RADMECH_MAX { get; set; }
        public int TULIPSNAKE_MAX { get; set; }
        public int MODDEDENEMYPOWERLEVEL1_MAX { get; set; }
        public int MODDEDENEMYPOWERLEVEL2_MAX { get; set; }
        public int MODDEDENEMYPOWERLEVEL3_MAX { get; set; }

        // bodystpawn weights
        public float NUTCRACKER_WEIGHT { get; set; }
        public float SPIDER_WEIGHT { get; set; }
        public float THUMPER_WEIGHT { get; set; }
        public float CENTIPEDE_WEIGHT { get; set; }
        public float HOARDER_WEIGHT { get; set; }
        public float BRACKEN_WEIGHT { get; set; }
        public float MOUTHDOG_WEIGHT { get; set; }
        public float BABOON_WEIGHT { get; set; }
        public float COILHEAD_WEIGHT { get; set; }
        public float GHOSTGIRL_WEIGHT { get; set; }
        public float FORESTKEEPER_WEIGHT { get; set; }
        public float JESTER_WEIGHT { get; set; }
        public float HYGRODORE_WEIGHT { get; set; }
        public float SPORELIZARD_WEIGHT { get; set; }
        public float MANTICOIL_WEIGHT { get; set; }
        public float RADMECH_WEIGHT { get; set; }
        public float TULIPSNAKE_WEIGHT { get; set; }
        public float MODDEDENEMYPOWERLEVEL1_WEIGHT { get; set; }
        public float MODDEDENEMYPOWERLEVEL2_WEIGHT { get; set; }
        public float MODDEDENEMYPOWERLEVEL3_WEIGHT { get; set; }

        //body value multiplier
        public float MULTIPLIER_VALUE { get; set; }
        public float MULTIPLIER_POWER_COUNT_SUBTRACTION { get; set; }
        public bool DISABLE_MULTIPLIER { get; set; }


        public PluginConfig(ConfigFile cfg)
        {
            configFile = cfg;
        }

        private T ConfigEntry<T>(string section, string key, T defaultVal, string description)
        {
            return configFile.Bind(section, key, defaultVal, description).Value;
        }

        public void InitBindings()
        {
            CENTIPEDE_MAX = ConfigEntry("Body Values", "Max price of Centipede Bodies", 70, "");
            CENTIPEDE_MIN = ConfigEntry("Body Values", "Min price of Centipede Bodies", 45, "");
            HOARDER_MAX = ConfigEntry("Body Values", "Max price of Hoarding Bug Bodies", 90, "");
            HOARDER_MIN = ConfigEntry("Body Values", "Min price of Hoarding Bug Bodies", 55, "");
            SPIDER_MAX = ConfigEntry("Body Values", "Max price of Spider Bodies", 110, "");
            SPIDER_MIN = ConfigEntry("Body Values", "Min price of Spider Bodies", 70, "");
            THUMPER_MAX = ConfigEntry("Body Values", "Max price of Thumper Bodies", 160, "");
            THUMPER_MIN = ConfigEntry("Body Values", "Min price of Thumper Bodies", 120, "");
            NUTCRACKER_MAX = ConfigEntry("Body Values", "Max price of Nutcracker Bodies", 150, "");
            NUTCRACKER_MIN = ConfigEntry("Body Values", "Min price of Nutcracker Bodies", 125, "");
            BRACKEN_MAX = ConfigEntry("Body Values", "Max price of Bracken Bodies", 140, "");
            BRACKEN_MIN = ConfigEntry("Body Values", "Min price of Bracken Bodies", 100, "");
            BABOON_MAX = ConfigEntry("Body Values", "Max price of Baboon Hawk Bodies", 155, "");
            BABOON_MIN = ConfigEntry("Body Values", "Min price of Baboon Hawk Bodies", 105, "");
            MOUTHDOG_MAX = ConfigEntry("Body Values", "Max price of Eyeless Dog Bodies", 200, "");
            MOUTHDOG_MIN = ConfigEntry("Body Values", "Min price of Eyeless Dog Bodies", 175, "");
            COILHEAD_MAX = ConfigEntry("Body Values", "Max price of Coil-Head Bodies", 225, "");
            COILHEAD_MIN = ConfigEntry("Body Values", "Min price of Coil-Head Bodies", 185, "");
            GHOSTGIRL_MAX = ConfigEntry("Body Values", "Max price of Ghost Girl Bodies", 250, "");
            GHOSTGIRL_MIN = ConfigEntry("Body Values", "Min price of Ghost Girl Bodies", 200, "");
            FORESTKEEPER_MAX = ConfigEntry("Body Values", "Max price of Forest Keeper Bodies", 275, "");
            FORESTKEEPER_MIN = ConfigEntry("Body Values", "Min price of Forest Keeper Bodies", 225, "");
            JESTER_MAX = ConfigEntry("Body Values", "Max price of Jester Bodies", 245, "");
            JESTER_MIN = ConfigEntry("Body Values", "Min price of Jester Bodies", 215, "");
            HYGRODORE_MAX = ConfigEntry("Body Values", "Max price of Hygrodere Bodies", 120, "");
            HYGRODORE_MIN = ConfigEntry("Body Values", "Min price of Hygrodere Bodies", 75, "");
            SPORELIZARD_MAX = ConfigEntry("Body Values", "Max price of Spore Lizard Bodies", 105, "");
            SPORELIZARD_MIN = ConfigEntry("Body Values", "Min price of Spore Lizard Bodies", 85, "");
            MANTICOIL_MAX = ConfigEntry("Body Values", "Max price of Manticoil Bodies", 15, "");
            MANTICOIL_MIN = ConfigEntry("Body Values", "Min price of Manticoil Bodies", 10, "");
            RADMECH_MAX = ConfigEntry("Body Values", "Max price of Rad Mech Bodies", 300, "");
            RADMECH_MIN = ConfigEntry("Body Values", "Min price of Rad Mech Bodies", 250, "");
            TULIPSNAKE_MAX = ConfigEntry("Body Values", "Max price of Tulip Snake Bodies", 40, "");
            TULIPSNAKE_MIN = ConfigEntry("Body Values", "Min price of Tulip Snake Bodies", 25, "");
            MODDEDENEMYPOWERLEVEL1_MAX = ConfigEntry("Body Values", "Max price of Modded Enemy Bodies With 1 Or Less PowerLevel", 100, "");
            MODDEDENEMYPOWERLEVEL1_MIN = ConfigEntry("Body Values", "Min price of Modded Enemy Bodies With 1 Or Less PowerLevel", 75, "");
            MODDEDENEMYPOWERLEVEL2_MAX = ConfigEntry("Body Values", "Max price of Modded Enemy Bodies With 2 PowerLevel", 150, "");
            MODDEDENEMYPOWERLEVEL2_MIN = ConfigEntry("Body Values", "Min price of Modded Enemy Bodies With 2 PowerLevel", 125, "");
            MODDEDENEMYPOWERLEVEL3_MAX = ConfigEntry("Body Values", "Max price of Modded Enemy Bodies With 3 Or Higher PowerLevel", 200, "");
            MODDEDENEMYPOWERLEVEL3_MIN = ConfigEntry("Body Values", "Min price of Modded Enemy Bodies With 3 Or Higher PowerLevel", 175, "");

            CENTIPEDE = ConfigEntry("Body Enabler", "Enable selling of Centipede Bodies", true, "");
            HOARDER = ConfigEntry("Body Enabler", "Enable selling of Hoarder Bodies", true, "");
            SPIDER = ConfigEntry("Body Enabler", "Enable selling of Spider Bodies", true, "");
            THUMPER = ConfigEntry("Body Enabler", "Enable selling of Thumper Bodies", true, "");
            NUTCRACKER = ConfigEntry("Body Enabler", "Enable selling of Nutcracker Bodies", true, "");
            MOUTHDOG = ConfigEntry("Body Enabler", "Enable selling of Eyeless Dog Bodies", true, "");
            BABOON = ConfigEntry("Body Enabler", "Enable selling of Baboon Hawk Bodies", true, "");
            BRACKEN = ConfigEntry("Body Enabler", "Enable selling of Bracken Bodies", true, "");
            COILHEAD = ConfigEntry("Body Enabler", "Enable selling of Coil-Head Bodies", true, "");
            GHOSTGIRL = ConfigEntry("Body Enabler", "Enable selling of Ghost Girl Bodies", true, "");
            FORESTKEEPER = ConfigEntry("Body Enabler", "Enable selling Forest Keeper Bodies", true, "");
            JESTER = ConfigEntry("Body Enabler", "Enable selling of Jester Bodies", true, "");
            HYGRODORE = ConfigEntry("Body Enabler", "Enable selling of Hygrodere Bodies", true, "");
            SPORELIZARD = ConfigEntry("Body Enabler", "Enable selling of Spore Lizard Bodies", true, "");
            MANTICOIL = ConfigEntry("Body Enabler", "Enable selling of Manticoil Bodies", true, "");
            RADMECH = ConfigEntry("Body Enabler", "Enable selling of Rad Mech Bodies", true, "");
            TULIPSNAKE = ConfigEntry("Body Enabler", "Enable selling of Tulip Snake Bodies", true, "");
            MODDEDENEMY = ConfigEntry("Body Enabler", "Enable selling of Modded Enemy Bodies", true, "");

            CENTIPEDE_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Centipede Bodies", false, "");
            HOARDER_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Hoarder Bodies", true, "");
            SPIDER_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Spider Bodies", true, "");
            THUMPER_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Thumper Bodies", true, "");
            NUTCRACKER_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Nutcracker Bodies", true, "");
            MOUTHDOG_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Eyeless Dog Bodies", true, "");
            BABOON_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Baboon Hawk Bodies", true, "");
            BRACKEN_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Bracken Bodies", true, "");
            COILHEAD_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Coil-Head Bodies", true, "");
            GHOSTGIRL_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Ghost Girl Bodies", false, "");
            FORESTKEEPER_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Forest Keeper Bodies", false, "");
            JESTER_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Jester Bodies", true, "");
            HYGRODORE_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Hygrodere Bodies", true, "");
            SPORELIZARD_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Spore Lizard Bodies", true, "");
            MANTICOIL_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Manticoil Bodies", false, "");
            RADMECH_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Rad Mech Bodies", false, "");
            TULIPSNAKE_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Tulip Snake Bodies", false, "");
            MODDEDENEMYPOWERLEVEL1_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Modded Enemy Bodies With 1 Or Less PowerLevel", false, "");
            MODDEDENEMYPOWERLEVEL2_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Modded Enemy Bodies With 2 PowerLevel", false, "");
            MODDEDENEMYPOWERLEVEL3_TWOHANDED = ConfigEntry("Body Two-Handed", "Enable Two-Handed Modded Enemy Bodies With 3 Or Higher PowerLevel", false, "");

            CENTIPEDE_WEIGHT = ConfigEntry("Body Weights", "Weight of Centipede Bodies", 1.35f, "");
            HOARDER_WEIGHT = ConfigEntry("Body Weights", "Weight of Hoarding Bug Bodies", 1.5f, "");
            SPIDER_WEIGHT = ConfigEntry("Body Weights", "Weight of Spider Bodies", 1.65f, "");
            THUMPER_WEIGHT = ConfigEntry("Body Weights", "Weight of Thumper Bodies", 2f, "");
            NUTCRACKER_WEIGHT = ConfigEntry("Body Weights", "Weight of Nutcracker Bodies", 1.9f, "");
            MOUTHDOG_WEIGHT = ConfigEntry("Body Weights", "Weight of Eyeless Dog Bodies", 2.5f, "");
            BABOON_WEIGHT = ConfigEntry("Body Weights", "Weight of Baboon Hawk Bodies", 2.15f, "");
            BRACKEN_WEIGHT = ConfigEntry("Body Weights", "Weight of Bracken Bodies", 1.75f, "");
            COILHEAD_WEIGHT = ConfigEntry("Body Weights", "Weight of Coil-Head Bodies", 1.95f, "");
            GHOSTGIRL_WEIGHT = ConfigEntry("Body Weights", "Weight of Ghost Girl Bodies", 1.2f, "");
            FORESTKEEPER_WEIGHT = ConfigEntry("Body Weights", "Weight of Forest Keeper Bodies", 1.85f, "");
            JESTER_WEIGHT = ConfigEntry("Body Weights", "Weight of Jester Bodies", 2.1f, "");
            HYGRODORE_WEIGHT = ConfigEntry("Body Weights", "Weight of Hygrodere Bodies", 1.6f, "");
            SPORELIZARD_WEIGHT = ConfigEntry("Body Weights", "Weight of Spore Lizard Bodies", 1.8f, "");
            MANTICOIL_WEIGHT = ConfigEntry("Body Weights", "Weight of Manticoil Bodies", 1.15f, "");
            RADMECH_WEIGHT = ConfigEntry("Body Weights", "Weight of Rad Mech Bodies", 1.5f, "");
            TULIPSNAKE_WEIGHT = ConfigEntry("Body Weights", "Weight of Tulip Snake Bodies", 1.4f, "");
            MODDEDENEMYPOWERLEVEL1_WEIGHT = ConfigEntry("Body Weights", "Weight of Modded Enemy Bodies With 1 Or Less PowerLevel", 1.35f, "");
            MODDEDENEMYPOWERLEVEL2_WEIGHT = ConfigEntry("Body Weights", "Weight of Modded Enemy Bodies With 2 PowerLevel", 1.65f, "");
            MODDEDENEMYPOWERLEVEL3_WEIGHT = ConfigEntry("Body Weights", "Weight of Modded Enemy Bodies With 3 Or Higher PowerLevel", 1.85f, "");

            MULTIPLIER_VALUE = ConfigEntry("Multiplier Options", "Multiplier Value", 4f, "Is used in the formula to calculate the multiplier (Muliplier = ((Total Power Count - Power Count Subtraction) / 100) * Multiplier Value))");
            MULTIPLIER_POWER_COUNT_SUBTRACTION = ConfigEntry("Multiplier Options", "Power Count Subtraction", 10f, "Is used in the formula to calculate the multiplier (Muliplier = ((Total Power Count - Power Count Subtraction) / 100) * Multiplier Value))");
            DISABLE_MULTIPLIER = ConfigEntry("Multiplier Options", "Disable Multiplier", false, "");
        }
    }
}
