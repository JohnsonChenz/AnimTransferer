using AnimTransferer.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AnimTransferer.Editor
{
    public class AnimTransferer : EditorWindow
    {
        [SerializeField] private List<AnimTransferGroup> _listAnimTransferGroup = new List<AnimTransferGroup>();

        private SerializedObject _serializedObject;              // 序列化對象

        private SerializedProperty _propertyOfSerializedObject;  // 序列化對象之屬性

        private Vector2 _scrollPosition;                         // 捲動座標暫存

        private string _jsonFileFolderPath;
        private string _jsonExportFolderPath;
        private bool _autoScanJson;

        private const string _keyJsonFileFolderPath = "_jsonFileFolderPath";
        private const string _keyJsonExportFolderPath = "_jsonExportFolderPath";
        private const string _keyAutoScanJson = "_keyAutoScanJson";

        [MenuItem("Plugins/AnimationTransferer")]
        static void _Open()
        {
            var window = EditorWindow.GetWindow<AnimTransferer>();
            window._listAnimTransferGroup = new List<AnimTransferGroup>();
            window.name = "AnimationTransferer";
            window.minSize = new Vector2(652, 573);
            window.Show();
        }

        private async void OnEnable()
        {
            this._serializedObject = new SerializedObject(this);
            this._propertyOfSerializedObject = this._serializedObject.FindProperty("_listAnimTransferGroup");

            this._InitSettings();

            await Task.Yield();

            this._InitLoad();
        }

        private void OnDisable()
        {
            this._Release();
            GC.Collect();
        }

        private void OnGUI()
        {
            this._serializedObject.Update();

            this._DrawFileFunction();
            this._DrawScrollView();
            this._DrawCommonFunction();

            // 提交修改
            this._serializedObject.ApplyModifiedProperties();
        }

        private void _InitSettings()
        {
            this._jsonFileFolderPath = EditorPrefs.GetString(AnimTransferer._keyJsonFileFolderPath, Application.dataPath);
            this._jsonExportFolderPath = EditorPrefs.GetString(AnimTransferer._keyJsonExportFolderPath, Application.dataPath);
            this._autoScanJson = EditorPrefs.GetBool(AnimTransferer._keyAutoScanJson, false);
        }

        private void _InitLoad()
        {
            if (this._autoScanJson)
            {
                if(!string.IsNullOrEmpty(this._jsonFileFolderPath))
                {
                    this._LoadFolder(this._jsonFileFolderPath);
                }
            }
        }

        private void _DrawFileFunction()
        {
            using (var verticalScope = new EditorGUILayout.VerticalScope(AnimTransferer._SetBackgroundColor("#A68C61", 0.3f)))
            {
                using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("File Settings");
                    GUILayout.FlexibleSpace();
                }

                using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.TextField("Json File Folder", this._jsonFileFolderPath);

                    this._DrawBackground(() =>
                    {
                        if (GUILayout.Button("Open", GUILayout.Width(60)))
                        {
                            if (Directory.Exists(this._jsonFileFolderPath))
                            {
                                System.Diagnostics.Process.Start("explorer.exe", this._jsonFileFolderPath.Replace("/", "\\"));
                            }
                        }
                    },
                    "#2EB774"
                    );

                    this._DrawBackground(() =>
                    {
                        if (GUILayout.Button("Browse", GUILayout.Width(60)))
                        {
                            string path = EditorUtility.OpenFolderPanel("Browse Json File Folder", this._jsonFileFolderPath, null);
                            if (!string.IsNullOrEmpty(path))
                            {
                                this._jsonFileFolderPath = path;
                                EditorPrefs.SetString(AnimTransferer._keyJsonFileFolderPath, path);
                                this._LoadFolder(path);
                            }
                        }
                    },
                    "#70FFFF"
                    );

                    this._DrawBackground(() =>
                    {
                        if (GUILayout.Button("Browse Json To Load", GUILayout.Width(140)))
                        {
                            string path = EditorUtility.OpenFilePanel("Browse Json To Load", this._jsonFileFolderPath, null);

                            if (!string.IsNullOrEmpty(path))
                            {
                                this._listAnimTransferGroup.Clear();
                                this._LoadFile(path);
                            }
                        }
                    },
                    "#70FFFF"
                    );
                }

                using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.TextField("Json Export Folder", this._jsonExportFolderPath);

                    this._DrawBackground(() =>
                    {
                        if (GUILayout.Button("Open", GUILayout.Width(60)))
                        {
                            if (Directory.Exists(this._jsonExportFolderPath))
                            {
                                System.Diagnostics.Process.Start("explorer.exe", this._jsonExportFolderPath.Replace("/", "\\"));
                            }
                        }
                    },
                    "#2EB774"
                    );

                    this._DrawBackground(() =>
                    {
                        if (GUILayout.Button("Browse", GUILayout.Width(60)))
                        {
                            string path = EditorUtility.OpenFolderPanel("Browse Json Export Folder", this._jsonExportFolderPath, null);

                            if (!string.IsNullOrEmpty(path))
                            {
                                this._jsonExportFolderPath = path;
                                EditorPrefs.SetString(AnimTransferer._keyJsonExportFolderPath, path);
                            }
                        }
                    },
                    "#70FFFF"
                    );
                }

                using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    this._DrawBackground(() =>
                    {
                        if (GUILayout.Button("Reset", GUILayout.Width(60)))
                        {
                            if (!EditorUtility.DisplayDialog("Reset", "This action will clear all Anim Transfer Group data. Are you sure you want to do that ?", "Yes", "No"))
                            {
                                return;
                            }

                            this._listAnimTransferGroup.Clear();
                        }
                    },
                    Color.red
                    );

                    GUILayout.FlexibleSpace();

                    this._autoScanJson = GUILayout.Toggle(this._autoScanJson, "Auto Scan");
                    EditorPrefs.SetBool(AnimTransferer._keyAutoScanJson, this._autoScanJson);

                    this._DrawBackground(() =>
                    {
                        if (GUILayout.Button("Scan", GUILayout.Width(60)))
                        {
                            if (!string.IsNullOrEmpty(this._jsonFileFolderPath))
                            {
                                this._LoadFolder(this._jsonFileFolderPath);
                            }
                        }
                    },
                    Color.green
                    );
                }
            }
        }

        private void _DrawScrollView()
        {
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(this._scrollPosition, AnimTransferer._SetBackgroundColor("#015AB0", 0.3f), GUILayout.MaxHeight(1000)))
            {
                this._scrollPosition = scrollViewScope.scrollPosition;

                using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Transfer Groups");
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.PropertyField(this._propertyOfSerializedObject, true, GUILayout.MinWidth(300));
            }
        }

        private void _DrawCommonFunction()
        {
            using (var horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                this._DrawBackground(() =>
                {
                    if (GUILayout.Button("Export Json File", GUILayout.Height(40)))
                    {
                        if (!string.IsNullOrEmpty(this._jsonExportFolderPath))
                        {
                            if (!Directory.Exists(this._jsonExportFolderPath))
                            {
                                Directory.CreateDirectory(this._jsonExportFolderPath);
                                Debug.Log($"<color=#F5AB00>此路徑無資料夾 : {this._jsonExportFolderPath}，將建議一個</color>");
                            }
                            this._Export(this._jsonExportFolderPath);
                        }
                    }
                },
                Color.yellow
                );

                this._DrawBackground(() =>
                {
                    if (GUILayout.Button("Transfer", GUILayout.Height(40)))
                    {
                        if (this._listAnimTransferGroup.Count == 0) return;

                        TransferProcessor transferProcessor = new TransferProcessor();
                        transferProcessor.StartTransfer(this._listAnimTransferGroup);
                        EditorUtility.DisplayDialog("Transfer Complete", "Transfer Completed", "OK");
                    }
                },
                Color.green
                );
            }
        }

        private void _Export(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (this._listAnimTransferGroup.Count == 0) return;

                foreach (var animTransferData in this._listAnimTransferGroup)
                {
                    if (animTransferData.exportAnimAssetData)
                    {
                        foreach (var animAssetData in animTransferData.animAssets)
                        {
                            animAssetData.AssignFileInfosByAssets();
                        }
                    }
                    else
                    {
                        foreach (var animAssetData in animTransferData.animAssets)
                        {
                            animAssetData.listSourceAnimationFileInfo?.Clear();
                            animAssetData.dictSourceAnimationFileInfo?.Clear();
                        }
                    }

                    string json = JsonConvert.SerializeObject(animTransferData, Formatting.None);
                    string fullPath = $"{path}/AnimTransferGroup_{(string.IsNullOrEmpty(animTransferData.groupName) ? this._listAnimTransferGroup.IndexOf(animTransferData) + 1 : animTransferData.groupName)}.json";
                    File.WriteAllText(fullPath, json);

                    Debug.Log($"<color=#02E300>Json配置檔輸出成功!! 路徑: {fullPath} </color>");
                }

                EditorUtility.DisplayDialog("Export Complete", "Export Completed", "OK");
            }
        }

        private async void _LoadFolder(string folderPath)
        {
            List<FileInfo> listFileInfo = await this._GetFolderFiles(folderPath, ".json");
            if (listFileInfo == null || listFileInfo.Count == 0) return;

            this._listAnimTransferGroup.Clear();

            foreach (var fileInfo in listFileInfo)
            {
                string filePath = fileInfo.FullName;

                if(!string.IsNullOrEmpty(filePath))
                {
                    this._LoadFile(filePath);
                }
            }
        }

        private void _LoadFile(string filePath)
        {
            string json = File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(json))
            {
                AnimTransferGroup animationTransferGroup = JsonConvert.DeserializeObject<AnimTransferGroup>(json);
                if (animationTransferGroup == null) return;

                Debug.Log($"<color=#02E300>Json配置檔載入成功!! 路徑: {filePath} </color>");

                bool hasPathModified = false;

                foreach (var animAsset in animationTransferGroup.animAssets)
                {
                    Debug.Log($"<color=#F38674>>>>>>>>>>>>>>>>>>>>> 加載 Anim Assets : {animationTransferGroup.animAssets.IndexOf(animAsset)} <<<<<<<<<<<<<<<<<<<<</color>");
                    hasPathModified = animAsset.AssignAssetsByFileInfos();
                }

                if (hasPathModified)
                {
                    EditorUtility.DisplayDialog("Hint", $"Changes to the resource path are detected during the process of reading AnimatorController or AnimationClip through the configuration file: {filePath}, please consider re-export the Json Config file", "OK");
                }
                this._listAnimTransferGroup.Add(animationTransferGroup);
            }
        }

        private void _DrawBackground(Action action, string colorHex, float alpha = 1f)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color color);
            color.a = alpha;

            this._DrawBackground(action, color);
        }

        private void _DrawBackground(Action action, Color color)
        {
            Color bc = GUI.backgroundColor;
            GUI.backgroundColor = color;
            action?.Invoke();
            GUI.backgroundColor = bc;
        }

        private static GUIStyle _SetBackgroundColor(string colorHex, float alpha = 1f)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color color);
            color.a = alpha;

            return AnimTransferer._SetBackgroundColor(color);
        }

        private static GUIStyle _SetBackgroundColor(Color color)
        {
            GUIStyle style = new GUIStyle();
            Texture2D texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, color);
            texture.Apply();

            style.normal.background = texture;

            return style;
        }

        private void _Release()
        {
            this._listAnimTransferGroup = null;
            this._propertyOfSerializedObject = null;
            this._serializedObject = null;
        }

        private async Task<List<FileInfo>> _GetFolderFiles(string path, params string[] extensions)
        {
            List<FileInfo> listFileInfo = new List<FileInfo>();

            DirectoryInfo folder = new DirectoryInfo(path);
            if (!folder.Exists) return null;

            // 將獲取到的所有Excel檔放進List中
            listFileInfo.AddRange(this._GetFilesByExtensions(folder, extensions));

            // 迴圈搜尋子資料夾
            foreach (var info in folder.GetDirectories())
            {
                await Task.Yield();
                listFileInfo.AddRange(await this._GetFolderFiles(info.FullName, extensions));
            }

            return listFileInfo;
        }

        IEnumerable<FileInfo> _GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null) return null;

            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension.ToLower()) && !f.FullName.Contains("~$"));
        }
    }
}
