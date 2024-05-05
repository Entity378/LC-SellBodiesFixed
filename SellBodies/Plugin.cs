using BepInEx;
using CleaningCompany.Misc;
using CleaningCompany.Monos;
using HarmonyLib;
using LethalLib.Modules;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CleaningCompany
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("evaisa.lethallib", "0.14.4")]
    public class Plugin : BaseUnityPlugin
    {
        readonly Harmony harmony = new Harmony(GUID);
        const string GUID = "Entity378.sellbodies";
        const string NAME = "Sell Bodies";
        const string VERSION = "1.8.4";

        static string root = "Assets/CleaningAssets/";

        Dictionary<string, int> minBodyValues;

        Dictionary<string, bool> BodiesToDrop;

        Dictionary<string, bool> BodiesTowHanded;

        Dictionary<string, int> maxBodyValues;

        Dictionary<string, float> bodyWeights;

        Dictionary<string, string> pathToName = new Dictionary<string, string>()
        {
            { root+"HoarderingBugBody.asset", "Hoarding bug"},
            { root+"BunkerSpiderBody.asset", "Bunker Spider"},
            { root+"CrawlerBody.asset", "Crawler"},
            { root+"CentipedeBody.asset", "Centipede"},
            { root+"NutcrackerBody.asset", "Nutcracker"},
            { root+"FlowerManBody.asset", "Flowerman"},
            { root+"BaboonHawkBody.asset", "Baboon hawk"},
            { root+"MouthDogBody.asset", "MouthDog"},
            { root+"SpringBody.asset", "Spring"},
            { root+"GirlBody.asset", "Girl"},
            { root+"ForestGiantBody.asset", "ForestGiant"},
            { root+"JesterBody.asset", "Jester"},
            { root+"BlobBody.asset", "Blob"},
            { root+"PufferBody.asset", "Puffer"},
            { root+"ManticoilBody.asset", "Manticoil"},
            { root+"RadMechBody.asset", "RadMech"},
            { root+"TulipSnakeBody.asset", "Tulip Snake"},
            { root+"ModdedEnemyPowerLevel1Body.asset", "ModdedEnemyPowerLevel1"},
            { root+"ModdedEnemyPowerLevel2Body.asset", "ModdedEnemyPowerLevel2"},
            { root+"ModdedEnemyPowerLevel3Body.asset", "ModdedEnemyPowerLevel3"},
        };

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
        };

        public List<GameObject> tools = new List<GameObject>();

        AssetBundle bundle;
        public static Plugin instance;

        public static PluginConfig cfg { get; private set; }

        void Awake()
        {
            cfg = new PluginConfig(base.Config);
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

            instance = this;

            ApplyConfig();
            SetupScrap();

            harmony.PatchAll();
            Logger.LogInfo($"Sell Bodies is patched!");
        }

        void ApplyConfig()
        {
            bodyWeights = new Dictionary<string, float>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER_WEIGHT},
                { root+"BunkerSpiderBody.asset", cfg.SPIDER_WEIGHT},
                { root+"CrawlerBody.asset", cfg.THUMPER_WEIGHT},
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER_WEIGHT},
                { root+"CentipedeBody.asset", cfg.CENTIPEDE_WEIGHT},
                { root+"FlowerManBody.asset", cfg.BRACKEN_WEIGHT},
                { root+"BaboonHawkBody.asset", cfg.BABOON_WEIGHT},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG_WEIGHT},
                { root+"SpringBody.asset", cfg.COILHEAD_WEIGHT},
                { root+"GirlBody.asset", cfg.GHOSTGIRL_WEIGHT},
                { root+"ForestGiantBody.asset", cfg.FORESTKEEPER_WEIGHT},
                { root+"JesterBody.asset", cfg.JESTER_WEIGHT},
                { root+"BlobBody.asset", cfg.HYGRODORE_WEIGHT},
                { root+"PufferBody.asset", cfg.SPORELIZARD_WEIGHT},
                { root+"ManticoilBody.asset", cfg.MANTICOIL_WEIGHT},
                { root+"RadMechBody.asset", cfg.RADMECH_WEIGHT},
                { root+"TulipSnakeBody.asset", cfg.TULIPSNAKE_WEIGHT},
                { root+"ModdedEnemyPowerLevel1Body.asset", cfg.MODDEDENEMYPOWERLEVEL1_WEIGHT},
                { root+"ModdedEnemyPowerLevel2Body.asset", cfg.MODDEDENEMYPOWERLEVEL2_WEIGHT},
                { root+"ModdedEnemyPowerLevel3Body.asset", cfg.MODDEDENEMYPOWERLEVEL3_WEIGHT},
            };

            maxBodyValues = new Dictionary<string, int>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER_MAX},
                { root+"BunkerSpiderBody.asset", cfg.SPIDER_MAX},
                { root+"CrawlerBody.asset", cfg.THUMPER_MAX},
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER_MAX},
                { root+"CentipedeBody.asset", cfg.CENTIPEDE_MAX},
                { root+"FlowerManBody.asset", cfg.BRACKEN_MAX},
                { root+"BaboonHawkBody.asset", cfg.BABOON_MAX},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG_MAX},
                { root+"SpringBody.asset", cfg.COILHEAD_MAX},
                { root+"GirlBody.asset", cfg.GHOSTGIRL_MAX},
                { root+"ForestGiantBody.asset", cfg.FORESTKEEPER_MAX},
                { root+"JesterBody.asset", cfg.JESTER_MAX},
                { root+"BlobBody.asset", cfg.HYGRODORE_MAX},
                { root+"PufferBody.asset", cfg.SPORELIZARD_MAX},
                { root+"ManticoilBody.asset", cfg.MANTICOIL_MAX},
                { root+"RadMechBody.asset", cfg.RADMECH_MAX},
                { root+"TulipSnakeBody.asset", cfg.TULIPSNAKE_MAX},
                { root+"ModdedEnemyPowerLevel1Body.asset", cfg.MODDEDENEMYPOWERLEVEL1_MAX},
                { root+"ModdedEnemyPowerLevel2Body.asset", cfg.MODDEDENEMYPOWERLEVEL2_MAX},
                { root+"ModdedEnemyPowerLevel3Body.asset", cfg.MODDEDENEMYPOWERLEVEL3_MAX},
            };

            minBodyValues = new Dictionary<string, int>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER_MIN},
                { root+"BunkerSpiderBody.asset", cfg.SPIDER_MIN},
                { root+"CrawlerBody.asset", cfg.THUMPER_MIN},
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER_MIN},
                { root+"CentipedeBody.asset", cfg.CENTIPEDE_MIN},
                { root+"FlowerManBody.asset", cfg.BRACKEN_MIN},
                { root+"BaboonHawkBody.asset", cfg.BABOON_MIN},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG_MIN},
                { root+"SpringBody.asset", cfg.COILHEAD_MIN},
                { root+"GirlBody.asset", cfg.GHOSTGIRL_MIN},
                { root+"ForestGiantBody.asset", cfg.FORESTKEEPER_MIN},
                { root+"JesterBody.asset", cfg.JESTER_MIN},
                { root+"BlobBody.asset", cfg.HYGRODORE_MIN},
                { root+"PufferBody.asset", cfg.SPORELIZARD_MIN},
                { root+"ManticoilBody.asset", cfg.MANTICOIL_MIN},
                { root+"RadMechBody.asset", cfg.RADMECH_MIN},
                { root+"TulipSnakeBody.asset", cfg.TULIPSNAKE_MIN},
                { root+"ModdedEnemyPowerLevel1Body.asset", cfg.MODDEDENEMYPOWERLEVEL1_MIN},
                { root+"ModdedEnemyPowerLevel2Body.asset", cfg.MODDEDENEMYPOWERLEVEL2_MIN},
                { root+"ModdedEnemyPowerLevel3Body.asset", cfg.MODDEDENEMYPOWERLEVEL3_MIN},

            };
            BodiesToDrop = new Dictionary<string, bool>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER},
                { root+"BunkerSpiderBody.asset", cfg.SPIDER},
                { root+"CrawlerBody.asset", cfg.THUMPER},
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER},
                { root+"CentipedeBody.asset", cfg.CENTIPEDE},
                { root+"FlowerManBody.asset", cfg.BRACKEN},
                { root+"BaboonHawkBody.asset", cfg.BABOON},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG},
                { root+"SpringBody.asset", cfg.COILHEAD},
                { root+"GirlBody.asset", cfg.GHOSTGIRL},
                { root+"ForestGiantBody.asset", cfg.FORESTKEEPER},
                { root+"JesterBody.asset", cfg.JESTER},
                { root+"BlobBody.asset", cfg.HYGRODORE},
                { root+"PufferBody.asset", cfg.SPORELIZARD},
                { root+"ManticoilBody.asset", cfg.MANTICOIL},
                { root+"RadMechBody.asset", cfg.RADMECH},
                { root+"TulipSnakeBody.asset", cfg.TULIPSNAKE},
                { root+"ModdedEnemyPowerLevel1Body.asset", cfg.MODDEDENEMY},
                { root+"ModdedEnemyPowerLevel2Body.asset", cfg.MODDEDENEMY},
                { root+"ModdedEnemyPowerLevel3Body.asset", cfg.MODDEDENEMY},
            };

            BodiesTowHanded = new Dictionary<string, bool>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER_TWOHANDED},
                { root+"BunkerSpiderBody.asset", cfg.SPIDER_TWOHANDED},
                { root+"CrawlerBody.asset", cfg.THUMPER_TWOHANDED},
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER_TWOHANDED},
                { root+"CentipedeBody.asset", cfg.CENTIPEDE_TWOHANDED},
                { root+"FlowerManBody.asset", cfg.BRACKEN_TWOHANDED},
                { root+"BaboonHawkBody.asset", cfg.BABOON_TWOHANDED},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG_TWOHANDED},
                { root+"SpringBody.asset", cfg.COILHEAD_TWOHANDED},
                { root+"GirlBody.asset", cfg.GHOSTGIRL_TWOHANDED},
                { root+"ForestGiantBody.asset", cfg.FORESTKEEPER_TWOHANDED},
                { root+"JesterBody.asset", cfg.JESTER_TWOHANDED},
                { root+"BlobBody.asset", cfg.HYGRODORE_TWOHANDED},
                { root+"PufferBody.asset", cfg.SPORELIZARD_TWOHANDED},
                { root+"ManticoilBody.asset", cfg.MANTICOIL_TWOHANDED},
                { root+"RadMechBody.asset", cfg.RADMECH_TWOHANDED},
                { root+"TulipSnakeBody.asset", cfg.TULIPSNAKE_TWOHANDED},
                { root+"ModdedEnemyPowerLevel1Body.asset", cfg.MODDEDENEMYPOWERLEVEL1_TWOHANDED},
                { root+"ModdedEnemyPowerLevel2Body.asset", cfg.MODDEDENEMYPOWERLEVEL2_TWOHANDED},
                { root+"ModdedEnemyPowerLevel3Body.asset", cfg.MODDEDENEMYPOWERLEVEL3_TWOHANDED},
            };
        }
        void SetupScrap()
        {
            foreach (KeyValuePair<string, string> pair in pathToName)
            {
                Item body = bundle.LoadAsset<Item>(pair.Key);
                Utilities.FixMixerGroups(body.spawnPrefab);
                body.twoHanded = BodiesTowHanded[pair.Key];
                body.spawnPrefab.AddComponent<BodySyncer>();
                body.maxValue = maxBodyValues[pair.Key];
                body.minValue = minBodyValues[pair.Key];
                body.weight = bodyWeights[pair.Key];
                NetworkPrefabs.RegisterNetworkPrefab(body.spawnPrefab);
                Items.RegisterItem(body);

                if (BodiesToDrop[pair.Key])
                {
                    Logger.LogInfo($"Set {pair.Value} to drop {body.itemName}");
                    BodySpawns.Add(pair.Value, body);
                }
                else
                {
                    Logger.LogInfo($"Disregarding {body.itemName} - disabled in config");
                }
            }
        }
    }
}
