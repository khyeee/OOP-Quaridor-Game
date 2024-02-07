using System;
using System.Collections.Generic;
using System.Text;
using SplashKitSDK;

namespace Distinction_Task

/*
    TO DO: HD TASK
    1. Find ways to optimize game design -> Currently running very slowly
        - Look into Factoring -> See lecture notes on best design practices
    2. HD. Document what i do and how i test the program implementation speed 
    3. Which part of my code needs refactoring
    4. how will refactoring help my code
    5. Include research and references and
    

    TO DO: Distinction
    1. Try to Free the bitmaps - maybe this is the main problem
    2. Alternative test - try making a blank screen and getting that to transition into main game. see if lag still occurs
    3. Another alternative fix - have everything on a single window. No transitioning between screens

    TO DO: Potentially the problem
    1. FREE the bitmap to make program go faster. -> Freeing the background bitmap from the main menu makes main game run slightly faster
        To figure out:
            1. WHEN to free bitmap without messing up graphics
            2. Can i just move the bitmap instead of redrawing it?
*/
{
    public class Program {
        public static void Main(string[] args)
        {   
            /*
                Tetative Sizes (Length x Height):
                Tile = 40 x 40
                    9 tiles per row/column total length of tiles = 40 * 9 = 360
                Gap = 40 x 40 -> fence in between is 36 x 36 (2 pixel padding)
                    8 gaps per row/column total length of gap = 40 * 8 = 320
                Overall Board = 680 x 680 -> Not included padding.
                With padding (Assuming 5 pixels on all sides), Board = 690 x 690 -> Make it 700 height? Max possible is 780
            */

            Window gameWindow = new Window("Quaridor", Constants.WindowWidth, Constants.WindowHeight);
            showMainMenu:
            Quaridor myGame = new Quaridor();
            GameMode mode = GameMode.Null;
            ProgramFlow programFlow = ProgramFlow.Unassigned;

            bool isProgramRunning = true;
            bool isShowingMainMenu = true;

            // Flow:
            // 1. Show Main Menu -> Single Player, Mutltiplayer, Exit
            // 2. Show Rules
            // 3. Show Game
            // 4. Show Winning Screen -> Restart / Close

            // Game ongoing block
            while(isShowingMainMenu && !SplashKit.WindowCloseRequested(gameWindow)) {
                SplashKit.ClearScreen();
                SplashKit.ProcessEvents();
                
                //myGame.StartGame(GameMode.Multiplayer, gameWindow);
                switch(programFlow) {
                    case ProgramFlow.ShowMainMenu: case ProgramFlow.Unassigned:
                        (programFlow, mode) = myGame.ShowGameMainMenu();
                        break;
                    case ProgramFlow.CloseGame:
                        isProgramRunning = false;
                        isShowingMainMenu = false;
                        SplashKit.CloseWindow(gameWindow);
                        goto ExitProgram;
                    default:
                        SplashKit.FreeAllBitmaps();
                        isShowingMainMenu = false;
                        break;
                }
            
                SplashKit.RefreshScreen();
            }

           if(!isShowingMainMenu) myGame.InitNewGame(mode);

           do {
                SplashKit.ClearScreen();
                SplashKit.ProcessEvents();

                switch(programFlow) {
                    case ProgramFlow.StartGame:
                        programFlow = myGame.StartGame(mode, gameWindow);
                        break;
                    case ProgramFlow.Reset:
                        myGame.InitNewGame(mode);
                        programFlow = ProgramFlow.StartGame;
                        break;
                    case ProgramFlow.Exit:
                        isProgramRunning = false;
                        isShowingMainMenu = true;
                        programFlow = ProgramFlow.ShowMainMenu;
                        goto showMainMenu;
                    case ProgramFlow.GameEndsPlayerWon:
                        programFlow = myGame.ShowGameOverMenu(myGame.GameWinner, gameWindow);
                        break;
                    default:
                        break;
                }

                SplashKit.RefreshScreen();
            } while (isProgramRunning && !SplashKit.WindowCloseRequested(gameWindow));
            
            ExitProgram:
            Console.WriteLine("Exited Program....");
        }
    }
}