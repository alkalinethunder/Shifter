using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Customization.User;
using Helpers;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Customization.ShiftOS
{
    public class OneShiftOSImporter : ShiftOSImporter
    {
        /// <summary>
        /// Format of a ShiftOS 1.0 skin.
        /// </summary>
        private class DataFormat
        {
            public string CloseButtonImage;
            public string MinimizeButtonImage;
            public string MaximizeButtonImage;
            public string DesktopBackgroundImage;
            public string AppLauncherImage;
            public string DesktopPanelBackground;
            public string TitleBarBackground;
            public string TitleLeftBG;
            public string TitleRightBG;
            public string BottomBorderBG;
            public string RightBorderBG;
            public string LeftBorderBG;
            public string BottomLBorderBG;
            public string BottomRBorderBG;
            public string PanelButtonBG;

            public string TitleFont;
            public int TitleButtonPosition; // not sure what to do with this
            public string ControlColor;
            public string ControlTextColor;
            public string TitleTextColor;
            public string TitleBackgroundColor;
            public string BorderRightBackground;
            public string BorderLeftBackground;
            public int PanelButtonFromTop;
            public string BorderBottomBackground;
            public int PanelButtonHolderFromLeft;
            public string BorderBottomRightBackground;
            public string BorderBottomLeftBackground;
            public string CloseButtonColor;
            public string MinimizeButtonColor;
            public string MaximizeButtonColor;
            public string DesktopPanelColor;
            public string DesktopPanelClockColor;
            public string DesktopPanelClockBackgroundColor;
            public string DesktopPanelClockFont;
            public string DesktopPanelClockFromRight;
            public int DesktopPanelHeight;
            public StatusBarPlacement DesktopPanelPosition;
            public int TitleBarHeight;
            public string CloseButtonSize;
            public string MinimizeButtonSize;
            public string MaximizeButtonSize;
            public string CloseButtonFromSide;
            public string MaximizeButtonFromSide;
            public string MinimizeButtonFromSide;
            public bool TitleTextCentered;
            public string TitleTextLeft;
            public string DesktopColor;
            public string AppLauncherTextColor;
            public string AppLauncherSelectedTextColor;
            public string AppLauncherFont;
            public string AppLauncherText;
            public string AppLauncherFromLeft;
            public string AppLauncherHolderSize;
            public string TerminalForeColor;
            public string TerminalBackColor;
            public bool ShowTitleCorners;
            public string TitleLeftCornerBackground;
            public string TitleRightCornerBackground;
            public int TitleLeftCornerWidth;
            public int TitleRightCornerWidth;
            public int LeftBorderWidth;
            public int RightBorderWidth;
            public int BottomBorderWidth;
            public string PanelButtonSize;          
            public string PanelButtonColor;
            public string PanelButtonTextColor;
            public string PanelButtonFromLeft;
            public string PanelButtonFont;
            public string TitlebarIconFromSide = "4, 4";
            public string SystemKey;

            public ImageLayouts SkinImageLayouts = new ImageLayouts();
            
            // ToolStripColorTable entries relevant to the app launcher
            public string Menu_MenuStripGradientBegin;
            public string Menu_MenuItemSelectedGradientBegin;
            public string Menu_MenuItemPressedGradientBegin;
        }

        private class ImageLayouts
        {
            public ImageLayout desktopbackground;
            public ImageLayout desktoppanel;
            public ImageLayout titleleft;
            public ImageLayout titleright;
            public ImageLayout leftborder;
            public ImageLayout rightborder;
            public ImageLayout bottomborder;
            public ImageLayout bottomrborder;
            public ImageLayout bottomlborder;
            public ImageLayout closebutton;
            public ImageLayout minimizebutton;
            public ImageLayout maximizebutton;
            public ImageLayout panelbutton;
            public ImageLayout titlebar;
            public ImageLayout applauncher;
        }

        private Color _transparencyKey;
        private DataFormat _skinData;
        private string _jsonSkinPath;
        private Dictionary<string, UserImageResource> _resources = new Dictionary<string, UserImageResource>();
        private string _resourcePath;

        public OneShiftOSImporter(string jsonSkin, string destinationPath) : base(destinationPath)
        {
            _jsonSkinPath = jsonSkin;
        }

        protected override IEnumerator LoadData()
        {
            try
            {
                var json = File.ReadAllText(_jsonSkinPath);
                _skinData = JsonConvert.DeserializeObject<DataFormat>(json);

                var key = ParseGdiColor(_skinData.SystemKey ?? "1, 0, 1");
                if (!ColorUtility.TryParseHtmlString(key, out _transparencyKey))
                {
                    _transparencyKey = new Color(1f / 255f, 0, 1f / 255f);
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
            }

            yield break;
        }

        protected override IEnumerator CopyImages()
        {
            var imageNames = new string[] {
                nameof(_skinData.CloseButtonImage),
                nameof(_skinData.MinimizeButtonImage),
                nameof(_skinData.MaximizeButtonImage),
                nameof(_skinData.DesktopBackgroundImage),
                nameof(_skinData.AppLauncherImage),
                nameof(_skinData.DesktopPanelBackground),
                nameof(_skinData.TitleBarBackground),
                nameof(_skinData.TitleLeftBG),
                nameof(_skinData.TitleRightBG),
                nameof(_skinData.BottomBorderBG),
                nameof(_skinData.RightBorderBG),
                nameof(_skinData.LeftBorderBG),
                nameof(_skinData.BottomLBorderBG),
                nameof(_skinData.BottomRBorderBG),
                nameof(_skinData.PanelButtonBG)
            };

            var images = new string[] 
            {
                _skinData.CloseButtonImage,
                _skinData.MinimizeButtonImage,
                _skinData.MaximizeButtonImage,
                _skinData.DesktopBackgroundImage,
                _skinData.AppLauncherImage,
                _skinData.DesktopPanelBackground,
                _skinData.TitleBarBackground,
                _skinData.TitleLeftBG,
                _skinData.TitleRightBG,
                _skinData.BottomBorderBG,
                _skinData.RightBorderBG,
                _skinData.LeftBorderBG,
                _skinData.BottomLBorderBG,
                _skinData.BottomRBorderBG,
                _skinData.PanelButtonBG
            };

            _resourcePath = Path.Combine(this.SkinDestination, "resources");
            if (!Directory.Exists(_resourcePath))
            {
                Debug.Log("Creating the resource folder");
                Directory.CreateDirectory(_resourcePath);
                yield return null;
            }

            for (var i = 0; i < imageNames.Length; i++)
            {
                var imageName = imageNames[i];
                var image = images[i];
                var resource = new UserImageResource();
                resource.Source = UserImageSource.None;
                
                if (!string.IsNullOrWhiteSpace(image))
                {
                    var bytes = Convert.FromBase64String(image);
                    yield return null;

                    var destPath = Path.Combine(_resourcePath, imageName + ".png");
                    var destTemp = destPath + ".temp";
                    
                    File.WriteAllBytes(destTemp, bytes);
                    yield return null;

                    var texture = new Texture2D(1, 1);
                    if (UnityHelpers.LoadImageFromPath(texture, destTemp, _transparencyKey))
                    {
                        var pngData = texture.EncodeToPNG();
                        File.WriteAllBytes(destPath, pngData);
                        yield return null;

                        resource.Source = UserImageSource.External;
                        resource.ResourcePath = $"/resources/{imageName}.png";

                        Debug.Log($"Extracted image {imageName} to {destPath}...");
                        File.Delete(destTemp);
                        yield return null;
                    }
                }
                
                _resources.Add(imageName, resource);
                yield return null;
            }
        }

        protected override IEnumerator BuildUserSkin(UserSkin skin)
        {
            skin.Metadata.HasDarkMode = false;
            skin.Metadata.HasPanicMode = false;

            // Wallpaper.
            skin.Wallpapers.Light.Wallpaper.Color = _skinData.DesktopColor;
            skin.Wallpapers.Light.Wallpaper.Image = _resources[nameof(_skinData.DesktopBackgroundImage)];

            // ShiftOS style client area.   
            var decoration = skin.Decorations.Light;
            decoration.ClientBackground.Background.Color = "#ffffff";

            // Close button.
            decoration.CloseButton.Size = ParseVector2(_skinData.CloseButtonSize);
            decoration.CloseButton.Offset = ParseVector2(_skinData.CloseButtonFromSide);
            decoration.CloseButton.Idle.Color = ParseGdiColor(_skinData.CloseButtonColor);
            decoration.CloseButton.Hovered.Color = ParseGdiColor(_skinData.CloseButtonColor);
            decoration.CloseButton.Pressed.Color = ParseGdiColor(_skinData.CloseButtonColor);
            decoration.CloseButton.Idle.Image = _resources[nameof(_skinData.CloseButtonImage)];
            decoration.CloseButton.Hovered.Image = _resources[nameof(_skinData.CloseButtonImage)];
            decoration.CloseButton.Pressed.Image = _resources[nameof(_skinData.CloseButtonImage)];

            // Maximize
            decoration.MaximizeButton.Size = ParseVector2(_skinData.MaximizeButtonSize);
            decoration.MaximizeButton.Offset = ParseVector2(_skinData.MaximizeButtonFromSide);
            decoration.MaximizeButton.Idle.Color = ParseGdiColor(_skinData.MaximizeButtonColor);
            decoration.MaximizeButton.Hovered.Color = ParseGdiColor(_skinData.MaximizeButtonColor);
            decoration.MaximizeButton.Pressed.Color = ParseGdiColor(_skinData.MaximizeButtonColor);
            decoration.MaximizeButton.Idle.Image = _resources[nameof(_skinData.MaximizeButtonImage)];
            decoration.MaximizeButton.Hovered.Image = _resources[nameof(_skinData.MaximizeButtonImage)];
            decoration.MaximizeButton.Pressed.Image = _resources[nameof(_skinData.MaximizeButtonImage)];

            // Minimize
            decoration.MinimizeButton.Size = ParseVector2(_skinData.MinimizeButtonSize);
            decoration.MinimizeButton.Offset = ParseVector2(_skinData.MinimizeButtonFromSide);
            decoration.MinimizeButton.Idle.Color = ParseGdiColor(_skinData.MinimizeButtonColor);
            decoration.MinimizeButton.Hovered.Color = ParseGdiColor(_skinData.MinimizeButtonColor);
            decoration.MinimizeButton.Pressed.Color = ParseGdiColor(_skinData.MinimizeButtonColor);
            decoration.MinimizeButton.Idle.Image = _resources[nameof(_skinData.MinimizeButtonImage)];
            decoration.MinimizeButton.Hovered.Image = _resources[nameof(_skinData.MinimizeButtonImage)];
            decoration.MinimizeButton.Pressed.Image = _resources[nameof(_skinData.MinimizeButtonImage)];

            // Titlebar
            decoration.TitleBackground.Size = _skinData.TitleBarHeight;
            decoration.TitleBackground.Background.Color = ParseGdiColor(_skinData.TitleBackgroundColor);
            decoration.TitleBackground.Background.Image = _resources[nameof(_skinData.TitleBarBackground)];
            
            // Border sizing.
            decoration.Left.Size = _skinData.LeftBorderWidth;
            decoration.Bottom.Size = _skinData.BottomBorderWidth;
            decoration.Right.Size = _skinData.RightBorderWidth;
            decoration.BottomRight.Size = Math.Min(_skinData.BottomBorderWidth, _skinData.RightBorderWidth);
            decoration.BottomLeft.Size = Math.Min(_skinData.LeftBorderWidth, _skinData.BottomBorderWidth);
            decoration.TopRight.Size = (_skinData.ShowTitleCorners) ? _skinData.TitleRightCornerWidth : 0;
            decoration.TopLeft.Size = (_skinData.ShowTitleCorners) ? _skinData.TitleLeftCornerWidth : 0;


            decoration.TitleIconOffset = ParseVector2(_skinData.TitlebarIconFromSide);
            decoration.TitleIconSize = new SafeVector2(16, 16);
            
            // Border texturing
            decoration.Left.Background.Color = ParseGdiColor(_skinData.BorderLeftBackground);
            decoration.Left.Background.Image = _resources[nameof(_skinData.LeftBorderBG)];
            decoration.Right.Background.Color = ParseGdiColor(_skinData.BorderRightBackground);
            decoration.Right.Background.Image = _resources[nameof(_skinData.RightBorderBG)];
            decoration.Bottom.Background.Color = ParseGdiColor(_skinData.BorderBottomBackground);
            decoration.Bottom.Background.Image = _resources[nameof(_skinData.BottomBorderBG)];
            decoration.BottomLeft.Background.Color = ParseGdiColor(_skinData.BorderBottomLeftBackground);
            decoration.BottomLeft.Background.Image = _resources[nameof(_skinData.BottomLBorderBG)];
            decoration.BottomRight.Background.Color = ParseGdiColor(_skinData.BorderBottomRightBackground);
            decoration.BottomRight.Background.Image = _resources[nameof(_skinData.BottomRBorderBG)];
            decoration.TopLeft.Background.Color = ParseGdiColor(_skinData.TitleLeftCornerBackground);
            decoration.TopLeft.Background.Image = _resources[nameof(_skinData.TitleLeftBG)];
            decoration.TopRight.Background.Color = ParseGdiColor(_skinData.TitleRightCornerBackground);
            decoration.TopRight.Background.Image = _resources[nameof(_skinData.TitleRightBG)];
            
            // Title text.
            decoration.TitleTextAlignment = _skinData.TitleTextCentered
                ? TitleTextAlignment.Centered
                : TitleTextAlignment.Left;
            decoration.TitleTextSize = ParseFontSize(_skinData.TitleFont);
            decoration.TitleTextOffset = ParseVector2(_skinData.TitleTextLeft);
            decoration.TitleTextColor = ParseGdiColor(_skinData.TitleTextColor);
            
            // Make sure the terminal uses its own background color.
            skin.ApplicationStyle.TerminalUsesWindowColor = false;
            skin.ApplicationStyle.OverrideTerminalPalette = true;
            skin.ApplicationStyle.TerminalBackground = ParseGdiColor(_skinData.TerminalBackColor);
            skin.ApplicationStyle.TerminalForeground = ParseGdiColor(_skinData.TerminalForeColor);
   
            // Set the window background to match what it'd be in ShiftOS.
            decoration.ClientBackground.Background.Color = ParseGdiColor(_skinData.ControlColor);
            
            // Status Bar style...
            var statusBar = skin.StatusBar.Light;
            statusBar.Background.Size = _skinData.DesktopPanelHeight;
            statusBar.Background.Background.Color = ParseGdiColor(_skinData.DesktopPanelColor);
            statusBar.Background.Background.Image = _resources[nameof(_skinData.DesktopPanelBackground)];
            statusBar.Tray.Background.Color = ParseGdiColor(_skinData.DesktopPanelClockBackgroundColor);
            statusBar.Tray.Background.Image = _resources[nameof(_skinData.DesktopPanelBackground)];
            statusBar.ClockTextColor = ParseGdiColor(_skinData.DesktopPanelClockColor);
            statusBar.MenuLabelColor = ParseGdiColor(_skinData.AppLauncherTextColor);
            statusBar.SystemMenu.Size = ParseVector2(_skinData.AppLauncherHolderSize);
            statusBar.SystemMenu.Offset = ParseVector2(_skinData.AppLauncherFromLeft);
            statusBar.SystemMenu.Idle.Color = ParseGdiColor(_skinData.Menu_MenuStripGradientBegin);
            statusBar.SystemMenu.Idle.Image = _resources[nameof(_skinData.AppLauncherImage)];
            statusBar.SystemMenu.Hovered.Color = ParseGdiColor(_skinData.Menu_MenuItemSelectedGradientBegin);
            statusBar.SystemMenu.Hovered.Image = _resources[nameof(_skinData.AppLauncherImage)];
            statusBar.SystemMenu.Pressed.Color = ParseGdiColor(_skinData.Menu_MenuItemPressedGradientBegin);
            statusBar.SystemMenu.Pressed.Image = _resources[nameof(_skinData.AppLauncherImage)];
            statusBar.ActiveSystemMenu = statusBar.SystemMenu;
            statusBar.TaskListButton.Size = ParseVector2(_skinData.PanelButtonSize);
            statusBar.TaskListButton.Offset = ParseVector2(_skinData.PanelButtonFromLeft);
            statusBar.TaskListButton.Idle.Color = ParseGdiColor(_skinData.PanelButtonColor);
            statusBar.TaskListButton.Idle.Image = _resources[nameof(_skinData.PanelButtonBG)];
            statusBar.TaskListButton.Hovered = statusBar.TaskListButton.Idle;
            statusBar.TaskListButton.Pressed = statusBar.TaskListButton.Idle;
            statusBar.ActiveTaskListButton = statusBar.TaskListButton;
            statusBar.TaskListButtonTextColor = ParseGdiColor(_skinData.PanelButtonTextColor);
            statusBar.ActiveTaskListButtonTextColor = ParseGdiColor(_skinData.PanelButtonTextColor);
            
            var statusBarSettings = skin.StatusBar.Settings;
            statusBarSettings.Placement = _skinData.DesktopPanelPosition;
            statusBarSettings.ClockFontSize = ParseFontSize(_skinData.DesktopPanelClockFont);
            statusBarSettings.ClockOffset = ParseVector2(_skinData.DesktopPanelClockFromRight);
            statusBarSettings.MenuLabel = _skinData.AppLauncherText;
            statusBarSettings.MenuLabelFontSize = ParseFontSize(_skinData.AppLauncherFont);
            statusBarSettings.TaskListOffset =
                new SafeVector2(_skinData.PanelButtonHolderFromLeft, _skinData.PanelButtonFromTop);
            statusBarSettings.TaskListIconSize = new SafeVector2(16, 16);
            statusBarSettings.TaskListIconOffset = new SafeVector2(4, 4);
            statusBarSettings.TaskListTextSize = ParseFontSize(_skinData.PanelButtonFont);
            statusBarSettings.TaskListTextOffset = new SafeVector2(24, 4);
            
            yield break;
        }

        private float ParseFontSize(string gdiFontDescriptor)
        {
            var commas = gdiFontDescriptor.Split(new[] {", "}, StringSplitOptions.None);

            var pointSize = commas.FirstOrDefault(x => x.Length > 0 && char.IsDigit(x[0]) && x.EndsWith("pt"));

            if (!string.IsNullOrWhiteSpace(pointSize))
            {
                var pointValue = pointSize.Substring(0, pointSize.Length - 2);
                return float.Parse(pointValue);
            }

            return 9;
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