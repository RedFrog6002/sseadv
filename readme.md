### About

This is a modified version of sseadv (by [nesrak1](https://github.com/nesrak1/)) to have export options that help with developing skins for Hollow Knight with Custom Knight Super Animation Addon (CKSAA).

[DOWNLOAD](https://github.com/RedFrog6002/sseadv/releases)

#### how to use (CKSAA):

**open level or sharedassets files with the open option**. one file opens a file you select, and all files open all level and sharedassets files in a folder. choosing automatically will try to find your game installation automatically and open a scene list. manually will let you choose the file yourself.

**save a custom spritesheet with the save option**. save like with normal ck, but use the options under "export custom sheet" (recommended to always export a animation)

**Export Options:**
- Auto-detect dimentions (Not recommended for large animations): export as close to a square grid as possible
    - If this is not checked, you will need to manually enter the dimentions for the grid you want
- Split multiple sheets by sprites (Recommended to be used with "Split by animations"): Splits into multiple custom sheets with x sprites each (or one animation each if using "Split by animations")
- Export all animations (Recommended for Knight): Will export every animation in the selected collection into the custom sheet (Only availible if exporting an animation)
    - Split by animations (Recommended): When used with "Export all animations" and "Split multiple sheets by sprites" will split every sheet by animations instead
- Export Json (CKSAA): Will export a json file for the original animation that can be used with CKSAA

#### how to use (Normal CK)

**open level or sharedassets files with the open option**. one file opens a file you select, and all files open all level and sharedassets files in a folder. choosing automatically will try to find your game installation automatically and open a scene list. manually will let you choose the file yourself.

**save sprites, sheets, and animations to pngs with the save option**. make sure to have whatever you want to save selected in the main form before you use this. (use options under "export original sprites")

**modify spritesheets with the edit option**. open the aaaaa_info.txt file that was created next to your exported sprites and look for an \_edit.png file once this is done.