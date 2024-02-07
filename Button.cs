using System;
using SplashKitSDK;

namespace Distinction_Task 
{
    public class Button {
        private int _x, _y;
        private int _width, _height;
        private Bitmap _buttonBitmap;
        private ButtonType _buttonType;

        public Button(ButtonType buttonType, int x, int y) {
            _width = Constants.ExitResetButtonWidth;
            _height = Constants.ExitResetButtonHeight;
            _buttonType = buttonType;
            _x = x;
            _y = y;

            switch(buttonType) {
                case ButtonType.ExitButton:
                    _buttonBitmap = new Bitmap("ExitButton", Constants.ExitGameImgPath);
                    break;
                case ButtonType.ResetButton:
                    _buttonBitmap = new Bitmap("ResetButton", Constants.RestartGameImgPath);
                    break;
                case ButtonType.SinglePlayerButton:
                    _width = Constants.MainMenuButtonWidth;
                    _height = Constants.MainMenuButtonHeight;
                    break;
                case ButtonType.MultiplayerButton:
                    _width = Constants.MainMenuButtonWidth;
                    _height = Constants.MainMenuButtonHeight;
                    break;
                case ButtonType.MainMenuExitButton:
                    _width = Constants.MainMenuButtonWidth;
                    _height = Constants.MainMenuButtonHeight;
                    break;
            }
        }

        public void DrawButton() { 
            switch(_buttonType) {
                case ButtonType.ResetButton: case ButtonType.ExitButton:
                    _buttonBitmap.Draw(_x, _y);
                    break;
                case ButtonType.MainMenuExitButton:
                    SplashKit.FillRectangle(Color.Red, _x, _y, _width, _height);
                    SplashKit.DrawText("Exit", Color.White,"Resources\\Adventure.otf", 46, _x + 45 + 85, _y + 10);
                    break;
                case ButtonType.MultiplayerButton:
                    SplashKit.FillRectangle(Color.Orange, _x, _y, _width, _height);
                    SplashKit.DrawText("Multiplayer", Color.White,"Resources\\Adventure.otf", 46, _x + 45, _y + 10);
                    break;
                case ButtonType.SinglePlayerButton:
                    SplashKit.FillRectangle(Color.Blue, _x, _y, _width, _height);
                    SplashKit.DrawText("Single Player", Color.White,"Resources\\Adventure.otf", 46, _x + 45, _y + 10);
                    break;
            }
        }

        public bool IsMouseHoverOnButton(Point2D mousePosition) {
            return (mousePosition.X >= _x && mousePosition.Y >= _y && mousePosition.X < _x + _width && mousePosition.Y < _y + _height);
        }

        public int X {
            get { return _x; }
            set { _x = value; }
        }
        public int Y {
            get { return _y; }
            set { _y = value; }
        }
        public int Width {
            get { return _width; }
            set { _width = value; }
        }
        public int Height {
            get { return _height; }
            set { _height = value; }
        }
        public Bitmap ButtonBitmap {
            get { return _buttonBitmap; }
        }

        public ButtonType ButtonType {
            get { return _buttonType; }
        }
    }
}