using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using System.Threading.Tasks;
using DynamicData;
using Noggog;

namespace BookUUNPCBPCKeywords
{
    public class Program
    {
        private static void AddTag(Armor AEO, IKeywordGetter tag, List<IKeywordGetter> checks, bool doChecks = true)
        {
            if (AEO.Keywords == null)
            {
                AEO.Keywords!.Add(tag);
            }
            else
            {
                if (doChecks)
                {
                    foreach (var checktag in checks) // let's not confuse CBPC by assigning more than one override
                    {
                        if (checktag != tag && AEO.Keywords.Contains(checktag))
                        {
                            AEO.Keywords!.Remove(checktag);
                        }
                    }
                }
                if (!AEO.Keywords.Contains(tag))
                {
                    AEO.Keywords!.Add(tag);
                }
            }
        }

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            //Your code here!
            JObject stuff = JObject.Parse(File.ReadAllText(state.RetrieveInternalFile("bookuunp.json")));

            var jsonMergeSettings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            };

            string? idp = state.InternalDataPath;

            Console.WriteLine(idp + "\n");

            if (idp == null) idp = "C:/thiswonthappenbutmakethecompilerhappy/";

            List<string> allFiles = Directory.EnumerateFiles(idp).ToList();

            bool overrideExists = false;
            
            uint recordcount = 0;
            uint enchcount = 0;

            foreach (var file in allFiles)
            {
                if (file.EndsWith("override.json"))
                {
                    overrideExists = true;
                }
                else if (!file.EndsWith("bookuunp.json"))
                {
                    Console.WriteLine("extra file search found " + file);
                    
                    JObject morestuff = JObject.Parse(File.ReadAllText(state.RetrieveInternalFile(file)));
                    foreach (var property in morestuff)
                    {
                        if (property.Value != null && !stuff.ContainsKey(property.Key))
                        {
                            stuff.Add(property.Key, property.Value);
                            //Console.WriteLine("Merging " + property.Key + "\n");
                        }
                    }
                }
            }

