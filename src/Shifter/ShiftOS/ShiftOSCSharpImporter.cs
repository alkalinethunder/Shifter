using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Customization.User;
using Helpers;
using Newtonsoft.Json;
using UnityEngine;

namespace Customization.ShiftOS
{
    public class ShiftOSCSharpImporter : ShiftOSImporter
    {
        // This class contains the fields encoded within the skin's
        // data.json file.
        private class DataFormat
        {
            // UI Layout Values
            public bool enablebordercorners;
            public string minbtnsize;
            public string rollbtnsize;
            public string closebtnsize;
            public int titlebarheight;
            public int titlebariconsize;
            public int closebtnfromtop;
            public int closebtnfromside;
            public int rollbtnfromside;
            public int rollbtnfromtop;
            public int minbtnfromtop;
            public int minbtnfromside;
            public int borderwidth;
            public bool enablecorners;
            public int titlebarcornerwidth;
            public int titleiconfromtop;
            public int titleiconfromside;
            public int titletextfontsize;
            public int titletextfromside;
            public int titletextfromtop;
            public TitleTextPosition titletextpos;
            public int desktoppanelheight;
            public StatusBarPlacement desktoppanelposition;
            public int panelclocktexttop;
            public int panelclocktextsize;
            public int applicationbuttonheight;
            public int applicationbuttontextsize;
            public string applicationlaunchername;
            public TitleTextPosition titletextposition;
            public int applaunchermenuholderwidth;
            public int panelbuttonicontop;
            public int panelbuttoniconside;
            public int panelbuttoniconsize;
            public int panelbuttonwidth;
            public int panelbuttonheight;
            public int panelbuttontextsize;
            public int panelbuttontextside;
            public int panelbuttontexttop;
            public int panelbuttongap;
            public int panelbuttonfromtop;
            public int panelbuttoninitialgap;
            
            // Image Layouts
            public ImageLayout borderbottomlayout;
            public ImageLayout borderrightlayout;
            public ImageLayout borderleftlayout;
            public ImageLayout titlebarlayout;
            public ImageLayout minbtnlayout;
            public ImageLayout rollbtnlayout;
            public ImageLayout closebtnlayout;
            public ImageLayout leftcornerlayout;
            public ImageLayout rightcornerlayout;
            public ImageLayout bottomrightcornerlayout;
            public ImageLayout bottomleftcornerlayout;
            public ImageLayout desktoppanellayout;
            public ImageLayout desktopbackgroundlayout;
            public ImageLayout panelclocklayout;
            public ImageLayout applauncherlayout;
            public ImageLayout panelbuttonlayout;
            
            // Image paths.
            public string applauncherclickpath;
            public string panelbuttonpath;
            public string applaunchermouseoverpath;
            public string applauncherpath;
            public string panelclockpath;
            public string desktopbackgroundpath;
            public string desktoppanelpath;
            public string minbtnhoverpath;
            public string minbtnclickpath;
            public string rightcornerpath;
            public string titlebarpath;
            public string borderrightpath;
            public string borderleftpath;
            public string borderbottompath;
            public string closebtnpath;
            public string closebtnhoverpath;
            public string closebtnclickpath;
            public string rollbtnpath;
            public string rollbtnhoverpath;
            public string rollbtnclickpath;
            public string leftcornerpath;
            public string minbtnpath;
            public string bottomleftcornerpath;
            public string bottomrightcornerpath;

