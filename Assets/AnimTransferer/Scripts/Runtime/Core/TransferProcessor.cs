using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimTransferer.Runtime
{
    public class TransferProcessor
    {
        public void StartTransfer(List<AnimTransferGroup> listAnimTransferGroup)
        {
            foreach (var animTransferGroup in listAnimTransferGroup)
            {
                Debug.Log($"<color=#00C114>>>>>>>>>>>>>>>>>>>>>GroupName : {animTransferGroup.groupName}<<<<<<<<<<<<<<<<<<<<<</color>");

                // 以StateClipPairs初始化一個Map，方便用於索引Controller內是否有和Map相同的State
                Dictionary<string, string> dictStateAnimationPair = animTransferGroup.clipStatePairs.ToDictionary(x => x.targetStateName, x => x.sourceClipName);

                foreach (var animAsset in animTransferGroup.animAssets)
                {
                    if (animAsset.targetAnimatorController == null) continue;

                    Debug.Log($"<color=#F38674>>>>>>>>>>>>>>>>>>>>> 轉換目標AnimatorController名稱 : {animAsset.targetAnimatorController.name}<<<<<<<<<<<<<<<<<<<<</color>");

                    // 1.從Controller內取得符合設定檔名稱的State
                    List<AnimatorState> listValidAnimationState = this._GetVaildAnimationStates(dictStateAnimationPair.Keys, animAsset.targetAnimatorController, animTransferGroup.findSimilarStateName, animTransferGroup.similarity, animTransferGroup.replaceMatchedStateNameWithConfig);

                    Debug.Log($"<color=#F5AB00>有效的State數量為 : {listValidAnimationState.Count}</color>");

                    foreach (var animationState in listValidAnimationState)
                    {
                        // 1-1.以設定檔的ClipName去Animation Clips資料中查找名稱最近似的Clip
                        string name = dictStateAnimationPair[animationState.name];
                        Debug.Log($"<color=#F5AB00>取得之Clip名稱為 : {name}</color>");

                        // 1-2 以名稱查找出名稱最相似的AnimationClip
                        AnimationClip animationClip = this._GetAnimationClipBySimilarName(animAsset.sourceAnimationClips, name);

                        // 1-3.指定給該Motion
                        if (animationClip != null)
                        {
                            animationState.motion = animationClip;
                            Debug.Log($"<color=#F5AB00>Motion置換成功!! StateName : {animationState.name}, ClipName : {animationClip.name}</color>");
                        }
                    }

                    // 完成後存檔
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animAsset.targetAnimatorController));
                    AssetDatabase.Refresh();
                }
            }
        }

        private List<AnimatorState> _GetVaildAnimationStates(Dictionary<string, string>.KeyCollection stateNames, AnimatorController animatorController, bool findSimilarStateName, float similarity, bool replaceFoundStateNameWithConfigs)
        {
            List<AnimatorState> listValidAnimationState = new List<AnimatorState>();

            // 1.檢查並取出Controller內符合有設定檔名稱的State
            // 層級 : Controller >>> Layer(Array) >>> StateMachine >>> States(Array) >>> 單一State
            foreach (var layer in animatorController.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    if (findSimilarStateName)
                    {
                        Debug.Log($"<color=#00D61B>>>>>>以相似度演算法查找AnimatorControllr內之State : {state.state.name}是否和Config設置之State名稱相似度 > {similarity}<<<<<</color>");

                        Dictionary<decimal, string> samples = new Dictionary<decimal, string>();

                        foreach (var stateName in stateNames)
                        {
                            decimal percentage = LevenshteinDistance.LevenshteinDistanceDecimal(stateName, state.state.name);
                            if (!samples.ContainsKey(percentage))
                            {
                                samples.Add(percentage, stateName);
                            }
                        }

                        if (samples.Count > 0)
                        {
                            decimal max = samples.Keys.Max();
                            if (max < (decimal)similarity)
                            {
                                Debug.Log($"<color=#00D61B>以名稱 {state.state.name} 查無相似Config設置之AnimationState，最大相似值 : {max}</color>");
                                continue;
                            }
                            string mostSimilarStateName = samples[max];
                            Debug.Log($"<color=#FF9000>以名稱 {state.state.name} 查找到最相似的Config設置之AnimationState名稱為 : {mostSimilarStateName}，最大相似值 : {max}</color>");

                            if (replaceFoundStateNameWithConfigs)
                            {
                                Debug.Log($"<color=#FF9000>替換State名稱 {state.state.name} 至 {mostSimilarStateName}</color>");
                                state.state.name = mostSimilarStateName;
                            }

                            listValidAnimationState.Add(state.state);
                        }
                    }
                    else
                    {
                        if (stateNames.Contains(state.state.name))
                        {
                            listValidAnimationState.Add(state.state);
                        }
                    }
                }
            }

            return listValidAnimationState;
        }

        private AnimationClip _GetAnimationClipBySimilarName(List<AnimationClip> listAnimationClips, string name)
        {
            Dictionary<decimal, AnimationClip> samples = new Dictionary<decimal, AnimationClip>();

            foreach (var animationClip in listAnimationClips)
            {
                decimal percentage = LevenshteinDistance.LevenshteinDistanceDecimal(animationClip.name, name);
                if (!samples.ContainsKey(percentage))
                {
                    samples.Add(percentage, animationClip);
                }
            }

            if (samples.Count > 0)
            {
                decimal max = samples.Keys.Max();
                if (max < 0.2m)
                {
                    Debug.Log($"<color=#F5AB00>以名稱 {name} 查無相似之AnimationClip，最大相似度 : {max}</color>");
                    return null;
                }
                AnimationClip clip = samples[max];
                Debug.Log($"<color=#F5AB00>以名稱 {name} 查找到最相似的AnimationClip名稱為 : {clip.name}</color>");
                return clip;
            }

            return null;
        }
    }
}
