using MandalaLogics.SurfaceTerminal.Text;

namespace TerminalExample;

public class WelcomeChar(ulong offset) : ConsoleChar
{
    public override ConsoleDecoration Decoration => default;

    public override char GetChar(ulong frameNumber)
    {
        return (frameNumber + offset) % 32 <= 4 ? '#' : '~';
    }
}