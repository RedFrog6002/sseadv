using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sseadv
{
    public partial class exportoptions : Form
    {
        private string name;
        private bool animation;

        public bool DialogOk { get; private set; }
        public bool ExportOriginal { get; private set; }
        public bool ConsistentSpriteSize { get; private set; }
        public bool EnableBorders { get; private set; }
        public bool UseSpriteName { get; private set; }
        public bool CustomSpriteSheet { get; private set; }
        public int SheetColumns { get; private set; }
        public int SheetRows { get; private set; }
        public bool ExportSeparate { get; private set; }
        public int SheetAmt { get; private set; }
        public bool AnimFullExport { get; private set; }
        public bool AnimSplitByAnim { get; private set; }
        public bool GenerateSimpleJSON { get; private set; }

        public exportoptions(string name, bool animation)
        {
            this.name = name;
            this.animation = animation;
            DialogOk = false;
            InitializeComponent();
        }

        private void exportoptions_Load(object sender, EventArgs e)
        {
            Text = $"export-options: {name}";
        }

        private void StateChanged(object sender, EventArgs e)
        {
            consistentSizeChk.Enabled = originalChk.Checked;
            enableBordersChk.Enabled = originalChk.Checked && consistentSizeChk.Checked;
            spriteNameChk.Enabled = originalChk.Checked;

            autoDetectSizeChk.Enabled = spriteSheetChk.Checked;
            spriteSheetColumnsTxt.Enabled = spriteSheetChk.Checked && !autoDetectSizeChk.Checked;
            spriteSheetRowsTxt.Enabled = spriteSheetChk.Checked && !autoDetectSizeChk.Checked;
            exportSeparateChk.Enabled = spriteSheetChk.Checked;
            exportAmtTxt.Enabled = spriteSheetChk.Checked && exportSeparateChk.Checked && !(animFullChk.Checked && exportByAnimChk.Checked);
            animFullChk.Enabled = spriteSheetChk.Checked;
            exportByAnimChk.Enabled = spriteSheetChk.Checked && animFullChk.Checked && exportSeparateChk.Checked && animation;
            exportSimpleJsonChk.Enabled = spriteSheetChk.Checked;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            DialogOk = true;
            ExportOriginal = originalChk.Checked;
            ConsistentSpriteSize = consistentSizeChk.Checked;
            if (ConsistentSpriteSize)
                EnableBorders = enableBordersChk.Checked;
            else
                EnableBorders = false;
            UseSpriteName = spriteNameChk.Checked;
            CustomSpriteSheet = spriteSheetChk.Checked;
            if (autoDetectSizeChk.Checked)
            {
                SheetColumns = -1;
                SheetRows = -1;
            }
            else
            {
                SheetColumns = int.Parse(spriteSheetColumnsTxt.Text);
                SheetRows = int.Parse(spriteSheetRowsTxt.Text);
            }
            ExportSeparate = exportSeparateChk.Checked;
            if (!ExportSeparate)
            {
                SheetAmt = -1;
            }
            else
            {
                SheetAmt = int.Parse(exportAmtTxt.Text);
            }
            AnimFullExport = animFullChk.Checked;
            AnimSplitByAnim = exportByAnimChk.Checked;
            GenerateSimpleJSON = exportSimpleJsonChk.Checked;
            Close();
        }
    }
}