            // Colors
            public string TerminalTextColor;
            public string TerminalBackColor;
            public string panelbuttontextcolour;
            public string panelbuttoncolour;
           public string applicationsbuttontextcolour;
           public string applaunchermouseovercolour;
           public string applauncherbackgroundcolour;
           public string applauncherbuttoncolour;
           public string applauncherbuttonclickedcolour;
           public string clockbackgroundcolor;
           public string clocktextcolour;
           public string desktopbackgroundcolour;
           public string desktoppanelcolour;
           public string titletextcolour;
           public string leftcornercolour;
           public string rightcornercolour;
           public string minbtncolour;
           public string minbtnhovercolour;
           public string minbtnclickcolour;
           public string rollbtncolour;
           public string rollbtnhovercolour;
           public string rollbtnclickcolour;
           public string closebtncolour;
           public string closebtnhovercolour;
           public string closebtnclickcolour;
           public string borderbottomcolour;
           public string borderrightcolour;
           public string borderleftcolour;
           public string titlebarcolour;
           public string bottomleftcornercolour;
           public string bottomrightcornercolour;
        }

        // Temporary directory for the extracted ShiftOS skin.
        private Color _transparencyKey = new Color(1f / 255f, 0, 1f / 255f);
        private string _tempDirectory;
        private string _dataFile; // data.json path.
        
        // Holds the loaded skin data.
        private DataFormat _skinData;
        
        // Just like Phil skins, we keep track of image resource paths.
        private Dictionary<string, UserImageResource> _resources = new Dictionary<string, UserImageResource>();

        public ShiftOSCSharpImporter(string temp, string destinationPath) : base(destinationPath)
        {
            _tempDirectory = temp;
            _dataFile = Path.Combine(temp, "data.json");
        }

        protected override IEnumerator LoadData()
        {
            Debug.Log($"Loading the skin JSON data in {_dataFile}...");
            if (File.Exists(_dataFile))
            {
                var wasError = false;
                try
                {
                    var json = File.ReadAllText(_dataFile);
                    _skinData = JsonConvert.DeserializeObject<DataFormat>(json);
                }
                catch (Exception ex)
                {
                    ReportError(ex.Message);
                    wasError = true;
                }

                if (wasError)
                    yield break;
            }
            else
            {
                this.ReportError(
                    "data.json file not found in this ShiftOS 0.1.x skin. Did you try to load a normal Zip file as a skin?");
                
                yield break;
            }

            yield return null;
        }

        protected override IEnumerator CopyImages()
        {
            var resourcePath = Path.Combine(this.SkinDestination, "resources");
            if (!Directory.Exists(resourcePath))
                Directory.CreateDirectory(resourcePath);
            yield return null;
            
            // This is fucking painful man
            var skinImagePaths = new string[]
            {
                _skinData.applauncherclickpath,
                _skinData.panelbuttonpath,
                _skinData.applaunchermouseoverpath,
                _skinData.applauncherpath,
                _skinData.panelclockpath,
                _skinData.desktopbackgroundpath,
                _skinData.desktoppanelpath,
                _skinData.minbtnhoverpath,
                _skinData.minbtnclickpath,
                _skinData.rightcornerpath,
                _skinData.titlebarpath,
                _skinData.borderrightpath,
                _skinData.borderleftpath,
                _skinData.borderbottompath,
                _skinData.closebtnpath,
                _skinData.closebtnhoverpath,
                _skinData.closebtnclickpath,
                _skinData.rollbtnpath,
                _skinData.rollbtnhoverpath,
                _skinData.rollbtnclickpath,
                _skinData.leftcornerpath,
                _skinData.minbtnpath,
                _skinData.bottomleftcornerpath,
                _skinData.bottomrightcornerpath
            };

            foreach (var skinImageName in skinImagePaths)
            {
                var resource = new UserImageResource();
                resource.Source = UserImageSource.None;
                
                var sourcePath = Path.Combine(_tempDirectory, skinImageName);

                if (File.Exists(sourcePath))
                {
                    var texture = new Texture2D(1, 1);
                    if (UnityHelpers.LoadImageFromPath(texture, sourcePath, _transparencyKey))
                    {
                        var destPath = Path.Combine(resourcePath, skinImageName + ".png");
                        var pngData = texture.EncodeToPNG();
                        File.WriteAllBytes(destPath, pngData);

                        Debug.Log($"Copied ShiftOS image {skinImageName} to Socially Distant resource {destPath}");
                        
                        resource.Source = UserImageSource.External;
                        resource.ResourcePath = "/resources/" + skinImageName + ".png";
                    }
                }

                _resources.Add(skinImageName, resource);
                yield return null;
            }
        }

