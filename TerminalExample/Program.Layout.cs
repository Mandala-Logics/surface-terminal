using System.Reflection.Metadata;
using MandalaLogics.Path;
using MandalaLogics.SurfaceTerminal.Layout;
using MandalaLogics.SurfaceTerminal.Layout.Components;
using MandalaLogics.SurfaceTerminal.Text;

namespace TerminalExample;

internal static partial class Program
{
    private static readonly SurfaceLayout MainLayout;
    
    private static readonly SurfaceLayout WelcomeLayout = new();
    private static readonly SurfaceLayout FileLayout = new();

    private static readonly TabPanel TabPanel = new();
    private static readonly SubLayoutPanel SubLayoutPanel = new();
    private static readonly TextDisplayPanel FooterPanel = new() { Options = SurfaceWriteOptions.Centered };
    private static readonly ListDisplayPanel FileInfoPanel = new();
    private static FileDialogPanel _fileDialog = null!;
    private static readonly ListPanel DataEntryPanel = new();
    private static readonly ListPanel ConfigurePanel = new();

    private static readonly TextInputPanel AboutPanel = new()
        { TextDisplay = SurfaceWriteOptions.WrapText | SurfaceWriteOptions.Centered };
    
    private static void SetUpLayout()
    {
        var headerPanel = new TextDisplayPanel()
        {
            Fill = true,
            Options = SurfaceWriteOptions.Centered,
            Text = new ConsoleString("Mandala Logics - Surface Terminal",
                new ConsoleDecoration(null, ConsoleColor.DarkGray))
        };
        
        MainLayout.SetPanel("header", headerPanel);

        FooterPanel.Text = "Welcome!";
        
        MainLayout.SetPanel("status_bar", FooterPanel);
        
        SetUpTabPanel();
        SetUpFileLayout();
        SetUpAboutPanel();
        SetUpDataEntryPanel();
        SetUpConfigurePanel();
        SetUpWelcomeLayout();
        
        SubLayoutPanel.Layout = WelcomeLayout;
        
        MainLayout.SetPanel("main", SubLayoutPanel);
        
        MainLayout.SelectPanel("main");
        
        MainLayout.BeforeKeyPressed += MainLayoutOnBeforeKeyPressed;
    }

    private static void MainLayoutOnBeforeKeyPressed(object sender, SurfaceLayoutKeyPressedEventArgs args)
    {
        if (args.KeyInfo.Key == ConsoleKey.Tab) TabPanel.SelectNext();
    }

    private static void TabPanelOnSelectedKeyChanged(object? sender, EventArgs e)
    {
        switch (TabPanel.SelectedKey)
        {
            case "welcome":
                SubLayoutPanel.Layout = WelcomeLayout;
                MainLayout.SetPanel("main", SubLayoutPanel);
                FooterPanel.Text = "Welcome!";  
                break;
            case "file":
                SubLayoutPanel.Layout = FileLayout;
                MainLayout.SetPanel("main", SubLayoutPanel);
                FooterPanel.Text = "File selection example.";
                break;
            case "enter":
                MainLayout.SetPanel("main", DataEntryPanel);
                FooterPanel.Text = "Data entry example.";
                break;
            case "options":
                MainLayout.SetPanel("main", ConfigurePanel);
                FooterPanel.Text = "Configuration example.";
                break;
            case "about":
                MainLayout.SetPanel("main", AboutPanel);
                FooterPanel.Text = "An about dialog.";
                break;
        }
    }

    private static void SetUpConfigurePanel()
    {
        ConfigurePanel.Add("a", new TextDisplayLine());
        
        ConfigurePanel.Add("1", new OptionsLine("Option 1")
        {
            { "1", "Choice 1" },
            { "2", "Choice 2" }
        });
        
        ConfigurePanel.Add("2", new ToggleLine("Option 2", true));
        
        ConfigurePanel.Add("b", new TextDisplayLine());
        
        ConfigurePanel.Add("reset", new MenuItemLine("Reset", () =>
        {
            ((OptionsLine)ConfigurePanel["1"]).SelectLine("1");
            ((ToggleLine)ConfigurePanel["2"]).ToggleState = true;
        }));
    }

    private static void SetUpDataEntryPanel()
    {
        DataEntryPanel.Add("a", new TextDisplayLine());
        
        DataEntryPanel.Add("name", new PromptLine() { Prompt = "Name"});
        DataEntryPanel.Add("email", new PromptLine() { Prompt = "Email"});
        DataEntryPanel.Add("website", new PromptLine() { Prompt = "Website"});
        
        DataEntryPanel.Add("b", new TextDisplayLine());
        
        DataEntryPanel.Add("button", new MenuItemLine("OK", () =>
        {
            foreach (var line in DataEntryPanel)
            {
                if (line.Value is PromptLine promptLine)
                {
                    promptLine.Text = string.Empty;
                }
            }
        }));
    }

