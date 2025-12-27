using BepInEx;
using CleaningCompany.Misc;
using HarmonyLib;
using LethalLib.Modules;
using SellBodies.Monos;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SellBodies
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("evaisa.lethallib", "0.16.1")]
    public class Plugin : BaseUnityPlugin
    {
        readonly Harmony harmony = new Harmony(GUID);
        const string GUID = "Entity378.sellbodies";
        const string NAME = "Sell Bodies";
        const string VERSION = "1.13.1";

        static string bodiesRoot = "Assets/LethalCompany/SellBodies/Items/";
        static string mainRoot = "Assets/LethalCompany/SellBodies/";

        public Dictionary<string, Item> BodySpawns = new Dictionary<string, Item>();

        public List<string> VanillaBody = new List<string>()
        {
            "Flowerman",
            "Crawler",
            "Hoarding bug",
            "Centipede",
            "Bunker Spider",
            "Puffer",
            "Jester",
            "Blob",
            "Girl",
            "Spring",
            "Nutcracker",
            "Masked",
            "Butler",
            "MouthDog",
            "Earth Leviathan",
            "ForestGiant",
            "Baboon hawk",
            "RadMech",
            "Red Locust Bees",
            "Docile Locust Bees",
            "Manticoil",
            "Tulip Snake",
            "Clay Surgeon",
            "Bush Wolf",
            "Maneater",
            "GiantKiwi"
        };

        public List<string> BlackListed = new List<string>();

        AssetBundle bundle;

        public static Plugin instance;

        public static PluginConfig cfg { get; private set; }

        public static GameObject ConfettiPrefab = new GameObject();

        public Item Gameboy;
        public Item GameboyCartridge;

        void Awake()
        {
            instance = this;

            cfg = new PluginConfig(Config);
            cfg.InitBindings();

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "sellbodies");
            bundle = AssetBundle.LoadFromFile(assetDir);

            ConfettiPrefab = bundle.LoadAsset<GameObject>(mainRoot + "KillEffects/ConfettiParticle.prefab");
            AudioClip Cheer = bundle.LoadAsset<AudioClip>(mainRoot + "KillEffects/Cheer.ogg");
            AudioClip Yippee = bundle.LoadAsset<AudioClip>(mainRoot + "KillEffects/Yippee.ogg");

            if (cfg.YIPPEE)
            {
                ConfettiPrefab.AddComponent<AudioSource>().clip = Yippee;
            }
            else
            {
                ConfettiPrefab.AddComponent<AudioSource>().clip = Cheer;
            }
            ConfettiPrefab.GetComponent<AudioSource>().playOnAwake = true;


            Gameboy = bundle.LoadAsset<Item>(mainRoot + "Gameboy/Gameboy.asset");
            GameboyCartridge = bundle.LoadAsset<Item>(mainRoot + "Gameboy/GameboyCartridge.asset");

            Utilities.FixMixerGroups(Gameboy.spawnPrefab);
            Utilities.FixMixerGroups(GameboyCartridge.spawnPrefab);

            NetworkPrefabs.RegisterNetworkPrefab(Gameboy.spawnPrefab);
            NetworkPrefabs.RegisterNetworkPrefab(GameboyCartridge.spawnPrefab);

            Items.RegisterShopItem(Gameboy, 20);
            Items.RegisterItem(GameboyCartridge);


            ApplyConfig();
            RegisterDefaultBodies();

            harmony.PatchAll();
            Logger.LogInfo($"Sell Bodies is patched!");
        }

        void ApplyConfig()
        {
            BlackListed.Clear();
            string[] blackList = cfg.BLACKLISTED.Split(',');

            foreach (string enemy in blackList)
            {
                string trimmed = enemy.Trim();
                if (trimmed.Length > 0)
                {
                    BlackListed.Add(trimmed);
                }
            }
        }

        void RegisterDefaultBodies()
        {
            RegisterBody("Hoarding bug", bodiesRoot + "HoarderingBugBody.asset", cfg.HOARDER_MIN, cfg.HOARDER_MAX, cfg.HOARDER_WEIGHT, cfg.HOARDER_TWOHANDED, cfg.HOARDER);
            RegisterBody("Bunker Spider", bodiesRoot + "BunkerSpiderBody.asset", cfg.SPIDER_MIN, cfg.SPIDER_MAX, cfg.SPIDER_WEIGHT, cfg.SPIDER_TWOHANDED, cfg.SPIDER);
            RegisterBody("Crawler", bodiesRoot + "CrawlerBody.asset", cfg.THUMPER_MIN, cfg.THUMPER_MAX, cfg.THUMPER_WEIGHT, cfg.THUMPER_TWOHANDED, cfg.THUMPER);
            RegisterBody("Centipede", bodiesRoot + "CentipedeBody.asset", cfg.CENTIPEDE_MIN, cfg.CENTIPEDE_MAX, cfg.CENTIPEDE_WEIGHT, cfg.CENTIPEDE_TWOHANDED, cfg.CENTIPEDE);
            RegisterBody("Nutcracker", bodiesRoot + "NutcrackerBody.asset", cfg.NUTCRACKER_MIN, cfg.NUTCRACKER_MAX, cfg.NUTCRACKER_WEIGHT, cfg.NUTCRACKER_TWOHANDED, cfg.NUTCRACKER);
            RegisterBody("Flowerman", bodiesRoot + "FlowerManBody.asset", cfg.BRACKEN_MIN, cfg.BRACKEN_MAX, cfg.BRACKEN_WEIGHT, cfg.BRACKEN_TWOHANDED, cfg.BRACKEN);
            RegisterBody("Baboon hawk", bodiesRoot + "BaboonHawkBody.asset", cfg.BABOON_MIN, cfg.BABOON_MAX, cfg.BABOON_WEIGHT, cfg.BABOON_TWOHANDED, cfg.BABOON);
            RegisterBody("MouthDog", bodiesRoot + "MouthDogBody.asset", cfg.MOUTHDOG_MIN, cfg.MOUTHDOG_MAX, cfg.MOUTHDOG_WEIGHT, cfg.MOUTHDOG_TWOHANDED, cfg.MOUTHDOG);
            RegisterBody("Spring", bodiesRoot + "SpringBody.asset", cfg.COILHEAD_MIN, cfg.COILHEAD_MAX, cfg.COILHEAD_WEIGHT, cfg.COILHEAD_TWOHANDED, cfg.COILHEAD);
            RegisterBody("Girl", bodiesRoot + "GirlBody.asset", cfg.GHOSTGIRL_MIN, cfg.GHOSTGIRL_MAX, cfg.GHOSTGIRL_WEIGHT, cfg.GHOSTGIRL_TWOHANDED, cfg.GHOSTGIRL);
            RegisterBody("ForestGiant", bodiesRoot + "ForestGiantBody.asset", cfg.FORESTKEEPER_MIN, cfg.FORESTKEEPER_MAX, cfg.FORESTKEEPER_WEIGHT, cfg.FORESTKEEPER_TWOHANDED, cfg.FORESTKEEPER);
            RegisterBody("Jester", bodiesRoot + "JesterBody.asset", cfg.JESTER_MIN, cfg.JESTER_MAX, cfg.JESTER_WEIGHT, cfg.JESTER_TWOHANDED, cfg.JESTER);
            RegisterBody("Blob", bodiesRoot + "BlobBody.asset", cfg.HYGRODORE_MIN, cfg.HYGRODORE_MAX, cfg.HYGRODORE_WEIGHT, cfg.HYGRODORE_TWOHANDED, cfg.HYGRODORE);
            RegisterBody("Puffer", bodiesRoot + "PufferBody.asset", cfg.SPORELIZARD_MIN, cfg.SPORELIZARD_MAX, cfg.SPORELIZARD_WEIGHT, cfg.SPORELIZARD_TWOHANDED, cfg.SPORELIZARD);
            RegisterBody("Manticoil", bodiesRoot + "ManticoilBody.asset", cfg.MANTICOIL_MIN, cfg.MANTICOIL_MAX, cfg.MANTICOIL_WEIGHT, cfg.MANTICOIL_TWOHANDED, cfg.MANTICOIL);
            RegisterBody("RadMech", bodiesRoot + "RadMechBody.asset", cfg.RADMECH_MIN, cfg.RADMECH_MAX, cfg.RADMECH_WEIGHT, cfg.RADMECH_TWOHANDED, cfg.RADMECH);
            RegisterBody("Tulip Snake", bodiesRoot + "TulipSnakeBody.asset", cfg.TULIPSNAKE_MIN, cfg.TULIPSNAKE_MAX, cfg.TULIPSNAKE_WEIGHT, cfg.TULIPSNAKE_TWOHANDED, cfg.TULIPSNAKE);
            RegisterBody("Clay Surgeon", bodiesRoot + "ClaySurgeonBody.asset", cfg.CLAYSURGEON_MIN, cfg.CLAYSURGEON_MAX, cfg.CLAYSURGEON_WEIGHT, cfg.CLAYSURGEON_TWOHANDED, cfg.CLAYSURGEON);
            RegisterBody("Bush Wolf", bodiesRoot + "BushWolfBody.asset", cfg.BUSHWOLF_MIN, cfg.BUSHWOLF_MAX, cfg.BUSHWOLF_WEIGHT, cfg.BUSHWOLF_TWOHANDED, cfg.BUSHWOLF);
            RegisterBody("Maneater", bodiesRoot + "CaveDwellerBody.asset", cfg.MANEATER_MIN, cfg.MANEATER_MAX, cfg.MANEATER_WEIGHT, cfg.MANEATER_TWOHANDED, cfg.MANEATER);
            RegisterBody("ModdedEnemyPowerLevel1", bodiesRoot + "ModdedEnemyPowerLevel1Body.asset", cfg.MODDEDENEMYPOWERLEVEL1_MIN, cfg.MODDEDENEMYPOWERLEVEL1_MAX, cfg.MODDEDENEMYPOWERLEVEL1_WEIGHT, cfg.MODDEDENEMYPOWERLEVEL1_TWOHANDED, cfg.MODDEDENEMY);
            RegisterBody("ModdedEnemyPowerLevel2", bodiesRoot + "ModdedEnemyPowerLevel2Body.asset", cfg.MODDEDENEMYPOWERLEVEL2_MIN, cfg.MODDEDENEMYPOWERLEVEL2_MAX, cfg.MODDEDENEMYPOWERLEVEL2_WEIGHT, cfg.MODDEDENEMYPOWERLEVEL2_TWOHANDED, cfg.MODDEDENEMY);
            RegisterBody("ModdedEnemyPowerLevel3", bodiesRoot + "ModdedEnemyPowerLevel3Body.asset", cfg.MODDEDENEMYPOWERLEVEL3_MIN, cfg.MODDEDENEMYPOWERLEVEL3_MAX, cfg.MODDEDENEMYPOWERLEVEL3_WEIGHT, cfg.MODDEDENEMYPOWERLEVEL3_TWOHANDED, cfg.MODDEDENEMY);
        }

        public void RegisterBody(string enemyName, string assetPath, int minValue, int maxValue, float weight, bool twoHanded, bool enabled = true)
        {
            if (string.IsNullOrWhiteSpace(enemyName) || string.IsNullOrWhiteSpace(assetPath))
            {
                Logger.LogWarning("Invalid body definition provided. Skipping.");
                return;
            }

            Item body = bundle.LoadAsset<Item>(assetPath);
            if (body == null)
            {
                Logger.LogWarning($"Could not load body asset at path: {assetPath}");
                return;
            }

            Utilities.FixMixerGroups(body.spawnPrefab);
            body.spawnPrefab.AddComponent<BodySyncer>();

            body.twoHanded = twoHanded;
            body.maxValue = maxValue;
            body.minValue = minValue;
            body.weight = weight;

            NetworkPrefabs.RegisterNetworkPrefab(body.spawnPrefab);
            Items.RegisterItem(body);

            if (enabled && !BlackListed.Contains(enemyName))
            {
                Logger.LogInfo($"Set {enemyName} to drop {body.itemName}");
                BodySpawns[enemyName] = body;
            }
            else
            {
                Logger.LogInfo($"Disregarding {body.itemName} - disabled in config");
            }
        }
    }
}