        protected override IEnumerator BuildUserSkin(UserSkin skin)
        {
            skin.Metadata.HasDarkMode = false;
            skin.Metadata.HasPanicMode = false;

            // Wallpaper.
            skin.Wallpapers.Light.Wallpaper.Color = _skinData.desktopbackgroundcolour;
            skin.Wallpapers.Light.Wallpaper.Image = _resources[_skinData.desktopbackgroundpath];

            // ShiftOS style client area.
            var decoration = skin.Decorations.Light;
            decoration.ClientBackground.Background.Color = "#ffffff";

            // Close button.
            decoration.CloseButton.Size = ParseVector2(_skinData.closebtnsize);
            decoration.CloseButton.Offset = new Vector2(_skinData.closebtnfromside, _skinData.closebtnfromtop);
            decoration.CloseButton.Idle.Color = ParseGdiColor(_skinData.closebtncolour);
            decoration.CloseButton.Hovered.Color = ParseGdiColor(_skinData.closebtnhovercolour);
            decoration.CloseButton.Pressed.Color = ParseGdiColor(_skinData.closebtnclickcolour);
            decoration.CloseButton.Idle.Image = _resources[_skinData.closebtnpath];
            decoration.CloseButton.Hovered.Image = _resources[_skinData.closebtnhoverpath];
            decoration.CloseButton.Pressed.Image = _resources[_skinData.closebtnclickpath];

            // Maximize
            decoration.MaximizeButton.Size = ParseVector2(_skinData.rollbtnsize);
            decoration.MaximizeButton.Offset = new SafeVector2(_skinData.rollbtnfromside, _skinData.rollbtnfromtop);
            decoration.MaximizeButton.Idle.Color = ParseGdiColor(_skinData.rollbtncolour);
            decoration.MaximizeButton.Hovered.Color = ParseGdiColor(_skinData.rollbtnhovercolour);
            decoration.MaximizeButton.Pressed.Color = ParseGdiColor(_skinData.rollbtnclickcolour);
            decoration.MaximizeButton.Idle.Image = _resources[_skinData.rollbtnpath];
            decoration.MaximizeButton.Hovered.Image = _resources[_skinData.rollbtnhoverpath];
            decoration.MaximizeButton.Pressed.Image = _resources[_skinData.rollbtnclickpath];

            // Minimize
            decoration.MinimizeButton.Size = ParseVector2(_skinData.minbtnsize);
            decoration.MinimizeButton.Offset = new SafeVector2(_skinData.minbtnfromside, _skinData.minbtnfromtop);
            decoration.MinimizeButton.Idle.Color = ParseGdiColor(_skinData.minbtncolour);
            decoration.MinimizeButton.Hovered.Color = ParseGdiColor(_skinData.minbtnhovercolour);
            decoration.MinimizeButton.Pressed.Color = ParseGdiColor(_skinData.minbtnclickcolour);
            decoration.MinimizeButton.Idle.Image = _resources[_skinData.minbtnpath];
            decoration.MinimizeButton.Hovered.Image = _resources[_skinData.minbtnhoverpath];
            decoration.MinimizeButton.Pressed.Image = _resources[_skinData.minbtnclickpath];

            // Titlebar
            decoration.TitleBackground.Size = _skinData.titlebarheight;
            decoration.TitleBackground.Background.Color = ParseGdiColor(_skinData.titlebarcolour);
            decoration.TitleBackground.Background.Image = _resources[_skinData.titlebarpath];
            
            // Border sizing.
            decoration.Left.Size = _skinData.borderwidth;
            decoration.Bottom.Size = _skinData.borderwidth;
            decoration.Right.Size = _skinData.borderwidth;
            decoration.BottomRight.Size = _skinData.borderwidth;
            decoration.BottomLeft.Size = _skinData.borderwidth;
            decoration.TopRight.Size = (_skinData.enablecorners) ? _skinData.titlebarcornerwidth : 0;
            decoration.TopLeft.Size = (_skinData.enablecorners) ? _skinData.titlebarcornerwidth : 0;

            decoration.TitleIconOffset = new SafeVector2(_skinData.titleiconfromside, _skinData.titleiconfromtop);
            decoration.TitleIconSize = new SafeVector2(_skinData.titlebariconsize, _skinData.titlebariconsize);
            
            // Border texturing
            decoration.Left.Background.Color = ParseGdiColor(_skinData.borderleftcolour);
            decoration.Left.Background.Image = _resources[_skinData.borderleftpath];
            decoration.Right.Background.Color = ParseGdiColor(_skinData.borderrightcolour);
            decoration.Right.Background.Image = _resources[_skinData.borderrightpath];
            decoration.Bottom.Background.Color = ParseGdiColor(_skinData.borderbottomcolour);
            decoration.Bottom.Background.Image = _resources[_skinData.borderbottompath];
            decoration.BottomLeft.Background.Color = ParseGdiColor(_skinData.bottomleftcornercolour);
            decoration.BottomLeft.Background.Image = _resources[_skinData.bottomleftcornerpath];
            decoration.BottomRight.Background.Color = ParseGdiColor(_skinData.bottomrightcornercolour);
            decoration.BottomRight.Background.Image = _resources[_skinData.bottomrightcornerpath];
            decoration.TopLeft.Background.Color = ParseGdiColor(_skinData.leftcornercolour);
            decoration.TopLeft.Background.Image = _resources[_skinData.leftcornerpath];
            decoration.TopRight.Background.Color = ParseGdiColor(_skinData.rightcornercolour);
            decoration.TopRight.Background.Image = _resources[_skinData.rightcornerpath];
            
            // Title text.
            decoration.TitleTextAlignment = _skinData.titletextposition == TitleTextPosition.Left
                ? TitleTextAlignment.Left
                : TitleTextAlignment.Centered;
            decoration.TitleTextSize = _skinData.titletextfontsize;
            decoration.TitleTextOffset = new SafeVector2(_skinData.titletextfromside, _skinData.titletextfromtop);
            decoration.TitleTextColor = ParseGdiColor(_skinData.titletextcolour);
            
            // Make sure the terminal uses its own background color.
            skin.ApplicationStyle.TerminalUsesWindowColor = false;
            skin.ApplicationStyle.OverrideTerminalPalette = true;
            skin.ApplicationStyle.TerminalBackground = ParseGdiColor(_skinData.TerminalBackColor);
            skin.ApplicationStyle.TerminalForeground = ParseGdiColor(_skinData.TerminalTextColor);

            decoration.ClientBackground.Background.Color = "#ffffff";
            
            // Status Bar style...
            var statusBar = skin.StatusBar.Light;
            statusBar.Background.Size = _skinData.desktoppanelheight;
            statusBar.Background.Background.Color = ParseGdiColor(_skinData.desktoppanelcolour);
            statusBar.Background.Background.Image = _resources[_skinData.desktoppanelpath];
            statusBar.Tray.Background.Color = ParseGdiColor(_skinData.clockbackgroundcolor);
            statusBar.Tray.Background.Image = _resources[_skinData.panelclockpath];
            statusBar.ClockTextColor = ParseGdiColor(_skinData.clocktextcolour);
            statusBar.MenuLabelColor = ParseGdiColor(_skinData.applicationsbuttontextcolour);
            statusBar.SystemMenu.Size = new SafeVector2(_skinData.applaunchermenuholderwidth, _skinData.applicationbuttonheight);
            statusBar.SystemMenu.Offset = Vector2.zero;
            statusBar.SystemMenu.Idle.Color = ParseGdiColor(_skinData.applauncherbuttoncolour);
            statusBar.SystemMenu.Idle.Image = _resources[_skinData.applauncherpath];
            statusBar.SystemMenu.Hovered.Color = ParseGdiColor(_skinData.applauncherbuttoncolour);
            statusBar.SystemMenu.Hovered.Image = _resources[_skinData.applaunchermouseoverpath];
            statusBar.SystemMenu.Pressed.Color = ParseGdiColor(_skinData.applauncherbuttonclickedcolour);
            statusBar.SystemMenu.Pressed.Image = _resources[_skinData.applauncherclickpath];
            statusBar.ActiveSystemMenu = statusBar.SystemMenu;
            statusBar.TaskListButton.Size = new SafeVector2(_skinData.panelbuttonwidth, _skinData.panelbuttonheight);
            statusBar.TaskListButton.Offset = new SafeVector2(_skinData.panelbuttongap, 0);
            statusBar.TaskListButton.Idle.Color = ParseGdiColor(_skinData.panelbuttoncolour);
            statusBar.TaskListButton.Idle.Image = _resources[_skinData.panelbuttonpath];
            statusBar.TaskListButton.Hovered = statusBar.TaskListButton.Idle;
            statusBar.TaskListButton.Pressed = statusBar.TaskListButton.Idle;
            statusBar.ActiveTaskListButton = statusBar.TaskListButton;
            statusBar.TaskListButtonTextColor = ParseGdiColor(_skinData.panelbuttontextcolour);
            statusBar.ActiveTaskListButtonTextColor = ParseGdiColor(_skinData.panelbuttontextcolour);
            
            var statusBarSettings = skin.StatusBar.Settings;
            statusBarSettings.Placement = _skinData.desktoppanelposition;
            statusBarSettings.ClockFontSize = _skinData.panelclocktextsize;
            statusBarSettings.ClockOffset = new SafeVector2(4, _skinData.panelclocktexttop);
            statusBarSettings.MenuLabel = _skinData.applicationlaunchername;
            statusBarSettings.MenuLabelFontSize = _skinData.applicationbuttontextsize;
            statusBarSettings.TaskListOffset =
                new SafeVector2(_skinData.panelbuttoninitialgap, _skinData.panelbuttonfromtop);
            statusBarSettings.TaskListIconSize =
                new SafeVector2(_skinData.panelbuttoniconsize, _skinData.panelbuttoniconsize);
            statusBarSettings.TaskListIconOffset =
                new SafeVector2(_skinData.panelbuttoniconside, _skinData.panelbuttonicontop);
            statusBarSettings.TaskListTextSize = _skinData.panelbuttontextsize;
            statusBarSettings.TaskListTextOffset =
                new SafeVector2(_skinData.panelbuttontextside, _skinData.panelbuttontexttop);
            
            // Ensure that the task list is relative to the left edge of the status panel.
            statusBarSettings.TaskListOffset = new SafeVector2(
                statusBar.SystemMenu.Offset.x + statusBar.SystemMenu.Size.x + statusBarSettings.TaskListOffset.x,
                statusBarSettings.TaskListOffset.y);

            yield break;
        }

        private SafeVector2 ParseVector2(string sizeText)
        {
            var parts = sizeText.Split(new[] {", "}, StringSplitOptions.None);
            var x = parts[0];
            var y = parts[1];

            var xn = int.Parse(x);
            var yn = int.Parse(y);

            return new SafeVector2(xn, yn);
        }

        private string ParseGdiColor(string gdiColor)
        {
            var converter = new System.Drawing.ColorConverter();
            var gdiColorStruct = (System.Drawing.Color) converter.ConvertFromString(gdiColor);

            var r = gdiColorStruct.R;
            var g = gdiColorStruct.G;
            var b = gdiColorStruct.B;
            
            return $"#{PhilUtility.GetHtmlByte(r)}{PhilUtility.GetHtmlByte(g)}{PhilUtility.GetHtmlByte(b)}";
        }
    }
}