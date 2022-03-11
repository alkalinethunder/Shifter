using System.Collections;
using System.Collections.Generic;
using System.IO;
using Customization.User;
using Helpers;
using UnityEngine;
using static Customization.ShiftOS.PhilUtility;

namespace Customization.ShiftOS
{
    public class PhilImporter : ShiftOSImporter
    {
        #region ShiftOS Skin Data

        private string _titleBarColor;
        private string _windowBorderColor;
        private int _windowBorderSize;
        private int _titleBarHeight;
        private string _closeButtonColor;
        private Vector2 _closeButtonSize;
        private Vector2 _closeButtonOffset;
        private string _titleTextColor;
        private Vector2 _titleTextOffset;
        private int _titleTextSize;
        private string _desktopPanelColor;
        private string _desktopBackgroundColor;
        private int _desktopPanelHeight;
        private string _desktopPanelPosition;
        private string _clockTextColor;
        private string _clockBackgroundColor;
        private Vector2 _clockOffset;
        private int _clockSize;
        private string _appLauncherButtonColor;
        private string _appLauncherButtonClickColor;
        private string _appLauncherBackgroundColor;
        private string _appLauncherButtonHoverColor;
        private string _appLauncherLabelColor;
        private Vector2 _appLauncherSize;
        private int _appLauncherLabelSize;
        private string _appLauncherLabel;
        private string _titleTextPosition;
        private string _rollButtonColor;
        private Vector2 _rollButtonSize;
        private Vector2 _rollButtonOffset;
        private Vector2 _titleIconOffset;
        private bool _showWindowCorners;
        private int _titleBarCornerWidth;
        private string _titleBarRightCornerColor;
        private string _titleBarLeftCornerColor;
        private string _windowBorderLeftColor;
        private string _windowBorderRightColor;
        private string _windowBorderBottomColor;
        private string _windowBorderBottomRightColor;
        private string _windowBorderBottomLeftColor;
        private Vector2 _panelButtonIconOffset;
        private Vector2 _panelButtonIconSize;
        private Vector2 _panelButtonSize;
        private string _panelButtonColor;
        private string _panelButtonTextColor;
        private int _panelButtonTextSize;
        private Vector2 _panelButtonTextOffset;
        private int _panelButtonGap;
        private Vector2 _panelButtonsOffset;
        private string _minimizeButtonColor;
        private Vector2 _minimizeButtonSize;
        private Vector2 _minimizeButtonOffset;
        
        #endregion

        // This array contains the names of each ShiftOS skin image, as required by the Socially Distant Skin Resource system.
        // string.Empty signifies that the path is unused and will not be imported.
        // Note that ShiftOS 0.0.7 does not have any discrete names for these images.
        private string[] _imageNames = new string[]
        {
            "closebutton",
            "closebuttonhover",
            "closebuttonpress",
            "top",
            string.Empty,
            string.Empty,
            "wallpaper",
            string.Empty,
            string.Empty,
            "maximizebutton",
            "maximizebuttonhover",
            "maximizebuttonpress",
            "topright",
            string.Empty,
            string.Empty,
            "topleft",
            string.Empty,
            string.Empty,
            "statusbar",
            string.Empty,
            string.Empty,
            "clock",
            string.Empty,
            string.Empty,
            "menu",
            "menuhover",
            "menupress",
            "left",
            string.Empty,
            string.Empty,
            "right",
            string.Empty,
            string.Empty,
            "bottom",
            string.Empty,
            string.Empty,
            "bottomright",
            string.Empty,
            string.Empty,
            "bottomleft",
            string.Empty,
            string.Empty,
            "minimizebutton",
            "minimizebuttonhover",
            "minimizebuttonpress",
            "statusbarbutton",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };
        // These are the ShiftOS paths for each skin image.
        private string[] _imagePaths = new string[50];
        private Color _transparencyKey = new Color(1f / 255f, 0, 1f / 255f);
        private string _skinTemp;
        private string _resourceDirectory;
        private Dictionary<string, UserImageResource> _resources = new Dictionary<string, UserImageResource>();

