## CHANGELOG

### v1.1.4
- Added : Editor will now display double check dialog window when clicking button "Reset".
- Fixed : Hint dialog window doesn't display correctly when detected change of resources path while loading json config files.

### v1.1.3
- Modified : Converted json data will no longer being formatted.

### v1.1.2
- Added : Editor will now display dialog window after Transfer & Export Config

### v1.1.1
- Bug Fixes : Wrongly log print in TransferPorcessor.

### v1.1.0
#### Modification of AnimTransferGroup :
1. Modified variable name and tooltip.
2. Added variable **findSimilarClipName(bool)** and **preferedSimilarityOfClipName(float)**, work exactly like **findSimilarStateName(bool)** and **preferedSimilarityOfStateName(float)** but it's for clips that in source animations to compare with config of Clip-State-Pairs while transfering.

### v1.0.1
- Added tooltip to the AnimTransferGroup.

### v1.0.0
- An editor tool that allows you transfer Animation Clip into Animator States with batch processing.