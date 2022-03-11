using System.Collections;
using System.Collections.Generic;
using System.IO;
using Customization.User;
using Helpers;
using UnityEngine;
using static Customization.ShiftOS.PhilUtility;

namespace Customization.ShiftOS
{
    public class NewPhilImporter : ShiftOSImporter
    {
        private string _description;
        private string _temp;
        private string _skinDataFile;
        private Color _transparencyKey = new Color(1f / 255f, 0, 1f / 255f);
        #region Skin Variables

        private Dictionary<string, UserImageResource> _resources = new Dictionary<string, UserImageResource>();
        private Vector2 _closeSize;
        private Vector2 _maximizeSize;
        private Vector2 _minimizeSize;
        private int _titleBarHeight;
        private Vector2 _closeOffset;
        private Vector2 _minimizeOffset;
        private Vector2 _maximizeOffset;
        private int _borderWidth;
        private bool _enableCorners;
        private int _titleCornerWidth;
        private Vector2 _iconOffset;
        private string _titleBarColor;
        private string _borderLeftColor;
        private string _borderBottomColor;
        private string _borderRightColor;
        private string _closeButtonColor;
        private string _closeButtonHoverColor;
        private string _closeButtonPressColor;
        private string _maxButtonColor;
        private string _maxButtonHoverColor;
        private string _maxButtonPressColor;
        private string _minButtonColor;
        private string _minButtonHoverColor;
        private string _minButtonPressColor;
        private string _rightCornerColor;
        private string _leftCornerColor;
        private string _bottomRightCornerColor;
        private string _bottomLeftCornerColor;
        private int _titleTextFontSize;
        private TitleTextAlignment _titleTextAlignment;
        private Vector2 _titleTextOffset;
        private string _titleTextColor;
        private string _desktopPanelColor;
        private string _desktopBackgroundColor;
        private int _desktopPanelHeight;
        private StatusBarPlacement _desktopPanelPlacement;
        private string _clockTextColor;
        private string _clockBackgroundColor;
        private Vector2 _clockOffset;
        private int _clockTextSize;
        private string _appLauncherButtonColor;
        private string _appLauncherClickColor;
        private string _appLauncherBackgroundColor;
        private string _appLauncherMouseOverColor;
        private string _appLauncherTextColor;
        private Vector2 _appLauncherSize;
        private int _appLauncherTextSize;
        private string _appLauncherText;
        private Vector2 _panelButtonIconOffset;
        private Vector2 _panelButtonSize;
        private Vector2 _panelButtonIconSize;
        private string _panelButtonColor;
        private string _panelButtonTextColor;
        private int _panelButtonTextSize;
        private Vector2 _panelButtonTextOffset;
        private Vector2 _panelButtonOffset;
        private Vector2 _panelButtonsOffset;
        
        #endregion
        
        public NewPhilImporter(string temp, string destinationPath) : base(destinationPath)
        {
            _temp = temp;
            _skinDataFile = Path.Combine(_temp, "data.dat");
        }

