using MandalaLogics.SurfaceTerminal;
using MandalaLogics.SurfaceTerminal.Layout.Components;
using MandalaLogics.SurfaceTerminal.Parsing;
using MandalaLogics.SurfaceTerminal.Text;

namespace TerminalExample;

internal static partial class Program
{
    static Program()
    {
        var sr = CommandHelper.GetAssemblyStreamReader("main.surf");

        MainLayout = LayoutDeserializer.Read(sr);
    }
    
    static void Main(string[] args)
    {
        SetUpLayout();
        
        SurfaceTerminal.Display(MainLayout);
        
        SurfaceTerminal.Start();
    }
}