using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class Constants
    {
        // Acceptable User Inputs
        public static readonly string[] AcceptableInputs = {
            // 0 to 7: Movement
            "UP", "MOVE UP", // 0 & 1
            "DOWN", "MOVE DOWN", // 2 & 3
            "LEFT", "MOVE LEFT", // 4 & 5
            "RIGHT", "MOVE RIGHT", // 6 & 7
            // 8 to 9: Fence Placement
            "PLACE FENCE", "FENCE", // 8 & 9
            // 10 to 15: Fence Orientation
            "INVALID", "HORIZONTALLY", "PLACE HORIZONTALLY", // 10, 11, 12
            "VERTICAL", "VERTICALLY", "PLACE VERTICALLY", // 13, 14, 15
            "RESET", // 16: Reset Game
            // > 16: Exit program
            "EXIT", "EXIT GAME", "EXIT APPLICATION",
            "QUIT", "QUIT GAME", "QUIT APPLICATION"
        };

        // Buffs and Debuffs
        public const string BoostedStaminaName = "Stamina Boost";
        public const string BoostedStaminaDesc = "Gives player a boost in stamina, allowing player to move to a new tile and move their own fence at the same turn. Lasts one turn.";
        public const string BoostedStaminaApplyDesc = "Player gets an extra turn";
        public const string BuffBoostedStaminaApplyDesc = "Gives you an extra move.";
        public const string DebuffBoostedStaminaApplyDesc = "Opponent gets an extra move.";
        public const string BoostedStaminaShort = "SB";
        public const string TimeStopName = "Time Stop";
        public const string TimeStopDesc = "Stops a player from taking action for one turn. Buff - Stops opponents time, Debuff - Stops players time.";
        public const string TimeStopShort = "TS";
        public const string TimeStopApplyDesc = "Your opponent's time is frozen for one turn";
        public const string BuffTimeStopApplyDesc = "Stops your opponent from moving (1 turn)";
        public const string DebufTimeStopApplyDesc = "Stops you from moving the next turn.";
        public const string GhostFormName = "Ghost Form";
        public const string GhostFormDesc = "Allows players to directly walk through walls. Buffs - applies on self, Debuff - applies on opponent.";
        public const string GhostFormShort = "GF";
        public const string GhostFormApplyDesc = "Can pass through walls";
        public const string BuffGhostFormApplyDesc = "You can pass through walls";
        public const string DebuffGhostFormApplyDesc = "Your opponent can pass through walls";

        public const string NormalStateName = "Normal";
        public const string NormalStateDesc = "No abnormal conditions";
        // Unicode to identify Players and Computer
        public const string PlayerOneSymbol = "\u03B1"; // alpha
        public const string PlayerTwoSymbol = "\u03B2"; // beta
        public const string AISymbol = "\u03A9"; // omega

        // Unicode to indicate buffs and debuffs
        public const string BuffIndicator = "\u25B2";
        public const string DebuffIndicator = "\u25BC";

        // Unicode for building Board UI
        // Box UI:
        // Corner - Horizontal - Horizontal - Horizontal - Corner
        // Vertical - Empty - Empty - Empty - Vertical
        // Corner - Horizontal - Horizontal - Horizontal - Corner
        public const string BoxHorizontalLine = "\u2500"; // Needs 3 horizontal lines per box
        public const string BoxTopLeftCorner = "\u250C";
        public const string BoxTopRightCorner = "\u2510";
        public const string BoxBottomRightCorner = "\u2518";
        public const string BoxBottomLeftCorner = "\u2514";
        public const string BoxVerticalLine = "\u2502"; // Needs 1 Vertical for 1 box
        public const string FenceHorizontal = "\u2550"; // Takes 5 to cover 1 box -> 10 + 1 empty space
        public const string FenceVertical = "\u2551"; // Takes 3 to cover 1 box -> 6 + 1 empty space
        public const string BlankSpace = " ";
        public const string BlankSpaceTwo = "  ";
        public const string BlankSpaceThree = "   ";
        public const string BlankSpaceFour = "    ";
        public const string BlankSpaceFive = "     ";

        // UI Building for SplashKit
        // Columns = X-Axis, Rows = Y-Axis
        public const int TileWidth = 40;
        public const int TileHeight = 40;
        public const int WindowPadding = 10;
        public const int PlayerTilePadding = 2;
        public const int BoardToBackBoardPadding = 15 + 15;
        public const int WindowWidth = 1200;
        public const int WindowHeight = 760;
        public const int ExitResetButtonWidth = 50;
        public const int ExitResetButtonHeight = 50;
        public const int DescriptionBackBoardX = 780;
        public const int DescriptionBackBoardY = 330;
        public const int DescriptionBackBoardWidth = 400;
        public const int DescriptionBackBoardHeight = 360;
        public const int MainMenuButtonWidth = 350;
        public const int MainMenuButtonHeight = 80;
        public static Color TransparentColor = SplashKit.RGBAColor(0,0,0,1);
        public static Color RedGlow = SplashKit.RGBAColor(255, 0, 0, 0.32);
        public static Color GreenGlow = SplashKit.RGBAColor(0, 235, 0, 0.28);
        public static Color GreyGlow = SplashKit.RGBAColor(164, 169, 164, 0.32);
        // Resource File Paths
        public const string TileImgPath = "Resources\\BoardTileD_40x40.png";
        public const string BackgroundImgPath = "Resources\\BackgroundD_1200x760.jpg";
        public const string PlayerOneImgPath = "Resources\\PlayerOne_36x36.png";
        public const string PlayerTwoImgPath = "Resources\\PlayerTwo_36x36.png";
        public const string FencePOneImgPath = "Resources\\BlockTilePlayerOne_36x36.png";
        public const string FencePTwoImgPath = "Resources\\BlockTilePlayerTwo_36x36.png";
        public const string BoostedStaminaImgPath = "Resources\\StaminaBoost_36x36.png";
        public const string BuffBoostedStaminaImgPath = "Resources\\BuffBoostedStamina_36x36.png";
        public const string DebuffBoostedStaminaImgPath = "Resources\\DebuffBoostedStamina_36x36.png";
        public const string GhostFormImgPath = "Resources\\GhostMode_36x36.png";
        public const string DebuffGhostFormImgPath = "Resources\\DebuffGhostMode_36x36.png";
        public const string BuffGhostFormImgPath = "Resources\\BuffGhostMode_36x36.png";
        public const string TimeStopImgPath = "Resources\\TimeStop_36x36.png";
        public const string BuffTimeStopImgPath = "Resources\\BuffTimeStop_36x36.png";
        public const string DebuffTimeStopImgPath = "Resources\\DebuffTimeStop_36x36.png";
        public const string ExitGameImgPath = "Resources\\cross_50x50.png";
        public const string RestartGameImgPath = "Resources\\reset_50x50.png";
        public const string FontPath = "Resources\\Adventure.otf";
        // Methods to Calculate X and Y Pixel Locations
        public static (int UIx, int UIy) ConvertArrayCoordinateToUICoordinateForItems((Row row, Column column) arrayCoordinate) {
            // For Bitmap Items with dimensions of 36 x 36. 2 pixel padding on all sides.
            int x = (int) arrayCoordinate.column * Constants.TileWidth + Constants.WindowPadding + Constants.BoardToBackBoardPadding + Constants.PlayerTilePadding;
            int y = (int) arrayCoordinate.row * Constants.TileHeight + Constants.WindowPadding + Constants.BoardToBackBoardPadding + Constants.PlayerTilePadding;
            return (x, y);
        }

        public static (int UIx, int UIy) ConvertArrayCoordinateToUICoordinateForTiles((Row row, Column column) arrayCoordinate) {
            // For Bitmap Tiles. Does not take into account inner padding used for items (i.e. Buffs, Players, fences, etc)
            int x = (int) arrayCoordinate.column * Constants.TileWidth + Constants.WindowPadding + Constants.BoardToBackBoardPadding;
            int y = (int) arrayCoordinate.row * Constants.TileHeight + Constants.WindowPadding + Constants.BoardToBackBoardPadding;
            return(x, y);
        }
    }
}