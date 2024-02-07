using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class Quaridor {
        private Player _playerOne, _playerTwo;
        private PlayerType _gameWinner;
        private Board _gameBoard;
        private bool _isGameInProgress, _isPlayerOneTurn;
        private int _itemSpawnCountdown;
        GameMode _currentGameMode;
        private List<Button> _inGameButtons;
        private List<Bitmap> _mainMenuBitmaps;
        
        public Quaridor() {
            _playerOne = new Player(PlayerType.PlayerOne);
            _playerTwo = new Player(PlayerType.PlayerTwo);
            _gameBoard = new Board(_playerOne, _playerTwo);
            _isGameInProgress = false;
            _itemSpawnCountdown = 2;
            _inGameButtons = new List<Button>();
            _mainMenuBitmaps = new List<Bitmap>();
        }

        public void InitNewGame(GameMode gameMode) {
            _playerOne = new Player(PlayerType.PlayerOne);
            _playerTwo = new Player(PlayerType.PlayerTwo);
            _gameBoard = new Board(_playerOne, _playerTwo);

            if(gameMode == GameMode.SinglePlayer) {
                _playerTwo.PlayerType = PlayerType.BasicAI;
            }

            _gameBoard.SetUpNewBoard(gameMode);
            
            int exitButtonX = Constants.WindowWidth - Constants.WindowPadding - Constants.ExitResetButtonWidth - 5;
            int exitButtonY = Constants.WindowHeight - Constants.WindowPadding - 5 - Constants.ExitResetButtonHeight;
            int resetButtonX = Constants.WindowWidth - Constants.WindowPadding - 5 - Constants.ExitResetButtonWidth * 2 - Constants.WindowPadding;
            int resetButtonY = Constants.WindowHeight - 10 - Constants.ExitResetButtonHeight - 5;

            Button exitButton = new Button(ButtonType.ExitButton, exitButtonX, exitButtonY);
            Button restartButton = new Button(ButtonType.ResetButton, resetButtonX, resetButtonY);

            _inGameButtons.Add(exitButton);
            _inGameButtons.Add(restartButton);

            _currentGameMode = gameMode;
        }

        public ProgramFlow StartGame(GameMode gameMode, Window gameWindow) {
            Bitmap background = new Bitmap("background", Constants.BackgroundImgPath);

            // Draw Background and backboard UI Elements
            // Drawing Background
            background.Draw(0,0);

            // Backboard for board component
            gameWindow.FillRectangle(SplashKit.RGBAColor(1,1,1,0.3), 10, 10, 740, 740);

            // Backboard for text components
            gameWindow.FillRectangle(SplashKit.RGBAColor(0,0,0,0.3), 770, 10, 420, 740);

            // Backboard for showing hover tile info
            gameWindow.FillRectangle(SplashKit.RGBAColor(252,252,252,.3), Constants.DescriptionBackBoardX, Constants.DescriptionBackBoardY, Constants.DescriptionBackBoardWidth, Constants.DescriptionBackBoardHeight);
            SplashKit.DrawLine(Color.Black, Constants.DescriptionBackBoardX, Constants.DescriptionBackBoardY, Constants.DescriptionBackBoardX, Constants.DescriptionBackBoardY + Constants.DescriptionBackBoardHeight);
            SplashKit.DrawLine(Color.Black, Constants.DescriptionBackBoardX, Constants.DescriptionBackBoardY, Constants.DescriptionBackBoardX + Constants.DescriptionBackBoardWidth, Constants.DescriptionBackBoardY);
            SplashKit.DrawLine(Color.Black, Constants.DescriptionBackBoardX, Constants.DescriptionBackBoardY + Constants.DescriptionBackBoardHeight, Constants.DescriptionBackBoardX + Constants.DescriptionBackBoardWidth, Constants.DescriptionBackBoardY + Constants.DescriptionBackBoardHeight);
            SplashKit.DrawLine(Color.Black, Constants.DescriptionBackBoardX + Constants.DescriptionBackBoardWidth, Constants.DescriptionBackBoardY, Constants.DescriptionBackBoardX + Constants.DescriptionBackBoardWidth, Constants.DescriptionBackBoardY + Constants.DescriptionBackBoardHeight);

            if(!_isGameInProgress) {
                _isGameInProgress = true;
                _itemSpawnCountdown = 2;
                _isPlayerOneTurn = true;
            }

            _gameBoard.DrawBoardElements();
                
            switch(_isPlayerOneTurn) {
                case true:
                    ProgramFlow playerOneFlow = RunGameSequenceFor(gameWindow, _playerOne, _playerTwo);

                    switch (playerOneFlow) {
                        case ProgramFlow.ActionComplete:
                            UpdateBuffDebuffItemsOnBoard();
                            _isPlayerOneTurn = !_isPlayerOneTurn;
                            break;
                        case ProgramFlow.Exit:
                            // Returns to Main Menu
                            _isGameInProgress = false;
                            return ProgramFlow.Exit;
                        case ProgramFlow.GameEndsPlayerWon:
                            // TO DO: SHow winner UI 
                            _isGameInProgress = false;
                            return ProgramFlow.GameEndsPlayerWon;
                        case ProgramFlow.Reset:
                            return playerOneFlow;
                    }
                    break;
                case false:
                    ProgramFlow playerTwoFlow = RunGameSequenceFor(gameWindow, _playerTwo, _playerOne);

                    switch (playerTwoFlow) {
                        case ProgramFlow.ActionComplete:
                            UpdateBuffDebuffItemsOnBoard();
                            _isPlayerOneTurn = !_isPlayerOneTurn;
                            break;
                        case ProgramFlow.Exit:
                            _isGameInProgress = false;
                            return ProgramFlow.Exit;
                        case ProgramFlow.GameEndsPlayerWon:
                            // TO DO: SHow winner UI
                            _isGameInProgress = false;
                            return ProgramFlow.GameEndsPlayerWon;
                        case ProgramFlow.Reset:
                            return playerTwoFlow;
                    }
                    break;
            }

            // Refresh board to see final updates before game ends
            _gameBoard.UpdateBoard();
            
            return ProgramFlow.StartGame;                
        }

        public void UpdateBuffDebuffItemsOnBoard() {
            // Check if there are any items already on the board. If yes, reduce their timers.
            if(_gameBoard.GameBuffDebuffs.Count() > 0) {
                foreach(BuffDebuff item in _gameBoard.GameBuffDebuffs) {
                    item.CountDownDuration();
                }

                List<BuffDebuff> itemsToRemove = _gameBoard.GameBuffDebuffs.Where(item => item.BuffDuration == 0).ToList();
                if(itemsToRemove.Count() > 0) {
                    foreach(BuffDebuff item in itemsToRemove) {
                        _gameBoard.RemoveBuffDebuffFromBoardSetTileToClear(item);
                    }
                }
            }
            
            // Decrease countdown by one each turn, if it reaches 0, reset to 2.Prevents spawned items from flooding the board
            _itemSpawnCountdown -= (_itemSpawnCountdown > 0) ? 1 : -2;
            Random randomNumGenerator = new Random();
            int spawnCount = randomNumGenerator.Next(1, 4); // Random number from 1 to 3
            if(_itemSpawnCountdown == 0) {
                for(int i = 0 ; i < spawnCount ; i++) {
                    SpawnGameItems(spawnCount);
                }
            }
        }
        public (string userInput, (Row row, Column column) potentialFenceCoordinate, FenceOrientation fenceOrientation) GetUserDynamicInput(Window gameWindow, Player player, Player opponent) {
            Point2D mousePosition = SplashKit.MousePosition();
            int mouseColumnPosition = Convert.ToInt32(Math.Floor((mousePosition.X - Constants.WindowPadding - Constants.BoardToBackBoardPadding) / 40));
            int mouseRowPosition = Convert.ToInt32(Math.Floor((mousePosition.Y  - Constants.WindowPadding - Constants.BoardToBackBoardPadding)/ 40));
            (Row row, Column column) mouseHoverCoordinate = ((Row) mouseRowPosition, (Column) mouseColumnPosition);
            (Row row, Column column) potentialFenceAnchor = (Row.R_Null, Column.C_Null);

            if(mouseColumnPosition < 17 && mouseRowPosition < 17 && mouseColumnPosition >= 0 && mouseRowPosition >= 0) {
                ShowMouseHoverDescription(_gameBoard.GetTileAt(mouseHoverCoordinate));
                // Mouse is within game board
                if(mouseColumnPosition % 2 == 0 && mouseRowPosition % 2 == 0) {
                    // Even Column and Rows - Mouse is on one of the tiles
                    // Glow green if possible to move for player
                    (Row row, Column column) potentialUp, potentialDown, potentialLeft, potentialRight;
                    (Row row, Column column) potentialJump = (Row.R_Null, Column.C_Null);
                    int rowDifference = (int) player.PlayerCoordinate.row - (int) opponent.PlayerCoordinate.row;
                    int colDifference = (int) player.PlayerCoordinate.column - (int) opponent.PlayerCoordinate.column;

                    // See if player can jump
                    if(Math.Abs(rowDifference) == 2 ^ Math.Abs(colDifference) == 2) {
                        if(Math.Abs(colDifference) == 2) {
                            if((int) player.PlayerCoordinate.column + colDifference < 17 && (int) player.PlayerCoordinate.column + colDifference > 0) {
                                // Ensure it is in the board
                                if(colDifference < 0) {
                                    // Jump in right direction - Check if path is blocked by fences before setting the potential jump coordinates
                                    potentialJump = (
                                        _gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Right) || 
                                        _gameBoard.CheckIsPlayerBlockedByFence((player.PlayerCoordinate.row, player.PlayerCoordinate.column + 2), PlayerMoves.Right)
                                        ) ? (Row.R_Null, Column.C_Null) : (player.PlayerCoordinate.row, player.PlayerCoordinate.column - colDifference + 2);
                                } else {
                                    // Jump in Left Direction - Check if path is blocked by fences before setting the potential jump coordinates
                                    potentialJump = (
                                        _gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Left) || 
                                        _gameBoard.CheckIsPlayerBlockedByFence((player.PlayerCoordinate.row, player.PlayerCoordinate.column - 2), PlayerMoves.Left)
                                        ) ? (Row.R_Null, Column.C_Null) : (player.PlayerCoordinate.row, player.PlayerCoordinate.column - colDifference - 2);
                                }
                            }
                        } else {
                            // rowDifference == 2
                            if((int) player.PlayerCoordinate.row + rowDifference < 17 && (int) player.PlayerCoordinate.row + rowDifference > 0) {
                                // Ensure it is in the board
                                if(rowDifference < 0) {
                                    // Jump in Down direction - Check if path is blocked by fences before setting the potential jump coordinates
                                    potentialJump = (
                                        _gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Down) ||
                                        _gameBoard.CheckIsPlayerBlockedByFence((player.PlayerCoordinate.row + 2, player.PlayerCoordinate.column), PlayerMoves.Down)
                                        ) ? (Row.R_Null, Column.C_Null) : (player.PlayerCoordinate.row - rowDifference + 2, player.PlayerCoordinate.column);
                                } else {
                                    // Jump in Up Direction - Check if path is blocked by fences before setting the potential jump coordinates
                                    potentialJump = (
                                        _gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Up) ||
                                        _gameBoard.CheckIsPlayerBlockedByFence((player.PlayerCoordinate.row - 2, player.PlayerCoordinate.column), PlayerMoves.Up)
                                        ) ? (Row.R_Null, Column.C_Null) : (player.PlayerCoordinate.row - rowDifference - 2, player.PlayerCoordinate.column);
                                }
                            }
                        }
                    }

                    potentialDown = (opponent.PlayerCoordinate == (player.PlayerCoordinate.row + 2, player.PlayerCoordinate.column)) ? potentialJump : (player.PlayerCoordinate.row + 2, player.PlayerCoordinate.column);
                    potentialUp = (opponent.PlayerCoordinate == (player.PlayerCoordinate.row - 2, player.PlayerCoordinate.column)) ? potentialJump : (player.PlayerCoordinate.row - 2, player.PlayerCoordinate.column);
                    potentialLeft = (opponent.PlayerCoordinate == (player.PlayerCoordinate.row, player.PlayerCoordinate.column - 2)) ? potentialJump : (player.PlayerCoordinate.row, player.PlayerCoordinate.column - 2);
                    potentialRight = (opponent.PlayerCoordinate == (player.PlayerCoordinate.row, player.PlayerCoordinate.column + 2)) ? potentialJump : (player.PlayerCoordinate.row, player.PlayerCoordinate.column + 2);

                    if(mouseHoverCoordinate == potentialDown || mouseHoverCoordinate == potentialUp || mouseHoverCoordinate == potentialLeft || mouseHoverCoordinate == potentialRight) {
                        HoverStatus tempHoverStatus = HoverStatus.ValidPlayerMove;
            
                        // Move player in direction if mouse is clicked
                        if(mouseHoverCoordinate == potentialDown) {
                            if(player.BuffDebuffOnPlayer != GameItems.GhostForm) {
                                tempHoverStatus = (_gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Down)) ? HoverStatus.Invalid : tempHoverStatus;
                            }

                            if(SplashKit.MouseClicked(MouseButton.LeftButton)) return ("DOWN", potentialFenceAnchor, FenceOrientation.OrientError);
                        } else if(mouseHoverCoordinate == potentialUp) {
                            if(player.BuffDebuffOnPlayer != GameItems.GhostForm) {
                                tempHoverStatus = (_gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Up)) ? HoverStatus.Invalid : tempHoverStatus;
                            }
                            
                            if(SplashKit.MouseClicked(MouseButton.LeftButton)) return ("UP", potentialFenceAnchor, FenceOrientation.OrientError);
                        } else if (mouseHoverCoordinate == potentialLeft) {
                            if(player.BuffDebuffOnPlayer != GameItems.GhostForm) {
                                tempHoverStatus = (_gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Left)) ? HoverStatus.Invalid : tempHoverStatus;
                            }
                            
                            if(SplashKit.MouseClicked(MouseButton.LeftButton)) return ("LEFT", potentialFenceAnchor, FenceOrientation.OrientError);
                        } else {
                            if(player.BuffDebuffOnPlayer != GameItems.GhostForm) {
                                tempHoverStatus = (_gameBoard.CheckIsPlayerBlockedByFence(player.PlayerCoordinate, PlayerMoves.Right)) ? HoverStatus.Invalid : tempHoverStatus;
                            }
                            
                            if(SplashKit.MouseClicked(MouseButton.LeftButton)) return ("RIGHT", potentialFenceAnchor, FenceOrientation.OrientError);
                        }

                        _gameBoard.GetTileAt(mouseHoverCoordinate).DrawMouseHover(gameWindow, tempHoverStatus);
                    } else {
                        _gameBoard.GetTileAt(mouseHoverCoordinate).DrawMouseHover(gameWindow, HoverStatus.Invalid);
                    }

                } else {
                    // Mouse is on fence tiles 
                    // Glow red if blocked, glow grey for potential fence placement orientation
                    List<Fence> playersFences = _gameBoard.GameFences.Where(fence => fence.Ownership == player.PlayerType).ToList();
                    List<Fence> availableFences = playersFences.Where(fence => fence.FenceStatus == FenceStatus.InStorage).ToList();

                    FenceOrientation defaultFenceOrientation = FenceOrientation.OrientError;

                    if(mouseRowPosition % 2 == 0) {
                        // On Even rows and Odd Columns, fences are always vertical
                        defaultFenceOrientation = FenceOrientation.Vertical;

                        if(mouseRowPosition < 15 ){
                            // Hovering between R0 to R15
                            HoverStatus hoverStatus = (_gameBoard.IsFenceFitAt(mouseHoverCoordinate, defaultFenceOrientation)) ? HoverStatus.ValidFencePlacement : HoverStatus.Invalid; 
                            hoverStatus = (availableFences.Count == 0) ? HoverStatus.Invalid : hoverStatus; // If there are no fences available, hover is invalid. Glow red

                            for(int i = 0; i < 3; i++) {
                                _gameBoard.GetTileAt((mouseHoverCoordinate.row + i, mouseHoverCoordinate.column)).DrawMouseHover(gameWindow, hoverStatus);
                            }

                            if(SplashKit.MouseClicked(MouseButton.LeftButton) && hoverStatus == HoverStatus.ValidFencePlacement) {
                                // Place fence
                                potentialFenceAnchor = mouseHoverCoordinate;
                                return ("FENCE", potentialFenceAnchor, defaultFenceOrientation); 
                            }
                        } else {
                            // Mouse is on the final row (R16) - draw glow red for invalid
                            _gameBoard.GetTileAt(mouseHoverCoordinate).DrawMouseHover(gameWindow, HoverStatus.Invalid);
                        }
                        
                    } else {
                        // On Odd Rows and Even Columns, fences are always horizontal. May switch orientation if at intersection between 4 tiles
                        defaultFenceOrientation = FenceOrientation.Horizontal;

                        if(mouseColumnPosition < 16) {
                            defaultFenceOrientation = (defaultFenceOrientation == FenceOrientation.OrientError) ? FenceOrientation.Horizontal : defaultFenceOrientation;

                            if(SplashKit.KeyDown(KeyCode.RKey) && mouseColumnPosition % 2 != 0 && mouseRowPosition < 15) {
                                // Dont know how to make changes stick if just typed
                                // Holding R will change orietation if mouse is hovering on 4 tile intersection
                                defaultFenceOrientation = (defaultFenceOrientation == FenceOrientation.Horizontal) ? FenceOrientation.Vertical : FenceOrientation.Horizontal;
                            }

                            HoverStatus hoverStatus = (_gameBoard.IsFenceFitAt(mouseHoverCoordinate, defaultFenceOrientation)) ? HoverStatus.ValidFencePlacement : HoverStatus.Invalid; 
                            hoverStatus = (availableFences.Count == 0) ? HoverStatus.Invalid : hoverStatus;
                            
                            switch(defaultFenceOrientation) {
                                case FenceOrientation.Horizontal:
                                    if(mouseColumnPosition > 14) {
                                        for(int i = 0; i <= 16 - mouseColumnPosition; i++) {
                                            _gameBoard.GetTileAt((mouseHoverCoordinate.row, mouseHoverCoordinate.column + i)).DrawMouseHover(gameWindow, hoverStatus);
                                        }
                                    } else {
                                        for(int i = 0; i < 3; i++) {
                                            _gameBoard.GetTileAt((mouseHoverCoordinate.row, mouseHoverCoordinate.column + i)).DrawMouseHover(gameWindow, hoverStatus);
                                        }
                                    }
                                    break;
                                case FenceOrientation.Vertical:
                                    if(mouseRowPosition > 14) {
                                        for(int i = 0; i <= 16 - mouseRowPosition; i++) {
                                            _gameBoard.GetTileAt((mouseHoverCoordinate.row + i, mouseHoverCoordinate.column)).DrawMouseHover(gameWindow, hoverStatus);
                                        }
                                    } else {
                                        for(int i = 0; i < 3; i++) {
                                            _gameBoard.GetTileAt((mouseHoverCoordinate.row + i, mouseHoverCoordinate.column)).DrawMouseHover(gameWindow, hoverStatus);
                                        }
                                    }
                                    break;
                            }

                            if(SplashKit.MouseClicked(MouseButton.LeftButton) && hoverStatus == HoverStatus.ValidFencePlacement) {
                                // Place fence
                                potentialFenceAnchor = mouseHoverCoordinate;
                                return ("FENCE", potentialFenceAnchor, defaultFenceOrientation);
                            }
                        } else {
                            // Mouse is on the final column - draw glow red for invalid
                            _gameBoard.GetTileAt(mouseHoverCoordinate).DrawMouseHover(gameWindow, HoverStatus.Invalid);
                        }
                    } 
                }
            } else {
                // Mouse is hovering outside of the game board
                // Check if mouse is on any of the buttons
                foreach(Button button in _inGameButtons) {
                    if(mousePosition.X >= button.X && mousePosition.Y >= button.Y && mousePosition.X < button.X + button.Width && mousePosition.Y < button.Y + button.Height) {
                        // Is on one of the buttons
                        
                        switch(button.ButtonType) {
                            case ButtonType.ExitButton:
                                if(SplashKit.MouseClicked(MouseButton.LeftButton)) {
                                    // Clicked on Exit button. Return to main menu
                                    Console.WriteLine("Clicked on exit");
                                    return ("EXIT", potentialFenceAnchor, FenceOrientation.OrientError);
                                }
                                break;
                            case ButtonType.ResetButton:
                                if(SplashKit.MouseClicked(MouseButton.LeftButton)) {
                                    // Clicked on reset button. Retart game.
                                    Console.WriteLine("Clicked on reset");
                                    return ("RESET", potentialFenceAnchor, FenceOrientation.OrientError);
                                }
                                break;
                        }
                    }
                }
            }

            return ("INVALID", potentialFenceAnchor, FenceOrientation.OrientError);
        }

        public void SpawnGameItems(int spawnNumber) {
            (Row row, Column column) randomItemCoordinates = GenerateRandomPlayerAnchoredCoordinates();
            Random randomNumGenerator = new Random();

            for(int i = 0; i < spawnNumber ; i++) {
                switch(randomNumGenerator.Next(3)) {
                    case 0:
                        // Make BoostedStamina Item   
                        BoostedStamina newBoostedStamina = new BoostedStamina(randomItemCoordinates);
                        _gameBoard.AddBuffDebuffToBoard(newBoostedStamina);
                        break;
                    case 1:
                        // Make Time Top Item
                        TimeStop newTimeStop = new TimeStop(randomItemCoordinates);
                        _gameBoard.AddBuffDebuffToBoard(newTimeStop);
                        break;
                    case 2:
                        // Make Ghost Item
                        GhostForm newGhostForm = new GhostForm(randomItemCoordinates);
                        _gameBoard.AddBuffDebuffToBoard(newGhostForm);
                        break;
                }
            }
        }

        public (Row row, Column column) GenerateRandomPlayerAnchoredCoordinates() {
            Random randomNumGenerator = new Random();
            int randomOneTwo = randomNumGenerator.Next(2); 
            // Random row/column will be 1 or 2 tiles away from player
            int randomColumn = randomNumGenerator.Next(3) * 2;
            int randomRow = randomNumGenerator.Next(3) * 2;

            Player chosenPlayer = (randomOneTwo == 0) ? _playerOne : _playerTwo;

            if(randomColumn == 0 && randomRow == 0) {
                randomRow += (randomNumGenerator.Next(1) == 0) ? 2 : 4;
                randomColumn += (randomNumGenerator.Next(1) == 0) ? 2 : 4;
            }

            randomRow *= (randomNumGenerator.Next(1) == 0) ? -1 : 1;
            randomColumn *= (randomNumGenerator.Next(1) == 0) ? -1 : 1;

            Row potentialRow = chosenPlayer.PlayerCoordinate.row + randomRow;
            Column potentialColumn = chosenPlayer.PlayerCoordinate.column + randomColumn;

            // Ensure the randomized coordinates are not out of bounds
            potentialRow -= ((int) potentialRow > 16) ? 8 : 0;
            potentialRow += ((int) potentialRow < 0) ? 8 : 0;
            potentialColumn -= ((int) potentialColumn > 16) ? 8 : 0;
            potentialColumn += ((int) potentialColumn < 0) ? 8 : 0;

            return (potentialRow, potentialColumn);
        }

        public ProgramFlow RunGameSequenceFor(Window gameWindow, Player player, Player opponent) {
            string userInput;
            (Row row, Column column) potentialFenceCoordinate;
            FenceOrientation potentialFenceOrientation;

            ShowPlayerOptions(player, opponent);

            if(player.NumberOfMoves == 0) {
                // Player is affected by Abnormal status if number of moves was not resetted
                switch(player.BuffDebuffOnPlayer) {
                    case GameItems.TimeStop:
                        Console.WriteLine("Skipping {0} turn because time stopped.", player.PlayerType);
                        // player.ResetPlayerMoves(); 
                        player.SetAbnormalStatus(PowerUpType.Normal);
                        player.SetBuffDebuff(GameItems.Nothing);
                        return ProgramFlow.ActionComplete; 
                    case GameItems.GhostForm:
                        player.ResetPlayerMoves();
                        break;
                    case GameItems.BoostedStamina:
                        player.ResetPlayerMoves(); 
                        player.SetAbnormalStatus(PowerUpType.Normal);
                        player.SetBuffDebuff(GameItems.Nothing);
                        break;
                    case GameItems.Nothing:
                        player.ResetPlayerMoves();
                        break;
                }
            } 

            (userInput, potentialFenceCoordinate, potentialFenceOrientation) = (player.PlayerType == PlayerType.BasicAI) ? player.DecideNextMove() : GetUserDynamicInput(gameWindow, player, opponent);

            int inputIndex = Array.FindIndex(Constants.AcceptableInputs, acceptableInputs => acceptableInputs.Contains(userInput.ToUpper()));
            if(inputIndex <= 7) {
                // User chose to move
                if(TryMovePlayer(ref player, inputIndex)) {
                    // Successfully moved player
                    if(_gameBoard.CheckHasPlayerWon(player)) {
                        _gameWinner = player.PlayerType;
                        return ProgramFlow.GameEndsPlayerWon;
                    }

                    // Check if stepped on buff debuff here. Apply to player here
                    bool hasPlayerPickedItem = _gameBoard.CheckHasPlayerPickedUpItem(player);
                    if(hasPlayerPickedItem) {
                        player.GetPlayerAbnormalStatus().ApplyOnPlayer(player, opponent);
                    }
                    
                    // TODO: Look at Player.UseMove() again. does not really make sense with how it controls program flow sequence!!
                    if(player.UseMove()) {
                        // UseMove returns true if numberOfMoves > 1 after use. Repeat turn since there's more moves
                        return ProgramFlow.Repeat;
                    } else {
                        // Player has no more moves left
                        if(!hasPlayerPickedItem && player.BuffDebuffOnPlayer == GameItems.Nothing) {
                            // Player has not picked up item and was not previously affected by abnormal status
                            // Reset player for next turn
                            Console.WriteLine("Didnt pick up anything. Reset moves.");
                            player.ResetPlayerMoves();

                        } else if (!hasPlayerPickedItem && player.BuffDebuffOnPlayer != GameItems.Nothing) {
                            // Completed turn with abnormal status. Reset to initial state
                            Console.WriteLine("Didnt pick up anything, but i have abnormal statuse");
                            player.SetAbnormalStatus(PowerUpType.Normal);
                            player.SetBuffDebuff(GameItems.Nothing);
                            player.ResetPlayerGhostState();
                        }
                        return ProgramFlow.ActionComplete;
                    }
                }
            } else if (inputIndex <= 9) {
                // Note: AI Does not have any implementation for fence orientation. Still utilizes old logic where fences cannot be placed at intersections
                // User chose to place fence
                (Row anchorRow, Column anchorColumn) placementCoordinate = (player.PlayerType == PlayerType.BasicAI) ? player.GenerateRandomFenceCoordinate(opponent) : potentialFenceCoordinate;
                potentialFenceOrientation = (player.PlayerType == PlayerType.BasicAI) ? player.GenerateRandomFenceOrientation(placementCoordinate) : potentialFenceOrientation;
                // Check if inputs are valid
                if(placementCoordinate.anchorRow == Row.R_Null || placementCoordinate.anchorColumn == Column.C_Null) {
                    Console.WriteLine("You've entered an invalid row or column.");
                    Console.WriteLine("Try again.");
                    Console.WriteLine("");
                    return ProgramFlow.Repeat;
                }

                // Check if Fence Anchor Coordinates are already occupied
                if(_gameBoard.GetTileStatusAt(placementCoordinate) == TileStatus.Blocked) {
                    Console.WriteLine("The tile is already occupied. Try again.");
                    return ProgramFlow.Repeat;
                }

                // Check if there is space on the board for the fence
                if(!_gameBoard.IsFenceFitAt(placementCoordinate, potentialFenceOrientation)) {
                    Console.WriteLine("Not enough room to place fence. Try again.");
                    return ProgramFlow.Repeat;
                }

                // Initialize holder variables
                List<Fence> tempFences = _gameBoard.GameFences;
                int indexOfFence;
            
                // Update Game Fences on the board
                (_gameBoard.GameFences, indexOfFence) = player.UseFence(ref tempFences, placementCoordinate, potentialFenceOrientation); 
                
                _gameBoard.UpdateBoardWithNewFence(indexOfFence);
                _gameBoard.ResetCheckedStatusAllTiles();
                _gameBoard.CheckIsAllPathsBlockedFrom(opponent.PlayerCoordinate);
                
                if(!_gameBoard.CheckIsPossibleToWinFor(opponent)) {
                    // Placement of fence prevents opponent from winning. Boxes opponent in.
                    // Reset Fence Placement
                    Console.WriteLine("Fence placement makes it impossible for your opponent to win. Try again.");
                    _gameBoard.SystemRemovesFenceAt(placementCoordinate);
                    return ProgramFlow.Repeat;
                }

                if(player.UseMove()) {
                    return ProgramFlow.Repeat;
                } else {
                    // Player has no more moves to use
                    switch(player.BuffDebuffOnPlayer) {
                        case GameItems.Nothing:
                            player.ResetPlayerMoves();
                            break;
                        default:
                            player.SetAbnormalStatus(PowerUpType.Normal);
                            player.SetBuffDebuff(GameItems.Nothing);
                            player.ResetPlayerGhostState();
                            break;
                    }

                    return ProgramFlow.ActionComplete;
                }
            } else if (inputIndex == 16) {
                // User chose to Reset Game board
                return ProgramFlow.Reset;
            } else if (inputIndex > 16) {
                // User chose to exit program
                return ProgramFlow.Exit;
            }

            return ProgramFlow.Repeat;
        }

        public bool TryMovePlayer(ref Player player, int indexOfAcceptableInput) {
            bool isMovePossible = false;
            bool isBlocked = false;
            bool isOpponentInPath = false;
            bool isEnoughSpaceForJump = false;

            (Row row, Column column) playerCurrentPosition = player.PlayerCoordinate;

            switch (indexOfAcceptableInput) {
                case 0: case 1: // Move Up
                    // Check if player is at the top row of the board R0
                    isMovePossible = ((int) playerCurrentPosition.row > 0);

                    if(isMovePossible) {
                        // Check if player is in ghost state. If not, check if player is blocked by fence
                        isBlocked = (player.IsPlayerGhostState()) ? false : _gameBoard.CheckIsPlayerBlockedByFence(playerCurrentPosition, PlayerMoves.Up);
                        // Check if opponent is on the path of movement. If yes, jump over opponent. Check if there is space on the other side also
                        isOpponentInPath = _gameBoard.GetTileStatusAt((playerCurrentPosition.row - 2, playerCurrentPosition.column)) == TileStatus.Player;
                    }

                    isEnoughSpaceForJump = ((int) playerCurrentPosition.row > 2); // Enough space at end of board for jumping
                    
                    if(isMovePossible && !isBlocked && isOpponentInPath && isEnoughSpaceForJump) {
                        player.MovePlayer(PlayerMoves.Up);
                        player.MovePlayer(PlayerMoves.Up);
                        break;
                    }

                    if(isMovePossible && !isBlocked && !isOpponentInPath) {
                        player.MovePlayer(PlayerMoves.Up);
                    } 
                    break;
                case 2: case 3: // Move Down
                    // Check if player is at the bottom row of the board R16
                    isMovePossible = ((int) playerCurrentPosition.row < 16);
                    if(isMovePossible) {
                        // Check if player is in ghost state. If not, check if player is blocked by fence
                        isBlocked = (player.IsPlayerGhostState()) ? false : _gameBoard.CheckIsPlayerBlockedByFence(playerCurrentPosition, PlayerMoves.Down);
                        isOpponentInPath = _gameBoard.GetTileStatusAt((playerCurrentPosition.row + 2, playerCurrentPosition.column)) == TileStatus.Player;
                    }

                    isEnoughSpaceForJump = ((int) playerCurrentPosition.row < 14); // Enough space at end of board for jumping

                    if(isMovePossible && !isBlocked && isOpponentInPath && isEnoughSpaceForJump) {
                        player.MovePlayer(PlayerMoves.Down);
                        player.MovePlayer(PlayerMoves.Down);
                        break;
                    }

                    if(isMovePossible && !isBlocked && !isOpponentInPath) {
                        player.MovePlayer(PlayerMoves.Down);
                    } 
                    break;
                case 4: case 5: // Move Left
                    // Check if player is at the left most column of the board C0
                    isMovePossible = ((int) playerCurrentPosition.column > 0);
                    if(isMovePossible) {
                        isBlocked = (player.IsPlayerGhostState()) ? false : _gameBoard.CheckIsPlayerBlockedByFence(playerCurrentPosition, PlayerMoves.Left);
                        isOpponentInPath = _gameBoard.GetTileStatusAt((playerCurrentPosition.row, playerCurrentPosition.column - 2)) == TileStatus.Player;
                    }
                    
                    isEnoughSpaceForJump = ((int) playerCurrentPosition.column > 2); // Enough space at end of board for jumping

                    if(isMovePossible && !isBlocked && isOpponentInPath && isEnoughSpaceForJump) {
                        player.MovePlayer(PlayerMoves.Left);
                        player.MovePlayer(PlayerMoves.Left);
                        break;
                    }

                    if(isMovePossible && !isBlocked && !isOpponentInPath) {
                        player.MovePlayer(PlayerMoves.Left);
                    } 
                    break;
                case 6: case 7: // Move Right
                    // Check if player is at the right most column of the board C16
                    isMovePossible = ((int) playerCurrentPosition.column < 16);      
                    if(isMovePossible) {
                        isBlocked = (player.IsPlayerGhostState()) ? false : _gameBoard.CheckIsPlayerBlockedByFence(playerCurrentPosition, PlayerMoves.Right);
                        isOpponentInPath = _gameBoard.GetTileStatusAt((playerCurrentPosition.row, playerCurrentPosition.column + 2)) == TileStatus.Player;
                    }
                    
                    isEnoughSpaceForJump = ((int) playerCurrentPosition.row < 14); // Enough space at end of board for jumping

                    if(isMovePossible && !isBlocked && isOpponentInPath && isEnoughSpaceForJump) {
                        player.MovePlayer(PlayerMoves.Right);
                        player.MovePlayer(PlayerMoves.Right);
                        break;
                    }

                    if(isMovePossible && !isBlocked && !isOpponentInPath) {
                        player.MovePlayer(PlayerMoves.Right);
                    } 
                    break;
            }

            if(isBlocked) {
                Console.WriteLine("There's a fence blocking your way.");
                isMovePossible = false;
            }

            if(isOpponentInPath && !isEnoughSpaceForJump) {
                Console.WriteLine("Not enough space to move in this direction");
            }

            _gameBoard.UpdatePlayerOnBoard(player, playerCurrentPosition);
            return isMovePossible;
        }

        public (Row row, Column column) GetInputFenceCoordinates() {
            int rowInput, columnInput;

            Console.Write("Please enter Row Number: ");
            Console.WriteLine("");
            bool rowSuccess = int.TryParse(Console.ReadLine(), out rowInput);
            Console.Write("Please enter Column Number: ");
            Console.WriteLine("");
            bool columnSuccess = int.TryParse(Console.ReadLine(), out columnInput);

            if(!rowSuccess || rowInput > 15) {
               rowInput = (int) Row.R_Null;
            }

            if(!columnSuccess || columnInput > 15) { 
                columnInput = (int) Column.C_Null;
            }

            // If Even Rows then Columns can only be odd
            if(rowInput % 2 == 0 && columnInput % 2 != 0) {
                return ((Row) rowInput, (Column) columnInput);
            }

            // If Chosen Odd row, then Columns MUST be even
            if(rowInput % 2 != 0 && columnInput % 2 == 0) {
                return ((Row) rowInput, (Column) columnInput);
            }

            return (Row.R_Null, Column.C_Null);
        }

        public void ShowPlayerOptions(Player player, Player opponent) {
            if(player.AbonormalStatus == PowerUpType.Normal) player.ResetPlayerMoves();           

            // Game Title and Sub title
            SplashKit.DrawText("Quaridor", Color.White,"Resources\\Adventure.otf",100, 770, 10);
            SplashKit.DrawText("The Digital Version", Color.White,"Resources\\Adventure.otf",20, 1010, 100);

            int playerInfoTopPadding = 150;
            int playerTitleFontSize = 40;
            int playerInfoFontSize = 22;
            int playerInfoFontPadding = 5;

            switch(player.PlayerType) {
                case PlayerType.PlayerOne:
                    SplashKit.DrawText("Player One's Turn", SplashKit.RGBColor(97,1,1),"Resources\\Adventure.otf",playerTitleFontSize, 780, playerInfoTopPadding);
                    break;
                case PlayerType.PlayerTwo:
                    SplashKit.DrawText("Player Two's Turn", SplashKit.RGBColor(21,1,92),"Resources\\Adventure.otf",playerTitleFontSize, 780, playerInfoTopPadding);
                    break;
                default:
                    SplashKit.DrawText("AI's Turn", Color.White,"Resources\\Adventure.otf",playerTitleFontSize, 780, playerInfoTopPadding);
                    break;
            }

            SplashKit.DrawText("Status: " + player.AbonormalStatus, Color.White,"Resources\\Adventure.otf",playerInfoFontSize, 780, playerInfoTopPadding + playerTitleFontSize + playerInfoFontPadding);
            SplashKit.DrawText("Active Buff/Debuff: " + player.GetPlayerAbnormalStatus().BuffDebuffName, Color.White,"Resources\\Adventure.otf",playerInfoFontSize, 780, playerInfoTopPadding + playerTitleFontSize + playerInfoFontSize + playerInfoFontPadding*2);
            SplashKit.DrawText("Effect on Player: ", Color.White,"Resources\\Adventure.otf",playerInfoFontSize, 780, playerInfoTopPadding + playerTitleFontSize + playerInfoFontSize * 2 + playerInfoFontPadding*3);
            SplashKit.DrawText(Constants.BlankSpaceThree + player.GetPlayerAbnormalStatus().BuffDebuffDescription, Color.White,"Resources\\Adventure.otf",playerInfoFontSize, 780, playerInfoTopPadding + playerTitleFontSize + playerInfoFontSize * 3 + playerInfoFontPadding*4);
            SplashKit.DrawText("Fences Available: " + player.GetNumberOfAvailableFences(_gameBoard.GameFences), Color.White,"Resources\\Adventure.otf",playerInfoFontSize, 780, playerInfoTopPadding + playerTitleFontSize + playerInfoFontSize * 4 + playerInfoFontPadding*5);

            List<Button> restartButton = _inGameButtons.Where(button => button.ButtonType == ButtonType.ResetButton).ToList();
            List<Button> exitButton = _inGameButtons.Where(button => button.ButtonType == ButtonType.ExitButton).ToList();
            exitButton[0].DrawButton();
            restartButton[0].DrawButton();
        }

        public void ShowMouseHoverDescription(BoardTile currentHoverTile) {
            int decriptionAlignmentX = Constants.DescriptionBackBoardX + 10;
            int descriptionAlignmentY = Constants.DescriptionBackBoardY + 10;
            int descriptionFontSize = 22;

            SplashKit.DrawText("Tile: " + currentHoverTile.TileCoordinate, Color.Black,Constants.FontPath, descriptionFontSize, decriptionAlignmentX, descriptionAlignmentY);
            SplashKit.DrawText("Tile: " + currentHoverTile.TileCoordinate, Color.Black,Constants.FontPath, descriptionFontSize, decriptionAlignmentX, descriptionAlignmentY);
            SplashKit.DrawText("Tile Type: " + currentHoverTile.TileType, Color.Black,Constants.FontPath, descriptionFontSize, decriptionAlignmentX, descriptionAlignmentY + descriptionFontSize + 10);
            SplashKit.DrawText("Tile Status: " + currentHoverTile.TileStatus, Color.Black,Constants.FontPath, descriptionFontSize, decriptionAlignmentX, descriptionAlignmentY + descriptionFontSize * 2 + 10 * 2);

            switch(currentHoverTile.TileStatus) {
            case TileStatus.Blocked:
                SplashKit.DrawText("A fence blocks your path.", Color.White,Constants.FontPath, descriptionFontSize, decriptionAlignmentX, descriptionAlignmentY + descriptionFontSize * 3 + 10 * 3);
                break;
            case TileStatus.Item:
                // Show item description
                // TO DO: Show Buff/Debuff description on mouse hover
                List<BuffDebuff> hoveredItem = _gameBoard.GameBuffDebuffs.Where(item => item.TileCoordinate == currentHoverTile.TileCoordinate).ToList();
                if(hoveredItem.Count > 0) {
                    Color fontColor = (hoveredItem[0].PowerUpType == PowerUpType.Buff) ? Color.LightGreen : Color.Orange;
                    SplashKit.DrawText(hoveredItem[0].BuffDebuffApplicationDescription, fontColor,Constants.FontPath, descriptionFontSize, decriptionAlignmentX, descriptionAlignmentY + descriptionFontSize * 3 + 10 * 3);
                }
                break;
            case TileStatus.Player:
                string descriptionOutput;
                Color descriptionColor;
                if(currentHoverTile.TileCoordinate == _playerOne.PlayerCoordinate) {
                    descriptionOutput = "Player One";
                    descriptionColor = Color.White;
                } else {
                    descriptionOutput = "Player Two";
                    descriptionColor = Color.White;
                }
                SplashKit.DrawText(descriptionOutput, descriptionColor,Constants.FontPath, descriptionFontSize, decriptionAlignmentX, descriptionAlignmentY + descriptionFontSize * 3 + 10 * 3);
                break; 
            }
        }

        public ProgramFlow ShowGameOverMenu(PlayerType winner, Window gameWindow) {
            gameWindow.Clear(Color.Black);
            //SplashKit.FreeAllBitmaps();
            string winningText = "";
            Color winnerColor = Color.White;

            switch(winner) {
                case PlayerType.PlayerOne:
                    winningText = "Player One Wins!";
                    winnerColor = Color.SwinburneRed;
                    break;
                case PlayerType.PlayerTwo:
                    winningText = "Player Two Wins!";
                    winnerColor = Color.DarkCyan;
                    break;
                case PlayerType.BasicAI:
                    winningText = "Computer Wins!";
                    winnerColor = Color.PeachPuff;
                    break;
            }
            SplashKit.DrawText(winningText, winnerColor, Constants.FontPath, 135, 120, 250);
            SplashKit.DrawText("Press [Space] to go back to main menu", Color.WhiteSmoke, Constants.FontPath, 30, 360, 410);

            if(SplashKit.KeyDown(KeyCode.SpaceKey)) {
                Console.WriteLine("Pressed Space inside");
                return ProgramFlow.Exit; // Leads to Main Menu in Program.cs
            } else {
                return ProgramFlow.GameEndsPlayerWon;
            }
        }

        public (ProgramFlow userDecision, GameMode selectedMode) ShowGameMainMenu() {
            // Show user main menu. Return values will determine what menu to show user next
            // COntrolled in the main program file
            List<Button> mainMenuButtons = new List<Button>();
            Bitmap background = new Bitmap("mainMenuBackground", Constants.BackgroundImgPath);
            background.Draw(0,0);
            SplashKit.FillRectangle(SplashKit.RGBAColor(0,0,0,0.65),0,0,Constants.WindowWidth,Constants.WindowHeight);

            SplashKit.DrawText("Quoridor", Color.White,Constants.FontPath, 200, 175, 80);

            int mainMenuButtonStartingX = 200;
            int mainMenuButtonStartingY = 340;

            Button singlePlayerButton = new Button(ButtonType.SinglePlayerButton, mainMenuButtonStartingX,mainMenuButtonStartingY);
            Button multiplayerButton = new Button(ButtonType.MultiplayerButton, mainMenuButtonStartingX, mainMenuButtonStartingY + 80 + 20);
            Button exitButton = new Button(ButtonType.MainMenuExitButton, mainMenuButtonStartingX,mainMenuButtonStartingY+ 80 + 80 + 20 + 20);

            int instructionFontSize = 30;
            int instructionBodyFontSize = 25;
            int newLineSpacing = instructionBodyFontSize + 10;

            SplashKit.DrawText("Controls:", Color.SteelBlue, Constants.FontPath, instructionFontSize, mainMenuButtonStartingX + singlePlayerButton.Width + 20, mainMenuButtonStartingY);
            SplashKit.DrawText("Click on board tiles to move.", Color.AliceBlue, Constants.FontPath, instructionBodyFontSize, mainMenuButtonStartingX + singlePlayerButton.Width + 20, mainMenuButtonStartingY + 10 + instructionFontSize);
            SplashKit.DrawText("Click on fenceTiles to place a fence.", Color.AliceBlue, Constants.FontPath, instructionBodyFontSize, mainMenuButtonStartingX + singlePlayerButton.Width + 20, mainMenuButtonStartingY + newLineSpacing * 2);
            SplashKit.DrawText("Hold [r] to change fence orientation", Color.AliceBlue, Constants.FontPath, instructionBodyFontSize, mainMenuButtonStartingX + singlePlayerButton.Width + 20, mainMenuButtonStartingY + newLineSpacing * 3);

            SplashKit.DrawText("Goal:", Color.SteelBlue, Constants.FontPath, instructionFontSize, mainMenuButtonStartingX + singlePlayerButton.Width + 20, mainMenuButtonStartingY + newLineSpacing * 5);
            SplashKit.DrawText("Get to the other side to win!", Color.AliceBlue, Constants.FontPath, instructionBodyFontSize, mainMenuButtonStartingX + singlePlayerButton.Width + 20, mainMenuButtonStartingY + newLineSpacing * 6);

            int creditSpacing = 950;
            SplashKit.DrawText("made by ", Color.SteelBlue, Constants.FontPath, 15, 0 + creditSpacing, 740);
            SplashKit.DrawText("wong khye yang (101228926)", Color.AliceBlue, Constants.FontPath, 15, 55 + creditSpacing, 740);

            mainMenuButtons.Add(singlePlayerButton);
            mainMenuButtons.Add(multiplayerButton);
            mainMenuButtons.Add(exitButton);

            singlePlayerButton.DrawButton();
            multiplayerButton.DrawButton();
            exitButton.DrawButton();

            foreach(Button button in mainMenuButtons) {
                if(button.IsMouseHoverOnButton(SplashKit.MousePosition())) {
                    SplashKit.FillRectangle(SplashKit.RGBAColor(0,0,0,0.38),button.X, button.Y, button.Width, button.Height);

                    if(SplashKit.MouseClicked(MouseButton.LeftButton)) {
                        switch(button.ButtonType) {
                            case ButtonType.MainMenuExitButton:
                                return (ProgramFlow.CloseGame, GameMode.Null);
                            case ButtonType.SinglePlayerButton:
                                return (ProgramFlow.StartGame, GameMode.SinglePlayer);
                            case ButtonType.MultiplayerButton:
                                return (ProgramFlow.StartGame, GameMode.Multiplayer);
                        }
                    }
                }
            }

            return (ProgramFlow.Unassigned, GameMode.Null);            
        }

        public bool IsUserInputValid(string userInput) {
            // Returns true if user input is acceptable, else returns false
            return Constants.AcceptableInputs.Contains(userInput.ToUpper());
        }

        private void ShowInvalidMessage(string userInput) {
            Console.WriteLine("{0} was an invalid input. Try again.", userInput);
        }

        public bool IsGameInProgress {
            get { return _isGameInProgress; }
            set { _isGameInProgress = value; }
        }

        public PlayerType GameWinner {
            get { return _gameWinner; }
            set { _gameWinner = value; }
        }
    }
}