using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimTransferer.Runtime
{
    [Serializable]
    public class AnimAsset
    {
        [JsonIgnore] public List<AnimationClip> sourceAnimationClips;
        [JsonIgnore] public AnimatorController targetAnimatorController;

        public string controllerFileInfo { get; set; }
        public Dictionary<string, List<string>> dictSourceAnimationFileInfo { get; set; }
        public List<string> listSourceAnimationFileInfo { get; set; }

        public void AssignFileInfosByAssets()
        {
            if (this.targetAnimatorController != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(this.targetAnimatorController);
                string guid = AssetDatabase.AssetPathToGUID(assetPath);

                this.controllerFileInfo = $"{assetPath},{guid}";
            }

            if (this.sourceAnimationClips != null && this.sourceAnimationClips.Count > 0)
            {
                this.dictSourceAnimationFileInfo = new Dictionary<string, List<string>>();
                this.listSourceAnimationFileInfo = new List<string>();

                foreach (var clips in this.sourceAnimationClips)
                {
                    // 針對MainAsset，使用List儲存，直接寫入其路徑 & Guid
                    if (AssetDatabase.IsMainAsset(clips))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(clips);
                        string guid = AssetDatabase.AssetPathToGUID(assetPath);

                        this.listSourceAnimationFileInfo.Add($"{assetPath},{guid}");
                    }
                    // 針對SubAsset，使用Map儲存
                    // Key = SubAsset隸屬的MainAsset之路徑 & Guid
                    // Value = SubAsset名稱
                    else if (AssetDatabase.IsSubAsset(clips))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(clips);
                        string guid = AssetDatabase.AssetPathToGUID(assetPath);

                        if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(guid)) continue;

                        string key = $"{assetPath},{guid}";

                        if (!this.dictSourceAnimationFileInfo.ContainsKey(key))
                        {
                            this.dictSourceAnimationFileInfo.Add(key, new List<string>());
                        }

                        this.dictSourceAnimationFileInfo[key].Add(clips.name);
                    }
                }
            }
        }

        public bool AssignAssetsByFileInfos()
        {
            bool hasPathModified = false;

            string[] pathInfo;

            // 加載AnimatorController
            if (!string.IsNullOrEmpty(this.controllerFileInfo) && this.controllerFileInfo != "0")
            {
                pathInfo = this.controllerFileInfo.Split(',');
                if (pathInfo.Length == 2)
                {
                    bool success = true;

                    string path = pathInfo[0];
                    string guid = pathInfo[1];

                    // 嘗試使用資料存下的路徑加載
                    AnimatorController asset = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                    // 無法用資料存下的路徑取得，就嘗試透過Guid獲取路徑並加載
                    if (asset == null)
                    {
                        Debug.LogWarning($"【目標AnimatorController加載】路徑 : {path} 查找不到該資源，將嘗試使用GUID : {guid}查找");
                        path = AssetDatabase.GUIDToAssetPath(guid);

                        if (!string.IsNullOrEmpty(path))
                        {
                            Debug.Log($"<color=#00C114>【目標AnimatorController加載】透過GUID得出的新路徑 : {path}</color>");
                            asset = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                            if (asset == null) success = false;
                            else hasPathModified = true;
                        }
                        else
                        {
                            Debug.LogError($"【目標AnimatorController加載】無法透過GUID得出新路徑!! : {path}");
                            success = false;
                        }
                    }

                    if (success)
                    {
                        this.targetAnimatorController = asset;
                        Debug.Log($"<color=#00C114>【目標AnimatorController加載】動畫控制器組件加載成功!! 名稱 : {asset.name}, 路徑 : {path}, GUID : {guid}</color>");
                    }
                    else
                    {
                        Debug.LogError($"【目標AnimatorController加載】動畫控制器組件加載失敗!! 路徑 : {path}, GUID : {guid}");
                    }
                }
            }

            this.sourceAnimationClips = new List<AnimationClip>();

            // 加載為MainAsset的AnimationClip
            if (this.listSourceAnimationFileInfo != null && this.listSourceAnimationFileInfo.Count > 0)
            {
                foreach (var sourceAnimationFileInfo in this.listSourceAnimationFileInfo)
                {
                    pathInfo = sourceAnimationFileInfo.Split(',');
                    if (pathInfo.Length == 2)
                    {
                        bool success = true;

                        string path = pathInfo[0];
                        string guid = pathInfo[1];

                        // 嘗試使用資料存下的路徑加載
                        AnimationClip asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                        // 無法用資料存下的路徑取得，就嘗試透過Guid獲取路徑並加載
                        if (asset == null)
                        {
                            Debug.LogWarning($"【來源Animation加載】【MainAsset類型】路徑 : {path} 查找不到該資源，將嘗試使用GUID : {guid}查找");
                            path = AssetDatabase.GUIDToAssetPath(guid);

                            if (!string.IsNullOrEmpty(path))
                            {
                                Debug.Log($"<color=#00B6FF>【來源Animation加載】【MainAsset類型】透過GUID得出的新路徑 : {path}</color>");
                                asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                                if (asset == null) success = false;
                                else hasPathModified = true;
                            }
                            else
                            {
                                Debug.LogError($"【來源Animation加載】【MainAsset類型】無法透過GUID得出新路徑!! : {path}");
                                success = false;
                            }
                        }

                        if (success)
                        {
                            this.sourceAnimationClips.Add(asset);
                            Debug.Log($"<color=#00B6FF>【來源Animation加載】【MainAsset類型】AnimationClip組件加載成功!! 名稱 : {asset.name}, 路徑 : {path}, GUID : {guid}</color>");
                        }
                        else
                        {
                            Debug.LogError($"【來源Animation加載】【MainAsset類型】AnimationClip組件加載失敗!! 路徑 : {path}, GUID : {guid}");
                        }
                    }
                }
            }

            // 加載為SubAsset的AnimationClip
            if (this.dictSourceAnimationFileInfo != null && this.dictSourceAnimationFileInfo.Count > 0)
            {
                foreach (var element in this.dictSourceAnimationFileInfo)
                {
                    string fileInfo = element.Key;
                    List<string> listAnimationClipName = element.Value;

                    // 嘗試獲取SubAsset隸屬的MainAsset
                    pathInfo = fileInfo.Split(',');
                    if (pathInfo.Length == 2)
                    {
                        string path = pathInfo[0];
                        string guid = pathInfo[1];

                        // 取得該MainAsset底下所有的SubAsset
                        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

                        // 無法用資料存下的路徑取得，就嘗試透過Guid獲取路徑並加載
                        if (assets == null || assets.Length == 0)
                        {
                            Debug.LogWarning($"【來源Animation加載】【SubAsset類型】路徑 : {pathInfo[0]} 查找不到該資源，將嘗試使用GUID : {pathInfo[1]}查找");
                            path = AssetDatabase.GUIDToAssetPath(guid);
                            Debug.Log($"<color=#FFBA00>【來源Animation加載】【SubAsset類型】透過GUID得出的新路徑 : {path}</color>");
                            assets = AssetDatabase.LoadAllAssetsAtPath(path);
                        }

                        if (assets != null && assets.Length > 0)
                        {
                            // 迴圈遍歷SubAsset列表
                            foreach (var asset in assets)
                            {
                                // 只有類型為AnimationClip的SubAsset才可做比對
                                if (asset is AnimationClip)
                                {
                                    if (listAnimationClipName.Contains(asset.name))
                                    {
                                        // 名稱符合資料存下的Clip名稱就加入到List中
                                        this.sourceAnimationClips.Add((AnimationClip)asset);
                                        Debug.Log($"<color=#FFBA00>【來源Animation加載】【SubAsset類型】AnimationClip組件加載成功!! 名稱 : {asset.name}, 所屬物件路徑 : {pathInfo[0]}, 所屬物件GUID : {pathInfo[1]}</color>");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return hasPathModified;
        }
    }
}
