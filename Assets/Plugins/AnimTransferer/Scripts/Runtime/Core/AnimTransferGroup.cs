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
        [Tooltip("When enable, AnimTransferer will find states in the target controller with similar name compared to the Clip-State-Pair and transfer motion into it")]
        public bool findSimilarStateName;
        [DrawIf("findSimilarStateName", true), Tooltip("When the similarity between the name of State in the animator controller and Clip-State-Pair is greater than the set Similarity value, the editor will treat it as a replaceable State")] 
        public float similarity;
        [DrawIf("findSimilarStateName", true), Tooltip("When enable, AnimTransferer will also replace matched state name from config")] 
        public bool replaceMatchedStateNameWithConfig;
        [Tooltip("AnimTransferer will try finding clips in SourceAnimationClips in AnimAssets with SourceClipName,and then, find matched state name in the TargetAnimatorController in AnimAssets with TargetStateName,finally,replace matched state's motion's clip with matched clip")]
        public List<ClipStatePair> clipStatePairs;
        [Tooltip("Drag and drop your SourceAnimationClips and TargetAnimatorController that you want to transfer here")]
        public List<AnimAsset> animAssets;
    }
}
