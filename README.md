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
**GroupName(string)** : Define you group name, when group name isn't empty, the exported json will use it as one part of file name.  
**ExportAnimAssetData(bool)** : When enable, exported json will include anim asset's file path & guid as path info. Allow editor load asset back from these path when loading json data.  
**FindSimilarClipName(bool)** : When enable, Clip that in source animations with similar name compared to the config of Clip-State-Pairs will also regarded as transferable Clip.  
**PreferedSimilarityOfClipName(float)(Show up when Find Find Similar Clip Name is enable)** : When the similarity between the name of Clip and any Source Clip Name in Clip-State-Pairs is greater or equal to the set value, the editor will treat it as a transferable Clip.  
**FindSimilarStateName(bool)** : When enable, State that in the target controller with similar name compared to the config of Clip-State-Pairs will also regarded as transferable State.  
**PreferedSimilarityOfStateName(float)(Show up when Find Similar State Name is enable)** : When the similarity between the name of State and any Target Animator Controller Name in Clip-State-Pairs is greater or equal to the set value, the editor will treat it as a transferable State.  
**ReplaceMatchedStateNameWithConfig(bool)(Show up when Find Similar State Name is enable)** : When enable, matched State's name will replace with most similar name found in the config of Clip-State-Pairs.  
**ClipStatePairs(List)** : AnimTransferer will try finding clips in SourceAnimationClips in AnimAssets with SourceClipName,and then, find matched state name in the TargetAnimatorController in AnimAssets with TargetStateName,finally,replace matched state's motion's clip with matched clip.  
**AnimAssets(List)** : Drag and drop your **SourceAnimationClips** and **TargetAnimatorController** that you want to transfer here.

## Installation
### Install via git URL  
Add https://github.com/JohnsonChenz/AnimTransferer.git?path=Assets/Plugins/AnimTransferer to Package Manager.

## License
This library is under the MIT License.