    private static void SetUpAboutPanel()
    {
        AboutPanel.Text = "The SurfaceTerminal is a static class which takes over the console and runs three threads," +
                          "one thread is for the rendering, one is for input, and one is a message thread which" +
                          "takes inputs and passes them to the current layout.\nAll components of the SurfaceTerminal" +
                          "are draw onto an ISurface<ConsoleChar>, which is a 2D array of non-static characters." +
                          "Panels persist in memory, even when not displaying, and the text on this one can even be edited!";
    }

    private static void SetUpTabPanel()
    {
        TabPanel.Add("welcome", new MenuItemLine("Welcome", null));
        TabPanel.Add("file", new MenuItemLine("Choose a File", null));
        TabPanel.Add("enter", new MenuItemLine("Enter Data", null));
        TabPanel.Add("options", new MenuItemLine("Configure", null));
        TabPanel.Add("about", new MenuItemLine("About", null));
        
        MainLayout.SetPanel("tabs", TabPanel);
        
        TabPanel.SelectedKeyChanged += TabPanelOnSelectedKeyChanged;
    }

    private static void SetUpFileLayout()
    {
        FileLayout.RootNode.Split(0.25d, LayoutSplitDirection.Vertical);

        if (OperatingSystem.IsWindows())
        {
            _fileDialog = new FileDialogPanel(new WinPath(@"C:\"), FileDialogType.Both);
        }
        else
        {
            _fileDialog = new FileDialogPanel(LinuxPath.Root, FileDialogType.Both);
        }
        
        _fileDialog.PathClicked += FilePanelOnPathClicked;
        
        FileLayout.RootNode[1].SetPanel("file", _fileDialog);
        FileLayout.RootNode[2].SetPanel("info", FileInfoPanel);
        
        FileInfoPanel.Add(new TextDisplayLine() 
            { Text = "Press enter to see file/dir information, use arrow keys to navigate"});
        
        FileLayout.SelectPanel("file");
    }

    private static void FilePanelOnPathClicked(object? sender, EventArgs e)
    {
        FileInfoPanel.Clear();  

        if (_fileDialog.SelectedPath is null) return;

        var path = (LinuxPath)_fileDialog.SelectedPath;
        
        FileInfoPanel.Add(new TextDisplayLine() 
            { Text = $"Name: {path.EndPointName}"});
        
        FileInfoPanel.Add(new TextDisplayLine() 
            { Text = $"Path: {path.Path}"});
        
        FileInfoPanel.Add(new TextDisplayLine() 
            { Text = $"Type: {path.EndType}"});

        var fileLen = path.IsDir ? "n/a" : PathBase.ParseFileLength(path.GetFileInfo().Length);
        
        FileInfoPanel.Add(new TextDisplayLine() 
            { Text = $"File length: { fileLen }"});

        var access = path.Access;
        
        FileInfoPanel.Add(new TextDisplayLine() 
            { Text = $"Access: {access}"});
    }

    private static void SetUpWelcomeLayout()
    {
        WelcomeLayout.RootNode.Split(0.5d, LayoutSplitDirection.Horizonal);
        WelcomeLayout.RootNode.DrawOutline = false;
        WelcomeLayout.RootNode[1].DrawOutline = false;
        WelcomeLayout.RootNode[2].DrawOutline = false;
        
        var topPanel = new ListDisplayPanel();
        
        topPanel.Add(new TextDisplayLine()
        {
            Options = SurfaceWriteOptions.Centered,
            Text = new ConsoleString("Welcome to Surface Terminal!",
                new ConsoleDecoration(ConsoleColor.Red, null))
        });
        
        topPanel.Add(new TextDisplayLine());
        
        topPanel.Add(new TextDisplayLine()
        {
            Options = SurfaceWriteOptions.Centered,
            Text = new ConsoleString("(Use tab key to change panel)")
        });
        
        topPanel.Add(new TextDisplayLine());
        
        topPanel.Add(new TextDisplayLine()
        {
            Options = SurfaceWriteOptions.Centered,
            Text = new ConsoleString("(c) Mandala Logics - MIT Licence")
        });
        
        topPanel.Add(new TextDisplayLine());

        var csb = new ConsoleStringBuilder(40);

        for (var x = 0UL; x < 40UL; x++)
        {
            csb.Append(new WelcomeChar(x * 2UL));
        }
        
        topPanel.Add(new TextDisplayLine()
        {
            Options = SurfaceWriteOptions.Centered,
            Text = csb.GetConsoleString()
        });

        var logoPanel = new ImageDisplayPanel()
        {
            UseColour = true,
            KeepRatio = true
        };

        logoPanel.Load(CommandHelper.GetAssemblyStream("logo.png"));

        WelcomeLayout.RootNode[1].SetPanel("top", topPanel);
        WelcomeLayout.RootNode[2].SetPanel("bottom", logoPanel);
    }
}