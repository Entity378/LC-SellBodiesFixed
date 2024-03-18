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
        const string VERSION = "1.5.0";

        static string root = "Assets/CleaningAssets/";

        Dictionary<string, int> minBodyValues;

        Dictionary<string, bool> BodiesToDrop;

        Dictionary<string, int> maxBodyValues;

        Dictionary<string, float> bodyWeights;

        Dictionary<string, string> pathToName = new Dictionary<string, string>()
        {
            { root+"HoarderingBugBody.asset", "Hoarding bug" },
            { root+"BunkerSpiderBody.asset", "Bunker Spider" },
            { root+"CrawlerBody.asset", "Crawler" },
            { root+"CentipedeBody.asset", "Centipede"},
            { root+"NutcrackerBody.asset", "Nutcracker"},
            { root+"FlowerManBody.asset", "Flowerman"},
            { root+"BaboonHawkBody.asset", "Baboon hawk"},
            { root+"MouthDogBody.asset", "MouthDog"},
        };

        public Dictionary<string, Item> BodySpawns = new Dictionary<string, Item>();
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
                { root+"HoarderingBugBody.asset", cfg.HOARDER_WEIGHT },
                { root+"BunkerSpiderBody.asset", cfg.SPIDER_WEIGHT },
                { root+"CrawlerBody.asset", cfg.THUMPER_WEIGHT },
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER_WEIGHT },
                { root+"CentipedeBody.asset", cfg.CENTIPEDE_WEIGHT },
                { root+"FlowerManBody.asset", cfg.BRACKEN_WEIGHT},
                { root+"BaboonHawkBody.asset", cfg.BABOON_WEIGHT},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG_WEIGHT},
            };

            maxBodyValues = new Dictionary<string, int>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER_MAX },
                { root+"BunkerSpiderBody.asset", cfg.SPIDER_MAX },
                { root+"CrawlerBody.asset", cfg.THUMPER_MAX },
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER_MAX },
                { root+"CentipedeBody.asset", cfg.CENTIPEDE_MAX },
                { root+"FlowerManBody.asset", cfg.BRACKEN_MAX},
                { root+"BaboonHawkBody.asset", cfg.BABOON_MAX},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG_MAX},
            };

            minBodyValues = new Dictionary<string, int>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER_MIN },
                { root+"BunkerSpiderBody.asset", cfg.SPIDER_MIN },
                { root+"CrawlerBody.asset", cfg.THUMPER_MIN },
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER_MIN },
                { root+"CentipedeBody.asset", cfg.CENTIPEDE_MIN },
                { root+"FlowerManBody.asset", cfg.BRACKEN_MIN},
                { root+"BaboonHawkBody.asset", cfg.BABOON_MIN},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG_MIN},
            };
            BodiesToDrop = new Dictionary<string, bool>()
            {
                { root+"HoarderingBugBody.asset", cfg.HOARDER },
                { root+"BunkerSpiderBody.asset", cfg.SPIDER },
                { root+"CrawlerBody.asset", cfg.THUMPER },
                { root+"NutcrackerBody.asset", cfg.NUTCRACKER },
                { root+"CentipedeBody.asset", cfg.CENTIPEDE },
                { root+"FlowerManBody.asset", cfg.BRACKEN},
                { root+"BaboonHawkBody.asset", cfg.BABOON},
                { root+"MouthDogBody.asset", cfg.MOUTHDOG},
            };
        }
        void SetupScrap()
        {
            foreach (KeyValuePair<string, string> pair in pathToName)
            {
                Item body = bundle.LoadAsset<Item>(pair.Key);
                Utilities.FixMixerGroups(body.spawnPrefab);
                body.twoHanded = true;
                body.spawnPrefab.AddComponent<BodySyncer>();
                body.maxValue = maxBodyValues[pair.Key];
                body.minValue = minBodyValues[pair.Key];
                body.weight = bodyWeights[pair.Key];
                LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(body.spawnPrefab);
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