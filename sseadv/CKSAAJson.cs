using System.Collections.Generic;
using System.Numerics;

namespace sseadv.CKSAAJson
{
    public class FolderInfo
    {
        public const string FileName = "Animations.json";
        public List<string> Animations = new List<string>();
    }
    public enum DefType
    {
        Simple,
        Advanced
    }
    public class AnimInfo
    {
        public const string FileName = "Info.json";
        public const string TextureName = "Sheet.png";
        public bool useOriginalData = true;
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public WrapMode LoopType = WrapMode.Once;
        public int loopStart = 0;
        public float fps = 15f;
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public DefType InfoType = DefType.Simple;
    }
    public class SimpleInfo
    {
        public const string FileName = "Simple.json";
        public int columns = 1;
        public int rows = 1;
        public int frames = 1;
        public Vector2 anchor = new Vector2(0f, 0f);
    }
    public class AdvancedInfo
    {
        public const string FileName = "Advanced.json";
        public List<AdvancedFrameInfo> frames = new List<AdvancedFrameInfo>();
    }

    public class AdvancedFrameInfo
    {
        public Vector2 position = new Vector2(0f, 0f);
        public Vector2 size = new Vector2(0f, 0f);
        public Vector2 anchor = new Vector2(0f, 0f);

        public bool useOriginalEventData = true;

        public bool triggerEvent = false;
        public string eventInfo = "";
        public int eventInt = 0;
        public float eventFloat = 0;
    }
    public class Vector2
    {
        public Vector2() {}
        public Vector2(float x, float y) {this.x = x; this.y = y;}
        public float x = 0;
        public float y = 0;
    }
}