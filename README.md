# AnimTransferer for Unity
An editor tool that allows you transfer Animation Clip into Animator States with batch processing.

## Quick Installnation:

### Required for installation:
- Unity **2021.3.5** or higher

## How to use:
"Plugins -> AnimTransferer" to open it.
1. Click **Browse** to setup both source/output json file folder.
2. Add and setup your own anim transfer group.
3. When everything is set-up,click button **Transfer**.
4. AnimTransferer will replace the animation clips in the states of **TargetAnimatorController** in **AnimAssets** according to the config of **ClipStatePairs**
5. **(Optional)** Click button **Export Json File** to export json data somewhere you like. 

## Simple explanation of AnimTransferGroup :
**GroupName** : Define you group name, when group name isn't empty, the exported json will use it as one part of file name.  
**ExportAnimAssetData** : When enable, exported json will include anim asset's file path & guid as path info. Allow editor load asset back from these path when loading json data.  
**FindSimilarStateName** : When enable, AnimTransferer will find states in the target controller with similar name compared to the Clip-State-Pair and transfer motion into it.  
**Similarity(Show up when Find Similar State Name is enable)** : When the similarity between the name of State in the animator controller  and Clip-State-Pair is greater than the set Similarity value, the editor will treat it as a replaceable State.  
**ReplaceMatchedStateNameWithConfig(Show up when Find Similar State Name is enable)** : When enable, AnimTransferer will also replace matched state name from config.  
**ClipStatePairs(List)** : AnimTransferer will try finding clips in **SourceAnimationClips** in **AnimAssets** with **SourceClipName**,and then, find matched state name in the **TargetAnimatorController** in **AnimAssets** with **TargetStateName**,finally,replace matched state's motion's clip with matched clip.  
**AnimAssets(List)** : Drag and drop your **SourceAnimationClips** and **TargetAnimatorController** that you want to transfer here.

## Installation
### Install via git URL  
Add https://github.com/JohnsonChenz/AnimTransferer.git?path=Assets/Plugins/AnimTransferer to Package Manager.

## License
This library is under the MIT License.