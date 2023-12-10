//Your average decay IIR filter
//This version does decay to zero with 0 input.
namespace DarkPLL;

class LowpassFilter
{
    private double offset = 0;
    public double Step(double input, double decay)
    {
        offset = decay * offset + (1.0 - decay) * input;
        //VCO lock range 119hz - 7639hz. This is arbitrary.
        if (offset < 1.0 / 64.0)
        {
            offset = 1.0 / 64.0;
        }
        if (offset > 1.0)
        {
            offset = 1.0;
        }
        return offset;
    }
}