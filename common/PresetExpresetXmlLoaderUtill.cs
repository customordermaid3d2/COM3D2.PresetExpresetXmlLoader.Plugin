using CM3D2.ExternalSaveData.Managed;
using LillyUtill.MyMaidActive;
//using COM3D2.LillyUtill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace COM3D2.PresetExpresetXmlLoader.Plugin
{
    public class PresetExpresetXmlLoaderUtill
    {

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
            itemps.Add(new Itemp("FARMFIX"));

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

            PresetExpresetXmlLoader.log.LogInfo(itemps.Count());
            

        }

        private static void SetItem(string name)
        {
            itemps.Add(new Itemp(name
                , new Item(name + ".x", 0f)
                , new Item(name + ".z", 0f)
                , new Item(name + ".y", 0f)
                ));
        }



        public static void SetMaid(Maid maid)
        {            
            if (maid == null)
            {
                PresetExpresetXmlLoader.log.LogWarning("SetMaid null");
                return;
            }                                   
            
            foreach (var itemp in itemps)
            {
                itemp.enable = ExSaveData.GetBool(maid, "CM3D2.MaidVoicePitch", itemp.name, false);
                                
                foreach (Item item in itemp.items)
                {
                    item.value=ExSaveData.GetFloat(maid, "CM3D2.MaidVoicePitch", item.name, item.defult);
                }
            }
        }


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
            Maid maid1 =MaidActiveUtill.GetMaid(maid);
            if (maid1 == null)
            {
                return;
            }
            for (int i = 0; i < nods.Count; i++)
            {
                PresetExpresetXmlLoader.log.LogInfo(nods[i].Attributes["name"].Value);
                ExSaveData.SetXml(maid1, nods[i].Attributes["name"].Value, nods[i]);
            }
            maid1.body0.bonemorph.Blend();
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
            Maid maid1 = MaidActiveUtill.GetMaid(maid);
            if (maid1 == null)
            {
                return;
            }
            XmlDocument xmlDocument = new XmlDocument();
            bool flag2 = false;
            XmlNode xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement("plugins"));

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
