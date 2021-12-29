using System.Drawing;

namespace ClientCore.Cockpit
{
    public class ArtificialHorizonData
    {
        Image image;
        const float MAX_PITCH_ANGLE = 90;
        const float MAX_YAW_ANGLE = 90;

        public ArtificialHorizonData(string filePath)
        {
            image = Image.FromFile(filePath);
        }

        public float GetPitchPixelsForDegree() => (image.Height / 2) / MAX_PITCH_ANGLE;
        public float GetYawPixelsForDegree() => (image.Height / 2) / MAX_YAW_ANGLE;
    }
}