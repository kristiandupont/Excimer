namespace Excimer.Drawing
{
    public interface IRenderer
    {
        Image RenderOrb(int size, Color frameColor, Color primaryColor, Color satelliteColor);
        Image RenderTimeSpanIcon(int size, Color frameColor, Color primaryColor, double minutes);
    }
}
