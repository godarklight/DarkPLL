namespace DarkPLL;

//Just to produce a raw audio
class WavOutput
{
    FileStream fs;

    public WavOutput(string filename)
    {
        fs = new FileStream(filename, FileMode.Create);
    }

    public void WriteSample(double left, double right)
    {
        short shortL = (short)(left * 0.1 * short.MaxValue);
        short shortR = (short)(right * 0.1 * short.MaxValue);
        fs.WriteByte((byte)(shortL & 0xFF));
        fs.WriteByte((byte)((shortL >> 8) & 0xFF));
        fs.WriteByte((byte)(shortR & 0xFF));
        fs.WriteByte((byte)((shortR >> 8) & 0xFF));

    }

    public void Close()
    {
        fs.Close();
    }
}