        public PhilImporter(string temp, string destinationPath) : base(destinationPath)
        {
            _skinTemp = temp;
            _resourceDirectory = Path.Combine(this.SkinDestination, "resources");
        }

        protected override IEnumerator LoadData()
        {
            var skinDataPath = Path.Combine(_skinTemp, "skindata.dat");
            if (!File.Exists(skinDataPath))
            {
                this.ReportError("skindata.dat does not exist! Not a ShiftOS 0.0.7 skin.");
                yield break;
            }

            using var stream = File.OpenRead(skinDataPath);
            using var reader = new StreamReader(stream);
            yield return null;

            _titleBarColor = ReadColor(reader);
            _windowBorderColor = ReadColor(reader);
            _windowBorderSize = ReadNumber(reader);
            _titleBarHeight = ReadNumber(reader);
            _closeButtonColor = ReadColor(reader);
            _closeButtonSize = ReadBackwardsVector(reader);
            _closeButtonOffset = ReadVector(reader);
            _titleTextColor = ReadColor(reader);
            _titleTextOffset = ReadBackwardsVector(reader);
            _titleTextSize = ReadNumber(reader);
            
            // Skip over the font style and family
            reader.ReadLine();
            reader.ReadLine();
            yield return null;

            _desktopPanelColor = ReadColorOrDefault(reader, "#808080");
            _desktopBackgroundColor = ReadColorOrDefault(reader, "black");
            _desktopPanelHeight = ReadNumber(reader);
            _desktopPanelPosition = reader.ReadLine();
            _clockTextColor = ReadColorOrDefault(reader, "#ffffff");
            _clockBackgroundColor = ReadColorOrDefault(reader, _desktopPanelColor);
            _clockOffset.y = ReadNumber(reader);
            _clockSize = ReadNumber(reader);
            
            // Skip over the clock font.
            reader.ReadLine();
            reader.ReadLine();
            yield return null;

            _appLauncherButtonColor = ReadColorOrDefault(reader, _desktopPanelColor);
            _appLauncherButtonClickColor = ReadColorOrDefault(reader, _appLauncherButtonColor);
            _appLauncherBackgroundColor = ReadColorOrDefault(reader, "#808080");
            _appLauncherButtonHoverColor = ReadColorOrDefault(reader, _appLauncherButtonColor);
            _appLauncherLabelColor = ReadColorOrDefault(reader, "black");
            _appLauncherSize.y = ReadNumber(reader);
            _appLauncherLabelSize = ReadNumber(reader);
            
            // Skip the font.
            reader.ReadLine();
            reader.ReadLine();
            yield return null;

            _appLauncherLabel = reader.ReadLine();

            _titleTextPosition = reader.ReadLine();

            _rollButtonColor = ReadColorOrDefault(reader, _closeButtonColor);
            _rollButtonSize = ReadBackwardsVector(reader);
            _rollButtonOffset = ReadVector(reader);

            _titleIconOffset = ReadVector(reader);

            _showWindowCorners = reader.ReadLine() == "True";
            _titleBarCornerWidth = ReadNumber(reader);
            _titleBarRightCornerColor = ReadColor(reader);
            _titleBarLeftCornerColor = ReadColor(reader);

            _appLauncherSize.x = ReadNumber(reader);
            
            // Some 0.0.7 skins don't have individual border colors, use
            // the old 0.0.6 value instead.
            _windowBorderLeftColor = ReadColorOrDefault(reader, _windowBorderColor);
            _windowBorderRightColor = ReadColorOrDefault(reader, _windowBorderColor);
            _windowBorderBottomColor = ReadColorOrDefault(reader, _windowBorderColor);
            _windowBorderBottomRightColor = ReadColorOrDefault(reader, _windowBorderColor);
            _windowBorderBottomLeftColor = ReadColorOrDefault(reader, _windowBorderColor);

            _panelButtonIconOffset = ReadBackwardsVector(reader);
            _panelButtonIconSize = ReadVector(reader);
            _panelButtonSize = ReadBackwardsVector(reader);
            _panelButtonColor = ReadColor(reader);
            _panelButtonTextColor = ReadColor(reader);
            _panelButtonTextSize = ReadNumber(reader);
            
            // Skip the font.
            reader.ReadLine();
            reader.ReadLine();
            yield return null;

            _panelButtonTextOffset = ReadVector(reader);
            _panelButtonGap = ReadNumber(reader);
            _panelButtonsOffset = ReadBackwardsVector(reader);

            _minimizeButtonColor = ReadColorOrDefault(reader, _rollButtonColor);
            _minimizeButtonSize = ReadBackwardsVector(reader);
            _minimizeButtonOffset = ReadVector(reader);
            
            // We are now at line 74 in the file. Images start at 100. Skip the remaining lines.
            for (var i = 73; i < 100; i++)
                reader.ReadLine();
            yield return null;
            
            // Now start reading image paths.
            for (var i = 0; i < _imagePaths.Length; i++)
            {
                _imagePaths[i] = reader.ReadLine();
            }

            yield return null;
        }

