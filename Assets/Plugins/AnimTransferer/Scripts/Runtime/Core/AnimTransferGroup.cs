using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimTransferer.Runtime
{
    [Serializable]
    public class AnimTransferGroup
    {
        [Tooltip("Define you group name, when group name isn't empty, the exported json will use it as one part of file name")]
        public string groupName;
        [Tooltip("When enable, exported json will include anim asset's file path & guid as path info. Allow editor load asset back from these path when loading json data")]
        public bool exportAnimAssetData;
        [Tooltip("When enable, Clip that in source animations with similar name compared to the config of Clip-State-Pairs will also seen as transferable Clip")]
        public bool findSimilarClipName;
        [DrawIf("findSimilarClipName", true), Tooltip("When the similarity between the name of Clip and any Source Clip Name in Clip-State-Pairs is greater or equal to the set value, the editor will treat it as a transferable Clip")]
        public float preferedSimilarityOfClipName;
        [Tooltip("When enable, State that in the target controller with similar name compared to the config of Clip-State-Pairs will also seen as transferable State")]
        public bool findSimilarStateName;
        [DrawIf("findSimilarStateName", true), Tooltip("When the similarity between the name of State and any Target Animator Controller Name in Clip-State-Pairs is greater or equal to the set value, the editor will treat it as a transferable State")]
        public float preferedSimilarityOfStateName;
        [DrawIf("findSimilarStateName", true), Tooltip("When enable, matched State's name will replace with most similar name found in the config of Clip-State-Pairs")]
        public bool replaceMatchedStateNameWithConfig;
        [Tooltip("AnimTransferer will try finding clips in SourceAnimationClips in AnimAssets with SourceClipName,and then, find matched state name in the TargetAnimatorController in AnimAssets with TargetStateName,finally,replace matched state's motion's clip with matched clip")]
        public List<ClipStatePair> clipStatePairs;
        [Tooltip("Drag and drop your SourceAnimationClips and TargetAnimatorController that you want to transfer here")]
        public List<AnimAsset> animAssets;
    }
}