            if (overrideExists)
            {
                foreach (var file in allFiles)
                {
                    if (file.EndsWith("override.json"))
                    {
                        Console.WriteLine("override file search found " + file);
                        int overridecount = 0;

                        JObject ORstuff = JObject.Parse(File.ReadAllText(state.RetrieveInternalFile(file)));
                        foreach (var property in ORstuff)
                        {
                            if (property.Value != null)
                            {
                                if (stuff.ContainsKey(property.Key))
                                {
                                    // override
                                    stuff[property.Key] = property.Value;
                                    overridecount++;
                                } 
                                else
                                {
                                    stuff.Add(property.Key, property.Value); // shouldn't encourage this but eh
                                }
                            }
                        }
                        Console.WriteLine(overridecount + " overrides applied");
                    }
                }
            }

            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsNakedL", out var AsNakedL); // use existing if already there
            if (AsNakedL == null)
            {
                AsNakedL = state.PatchMod.Keywords.AddNew("CBPCAsNakedL"); // make it if not
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsNakedR", out var AsNakedR);
            if (AsNakedR == null)
            {
                AsNakedR = state.PatchMod.Keywords.AddNew("CBPCAsNakedR");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsClothingL", out var AsClothingL);
            if (AsClothingL == null)
            {
                AsClothingL = state.PatchMod.Keywords.AddNew("CBPCAsClothingL");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsClothingR", out var AsClothingR);
            if (AsClothingR == null)
            {
                AsClothingR = state.PatchMod.Keywords.AddNew("CBPCAsClothingR");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsLightL", out var AsLightL);
            if (AsLightL == null)
            {
                AsLightL = state.PatchMod.Keywords.AddNew("CBPCAsLightL");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsLightR", out var AsLightR);
            if (AsLightR == null)
            {
                AsLightR = state.PatchMod.Keywords.AddNew("CBPCAsLightR");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsHeavyL", out var AsHeavyL);
            if (AsHeavyL == null)
            {
                AsHeavyL = state.PatchMod.Keywords.AddNew("CBPCAsHeavyL");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCAsHeavyR", out var AsHeavyR);
            if (AsHeavyR == null)
            {
                AsHeavyR = state.PatchMod.Keywords.AddNew("CBPCAsHeavyR");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCNoPushUpL", out var NoPushUpL);
            if (NoPushUpL == null)
            {
                NoPushUpL = state.PatchMod.Keywords.AddNew("CBPCNoPushUpL");
            }
            state.LinkCache.TryResolve<IKeywordGetter>("CBPCNoPushUpR", out var NoPushUpR);
            if (NoPushUpR == null)
            {
                NoPushUpR = state.PatchMod.Keywords.AddNew("CBPCNoPushUpR");
            }

            List<IKeywordGetter> lefties = new List<IKeywordGetter>
            {
                AsNakedL,
                AsClothingL,
                AsLightL,
                AsHeavyL
            };
            List<IKeywordGetter> righties = new List<IKeywordGetter>
            {
                AsNakedR,
                AsClothingR,
                AsLightR,
                AsHeavyR
            };

            Console.WriteLine("loaded " + stuff.Count + " EditorIDs");

            foreach (var armor in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                if (armor.EditorID != null)
                {
                    bool isEnch = armor.EditorID.StartsWith("Ench");
                    bool hasHooded = armor.EditorID.Contains("Hooded");
                    bool hasRobes = armor.EditorID.Contains("Robes");
                    bool hasCuirass = armor.EditorID.Contains("Cuirass");

                    string keycheck = armor.EditorID;
                    int endpos = 999;

                    if (isEnch)
                    {
                        if (hasHooded)
                        {
                            endpos = keycheck.IndexOf("Hooded") + 6;
                        }
                        else if (hasRobes)
                        {
                            endpos = keycheck.IndexOf("Robes") + 5;
                        }
                        else if (hasCuirass)
                        {
                            endpos = keycheck.IndexOf("Cuirass") + 7;
                        }
                        if (endpos > 4 && endpos < 999)
                        {
                            keycheck = armor.EditorID[4..endpos]; // match enchanted vanilla armor varieties to their common substring instead of an explicit editor ID
                        }
                    }

                    if (stuff.ContainsKey(keycheck))
                    {
                        JArray? tags = stuff[keycheck] as JArray;

                        if (tags != null)
                        {
                            if (isEnch)
                            {
                                enchcount++;
                            }
                            else
                            {
                                recordcount++;
                            }
                            var armorEditObj = state.PatchMod.Armors.GetOrAddAsOverride(armor);

                            string?[] tagstext = tags.Select(c => (string?)c).ToArray();

                            foreach (var tt in tagstext)
                            {

                                switch (tt)
                                {
                                    case "CBPCAsNakedL":
                                        AddTag(armorEditObj, AsNakedL, lefties);
                                        break;
                                    case "CBPCAsNakedR":
                                        AddTag(armorEditObj, AsNakedR, righties);
                                        break;
                                    case "CBPCAsClothingL":
                                        AddTag(armorEditObj, AsClothingL, lefties);
                                        break;
                                    case "CBPCAsClothingR":
                                        AddTag(armorEditObj, AsClothingR, righties);
                                        break;
                                    case "CBPCAsLightL":
                                        AddTag(armorEditObj, AsLightL, lefties);
                                        break;
                                    case "CBPCAsLightR":
                                        AddTag(armorEditObj, AsLightR, righties);
                                        break;
                                    case "CBPCAsHeavyL":
                                        AddTag(armorEditObj, AsHeavyL, lefties);
                                        break;
                                    case "CBPCAsHeavyR":
                                        AddTag(armorEditObj, AsHeavyR, righties);
                                        break;
                                    case "CBPCNoPushUpL":
                                        AddTag(armorEditObj, NoPushUpL, lefties, false);
                                        break;
                                    case "CBPCNoPushUpR":
                                        AddTag(armorEditObj, NoPushUpR, righties, false);
                                        break;

                                }
                            }
                        }

                    }
                }

            }
            //second pass stuff goes here if any
            Console.WriteLine("Processed " + recordcount + " armors plus " + enchcount + " loot variants");
        }
    }
}
