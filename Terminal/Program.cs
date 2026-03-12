using MandalaLogics.SurfaceTerminal;
using MandalaLogics.SurfaceTerminal.Layout;
using MandalaLogics.SurfaceTerminal.Parsing;
using MandalaLogics.SurfaceTerminal.Text;

namespace Terminal;

internal static class Program
{
    static void Main(string[] args)
    {
        var sr = CommandHelper.GetAssemblyStreamReader("main.surf");

        var layout = LayoutDeserializer.Read(sr);

        var listDisplay = new ListDisplayPanel();
        
        listDisplay.Add(new TextDisplayLine
        {
            Options = SurfaceWriteOptions.Centered, 
            Decoration = new ConsoleDecoration(null, ConsoleColor.Gray),
            Text = "Mandala Logics"
        });
        
        layout.SetPanel("header", listDisplay);

        var list = new ListPanel
        {
            new PromptLine() { Prompt = "Name" },
            new PromptLine() { Prompt = "Email" },
            new PromptLine() { Prompt = "Website" }
        };
        
        layout.SetPanel("main", list);

        var statusPanel = new TextDisplayPanel();
        
        layout.SetPanel("status_bar", statusPanel);

        statusPanel.Text = "Use the arrow keys to change field.";

        foreach (var line in list)
        {
            line.OnStateChanged += l =>
            {
                if (l.State == SurfaceLineState.Selected)
                    statusPanel.Text = new ConsoleString($"Please enter your {((PromptLine)l).Prompt.ToLower()}.");
            };
        }
        
        SurfaceTerminal.Display(layout);
        
        SurfaceTerminal.Start();
    }
}