        protected override IEnumerator CopyImages()
        {
            if (!Directory.Exists(_resourceDirectory))
            {
                Directory.CreateDirectory(_resourceDirectory);
                Debug.Log("Created skin resources directory in " + _resourceDirectory);
                yield return null;
            }

            for (var i = 0; i < _imageNames.Length && i < _imagePaths.Length; i++)
            {
                var name = _imageNames[i];
                var path = _imagePaths[i];
                
                // Skip un-named images.
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                
                // Create the skin resource. By default it'll be empty.
                var resource = new UserImageResource();
                resource.Source = UserImageSource.None;
                resource.ResourcePath = string.Empty;
                
                // If the path isn't empty we need to figure out how to import it...
                if (!string.IsNullOrWhiteSpace(path))
                {
                    // So, about ShiftOS 0.0.7 image paths...
                    // - File names aren't standardized.
                    // - The game extracts the skin to C:\ShiftOS\Shiftum42\Skins\Current.
                    // - That's a Windows path.
                    // - And yes, the ENTIRE FUCKING FOLDER PATH INCLUDING THE DRIVE LETTER is stored in the skindata.dat.
                    // - And yes. We need to parse it out.
                    
                    // Start by replacing "\" with the system path char.
                    path = path.Replace("\\", Path.DirectorySeparatorChar.ToString());
                    
                    // Now we can get the file name.
                    var filename = Path.GetFileName(path);
                    
                    // Then we can map it to the temp directory.
                    var tempPath = Path.Combine(_skinTemp, filename);
                    
                    // Check if the file does, in deed, exist.
                    if (File.Exists(tempPath))
                    {
                        // Let's start loading it into a texture!
                        var texture = new Texture2D(1, 1);
                        if (UnityHelpers.LoadImageFromPath(texture, tempPath, _transparencyKey))
                        {
                            // Now we have an actual unity texture for the skin image. Let's encode it as a PNG!
                            var pngData = texture.EncodeToPNG();
                            
                            // Destination path is in the skin resources folder.
                            var destPath = Path.Combine(_resourceDirectory, name + ".png");
                            File.WriteAllBytes(destPath, pngData);
                            
                            // Store the local resource path.
                            resource.Source = UserImageSource.External;
                            resource.ResourcePath = $"/resources/{name}.png";
                            
                            // We're done!
                            Debug.Log($"Copied {tempPath} to {destPath}...");
                        }
                    }
                }
                
                // Add the resource to the dictionary!
                _resources.Add(name, resource);
                yield return null;
            }
        }

