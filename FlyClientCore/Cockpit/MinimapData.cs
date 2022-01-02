using System.Drawing;

namespace ClientCore.Cockpit
{
    public class MinimapData
    {
        public const float CAPTURE_SIZE_X = 768;
        public const float CAPTURE_SIZE_Y = 1366;
        public const float CAPTURE_CUT_SIZE = CAPTURE_SIZE_X;
        int terrainNumber;

        public MinimapData(int terrainNumber)
        {
            this.terrainNumber = terrainNumber;
        }

        public void CutCapturedImage()
        {
            Image img = Image.FromFile(GetCapturePath());
            Bitmap bitmap = new Bitmap(img);
            Bitmap crop = bitmap.Clone(getCaptureRect(), bitmap.PixelFormat);
            crop.Save(GetCapturePath());
        }

        public string GetCapturePath() => $"Resources/Terrains/Terrain{terrainNumber}/minimap.png";

        RectangleF getCaptureRect()
        {
            float cutOffset = (CAPTURE_SIZE_Y - CAPTURE_SIZE_X) / 2;
            RectangleF captureRect = new RectangleF(cutOffset, 0, CAPTURE_CUT_SIZE, CAPTURE_CUT_SIZE);
            return captureRect;
        }
    }
}