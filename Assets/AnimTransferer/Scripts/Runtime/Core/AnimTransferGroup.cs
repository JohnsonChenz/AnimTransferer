using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimTransferer.Runtime
{
    [Serializable]
    public class AnimTransferGroup
    {
        public string groupName;
        public bool exportAnimAssetData;
        public bool findSimilarStateName;
        [DrawIf("findSimilarStateName", true)] public float similarity;
        [DrawIf("findSimilarStateName", true)] public bool replaceMatchedStateNameWithConfig;
        public List<ClipStatePair> clipStatePairs;
        public List<AnimAsset> animAssets;
    }
}