        protected override IEnumerator LoadData()
        {
            using var stream = File.OpenRead(_skinDataFile);
            using var reader = new StreamReader(stream);

            _description = reader.ReadLine();
            
            // Caption button sizes.
            _closeSize = ReadVector(reader);
            _maximizeSize = ReadVector(reader);
            _minimizeSize = ReadVector(reader);
            _titleBarHeight = ReadNumber(reader);

            _closeOffset = ReadBackwardsVector(reader);
            _maximizeOffset = ReadBackwardsVector(reader);
            _minimizeOffset = ReadBackwardsVector(reader);

            _borderWidth = ReadNumber(reader);

            _enableCorners = ReadBoolean(reader);

            _titleCornerWidth = ReadNumber(reader);

            _iconOffset = ReadVector(reader);

            _titleBarColor = ReadColor(reader);
            _borderLeftColor = ReadColor(reader);
            _borderRightColor = ReadColor(reader);
            _borderBottomColor = ReadColor(reader);

            _closeButtonColor = ReadColor(reader);
            _closeButtonHoverColor = ReadColor(reader);
            _closeButtonPressColor = ReadColor(reader);
            
            _maxButtonColor = ReadColor(reader);
            _maxButtonHoverColor = ReadColor(reader);
            _maxButtonPressColor = ReadColor(reader);
            
            _minButtonColor = ReadColor(reader);
            _minButtonHoverColor = ReadColor(reader);
            _minButtonPressColor = ReadColor(reader);

            _rightCornerColor = ReadColor(reader);
            _leftCornerColor = ReadColor(reader);
            _bottomRightCornerColor = ReadColor(reader);
            _bottomLeftCornerColor = ReadColor(reader);
            
            // Skip title text font family.
            reader.ReadLine();
            
            _titleTextFontSize = ReadNumber(reader);
            
            // Skip the font style.
            reader.ReadLine();
            yield return null;
            
            _titleTextAlignment = reader.ReadLine() == "Center" ? TitleTextAlignment.Centered : TitleTextAlignment.Left;

            _titleTextOffset = ReadBackwardsVector(reader);

            _titleTextColor = ReadColor(reader);

            _desktopPanelColor = ReadColor(reader);
            _desktopBackgroundColor = ReadColor(reader);
            _desktopPanelHeight = ReadNumber(reader);
            _desktopPanelPlacement = reader.ReadLine() == "Top" ? StatusBarPlacement.Top : StatusBarPlacement.Bottom;

            _clockTextColor = ReadColor(reader);
            _clockBackgroundColor = ReadColor(reader);
            _clockOffset.y = ReadNumber(reader);
            _clockTextSize = ReadNumber(reader);
            
            // skip font and style.
            reader.ReadLine();
            reader.ReadLine();
            yield return null;

            _appLauncherButtonColor = ReadColor(reader);
            _appLauncherClickColor = ReadColor(reader);
            _appLauncherBackgroundColor = ReadColor(reader);
            _appLauncherMouseOverColor = ReadColor(reader);
            _appLauncherTextColor = ReadColor(reader);

            _appLauncherSize.y = ReadNumber(reader);
            _appLauncherTextSize = ReadNumber(reader);
                
            // Skip the font.
            reader.ReadLine();
            reader.ReadLine();
            yield return null;
            
            _appLauncherText = reader.ReadLine();
            
            // This...exists again for some reason.
            _titleTextAlignment = reader.ReadLine() == "Center" ? TitleTextAlignment.Centered : TitleTextAlignment.Left;

            _appLauncherSize.x = ReadNumber(reader);

            _panelButtonIconOffset = ReadBackwardsVector(reader);
            _panelButtonIconSize.x = ReadNumber(reader);
            _panelButtonIconSize.y = _panelButtonIconSize.x;
            _panelButtonSize = ReadBackwardsVector(reader);
            _panelButtonColor = ReadColor(reader);
            _panelButtonTextColor = ReadColor(reader);
            _panelButtonTextSize = ReadNumber(reader);
            
            // skip the fucking FONT
            reader.ReadLine();
            reader.ReadLine();
            yield return null;

            _panelButtonTextOffset = ReadVector(reader);

            _panelButtonOffset = ReadVector(reader);
            _panelButtonsOffset.x = ReadNumber(reader);
            
            // TODO: Image layouts. Skip 'em for now.
            for (var i = 89; i < 109; i++)
                reader.ReadLine();
            
            // Everything beyond this point is Late 0.0.8 / 0.0.9 stuff, that was added
            // to ShiftOS when I was a developer.  These values WILL NEVER be applicable to
            // Socially Distant, don't even BOTHER trying to load them.
            //  - Michael
        }

