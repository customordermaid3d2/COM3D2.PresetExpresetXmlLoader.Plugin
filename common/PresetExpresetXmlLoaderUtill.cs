using CM3D2.ExternalPreset.Managed;
using CM3D2.ExternalSaveData.Managed;
using COM3D2.LillyUtill;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.PresetExpresetXmlLoader.Plugin
{
    public class PresetExpresetXmlLoaderUtill
    {
        public static string[] item = new string[]
        {
                "WIDESLIDER.enable",
                "PROPSET_OFF.enable",
                "LIPSYNC_OFF.enable",
                "HYOUJOU_OFF.enable",
                "EYETOCAMERA_OFF.enable",
                "MUHYOU.enable",
                "FARMFIX.enable",
                "EYEBALL.enable",
                "EYE_ANG.enable",
                "PELSCL.enable",
                "SKTSCL.enable",
                "THISCL.enable",
                "THIPOS.enable",
                "PELVIS.enable",
                "FARMFIX.enable",
                "SPISCL.enable",
                "S0ASCL.enable",
                "S1_SCL.enable",
                "S1ASCL.enable",
                "FACE_OFF.enable",
                "FACEBLEND_OFF.enable",
                "FACE_ANIME_SPEED",
                "MABATAKI_SPEED",
                "PELVIS",
                "PELVIS.x",
                "PELVIS.y",
                "PELVIS.z"
        };

        private static string[][] boneAndPropNameList = new string[][]
{
            new string[]
            {
                "Bip01 ? Thigh",
                "THISCL"
            },
            new string[]
            {
                "momotwist_?",
                "MTWSCL"
            },
            new string[]
            {
                "momoniku_?",
                "MMNSCL"
            },
            new string[]
            {
                "Bip01 Pelvis_SCL_",
                "PELSCL"
            },
            new string[]
            {
                "Bip01 ? Thigh_SCL_",
                "THISCL2"
            },
            new string[]
            {
                "Bip01 ? Calf",
                "CALFSCL"
            },
            new string[]
            {
                "Bip01 ? Foot",
                "FOOTSCL"
            },
            new string[]
            {
                "Skirt",
                "SKTSCL"
            },
            new string[]
            {
                "Bip01 Spine_SCL_",
                "SPISCL"
            },
            new string[]
            {
                "Bip01 Spine0a_SCL_",
                "S0ASCL"
            },
            new string[]
            {
                "Bip01 Spine1_SCL_",
                "S1_SCL"
            },
            new string[]
            {
                "Bip01 Spine1a_SCL_",
                "S1ASCL"
            },
            new string[]
            {
                "Bip01 Spine1a",
                "S1ABASESCL"
            },
            new string[]
            {
                "Kata_?",
                "KATASCL"
            },
            new string[]
            {
                "Bip01 ? UpperArm",
                "UPARMSCL"
            },
            new string[]
            {
                "Bip01 ? Forearm",
                "FARMSCL"
            },
            new string[]
            {
                "Bip01 ? Hand",
                "HANDSCL"
            },
            new string[]
            {
                "Bip01 ? Clavicle",
                "CLVSCL"
            },
            new string[]
            {
                "Mune_?",
                "MUNESCL"
            },
            new string[]
            {
                "Mune_?_sub",
                "MUNESUBSCL"
            },
            new string[]
            {
                "Bip01 Neck_SCL_",
                "NECKSCL"
            }
};

        public static List<Itemp> itemps = new List<Itemp>();

        public class Itemp
        {
            public string name;
            public bool enable;

            public List<Item> items = new List<Item>();

            public Itemp(string name, params Item[] item)
            {
                this.name = name;
                if (item != null || item.Length > 0)
                {
                    items = item.ToList();
                }
            }
        }

        public class Item
        {
            public string name;
            public float defult;
            public float value;

            public Item(string name, float defult)
            {
                this.name = name;
                this.defult = defult;
                value = defult;
            }
        }

        public static void init()
        {
            string name;

            itemps.Add(new Itemp("WIDESLIDER"));

            name = "EYE_ANG";
            itemps.Add(new Itemp(name
                , new Item(name + ".angle", 0f)
                , new Item(name + ".x", 0f)
                , new Item(name + ".y", 0f)
                ));
            
            name = "EYEBALL";
            itemps.Add(new Itemp(name
                , new Item(name + ".width", 1f)
                , new Item(name + ".height", 1f)                
                ));

            name = "THISCL";
            itemps.Add(new Itemp(name
                , new Item(name + ".depth", 1f)
                , new Item(name + ".width", 1f)
                ));

            name = "THIPOS";
            itemps.Add(new Itemp(name
                , new Item(name + ".x", 0f)
                , new Item(name + ".z", 0f)
                //, new Item(name + ".y", 0f)
                ));

            SetItem("THI2POS");
            SetItem("HIPPOS");
            SetItem("MTWPOS");
            SetItem("MMNPOS");
            SetItem("SKTPOS");
            SetItem("SPIPOS");
            SetItem("S0APOS");
            SetItem("S1POS");
            SetItem("S1APOS");
            SetItem("NECKPOS");
            SetItem("CLVPOS");
            SetItem("MUNESUBPOS");
            SetItem("MUNEPOS");

            name = "PELSCL";
            itemps.Add(new Itemp(name
                , new Item(name + ".height", 1f)
                , new Item(name + ".depth", 1f)
                , new Item(name + ".width", 1f)
                ));
            
            name = "HIPSCL";
            itemps.Add(new Itemp(name
                , new Item(name + ".height", 1f)
                , new Item(name + ".depth", 1f)
                , new Item(name + ".width", 1f)
                ));

            foreach (var item in boneAndPropNameList)
            {
                name = item[1];
                itemps.Add(new Itemp(name
                , new Item(name + "L.height", 1f)
                , new Item(name + "L.depth", 1f)
                , new Item(name + "L.width", 1f)
                ));
            }

            PresetExpresetXmlLoader.myLog.LogInfo(itemps.Count());
            

        }

        private static void SetItem(string name)
        {
            itemps.Add(new Itemp(name
                , new Item(name + ".x", 0f)
                , new Item(name + ".z", 0f)
                , new Item(name + ".y", 0f)
                ));
        }

        public static void SetMaid(int seleted)
        {
            //MaidActivePatch.GetMaid(seleted)
            SetMaid(MaidActivePatch.GetMaid(seleted));
        }

        public static void SetMaid2(Maid maid)
        {
            if(MaidActivePatch.GetMaid(PresetExpresetXmlLoaderGUI.seleted)== maid)
            {
                PresetExpresetXmlLoader.myLog.LogInfo("SetMaid2", maid.status.fullNameEnStyle, itemps.Count());
                SetMaid(maid);
            }
        }

        public static void SetMaid(Maid maid)
        {
            //var maid = LillyUtill.MaidActivePatch.maids[PresetExpresetXmlLoaderGUI.seleted];
            if (maid == null)
            {
                PresetExpresetXmlLoader.myLog.LogWarning("SetMaid null");
                return;
            }            
            
            PresetExpresetXmlLoader.myLog.LogInfo("SetMaid-1",maid.status.fullNameEnStyle, itemps.Count());
            
            foreach (var itemp in itemps)
            {
                itemp.enable = ExSaveData.GetBool(maid, "CM3D2.MaidVoicePitch", itemp.name, false);

                PresetExpresetXmlLoader.myLog.LogInfo("SetMaid-2",maid.status.fullNameEnStyle, itemp.name, itemp.items.Count());
                foreach (Item item in itemp.items)
                {
                    item.value=ExSaveData.GetFloat(maid, "CM3D2.MaidVoicePitch", item.name, item.defult);
                }
            }
        }

        // ExSaveData.GetFloat(maid, MaidVoicePitch.PluginName, "EYE_ANG.angle", 0f);

        public static void Load(int maid, string strFileName)
        {
            //MyLog.LogMessage("Load : " + strFileName);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strFileName);
            if (xmlDocument == null)
            {
                return;
            }
            XmlNodeList nods = xmlDocument.SelectNodes("//plugin");
            if (nods == null || nods.Count == 0)
            {
                return;
            }
            Maid maid1 = MaidActivePatch.GetMaid(maid);
            if (maid1 == null)
            {
                return;
            }
            for (int i = 0; i < nods.Count; i++)
            {
                PresetExpresetXmlLoader.myLog.LogInfo(nods[i].Attributes["name"].Value);
                ExSaveData.SetXml(maid1, nods[i].Attributes["name"].Value, nods[i]);
            }
            maid1.body0.bonemorph.Blend();
            //MaidVoicePitch_UpdateSliders(LillyUtill.MaidActivePatch.maids[maid]);
        }


        public static void MaidVoicePitch_UpdateSliders(Maid stockMaid)
        {
            if (GameMain.Instance != null && GameMain.Instance.CharacterMgr != null)
            {
                if (stockMaid != null && stockMaid.body0 != null && stockMaid.body0.bonemorph != null)
                {
                    bool flag = true;
                    foreach (BoneMorphLocal boneMorphLocal in stockMaid.body0.bonemorph.bones)
                    {
                        if (boneMorphLocal.linkT == null)
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        try
                        {
                            float scale_Sintyou = stockMaid.body0.bonemorph.SCALE_Sintyou;
                            stockMaid.body0.BoneMorph_FromProcItem("sintyou", scale_Sintyou);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// xml 파일로 저장
        /// </summary>
        /// <param name="maid">선택된 grid 위치 번호</param>
        /// <param name="f_strFileName">저장될 파일명</param>
        public static void Save(int maid, string f_strFileName)
        {
            // MyLog.LogMessage("save : " + f_strFileName);
            Maid maid1 = MaidActivePatch.GetMaid(maid);
            if (maid1 == null)
            {
                return;
            }
            XmlDocument xmlDocument = new XmlDocument();
            bool flag2 = false;
            XmlNode xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement("plugins"));
            //AccessTools.Field(typeof(ExPreset), "exsaveNodeNameMap").GetValue(null)
            //var a= typeof(ExPreset).GetField("exsaveNodeNameMap").GetValue(null) as HashSet<string>;
            //if (a==null)
            //{
            //    MyLog.LogMessage("exsaveNodeNameMap null");
            //    return;
            //}
            foreach (string pluginName in new string[] { "CM3D2.MaidVoicePitch", "COM3D2.AutoConverter" })
            {
                XmlElement xmlElement = xmlDocument.CreateElement("plugin");

                if (ExSaveData.TryGetXml(maid1, pluginName, xmlElement))
                {
                    xmlNode.AppendChild(xmlElement);
                    flag2 = true;
                }
            }
            if (flag2)
            {
                xmlDocument.Save(f_strFileName);// 실제로 저장됨
            }

        }
    }
}
