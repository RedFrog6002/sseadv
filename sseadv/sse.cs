using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sseadv
{
    public partial class sse : Form
    {
        private static string TITLE_DEF = "super-sprite-extractor advanced tk2d edition";

        private AssetsManager am;
        private Dictionary<AssetID, Bitmap> bitmapCache;
        //private AssetsFileInstance activeFile;
        //
        //private List<TkSpriteCollection> collections;
        //private List<TkSpriteAnimation> animations;
        //private List<TkSpriteAnimationClip> animationClips;
        //
        private FileSpriteData activeSpriteData;
        private TkSpriteCollection activeCollection;
        private TkSpriteAnimationClip activeAnimation;
        private TkSpriteDefinition activeSprite;
        private bool playingAnimation;
        private int activeFrame;
        private Timer animationTimer;
        private Dictionary<string, MonoCecilTempGenerator> monoGenerators = new Dictionary<string, MonoCecilTempGenerator>();

        private bool onMono;

        public sse()
        {
            InitializeComponent();

            onMono = Type.GetType("Mono.Runtime") != null;

            if (onMono)
                TITLE_DEF += " [compat mode]";

            am = new AssetsManager();
            am.LoadClassPackage("classdata.tpk");
            //am.useTemplateFieldCache = true;
            //am.MonoTempGenerator = new MonoCecilTempGenerator();
            am.UseMonoTemplateFieldCache = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("sse-adv v2", "about", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void automaticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckPathInfo())
                return;

            string gameDataPath = GetGamePath();

            if (string.IsNullOrEmpty(gameDataPath))
                return;
            
            UpdateMonoGenerator(gameDataPath);

            string selectedFile = PickSceneFile(gameDataPath);

            if (selectedFile != string.Empty)
            {
                LoadSpriteCollection(Path.Combine(gameDataPath, selectedFile));
            }
        }

        private void pickFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select a file from game_Data"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadSpriteCollection(ofd.FileName);
            }
        }

        private void automaticallyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckPathInfo())
                return;
        }

        private void pickFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string scanFolder = SelectFolder();
            
            if (scanFolder != string.Empty)
            {
                MessageBox.Show("this will take a while. please be patient.");
            
                string resourcesFileName = Path.Combine(scanFolder, "resources.assets");
                LoadSpriteCollection(resourcesFileName);
            
                int sceneIdx = 0;
                while (true)
                {
                    string levelFileName = Path.Combine(scanFolder, "level" + sceneIdx);
                    string saFileName = Path.Combine(scanFolder, "sharedassets" + sceneIdx + ".assets");
                    if (File.Exists(levelFileName))
                    {
                        if (File.Exists(levelFileName))
                        {
                            LoadSpriteCollection(levelFileName);
                        }
                        if (File.Exists(saFileName))
                        {
                            LoadSpriteCollection(saFileName);
                        }
                        sceneIdx++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox.Image = null;
            ResetUI();

            frameSlider.Value = 0;
            frameSlider.Enabled = false;

            if (animationTimer != null)
                StopAnimation();

            ClearTextures();
        }

        private void LoadSpriteCollection(string fileName)
        {
            Text = TITLE_DEF + " [loading " + Path.GetFileName(fileName) + "...]";

            AssetsFileInstance inst = am.LoadAssetsFile(fileName, true);
            am.LoadClassDatabaseFromPackage(inst.file.Metadata.UnityVersion);
            //UpdateDependency(am, inst);

            if (bitmapCache != null)
            {
                foreach (var entry in bitmapCache)
                {
                    entry.Value.Dispose();
                }
            }
            bitmapCache = new Dictionary<AssetID, Bitmap>();

            FileSpriteData data = GetTk2dSprites(inst);
            List<TkSpriteCollection> collections = data.collections;

            if (collections.Count > 0)
            {
                spriteCollectionsList.Items.Add(@"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                spriteCollectionsList.Items.Add(Path.GetFileName(fileName));
                spriteCollectionsList.Items.AddRange(collections.ToArray());
                spriteCollectionsList.Items.Add(@"//////////////////////////////");
            }

            Text = TITLE_DEF;
        }

        /*private void UpdateDependency(AssetsManager am, AssetsFileInstance ofFile)
        {
            for (int i = 0; i < ofFile.dependencies.Count; i++)
            {
                AssetsFileInstance depFile = ofFile.dependencies[i];
                if (depFile == null)
                {
                    AssetsFileInstance foundDepFile = SearchDependency(am, ofFile.file.dependencies.dependencies[i]);
                    ofFile.dependencies[i] = foundDepFile;
                    if (foundDepFile != null)
                    {
                        UpdateDependency(am, foundDepFile);
                    }
                }
            }
        }

        private AssetsFileInstance SearchDependency(AssetsManager am, AssetsFileDependency dep)
        {
            string matchName = Path.GetFileName(dep.assetPath.ToLower());
            return am.files.FirstOrDefault(f => matchName == Path.GetFileName(f.path.ToLower()));
        }*/

        private bool CheckPathInfo()
        {
            if (!File.Exists("pathinfo.txt"))
            {
                DialogResult res = MessageBox.Show("pathinfo.txt does not exist, would you like to create it?", "sse-adv",
                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    res = MessageBox.Show("are you using steam?", "sse-adv",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        File.WriteAllLines("pathinfo.txt", new[] { "steam", "appid", "game title", "game data folder name" });
                        MessageBox.Show("please fill in the values in pathinfo.txt with your game info", "sse-adv");
                        return false;
                    }
                    else
                    {
                        string newGameDataPath = SelectFolder("select game's _data path");
                        if (newGameDataPath != string.Empty)
                        {
                            File.WriteAllLines("pathinfo.txt", new[] { "path", newGameDataPath });
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private string GetGamePath()
        {
            string[] pathInfoCfg = File.ReadAllLines("pathinfo.txt");
            string gameDataPath;

            if (pathInfoCfg[0] == "steam")
            {
                int appId = int.Parse(pathInfoCfg[1]);
                string gameName = pathInfoCfg[2];
                string dataFolderName = pathInfoCfg[3];
                gameDataPath = Path.Combine(SteamHelper.FindSteamGamePath(appId, gameName), dataFolderName);
            }
            else if (pathInfoCfg[0] == "path")
            {
                gameDataPath = pathInfoCfg[1];
            }
            else
            {
                MessageBox.Show("pathinfo.txt invalid", "sse-adv");
                return null;
            }
            return gameDataPath;
        }

        private string PickSceneFile(string gameDataPath)
        {
            AssetsFileInstance inst = am.LoadAssetsFile(Path.Combine(gameDataPath, "globalgamemanagers"), false);
            am.LoadClassDatabaseFromPackage(inst.file.Metadata.UnityVersion);
            AssetFileInfo buildSettings = inst.file.GetAssetInfo(11);

            List<string> scenes = new List<string>();
            AssetTypeValueField baseField = am.GetBaseField(inst, buildSettings);
            AssetTypeValueField sceneArray = baseField.Get("scenes").Get("Array");
            for (int i = 0; i < sceneArray.Value.AsArray.size; i++)
            {
                scenes.Add(sceneArray[i].Value.AsString);
            }
            sceneselect sel = new sceneselect(scenes);
            sel.ShowDialog();

            return sel.selectedFile;
        }

        private FileSpriteData GetTk2dSprites(AssetsFileInstance inst)
        {
            List<TkSpriteCollection> collections = new List<TkSpriteCollection>();
            List<TkSpriteAnimation> animations = new List<TkSpriteAnimation>();
            List<TkSpriteAnimationClip> animationClips = new List<TkSpriteAnimationClip>();
            List<TkSpriteFrame> sprFrames = new List<TkSpriteFrame>();
            FileSpriteData spriteData = new FileSpriteData(collections, animations, animationClips);

            string managedPath = Path.Combine(Path.GetDirectoryName(inst.path), "Managed");
            UpdateMonoGenerator(managedPath);
        
            Dictionary<AssetID, TkSpriteCollection> collectionLookup = new Dictionary<AssetID, TkSpriteCollection>();
            int tk2dSCid = -1;
            int tk2dSAid = -1;
            foreach (AssetFileInfo mbInf in inst.file.GetAssetsOfType(0x72))
            {
                string scriptName = null;
                ushort scriptIndex = inst.file.GetScriptIndex(mbInf); // AssetHelper.GetScriptIndex(inst.file, mbInf);
                if (tk2dSCid != -1 && scriptIndex == tk2dSCid)
                {
                    scriptName = "tk2dSpriteCollectionData";
                }
                else if (tk2dSAid != -1 && scriptIndex == tk2dSAid)
                {
                    scriptName = "tk2dSpriteAnimation";
                }
        
                if (tk2dSCid == -1 || tk2dSAid == -1 && scriptName == null) //still looking for script ids
                {
                    AssetTypeValueField mbBase = am.GetBaseField(inst, mbInf);
                    AssetTypeValueField scBase = am.GetExtAsset(inst, mbBase.Get("m_Script")).baseField;
                    scriptName = scBase.Get("m_Name").Value.AsString;
                    if (scriptName == "tk2dSpriteCollectionData")
                        tk2dSCid = scriptIndex;
                    else if (scriptName == "tk2dSpriteAnimation")
                        tk2dSAid = scriptIndex;
                    else
                        continue; //nope, nobody cares
                }
        
                if (scriptName == null && (scriptName == "tk2dSpriteAnimation" || scriptName == "tk2dSpriteCollectionData"))
                    continue;

                AssetTypeValueField mbSerialBase = am.GetBaseField(inst, mbInf);
                if (scriptName == "tk2dSpriteCollectionData")
                {
                    AssetTypeValueField textures = mbSerialBase.Get("textures")[0];
        
                    List<AssetExternal> textureExts = new List<AssetExternal>();
                    List<int> textureWidths = new List<int>();
                    List<int> textureHeights = new List<int>();
                    for (int i = 0; i < textures.Children.Count; i++)
                    {
                        AssetExternal textureExt = am.GetExtAsset(inst, textures[i], true);
                        if (textureExt.info.ByteSize > 100000)
                        {
                            //bad news, unity probably stored the entire image into an array which is gonna
                            //take up too much memory when we decode it, so we'll change the data to a byte array
                            ClassDatabaseType textureType = am.ClassDatabase.FindAssetClassByID(textureExt.info.TypeId);
                            AssetTypeTemplateField textureTemp = new AssetTypeTemplateField();
                            textureTemp.FromClassDatabase(am.ClassDatabase, textureType);
                            AssetTypeTemplateField image_data = textureTemp.Children[textureTemp.Children.Count - 1];
                            image_data.ValueType = AssetValueType.ByteArray; //convert array to bytearray, much better
                            //AssetTypeInstance textureTypeInstance = new AssetTypeInstance(new[] {  }, inst.file.reader, textureExt.info.absoluteFilePos);
                            AssetTypeValueField textureBase = textureTemp.MakeValue(inst.file.Reader, textureExt.info.GetAbsoluteByteOffset(inst.file));
                            textureExt.baseField = textureBase;
                            textureExts.Add(textureExt);
                            textureWidths.Add(textureBase.Get("m_Width").Value.AsInt);
                            textureHeights.Add(textureBase.Get("m_Height").Value.AsInt);
                        }
                        else
                        {
                            textureExt = am.GetExtAsset(inst, textures[i]);
                            AssetTypeValueField textureBase = textureExt.baseField;
                            textureExts.Add(textureExt);
                            textureWidths.Add(textureBase.Get("m_Width").Value.AsInt);
                            textureHeights.Add(textureBase.Get("m_Height").Value.AsInt);
                        }
                    }
        
                    TkSpriteCollection collection = new TkSpriteCollection()
                    {
                        name = mbSerialBase.Get("spriteCollectionName").Value.AsString,
                        version = mbSerialBase.Get("version").Value.AsInt,
                        baseTexture = null, //do later
                        textures = new Dictionary<int, Bitmap>(), //same
                        textureExts = textureExts,
                        textureWidths = textureWidths,
                        textureHeights = textureHeights,
                        sprites = new List<TkSpriteDefinition>(),
                        baseFile = spriteData
                    };
                    collectionLookup[new AssetID(inst.name, mbInf.PathId)] = collection;
                    AssetTypeValueField spriteDefinitions = mbSerialBase.Get("spriteDefinitions")[0];
                    foreach (AssetTypeValueField def in spriteDefinitions.Children)
                    {
                        bool flipped = def.Get("flipped").Value.AsInt == 1;
                        int materialId = def.Get("materialId").Value.AsInt;
                        int textureWidth = textureWidths[materialId];
                        int textureHeight = textureHeights[materialId];
        
                        double uxn = double.MaxValue;
                        double uxp = 0;
                        double uyn = double.MaxValue;
                        double uyp = 0;
                        double pxn = double.MaxValue;
                        double pyn = double.MaxValue;
                        AssetTypeValueField positions = def.Get("positions")[0];
                        AssetTypeValueField uvs = def.Get("uvs")[0];
                        for (int i = 0; i < 4; i++)
                        {
                            AssetTypeValueField pos = positions[i];
                            AssetTypeValueField uv = uvs[i];
                            double posX = pos.Get("x").Value.AsFloat;
                            double posY = pos.Get("y").Value.AsFloat;
                            double uvX = Math.Round(uv.Get("x").Value.AsFloat * textureWidth);
                            double uvY = textureHeight - Math.Round(uv.Get("y").Value.AsFloat * textureHeight);
                            if (posX < pxn)
                                pxn = posX;
                            if (posY < pyn)
                                pyn = posY;
        
                            if (uvX < uxn)
                                uxn = uvX;
                            if (uvX > uxp)
                                uxp = uvX;
                            if (uvY < uyn)
                                uyn = uvY;
                            if (uvY > uyp)
                                uyp = uvY;
                        }
                        int spriteX = (int)uxn;
                        int spriteY = (int)uyn;
                        int spriteWidth = (int)(uxp - uxn);
                        int spriteHeight = (int)(uyp - uyn);
        
                        AssetTypeValueField boundsData = def.Get("boundsData");
                        AssetTypeValueField untrimmedBoundsData = def.Get("untrimmedBoundsData")[0];
                        AssetTypeValueField texelSize = def.Get("texelSize");
        
                        float untrimmedBoundsDataX = untrimmedBoundsData[0].Get("x").Value.AsFloat;
                        float untrimmedBoundsDataY = untrimmedBoundsData[0].Get("y").Value.AsFloat;
        
                        float texelX = texelSize.Get("x").Value.AsFloat;
                        float texelY = texelSize.Get("y").Value.AsFloat;
        
                        float realX = ((float)pxn) / texelX;
                        float realY = -((flipped ? spriteWidth : spriteHeight) + ((float)pyn) / texelY);

                        realX += untrimmedBoundsDataX / texelX;
                        realY += untrimmedBoundsDataY / texelY;
        
                        TkSpriteDefinition sprite = new TkSpriteDefinition()
                        {
                            parent = collection,
                            name = def.Get("name").Value.AsString,
                            x = spriteX,
                            y = spriteY,
                            width = spriteWidth,
                            height = spriteHeight,
                            xOff = realX,
                            yOff = realY,
                            materialId = materialId,
                            fullWidth = untrimmedBoundsData[1].Get("x").Value.AsFloat / texelX,
                            fullHeight = untrimmedBoundsData[1].Get("y").Value.AsFloat / texelY,
                            flipped = flipped
                        };
                        collection.sprites.Add(sprite);
                    }
                    collections.Add(collection);
                }
                else if (scriptName == "tk2dSpriteAnimation")
                {
                    AssetFileInfo gameObjectInfo = inst.file.GetAssetInfo(mbSerialBase.Get("m_GameObject").Get("m_PathID").Value.AsLong);
                    TkSpriteAnimation animation = new TkSpriteAnimation()
                    {
                        parents = new List<TkSpriteCollection>(), //do later
                        parentIds = new List<AssetID>(),
                        gameObjectName = AssetHelper.GetAssetNameFast(inst.file, am.ClassDatabase, gameObjectInfo),
                        clips = new List<TkSpriteAnimationClip>()
                    };
        
                    AssetTypeValueField clips = mbSerialBase.Get("clips")[0];
                    foreach (AssetTypeValueField clip in clips.Children)
                    {
                        TkSpriteAnimationClip aniClip = new TkSpriteAnimationClip()
                        {
                            parent = animation,
                            name = clip.Get("name").Value.AsString,
                            fps = clip.Get("fps").Value.AsFloat,
                            loopStart = clip.Get("loopStart").Value.AsInt,
                            wrapMode = (WrapMode)clip.Get("wrapMode").Value.AsInt,
                            frames = new List<TkSpriteFrame>()
                        };
                        animation.clips.Add(aniClip);
                        animationClips.Add(aniClip);
        
                        AssetTypeValueField frames = clip.Get("frames")[0];
                        foreach (AssetTypeValueField frame in frames.Children)
                        {
                            AssetExternal collectionExt = am.GetExtAsset(inst, frame.Get("spriteCollection"));
                            AssetID collectionId = new AssetID(collectionExt.file.name, collectionExt.info.PathId);
                            if (!animation.parentIds.Contains(collectionId))
                            {
                                animation.parentIds.Add(collectionId);
                            }
                            TkSpriteFrame sprFrame = new TkSpriteFrame()
                            {
                                collection = null, //do later
                                collectionId = collectionId,
                                spriteId = frame.Get("spriteId").Value.AsInt
                            };
                            sprFrames.Add(sprFrame);
                            aniClip.frames.Add(sprFrame);
                        }
                    }
                    animations.Add(animation);
                }
            }
        
            foreach (TkSpriteAnimation animation in animations)
            {
                foreach (AssetID parentId in animation.parentIds)
                {
                    if (collectionLookup.ContainsKey(parentId))
                    {
                        animation.parents.Add(collectionLookup[parentId]);
                    }
                }
            }

            foreach (TkSpriteFrame frame in sprFrames)
            {
                if (collectionLookup.ContainsKey(frame.collectionId))
                {
                    frame.collection = collectionLookup[frame.collectionId];
                }
            }

            return spriteData;
        }

        private void thisSpriteCollectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeCollection == null)
                return;
        
            string saveFolder = SelectFolder();
        
            if (saveFolder != string.Empty)
            {
                exportoptions options = new exportoptions(activeCollection.name, false);
                options.ShowDialog();
        
                if (options.DialogOk)
                {
                    if (options.ExportOriginal)
                    {
                        int counter = 0;
                        List<TkSpriteDefinition> sortedSpriteList = activeCollection.sprites.OrderBy(s => s.name).ToList();
                        List<string> infoLines = new List<string>();
                        infoLines.Add("version: 2");
                        infoLines.Add("type: collection");
                        infoLines.Add("name: " + activeCollection.name);
                        infoLines.Add("consistent_sprite_size: " + (options.ConsistentSpriteSize ? "true" : "false"));
                        infoLines.Add("enable_borders: " + (options.EnableBorders ? "true" : "false"));
                        infoLines.Add("use_sprite_name: " + (options.UseSpriteName ? "true" : "false"));
                        infoLines.Add("sprites: " + sortedSpriteList.Count);
                        foreach (TkSpriteDefinition sprite in sortedSpriteList)
                        {
                            string spriteName = sprite.name;
                            string fileName;
                            if (options.UseSpriteName)
                            {
                                if (sprite.name != "")
                                    fileName = $"{activeCollection.name}_{sprite.name}.png";
                                else
                                    fileName = $"{activeCollection.name}_unnamed_{counter}.png";
                            }
                            else
                            {
                                fileName = $"{activeCollection.name}_{counter.ToString().PadLeft(3, '0')}.png";
                            }
                            infoLines.Add(spriteName + "\t" + fileName);

                            Bitmap croppedImage = GetCroppedSprite(sprite, false, true, options.EnableBorders, options.ConsistentSpriteSize);
                            croppedImage.Save(Path.Combine(saveFolder, fileName));
                            counter++;
                        }

                        TkSpriteCollection col = activeCollection;
                        for (int i = 0; i < col.textureExts.Count; i++)
                        {
                            if (!col.textures.ContainsKey(i) || col.textures[i] == null)
                            {
                                col.textures[i] = GetTexture(col.textureExts[i]);
                            }
                            col.textures[i].Save(Path.Combine(saveFolder, $"{col.name}_full_{i}.png"));
                        }

                        File.WriteAllLines(Path.Combine(saveFolder, activeCollection.name + "_aaaaa_info.txt"), infoLines.ToArray());
                    }

                    if (options.CustomSpriteSheet)
                        exportAsCustomSpriteSheet(saveFolder, activeCollection, options);
                    //    exportAsCustomSpriteSheet(saveFolder, sortedSpriteList, options.SheetColumns, options.SheetRows, activeCollection.name);
                }
            }
        }
        
        private void thisAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeAnimation == null)
                return;
        
            string saveFolder = SelectFolder();
        
            if (saveFolder != string.Empty)
            {
                exportoptions options = new exportoptions(activeAnimation.name, true);
                options.ShowDialog();
        
                if (options.DialogOk)
                {
                    if (options.ExportOriginal)
                    {
                        int counter = 0;
                        List<string> infoLines = new List<string>();
                        infoLines.Add("version: 2");
                        infoLines.Add("type: animation");
                        infoLines.Add("name: " + activeAnimation.name);
                        infoLines.Add("consistent_sprite_size: " + (options.ConsistentSpriteSize ? "true" : "false"));
                        infoLines.Add("enable_borders: " + (options.EnableBorders ? "true" : "false"));
                        infoLines.Add("use_sprite_name: " + (options.UseSpriteName ? "true" : "false"));
                        infoLines.Add("sprites: " + activeAnimation.frames.Count);
                        foreach (TkSpriteFrame frame in activeAnimation.frames)
                        {
                            string spriteName = frame.collection.sprites[frame.spriteId].name;
                            string fileName;
                            if (options.UseSpriteName)
                            {
                                fileName = $"{activeAnimation.name}_{spriteName}_{counter.ToString().PadLeft(3, '0')}.png";
                            }
                            else
                            {
                                fileName = $"{activeAnimation.name}_{counter.ToString().PadLeft(3, '0')}.png";
                            }
                            infoLines.Add(counter + "\t" + fileName);

                            Bitmap croppedImage = GetCroppedSprite(frame.collection.sprites[frame.spriteId], false, true, options.EnableBorders, options.ConsistentSpriteSize);
                            croppedImage.Save(Path.Combine(saveFolder, fileName));
                            counter++;
                        }

                        TkSpriteCollection col = activeCollection;
                        for (int i = 0; i < col.textureExts.Count; i++)
                        {
                            if (!col.textures.ContainsKey(i) || col.textures[i] == null)
                            {
                                col.textures[i] = GetTexture(col.textureExts[i]);
                            }
                            col.textures[i].Save(Path.Combine(saveFolder, $"{col.name}_full_{i}.png"));
                        }

                        File.WriteAllLines(Path.Combine(saveFolder, activeCollection.name + "_aaaaa_info.txt"), infoLines.ToArray());
                    }

                    if (options.CustomSpriteSheet)
                        exportAsCustomSpriteSheet(saveFolder, activeAnimation, options);
                }
            }
        }

        private void exportAsCustomSpriteSheet(string saveFolder, TkSpriteCollection collection, exportoptions options)
        {
            Dictionary<string, List<TkSpriteDefinition>> defs = new Dictionary<string, List<TkSpriteDefinition>>();
            if (options.ExportSeparate && options.SheetAmt > 0)
            {
                int i = 0;
                List<TkSpriteDefinition> cur = null;
                while (i < collection.sprites.Count)
                {
                    if (cur == null)
                        cur = new List<TkSpriteDefinition>();
                    cur.Add(collection.sprites[i]);
                    if (cur.Count >= options.SheetAmt)
                    {
                        defs.Add(defs.Count.ToString(), cur);
                        cur = null;
                    }
                    i++;
                }
                if (cur != null)
                    defs.Add(defs.Count.ToString(), cur);
            }
            else
            {
                defs.Add("0", collection.sprites);
            }
            exportAsCustomSpriteSheet(saveFolder, defs, options.SheetColumns, options.SheetRows, collection.name, options.GenerateSimpleJSON);
        }

        private void exportAsCustomSpriteSheet(string saveFolder, TkSpriteAnimationClip clip, exportoptions options)
        {
            Dictionary<string, List<TkSpriteDefinition>> defs = new Dictionary<string, List<TkSpriteDefinition>>();
            string name = clip.name;
            if (options.ExportSeparate && ((options.AnimFullExport && options.AnimSplitByAnim) || options.SheetAmt > 0))
            {
                if ((!options.AnimFullExport || !options.AnimSplitByAnim) && options.SheetAmt > 0)
                {
                    int i = 0;
                    List<TkSpriteDefinition> cur = null;
                    while (i < clip.frames.Count)
                    {
                        if (cur == null)
                            cur = new List<TkSpriteDefinition>();
                        cur.Add(clip.frames[i].collection.sprites[clip.frames[i].spriteId]);
                        if (cur.Count >= options.SheetAmt)
                        {
                            defs.Add(defs.Count.ToString(), cur);
                            cur = null;
                        }
                        i++;
                    }
                    if (cur != null)
                        defs.Add(defs.Count.ToString(), cur);
                }
                else
                {
                    foreach (TkSpriteAnimationClip clip2 in clip.parent.clips)
                    {
                        string clipName = clip2.name;
                        if (defs.ContainsKey(clipName))
                        {
                            int cloneNum = 2;
                            clipName = $"{clip2.name} ({cloneNum})";
                            while (defs.ContainsKey(clipName))
                            {
                                cloneNum++;
                                clipName = $"{clip2.name} ({cloneNum})";
                            }
                        }
                        defs.Add(clipName, clip2.frames.Select(frame => frame.collection.sprites[frame.spriteId]).ToList());
                        if (options.GenerateSimpleJSON)
                        {
                            File.WriteAllText(Path.Combine(saveFolder, $"{clipName}-{CKSAAJson.AnimInfo.FileName}"), Newtonsoft.Json.JsonConvert.SerializeObject(
                                new CKSAAJson.AnimInfo() {useOriginalData = true, LoopType = clip2.wrapMode, loopStart = clip2.loopStart, fps = clip2.fps, InfoType = CKSAAJson.DefType.Simple}, Newtonsoft.Json.Formatting.Indented));
                        }
                    }
                    name = clip.parent.gameObjectName;
                }
            }
            else
            {
                if (options.AnimFullExport)
                {
                    defs.Add("0", clip.parent.clips.SelectMany(clip2 => clip2.frames.Select(frame => frame.collection.sprites[frame.spriteId])).ToList());
                    name = clip.parent.gameObjectName;
                }
                else
                {
                    defs.Add("0", clip.frames.Select(frame => frame.collection.sprites[frame.spriteId]).ToList());
                    if (options.GenerateSimpleJSON)
                    {
                        File.WriteAllText(Path.Combine(saveFolder, $"{clip.name}-{CKSAAJson.AnimInfo.FileName}"), Newtonsoft.Json.JsonConvert.SerializeObject(
                            new CKSAAJson.AnimInfo() {useOriginalData = true, LoopType = clip.wrapMode, loopStart = clip.loopStart, fps = clip.fps, InfoType = CKSAAJson.DefType.Simple}, Newtonsoft.Json.Formatting.Indented));
                    }
                }
            }
            exportAsCustomSpriteSheet(saveFolder, defs, options.SheetColumns, options.SheetRows, name, options.GenerateSimpleJSON);
        }

        private void exportAsCustomSpriteSheet(string saveFolder, Dictionary<string, List<TkSpriteDefinition>> defs, int columns, int rows, string name, bool json)
        {
            if (defs.Count == 1)
            {
                exportAsCustomSpriteSheet(saveFolder, defs.First().Value, columns, rows, name, json);
                return;
            }
            foreach (var sprites in defs)
            {
                exportAsCustomSpriteSheet(saveFolder, sprites.Value, columns, rows, name + "-" + sprites.Key, json);
            }
        }

        private void exportAsCustomSpriteSheet(string saveFolder, List<TkSpriteDefinition> defs, int columns, int rows, string name, bool json)
        {
            if (defs.Count <= 0)
                return;
            if (columns <= 0 && rows <= 0)
            {
                double rt = Math.Sqrt(defs.Count);
                columns = (int)Math.Ceiling(rt);
                rows = (int)Math.Floor(rt);
            }
            else if (columns <= 0)
                columns = (int)Math.Ceiling((double)defs.Count / (double)rows);
            else if (rows <= 0)
                rows = (int)Math.Ceiling((double)defs.Count / (double)columns);
            if (columns * rows < defs.Count)
                rows = (int)Math.Ceiling((double)defs.Count / (double)columns);
            float maxWidth = defs.Select(def => def.fullWidth).Max();
            float maxHeight = defs.Select(def => def.fullHeight).Max();
            int maxWidthInt = (int)Math.Round(maxWidth);
            int maxHeightInt = (int)Math.Round(maxHeight);
            int width = (int)Math.Ceiling(maxWidth * columns);
            int height = (int)Math.Ceiling(maxHeight * rows);
        
            Bitmap bitmap = new Bitmap(width, height);
        
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        if (defs.Count <= (y * columns) + x)
                            break;
                        Bitmap croppedImage = GetCroppedSprite(defs[(y * columns) + x], false, true, false, true);
                        int offsetX = (maxWidthInt - croppedImage.Width) / 2;
                        int offsetY = (maxHeightInt - croppedImage.Height) / 2;
                        graphics.DrawImage(croppedImage, new Rectangle((x * maxWidthInt) + offsetX, (y * maxHeightInt) + offsetY, croppedImage.Width, croppedImage.Height), new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), GraphicsUnit.Pixel);
                    }
                }
            }
            bitmap.Save(Path.Combine(saveFolder, $"{name}-CustomSheet-{defs.Count}.png"));
            if (json)
            {
                File.WriteAllText(Path.Combine(saveFolder, $"{name}-{CKSAAJson.SimpleInfo.FileName}"), Newtonsoft.Json.JsonConvert.SerializeObject(
                    new CKSAAJson.SimpleInfo() {columns = columns, rows = rows, frames = defs.Count, anchor = new CKSAAJson.Vector2(maxWidth / 2f, maxHeight / 2f)}, Newtonsoft.Json.Formatting.Indented));
            }
        }

        private void thisAtlasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeCollection == null)
                return;

            string saveFolder = SelectFolder();

            if (saveFolder != string.Empty)
            {
                TkSpriteCollection col = activeCollection;
                for (int i = 0; i < col.textureExts.Count; i++)
                {
                    if (!col.textures.ContainsKey(i) || col.textures[i] == null)
                    {
                        col.textures[i] = GetTexture(col.textureExts[i]);
                    }
                    col.textures[i].Save(Path.Combine(saveFolder, $"{col.name}_full_{i}.png"));
                }
            }
        }

        private void editThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeCollection == null)
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "please open the _info.txt file that goes with the currently selected collection";
            ofd.Filter = "sse sprite info files (*_info.txt)|*_info.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string infoFile = ofd.FileName;
                string saveFolder = Path.GetDirectoryName(ofd.FileName);

                string[] lines = File.ReadAllLines(infoFile);
                string versionLine = lines[0];
                string typeLine = lines[1];
                string nameLine = lines[2];
                string sett1Line = lines[3];
                string sett2Line = lines[4];
                string sett3Line = lines[5];
                string spritesLine = lines[6];

                if (!versionLine.StartsWith("version: ") ||
                    !typeLine.StartsWith("type: ") ||
                    !nameLine.StartsWith("name: ") ||
                    !sett1Line.StartsWith("consistent_sprite_size: ") ||
                    !sett2Line.StartsWith("enable_borders: ") ||
                    !sett3Line.StartsWith("use_sprite_name: ") ||
                    !spritesLine.StartsWith("sprites: "))
                {
                    MessageBox.Show("bad file - missing all header values", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                versionLine = versionLine.Substring(9);
                typeLine = typeLine.Substring(6);
                nameLine = nameLine.Substring(6);
                sett1Line = sett1Line.Substring(24);
                sett2Line = sett2Line.Substring(16);
                sett3Line = sett3Line.Substring(17);
                spritesLine = spritesLine.Substring(9);

                if (!int.TryParse(versionLine, out int version) ||
                    (typeLine != "animation" && typeLine != "collection") ||
                    nameLine == string.Empty ||
                    !bool.TryParse(sett1Line, out bool sett1) ||
                    !bool.TryParse(sett2Line, out bool sett2) ||
                    !bool.TryParse(sett3Line, out bool sett3) ||
                    !int.TryParse(spritesLine, out int spriteCount))
                {
                    MessageBox.Show("bad file - header had invalid data", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (version != 2)
                {
                    MessageBox.Show($"this file seems to be version {version} which is not supported.", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (typeLine == "collection")
                {
                    if (nameLine != activeCollection.name)
                    {
                        MessageBox.Show("this file's name doesn't seem to match the info file's name.", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    List<Bitmap> newTextures = new List<Bitmap>();
                    for (int i = 0; i < activeCollection.textureExts.Count; i++)
                    {
                        int width = activeCollection.textureWidths[i];
                        int height = activeCollection.textureHeights[i];
                        newTextures.Add(new Bitmap(width, height));
                    }

                    for (int i = 0; i < spriteCount; i++)
                    {
                        string entryLine = lines[i + 7];
                        string spriteName = entryLine.Split('\t')[0];
                        string spriteFile = entryLine.Split('\t')[1];
                        TkSpriteDefinition def = activeCollection.sprites.FirstOrDefault(s => s.name == spriteName);
                        if (def != null)
                        {
                            Bitmap newFile = (Bitmap)Image.FromFile(Path.Combine(saveFolder, spriteFile));
                            Bitmap oldFile = newTextures[def.materialId];
                            EditCroppedSprite(oldFile, newFile, def);
                        }
                        else
                        {
                            MessageBox.Show($"missing sprite {def}, skipping.", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    int counter = 0;
                    foreach (Bitmap texture in newTextures)
                    {
                        texture.Save(Path.Combine(saveFolder, $"{activeCollection.name}_{counter}_edit.png"));
                        counter++;
                    }
                }
                else if (typeLine == "animation")
                {
                    TkSpriteAnimationClip clip = activeSpriteData.animationClips.FirstOrDefault(a =>
                        a.parent.parents.Contains(activeCollection) && a.name == nameLine
                    );

                    if (clip == null)
                    {
                        MessageBox.Show("couldn't find this animation in the selected sprite collection.", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    List<Bitmap> newTextures = new List<Bitmap>();
                    TkSpriteCollection col = activeCollection;
                    for (int i = 0; i < col.textureExts.Count; i++)
                    {
                        int width = col.textureWidths[i];
                        int height = col.textureHeights[i];
                        if (!col.textures.ContainsKey(i) || col.textures[i] == null)
                        {
                            col.textures[i] = GetTexture(col.textureExts[i]);
                        }
                        newTextures.Add(new Bitmap(col.textures[i]));
                    }

                    HashSet<string> setFrames = new HashSet<string>();
                    for (int i = 0; i < spriteCount; i++)
                    {
                        string entryLine = lines[i + 7];
                        string animationIndexStr = entryLine.Split('\t')[0];
                        string spriteFile = entryLine.Split('\t')[1];
                        if (!int.TryParse(animationIndexStr, out int animationIndex))
                        {
                            MessageBox.Show("bad file - couldn't parse index", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (animationIndex >= clip.frames.Count)
                        {
                            MessageBox.Show("sprite index out of bounds", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (setFrames.Contains(spriteFile))
                        {
                            continue;
                        }
                        setFrames.Add(spriteFile);
                        TkSpriteFrame frame = clip.frames[animationIndex];
                        TkSpriteDefinition def = frame.collection.sprites[frame.spriteId];
                        if (def != null)
                        {
                            Bitmap newFile = (Bitmap)Image.FromFile(Path.Combine(saveFolder, spriteFile));
                            Bitmap oldFile = newTextures[def.materialId];
                            EditCroppedSprite(oldFile, newFile, def, true);
                        }
                        else
                        {
                            MessageBox.Show($"missing sprite {def}, skipping.", "sse-adv", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    int counter = 0;
                    foreach (Bitmap texture in newTextures)
                    {
                        texture.Save(Path.Combine(saveFolder, $"{activeCollection.name}_{counter}_edit.png"));
                        counter++;
                    }
                }
            }
        }

        private void saveThisFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeCollection == null)
                return;
        
            string saveFolder = SelectFolder();
        
            if (saveFolder != string.Empty && framesList.SelectedItem is TkSpriteDefinition spriteDef)
            {
                Bitmap croppedImage = GetCroppedSprite(spriteDef, false, true, true, true);
                croppedImage.Save(Path.Combine(saveFolder, $"{spriteDef.name}.png"));
            }
        }

        private void spriteCollectionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (animationTimer != null)
                StopAnimation();
        
            frameSlider.Value = 0;
            frameSlider.Enabled = false;
        
            object item = spriteCollectionsList.SelectedItem;
            if (item is TkSpriteCollection col)
            {
                FileSpriteData newSpriteData = col.baseFile;
                if (activeSpriteData != null && activeSpriteData != newSpriteData && activeSpriteData.collections != null)
                {
                    //clean textures from files we aren't looking at right now
                    //(do we really need to do this? is the less memory worth the more time?)
                    ClearTextures();
                }
                activeSpriteData = newSpriteData;

                if (col.baseTexture == null)
                {
                    Text = TITLE_DEF + " [loading texture...]";
                    if (col.textureExts.Count > 0)
                        col.baseTexture = GetTexture(col.textureExts[0]);
                    else
                        col.baseTexture = null;
                    Text = TITLE_DEF;
                }
                activeCollection = col;
        
                List<TkSpriteAnimationClip> colAnis = activeSpriteData.animationClips.Where(a => a.parent.parents.Contains(col)).ToList();
        
                framesList.Items.Clear();
                framesList.Items.AddRange(col.sprites.OrderBy(s => s.name).ToArray());
                animationsList.Items.Clear();
                animationsList.Items.Add("[none]");
                animationsList.Items.AddRange(colAnis.OrderBy(a => a.name).ToArray());
                pictureBox.Image = col.baseTexture;
            }
        }
        private void framesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            object item = framesList.SelectedItem;
            if (item is TkSpriteDefinition def)
            {
                int materialId = def.materialId;
                TkSpriteCollection col = def.parent;
                if (!col.textures.ContainsKey(materialId) || col.textures[materialId] == null)
                {
                    col.textures[def.materialId] = GetTexture(col.textureExts[materialId]);
                }

                Bitmap croppedSprite = GetCroppedSprite(def, true, false, false, true);
                def.cropCache = croppedSprite;
                //if (def.cropCache != null)
                //{
                //    pictureBoxBitmap = def.cropCache;
                //}
                //else
                //{
                //    pictureBoxBitmap = GetCroppedSprite(def, true, false, false, true);
                //    def.cropCache = pictureBoxBitmap;
                //}

                pictureBox.Image = croppedSprite;

                //clean cropped sprite from last frame
                if (activeSprite != null && activeSprite != def && activeSprite.cropCache != null)
                {
                    activeSprite.cropCache.Dispose();
                    activeSprite.cropCache = null;
                }
                activeSprite = def;
            }
        }

        private void animationsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (animationTimer != null)
                StopAnimation();
        
            object item = animationsList.SelectedItem;
            if (item is TkSpriteAnimationClip anim)
            {
                framesList.Items.Clear();
                foreach (TkSpriteFrame frame in anim.frames)
                {
                    if (frame.spriteId < frame.collection.sprites.Count)
                        framesList.Items.Add(frame.collection.sprites[frame.spriteId]);
                    else
                        framesList.Items.Add("[nonexistant sprite]");
                }
                frameSlider.Minimum = 0;
                frameSlider.Maximum = anim.frames.Count - 1;
                frameSlider.Enabled = true;
                frameInfo.Text = $"{anim.name} at {anim.fps} fps";
                activeAnimation = anim;
            }
            else
            {
                object colItem = spriteCollectionsList.SelectedItem;
                if (colItem is TkSpriteCollection col)
                {
                    if (col.baseTexture == null)
                    {
                        col.baseTexture = GetTexture(col.textureExts[0]);
                    }
        
                    List<TkSpriteAnimationClip> colAnis = activeSpriteData.animationClips.Where(a => a.parent.parents.Contains(col)).ToList();
        
                    framesList.Items.Clear();
                    framesList.Items.AddRange(col.sprites.OrderBy(s => s.name).ToArray());
                    pictureBox.Image = col.baseTexture;
                }
                frameSlider.Enabled = false;
                frameInfo.Text = "nothing playing";
                activeAnimation = null;
            }
        
            activeFrame = 0;
            if (framesList.Items.Count > 0)
                framesList.SelectedIndex = 0;
            if (frameSlider.Maximum > 0)
                frameSlider.Value = 0;
        }

        private void playFrames_Click(object sender, EventArgs e)
        {
            if (!playingAnimation)
            {
                if (activeAnimation == null)
                    return;
        
                playingAnimation = true;
                activeFrame = 0;
                framesList.SelectedIndex = activeFrame;
                frameSlider.Value = activeFrame;
                animationTimer = new Timer()
                {
                    Interval = (int)(1f / activeAnimation.fps * 1000f)
                };
                animationTimer.Tick += animationTimer_Tick;
                animationTimer.Start();
            }
            else
            {
                StopAnimation();
            }
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            activeFrame++;
            if (activeFrame >= activeAnimation.frames.Count)
            {
                activeFrame = activeAnimation.loopStart;
                if (activeAnimation.loopStart >= activeAnimation.frames.Count)
                {
                    activeFrame = activeAnimation.frames.Count - 1;
                }
            }
            framesList.SelectedIndex = activeFrame;
            frameSlider.Value = activeFrame;
        }

        private void frameSlider_ValueChanged(object sender, EventArgs e)
        {
            activeFrame = frameSlider.Value;
            if (framesList.Items.Count > 0)
                framesList.SelectedIndex = activeFrame;
        }

        private void ResetUI()
        {
            spriteCollectionsList.Items.Clear();
            framesList.Items.Clear();
            animationsList.Items.Clear();
        }

        private void StopAnimation()
        {
            playingAnimation = false;
            animationTimer.Stop();
            animationTimer.Dispose();
            animationTimer = null;
        }

        private string SelectFolder(string title = "select folder")
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = false,
                FileName = "[select folder]",
                Title = title
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string dirName = Path.GetDirectoryName(ofd.FileName);
                if (Directory.Exists(dirName))
                {
                    UpdateMonoGenerator(dirName);
                    return dirName;
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        private void UpdateMonoGenerator(string path)
        {
            if (monoGenerators.TryGetValue(path, out MonoCecilTempGenerator generator))
                am.MonoTempGenerator = generator;
            else
            {
                MonoCecilTempGenerator generator2 = new MonoCecilTempGenerator(path);
                am.MonoTempGenerator = generator2;
                monoGenerators.Add(path, generator2);
            }
        }

        private Bitmap GetTexture(AssetExternal textureExt)
        {
            if (textureExt.baseField == null)
                return null;
        
            AssetTypeValueField textureBase = textureExt.baseField;
            AssetID id = new AssetID(textureExt.file.name, textureExt.info.PathId);
            if (bitmapCache.ContainsKey(id))
                return bitmapCache[id];
        
            TextureFile texture = TextureFile.ReadTextureFile(textureBase);
            byte[] textureBytes = texture.GetTextureData(textureExt.file);
            if (textureBytes != null && textureBytes.Length > 0)
            {
                Bitmap canvas = new Bitmap(texture.m_Width, texture.m_Height, PixelFormat.Format32bppArgb);
                
                Rectangle dimension = new Rectangle(0, 0, canvas.Width, canvas.Height);
                BitmapData picData = canvas.LockBits(dimension, ImageLockMode.ReadWrite, canvas.PixelFormat);
                picData.Stride = texture.m_Width * 4;
                IntPtr pixelStartAddress = picData.Scan0;
                Marshal.Copy(textureBytes, 0, pixelStartAddress, textureBytes.Length);
                
                canvas.UnlockBits(picData);

                canvas.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bitmapCache[id] = canvas;
                return canvas;
            }
            return null;
        }

        private void ClearTextures()
        {
            if (activeSpriteData != null && activeSpriteData.collections != null)
            {
                foreach (TkSpriteCollection oldCol in activeSpriteData.collections)
                {
                    if (oldCol.baseTexture != null)
                    {
                        oldCol.baseTexture.Dispose();
                        oldCol.baseTexture = null;
                    }
                    if (oldCol.textures != null)
                    {
                        foreach (var texture in oldCol.textures)
                        {
                            texture.Value.Dispose();
                        }
                    }
                }
            }
            if (bitmapCache != null)
            {
                foreach (var bitmap in bitmapCache)
                {
                    if (bitmap.Value != null)
                    {
                        bitmap.Value.Dispose();
                    }
                }
            }
            bitmapCache = new Dictionary<AssetID, Bitmap>();
        }

        private Bitmap GetCroppedSprite(TkSpriteDefinition def, bool bg, bool round, bool border, bool full)
        {
            TkSpriteCollection col = def.parent;
        
            Bitmap croppedBitmap = new Bitmap(def.width, def.height);
        
            using (Graphics graphics = Graphics.FromImage(croppedBitmap))
            {
                graphics.DrawImage(GetTexture(col.textureExts[def.materialId]), new Rectangle(0, 0, def.width, def.height), new Rectangle(def.x, def.y, def.width, def.height), GraphicsUnit.Pixel);
            }
        
            Bitmap croppedBitmapGdiSafe;
            if (onMono)
            {
                //pretty crappy, but there's a bug with mono's internal
                //image format that won't let us rotate it after we drawimage it
                using (MemoryStream ms = new MemoryStream())
                {
                    croppedBitmap.Save(ms, ImageFormat.Png);
                    croppedBitmapGdiSafe = new Bitmap(ms);
                    croppedBitmap.Dispose();
                }
            }
            else
            {
                croppedBitmapGdiSafe = croppedBitmap;
            }
        
            if (def.flipped)
                croppedBitmapGdiSafe.RotateFlip(RotateFlipType.Rotate270FlipX);
        
            Bitmap resizedBitmap;
            if (full)
            {
                resizedBitmap = new Bitmap((int)def.fullWidth, (int)def.fullHeight);
                using (Graphics graphics = Graphics.FromImage(resizedBitmap))
                {
                    float drawX = def.xOff + def.fullWidth / 2;
                    float drawY = def.yOff + def.fullHeight / 2;
                    float drawXRound = (float)Math.Round(drawX, MidpointRounding.AwayFromZero);
                    float drawYRound = (float)Math.Round(drawY, MidpointRounding.AwayFromZero);
        
                    if (round)
                    {
                        drawX = drawXRound;
                        drawY = drawYRound;
                    }
        
                    if (bg)
                    {
                        graphics.FillRectangle(Brushes.Gray, new Rectangle(0, 0, (int)def.fullWidth, (int)def.fullHeight));
                    }
        
                    if (border)
                    {
                        graphics.DrawRectangle(Pens.Red, drawXRound - 1, drawYRound - 1,
                                               croppedBitmapGdiSafe.Width + 1, croppedBitmapGdiSafe.Height + 1);
                    }
        
                    graphics.DrawImage(croppedBitmapGdiSafe, drawX, drawY);
                    croppedBitmapGdiSafe.Dispose();
                }
            }
            else
            {
                resizedBitmap = new Bitmap(croppedBitmapGdiSafe.Width, croppedBitmapGdiSafe.Height);
                using (Graphics graphics = Graphics.FromImage(resizedBitmap))
                {
                    if (bg)
                    {
                        graphics.FillRectangle(Brushes.Gray, new Rectangle(0, 0, croppedBitmapGdiSafe.Width, croppedBitmapGdiSafe.Height));
                    }
        
                    graphics.DrawImage(croppedBitmapGdiSafe, 0, 0);
                    croppedBitmapGdiSafe.Dispose();
                }
            }
            return resizedBitmap;
        }

        private void EditCroppedSprite(Bitmap canvas, Bitmap newImage, TkSpriteDefinition def, bool clear = false)
        {
            int width, height;
            if (!def.flipped)
            {
                width = def.width;
                height = def.height;
            }
            else
            {
                width = def.height;
                height = def.width;
            }
        
            Bitmap croppedBitmap = new Bitmap(width, height);
        
            using (Graphics graphics = Graphics.FromImage(croppedBitmap))
            {
                float readX = def.xOff + def.fullWidth / 2;
                float readY = def.yOff + def.fullHeight / 2;
                //do NOT remove (int)(float) it is needed for accurate rounding
                int readXRound = (int)(float)Math.Round(readX, MidpointRounding.AwayFromZero);
                int readYRound = (int)(float)Math.Round(readY, MidpointRounding.AwayFromZero);

                graphics.DrawImage(newImage, new Rectangle(0, 0, width, height), new Rectangle(readXRound, readYRound, width, height), GraphicsUnit.Pixel);
            }
        
            Bitmap croppedBitmapGdiSafe;
            if (onMono)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    croppedBitmap.Save(ms, ImageFormat.Png);
                    croppedBitmapGdiSafe = new Bitmap(ms);
                    croppedBitmap.Dispose();
                }
            }
            else
            {
                croppedBitmapGdiSafe = croppedBitmap;
            }
            
            if (def.flipped)
                croppedBitmapGdiSafe.RotateFlip(RotateFlipType.Rotate270FlipX);
            
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                float drawX = def.x;
                float drawY = def.y;

                if (clear)
                {
                    graphics.SetClip(new Rectangle(def.x, def.y, croppedBitmapGdiSafe.Width, croppedBitmapGdiSafe.Height));
                    graphics.Clear(Color.Transparent);
                    graphics.ResetClip();
                }

                graphics.DrawImage(croppedBitmapGdiSafe, drawX, drawY);
                croppedBitmapGdiSafe.Dispose();
            }
        }
    }
}