        protected override IEnumerator CopyImages()
        {
            var resPath = Path.Combine(SkinDestination, "resources");
            if (!Directory.Exists(resPath))
                Directory.CreateDirectory(resPath);
            
            var imageNames = new string[]
            {
                "titlebar",
                "borderleft",
                "borderright",
                "borderbottom",
                "closebtn",
                "closebtnhover",
                "closebtnclick",
                "rollbtn",
                "rollbtnhover",
                "rollbtnclick",
                "minbtn",
                "minbtnhover",
                "minbtnclick",
                "rightcorner",
                "leftcorner",
                "desktoppanel",
                "desktopbackground",
                "panelbutton",
                "applaunchermouseover",
                "applauncher",
                "applauncherclick",
                "panelclock",
                "bottomleftcorner",
                "bottomrightcorner"
            };

            foreach (var name in imageNames)
            {
                var tempPath = Path.Combine(_temp, name);
                if (File.Exists(tempPath))
                {
                    // Load the image as a unity texture.
                    var texture = new Texture2D(1, 1);
                    if (UnityHelpers.LoadImageFromPath(texture, tempPath, _transparencyKey))
                    {
                        // Save the texture to its new location!
                        var png = texture.EncodeToPNG();
                        var newPath = Path.Combine(resPath, name + ".png");
                        File.WriteAllBytes(newPath, png);

                        _resources.Add(name, new UserImageResource()
                        {
                            ResourcePath = "/resources/" + name + ".png",
                            Source = UserImageSource.External
                        });
                        yield return null;
                        continue;
                    }
                }

                _resources.Add(name, new UserImageResource
                {
                    ResourcePath = null,
                    Source = UserImageSource.None
                });
                yield return null;
            }
        }