        protected override IEnumerator BuildUserSkin(UserSkin skin)
        {
            skin.Metadata.HasDarkMode = false;
            skin.Metadata.HasPanicMode = false;

            // Wallpaper.
            skin.Wallpapers.Light.Wallpaper.Color = _desktopBackgroundColor;
            skin.Wallpapers.Light.Wallpaper.Image = _resources["wallpaper"];

            // ShiftOS style client area.
            var decoration = skin.Decorations.Light;
            decoration.ClientBackground.Background.Color = "#ffffff";

            // Close button.
            decoration.CloseButton.Size = _closeButtonSize;
            decoration.CloseButton.Offset = _closeButtonOffset;
            decoration.CloseButton.Idle.Color = _closeButtonColor;
            decoration.CloseButton.Hovered.Color = _closeButtonColor;
            decoration.CloseButton.Pressed.Color = _closeButtonColor;
            decoration.CloseButton.Idle.Image = _resources["closebutton"];
            decoration.CloseButton.Hovered.Image = _resources["closebuttonhover"];
            decoration.CloseButton.Pressed.Image = _resources["closebuttonpress"];

            // Maximize
            decoration.MaximizeButton.Size = _rollButtonSize;
            decoration.MaximizeButton.Offset = _rollButtonOffset;
            decoration.MaximizeButton.Idle.Color = _rollButtonColor;
            decoration.MaximizeButton.Hovered.Color = _rollButtonColor;
            decoration.MaximizeButton.Pressed.Color = _rollButtonColor;
            decoration.MaximizeButton.Idle.Image = _resources["maximizebutton"];
            decoration.MaximizeButton.Hovered.Image = _resources["maximizebuttonhover"];
            decoration.MaximizeButton.Pressed.Image = _resources["maximizebuttonpress"];

            // Minimize
            decoration.MinimizeButton.Size = _minimizeButtonSize;
            decoration.MinimizeButton.Offset = _minimizeButtonOffset;
            decoration.MinimizeButton.Idle.Color = _minimizeButtonColor;
            decoration.MinimizeButton.Hovered.Color = _minimizeButtonColor;
            decoration.MinimizeButton.Pressed.Color = _minimizeButtonColor;
            decoration.MinimizeButton.Idle.Image = _resources["minimizebutton"];
            decoration.MinimizeButton.Hovered.Image = _resources["minimizebuttonhover"];
            decoration.MinimizeButton.Pressed.Image = _resources["minimizebuttonpress"];

            // Titlebar
            decoration.TitleBackground.Size = _titleBarHeight;
            decoration.TitleBackground.Background.Color = _titleBarColor;
            decoration.TitleBackground.Background.Image = _resources["top"];
            
            // Border sizing.
            decoration.Left.Size = _windowBorderSize;
            decoration.Bottom.Size = _windowBorderSize;
            decoration.Right.Size = _windowBorderSize;
            decoration.BottomRight.Size = _windowBorderSize;
            decoration.BottomLeft.Size = _windowBorderSize;
            decoration.TopRight.Size = _showWindowCorners ? _titleBarCornerWidth : 0;
            decoration.TopLeft.Size = _showWindowCorners ? _titleBarCornerWidth : 0;

            decoration.TitleIconOffset = _titleIconOffset;
            decoration.TitleIconSize = _panelButtonIconSize;
            
            // Border texturing
            decoration.Left.Background.Color = _windowBorderLeftColor;
            decoration.Left.Background.Image = _resources["left"];
            decoration.Right.Background.Color = _windowBorderRightColor;
            decoration.Right.Background.Image = _resources["right"];
            decoration.Bottom.Background.Color = _windowBorderBottomColor;
            decoration.Bottom.Background.Image = _resources["bottom"];
            decoration.BottomLeft.Background.Color = _windowBorderBottomLeftColor;
            decoration.BottomLeft.Background.Image = _resources["bottomleft"];
            decoration.BottomRight.Background.Color = _windowBorderBottomRightColor;
            decoration.BottomRight.Background.Image = _resources["bottomright"];
            decoration.TopLeft.Background.Color = _titleBarLeftCornerColor;
            decoration.TopLeft.Background.Image = _resources["topleft"];
            decoration.TopRight.Background.Color = _titleBarRightCornerColor;
            decoration.TopRight.Background.Image = _resources["topright"];
            
            // Title text.
            decoration.TitleTextAlignment =
                _titleTextPosition == "Centre" ? TitleTextAlignment.Centered : TitleTextAlignment.Left;
            decoration.TitleTextSize = _titleTextSize;
            decoration.TitleTextOffset = _titleTextOffset;
            decoration.TitleTextColor = _titleTextColor;
            
            // Make sure the terminal uses its own background color.
            skin.ApplicationStyle.TerminalUsesWindowColor = false;
            skin.ApplicationStyle.OverrideTerminalPalette = true;
            skin.ApplicationStyle.TerminalBackground = "#000000";
            skin.ApplicationStyle.TerminalForeground = "#858585";
            
            // Window client background should be white.
            decoration.ClientBackground.Background.Color = "#ffffff";
            
            // Status Bar style...
            var statusBar = skin.StatusBar.Light;
            statusBar.Background.Size = _desktopPanelHeight;
            statusBar.Background.Background.Color = _desktopPanelColor;
            statusBar.Background.Background.Image = _resources["statusbar"];
            statusBar.Tray.Background.Color = _clockBackgroundColor;
            statusBar.Tray.Background.Image = _resources["clock"];
            statusBar.ClockTextColor = _clockTextColor;
            statusBar.MenuLabelColor = _appLauncherLabelColor;
            statusBar.SystemMenu.Size = _appLauncherSize;
            statusBar.SystemMenu.Offset = Vector2.zero;
            statusBar.SystemMenu.Idle.Color = _appLauncherButtonColor;
            statusBar.SystemMenu.Idle.Image = _resources["menu"];
            statusBar.SystemMenu.Hovered.Color = _appLauncherButtonHoverColor;
            statusBar.SystemMenu.Hovered.Image = _resources["menuhover"];
            statusBar.SystemMenu.Pressed.Color = _appLauncherButtonClickColor;
            statusBar.SystemMenu.Pressed.Image = _resources["menupress"];
            statusBar.ActiveSystemMenu = statusBar.SystemMenu;
            statusBar.TaskListButton.Size = _panelButtonSize;
            statusBar.TaskListButton.Offset = new Vector2(_panelButtonGap, 0);
            statusBar.TaskListButton.Idle.Color = _panelButtonColor;
            statusBar.TaskListButton.Idle.Image = _resources["statusbarbutton"];
            statusBar.TaskListButton.Hovered = statusBar.TaskListButton.Idle;
            statusBar.TaskListButton.Pressed = statusBar.TaskListButton.Idle;
            statusBar.ActiveTaskListButton = statusBar.TaskListButton;
            statusBar.TaskListButtonTextColor = _panelButtonTextColor;
            statusBar.ActiveTaskListButtonTextColor = _panelButtonTextColor;
            
            var statusBarSettings = skin.StatusBar.Settings;
            statusBarSettings.Placement =
                _desktopPanelPosition == "Top" ? StatusBarPlacement.Top : StatusBarPlacement.Bottom;
            statusBarSettings.ClockFontSize = _clockSize;
            statusBarSettings.ClockOffset = _clockOffset;
            statusBarSettings.MenuLabel = _appLauncherLabel;
            statusBarSettings.MenuLabelFontSize = _appLauncherLabelSize;
            statusBarSettings.TaskListOffset = _panelButtonsOffset;
            statusBarSettings.TaskListIconSize = _panelButtonIconSize;
            statusBarSettings.TaskListIconOffset = _panelButtonIconOffset;
            statusBarSettings.TaskListTextSize = _panelButtonTextSize;
            statusBarSettings.TaskListTextOffset = _panelButtonTextOffset;

            // Ensure that the task list is relative to the left edge of the status panel.
            statusBarSettings.TaskListOffset = new SafeVector2(
                statusBar.SystemMenu.Offset.x + statusBar.SystemMenu.Size.x + statusBarSettings.TaskListOffset.x,
                statusBarSettings.TaskListOffset.y);
            
            yield break;
        }
    }
}