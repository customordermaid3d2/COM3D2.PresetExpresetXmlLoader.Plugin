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
            if (LillyUtill.MaidActivePatch.maids[maid] == null)
            {
                return;
            }
            for (int i = 0; i < nods.Count; i++)
            {
                //MyLog.LogMessage(nods[i].Attributes["name"].Value);
                ExSaveData.SetXml(LillyUtill.MaidActivePatch.maids[maid], nods[i].Attributes["name"].Value, nods[i]);
            }
            MaidVoicePitch_UpdateSliders(LillyUtill.MaidActivePatch.maids[maid]);
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
            if (LillyUtill.MaidActivePatch.maids[maid] == null)
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

                if (ExSaveData.TryGetXml(LillyUtill.MaidActivePatch.maids[maid], pluginName, xmlElement))
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
