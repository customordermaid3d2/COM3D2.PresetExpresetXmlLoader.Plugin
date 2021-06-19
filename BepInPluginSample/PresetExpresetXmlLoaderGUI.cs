using BepInEx;
using BepInEx.Configuration;
using BepInPluginSample;
using COM3D2.Lilly.Plugin;
using COM3D2API;
//using Ookii.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.PresetExpresetXmlLoader.Plugin
{
    public class PresetExpresetXmlLoaderGUI : MonoBehaviour
    {
        public static PresetExpresetXmlLoaderGUI instance;

        private static ConfigFile config;

        private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        private static int windowId = new System.Random().Next();

        private static Vector2 scrollPosition;

        // 위치 저장용 테스트 json
        public static MyWindowRect myWindowRect;

        public static bool IsOpen
        {
            get => myWindowRect.IsOpen;
            set => myWindowRect.IsOpen = value;
        }

        // GUI ON OFF 설정파일로 저장
        private static ConfigEntry<bool> IsGUIOn;

        public static bool isGUIOn
        {
            get => IsGUIOn.Value;
            set => IsGUIOn.Value = value;
        }

        public static System.Windows.Forms.OpenFileDialog openDialog;
        public static System.Windows.Forms.SaveFileDialog saveDialog;
        //VistaOpenFileDialog openDialog = new VistaOpenFileDialog();
        //VistaSaveFileDialog saveDialog = new VistaSaveFileDialog();


        private int seleted;
        private int all;

        internal static PresetExpresetXmlLoaderGUI Install(GameObject parent, ConfigFile config)
        {
            PresetExpresetXmlLoaderGUI.config = config;
            instance = parent.GetComponent<PresetExpresetXmlLoaderGUI>();
            if (instance == null)
            {
                instance = parent.AddComponent<PresetExpresetXmlLoaderGUI>();
                MyLog.LogMessage("PresetExpresetXmlLoaderGUI.Install", instance.name);
            }
            return instance;
        }

        public void Awake()
        {
            MyLog.LogMessage("PresetExpresetXmlLoaderGUI.OnEnable");

            myWindowRect = new MyWindowRect(config);
            IsGUIOn = config.Bind("GUI", "isGUIOn", false);
            ShowCounter = config.Bind("GUI", "isGUIOnKey", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftControl));
            SystemShortcutAPI.AddButton(MyAttribute.PLAGIN_FULL_NAME, new Action(delegate () { PresetExpresetXmlLoaderGUI.isGUIOn = !PresetExpresetXmlLoaderGUI.isGUIOn; }), MyAttribute.PLAGIN_NAME + " : " + ShowCounter.Value.ToString(), MyUtill.ExtractResource(COM3D2.PresetExpresetXmlLoader.Plugin.Properties.Resources.icon));


            openDialog = new System.Windows.Forms.OpenFileDialog()
            {
                DefaultExt = "xml",
                InitialDirectory = Path.Combine(GameMain.Instance.SerializeStorageManager.StoreDirectoryPath, "preset"),
                Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"
            };
            saveDialog = new System.Windows.Forms.SaveFileDialog()
            {
                DefaultExt = "xml",
                InitialDirectory = Path.Combine(GameMain.Instance.SerializeStorageManager.StoreDirectoryPath, "preset"),
                Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"
            };

        }

        public void OnEnable()
        {
            MyLog.LogMessage("PresetExpresetXmlLoaderGUI.OnEnable");

            PresetExpresetXmlLoaderGUI.myWindowRect.load();
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        public void Start()
        {
            MyLog.LogMessage("PresetExpresetXmlLoaderGUI.Start");
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PresetExpresetXmlLoaderGUI.myWindowRect.save();
        }

        private void Update()
        {
            //if (ShowCounter.Value.IsDown())
            //{
            //    MyLog.LogMessage("IsDown", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsPressed())
            //{
            //    MyLog.LogMessage("IsPressed", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            if (ShowCounter.Value.IsUp())
            {
                isGUIOn = !isGUIOn;
                MyLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            }
        }

        public void OnGUI()
        {
            if (!isGUIOn)
                return;

            //GUI.skin.window = ;

            //myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUI.skin.box);
            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        string[] type = new string[] { "one", "all" };

        public void WindowFunction(int id)
        {
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Label(MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { IsOpen = !IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn = false; }
            GUILayout.EndHorizontal();

            if (!IsOpen)
            {

            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                GUI.enabled = PresetExpresetXmlLoaderPatch.maids[seleted] != null;
                if (GUI.enabled)
                {
                    GUILayout.Label("");
                }
                else
                {
                    GUILayout.Label("maid null");
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("load"))
                {
                    if (!isShowDialogLoadRun)
                    {
                        //Task.Factory.StartNew(ShowDialogLoadRun);
                        ShowDialogLoadRun();
                    }
                }
                if (GUILayout.Button("save"))
                {
                    if (!isShowDialogSaveRun)
                    {
                        //Task.Factory.StartNew(ShowDialogSaveRun);
                        ShowDialogSaveRun();
                    }
                }

                GUILayout.EndHorizontal();

                GUI.enabled = true;
                GUILayout.Label("option");
                all = GUILayout.SelectionGrid(all, type, 2);
                if (all == 1)
                {
                    GUI.enabled = false;
                }
                GUILayout.Label("maid select");
                seleted = GUILayout.SelectionGrid(seleted, PresetExpresetXmlLoaderPatch.maidNames, 1);


                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        static bool isShowDialogSaveRun = false;
        static bool isShowDialogLoadRun = false;

        private void ShowDialogSaveRun()
        {
            isShowDialogSaveRun = true;
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (all == 0)
                {
                    PresetExpresetXmlLoaderUtill.Save(seleted, saveDialog.FileName);
                }
                else
                {
                    int s = saveDialog.FileName.LastIndexOf(".xml");
                    for (int i = 0; i < 18; i++)
                    {
                        PresetExpresetXmlLoaderUtill.Save(i, saveDialog.FileName.Insert(s, "_" + PresetExpresetXmlLoaderPatch.maidNames[i]));
                    }
                }
            }
            isShowDialogSaveRun = false;
        }

        private void ShowDialogLoadRun()
        {
            isShowDialogLoadRun = true;
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (all == 0)
                {
                    PresetExpresetXmlLoaderUtill.Load(seleted, openDialog.FileName);
                }
                else
                {
                    for (int i = 0; i < 18; i++)
                        PresetExpresetXmlLoaderUtill.Load(i, openDialog.FileName);
                }
            }
            isShowDialogLoadRun = false;
        }

        public void OnDisable()
        {

            PresetExpresetXmlLoaderGUI.myWindowRect.save();
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

    }
}