        protected override IEnumerator BuildUserSkin(UserSkin skin)
        {
            skin.Metadata.HasDarkMode = false;
            skin.Metadata.HasPanicMode = false;

            // Wallpaper.
            skin.Wallpapers.Light.Wallpaper.Color = _desktopBackgroundColor;
            skin.Wallpapers.Light.Wallpaper.Image = _resources["desktopbackground"];

            // ShiftOS style client area.
            var decoration = skin.Decorations.Light;
            decoration.ClientBackground.Background.Color = "#ffffff";

            // Close button.
            decoration.CloseButton.Size = _closeSize;
            decoration.CloseButton.Offset = _closeOffset;
            decoration.CloseButton.Idle.Color = _closeButtonColor;
            decoration.CloseButton.Hovered.Color = _closeButtonHoverColor;
            decoration.CloseButton.Pressed.Color = _closeButtonPressColor;
            decoration.CloseButton.Idle.Image = _resources["closebtn"];
            decoration.CloseButton.Hovered.Image = _resources["closebtnhover"];
            decoration.CloseButton.Pressed.Image = _resources["closebtnclick"];

            // Maximize
            decoration.MaximizeButton.Size = _maximizeSize;
            decoration.MaximizeButton.Offset = _maximizeOffset;
            decoration.MaximizeButton.Idle.Color = _maxButtonColor;
            decoration.MaximizeButton.Hovered.Color = _maxButtonHoverColor;
            decoration.MaximizeButton.Pressed.Color = _maxButtonPressColor;
            decoration.MaximizeButton.Idle.Image = _resources["rollbtn"];
            decoration.MaximizeButton.Hovered.Image = _resources["rollbtnhover"];
            decoration.MaximizeButton.Pressed.Image = _resources["rollbtnclick"];

            // Minimize
            decoration.MinimizeButton.Size = _minimizeSize;
            decoration.MinimizeButton.Offset = _minimizeOffset;
            decoration.MinimizeButton.Idle.Color = _minButtonColor;
            decoration.MinimizeButton.Hovered.Color = _minButtonHoverColor;
            decoration.MinimizeButton.Pressed.Color = _minButtonPressColor;
            decoration.MinimizeButton.Idle.Image = _resources["minbtn"];
            decoration.MinimizeButton.Hovered.Image = _resources["minbtnhover"];
            decoration.MinimizeButton.Pressed.Image = _resources["minbtnclick"];

            // Titlebar
            decoration.TitleBackground.Size = _titleBarHeight;
            decoration.TitleBackground.Background.Color = _titleBarColor;
            decoration.TitleBackground.Background.Image = _resources["titlebar"];
            
            // Border sizing.
            decoration.Left.Size = _borderWidth;
            decoration.Bottom.Size = _borderWidth;
            decoration.Right.Size = _borderWidth;
            decoration.BottomRight.Size = _borderWidth;
            decoration.BottomLeft.Size = _borderWidth;
            decoration.TopRight.Size = _enableCorners ? _titleCornerWidth : 0;
            decoration.TopLeft.Size = _enableCorners ? _titleCornerWidth : 0;

            decoration.TitleIconOffset = _iconOffset;
            decoration.TitleIconSize = _panelButtonIconSize;
            
            // Border texturing
            decoration.Left.Background.Color = _borderLeftColor;
            decoration.Left.Background.Image = _resources["borderleft"];
            decoration.Right.Background.Color = _borderRightColor;
            decoration.Right.Background.Image = _resources["borderright"];
            decoration.Bottom.Background.Color = _borderBottomColor;
            decoration.Bottom.Background.Image = _resources["borderbottom"];
            decoration.BottomLeft.Background.Color = _bottomLeftCornerColor;
            decoration.BottomLeft.Background.Image = _resources["bottomleftcorner"];
            decoration.BottomRight.Background.Color = _bottomRightCornerColor;
            decoration.BottomRight.Background.Image = _resources["bottomrightcorner"];
            decoration.TopLeft.Background.Color = _leftCornerColor;
            decoration.TopLeft.Background.Image = _resources["leftcorner"];
            decoration.TopRight.Background.Color = _rightCornerColor;
            decoration.TopRight.Background.Image = _resources["rightcorner"];
            
            // Title text.
            decoration.TitleTextAlignment = _titleTextAlignment;
            decoration.TitleTextSize = _titleTextFontSize;
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
            statusBar.Background.Background.Image = _resources["desktoppanel"];
            statusBar.Tray.Background.Color = _clockBackgroundColor;
            statusBar.Tray.Background.Image = _resources["panelclock"];
            statusBar.ClockTextColor = _clockTextColor;
            statusBar.MenuLabelColor = _appLauncherTextColor;
            statusBar.SystemMenu.Size = _appLauncherSize;
            statusBar.SystemMenu.Offset = Vector2.zero;
            statusBar.SystemMenu.Idle.Color = _appLauncherButtonColor;
            statusBar.SystemMenu.Idle.Image = _resources["applauncher"];
            statusBar.SystemMenu.Hovered.Color = _appLauncherMouseOverColor;
            statusBar.SystemMenu.Hovered.Image = _resources["applaunchermouseover"];
            statusBar.SystemMenu.Pressed.Color = _appLauncherClickColor;
            statusBar.SystemMenu.Pressed.Image = _resources["applauncherclick"];
            statusBar.ActiveSystemMenu = statusBar.SystemMenu;
            statusBar.TaskListButton.Size = _panelButtonSize;
            statusBar.TaskListButton.Offset = _panelButtonOffset;
            statusBar.TaskListButton.Idle.Color = _panelButtonColor;
            statusBar.TaskListButton.Idle.Image = _resources["panelbutton"];
            statusBar.TaskListButton.Hovered = statusBar.TaskListButton.Idle;
            statusBar.TaskListButton.Pressed = statusBar.TaskListButton.Idle;
            statusBar.ActiveTaskListButton = statusBar.TaskListButton;
            statusBar.TaskListButtonTextColor = _panelButtonTextColor;
            statusBar.ActiveTaskListButtonTextColor = _panelButtonTextColor;
            
            var statusBarSettings = skin.StatusBar.Settings;
            statusBarSettings.Placement = _desktopPanelPlacement;
            statusBarSettings.ClockFontSize = _clockTextSize;
            statusBarSettings.ClockOffset = _clockOffset;
            statusBarSettings.MenuLabel = _appLauncherText;
            statusBarSettings.MenuLabelFontSize = _appLauncherTextSize;
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