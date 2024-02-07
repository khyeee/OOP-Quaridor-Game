using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class Board
    {
        private Player _playerOne, _playerTwo;
        private List<BoardRow> _boardRows;
        private List<Fence> _gameFences;
        private List<BuffDebuff> _gameBuffDebuffs;

        public Board(Player playerOne, Player playerTwo) {
            _gameBuffDebuffs = new List<BuffDebuff>();
            _boardRows = new List<BoardRow>();
            _gameFences = new List<Fence>();
            _playerOne = playerOne;
            _playerTwo = playerTwo;
        }

        public void SetUpNewBoard(GameMode gameMode) {
            // Init rows on the board
            for (int i = 0; i <= 16; i++) {
                BoardRow newRow = new BoardRow((Row) i);
                _boardRows.Add(newRow);
            }

            // Init the fences for each player - 10 each
            for (int i = 0; i < 20; i++) {
                PlayerType playerOwnership = (i < 10) ? PlayerType.PlayerOne : (gameMode == GameMode.Multiplayer) ? PlayerType.PlayerTwo : PlayerType.BasicAI;
                string playerIdentifier = (i < 10) ? Constants.PlayerOneSymbol : (gameMode == GameMode.Multiplayer) ? Constants.PlayerTwoSymbol : Constants.AISymbol;
                string fenceID = (i < 10) ? playerIdentifier + "0" + i.ToString() : playerIdentifier + "0" + (i - 10).ToString();
                Fence newFence = new Fence(playerOwnership, fenceID);

                _gameFences.Add(newFence);
            }
            
            // Init the tiles of player's starting positions
            _boardRows[(int) _playerOne.PlayerCoordinate.row].RowTiles[(int) _playerOne.PlayerCoordinate.column].TileStatus = TileStatus.Player;
            _boardRows[(int) _playerTwo.PlayerCoordinate.row].RowTiles[(int) _playerTwo.PlayerCoordinate.column].TileStatus = TileStatus.Player;
        }

        public void DrawBoardElements() {
            DrawColumnAndRowAxis();
            DrawBoardTiles();

            foreach (BoardRow row in _boardRows) {
                if((int) row.RowNumber % 2 != 0) {
                    // Fence Row
                    row.DrawFenceRow(_gameFences);
                } else {
                    // Mixed Row
                    row.DrawMixedRows(_gameFences, _gameBuffDebuffs, _playerOne, _playerTwo);
                }
            }
        }

        public void DrawColumnAndRowAxis() {
            int fontSize = 18;

            // Drawing Row Labels
            for(int row = 0; row < 17; row++) {
                SplashKit.DrawText("R" + row, Color.Black,"Resources\\Adventure.otf",fontSize, 10, 47 + Constants.TileHeight * row);
            }
            // Drawing Column Labels
            for(int column = 0; column < 17; column++) {
                SplashKit.DrawText("C" + column, Color.Black, "Resources\\Adventure.otf", fontSize, 47 + Constants.TileWidth * column, 10);
            }
        }

        public void DrawBoardTiles() {
            // Draw Tiles First, everything else goes on top.
            foreach(BoardRow row in _boardRows) {
                foreach(BoardTile tile in row.RowTiles) {
                    if(tile.TileType == TileType.BoardTile) {
                        // On even columns, this means theres a tile.
                        tile.TileBitmap.Draw(tile.UIx, tile.UIy);
                        tile.DrawOutline(Color.Black);
                    }
                }
            }
        }

        public void UpdateBoard() {
            // IMPORTANT TO DO: No implementation yet to update board for buffdebuff locations
            List<Fence> fencesInPlay = _gameFences.Where(fence => fence.FenceStatus == FenceStatus.InPlay).ToList();
            
            // Update for active fence locations
            foreach(Fence fence in fencesInPlay) {
                // Update tiles for fences currently on the board
                GetTileAt(fence.Coordinates).TileStatus = TileStatus.Blocked;
                GetTileAt(fence.CoordinateBody).TileStatus = TileStatus.Blocked;
                GetTileAt(fence.CoordinateTail).TileStatus = TileStatus.Blocked;
            }

            // Update for active buff/debuffs
            foreach(BuffDebuff item in _gameBuffDebuffs) {
                GetTileAt(item.TileCoordinate).TileStatus = TileStatus.Item;
            }
        }

        public void UpdateBoardWithNewFence(int indexOfNewFence) {
            // There's a fence available to be used
            if(indexOfNewFence > 0) {
                Fence newFence = _gameFences[indexOfNewFence];

                GetTileAt(newFence.Coordinates).TileStatus = TileStatus.Blocked;
                GetTileAt(newFence.CoordinateBody).TileStatus = TileStatus.Blocked;
                GetTileAt(newFence.CoordinateTail).TileStatus = TileStatus.Blocked;
            }
        }

        public void UpdatePlayerOnBoard(Player player, (Row row, Column col) playerOldLocation) {
            GetTileAt(playerOldLocation).TileStatus = TileStatus.Clear;

            if(player.PlayerType == PlayerType.PlayerOne) {
                _playerOne = player;
                GetTileAt(_playerOne.PlayerCoordinate).TileStatus = TileStatus.Player;
                _playerOne.UpdatePlayerUIxUIyCoordinates();
            } else {
                _playerTwo = player;
                GetTileAt(_playerTwo.PlayerCoordinate).TileStatus = TileStatus.Player;
                _playerTwo.UpdatePlayerUIxUIyCoordinates();
            }
        }

        public void SystemRemovesFenceAt((Row row, Column column) fenceAnchorCoordinate) {
            List<Fence> selectedFence = _gameFences.Where(fence => fence.Coordinates == fenceAnchorCoordinate).ToList();
            selectedFence[0].RemoveFenceFrom(fenceAnchorCoordinate);
            
            (Row rowBody, Column columnBody) bodyCoordinate;
            (Row rowTail, Column columnTail) tailCoordinate;

            if((int) fenceAnchorCoordinate.row % 2 == 0) {
                // Even rows - Fences are oriented vertically
                bodyCoordinate = (fenceAnchorCoordinate.row + 1, fenceAnchorCoordinate.column);
                tailCoordinate = (fenceAnchorCoordinate.row + 2, fenceAnchorCoordinate.column);
            } else {
                // Odd rows - Fences are oriented horizontally
                bodyCoordinate = (fenceAnchorCoordinate.row, fenceAnchorCoordinate.column + 1);
                tailCoordinate = (fenceAnchorCoordinate.row, fenceAnchorCoordinate.column + 2);
            }

            GetTileAt(fenceAnchorCoordinate).TileStatus = TileStatus.Clear;
            GetTileAt(bodyCoordinate).TileStatus = TileStatus.Clear;
            GetTileAt(tailCoordinate).TileStatus = TileStatus.Clear;
        }

        public TileStatus GetTileStatusAt((Row row, Column column) tileCoordinate) {
            return _boardRows[(int) tileCoordinate.row].RowTiles[(int) tileCoordinate.column].TileStatus;
        }

        public BoardTile GetTileAt((Row row, Column column) tileCoordinate) {
            return _boardRows[(int) tileCoordinate.row].RowTiles[(int) tileCoordinate.column];
        }

        public bool IsFenceFitAt((Row anchorRow, Column anchorCol) anchorCoordinates, FenceOrientation orientation) {
            bool isFit = true;
            (Row row, Column column) tempBodyCoordinate = anchorCoordinates; 
            (Row row, Column column) tempTailCoordinate = anchorCoordinates;

            switch(orientation) {
                case FenceOrientation.Vertical:
                    // Even rows means fence MUST be vertical
                    // Check if it is at the bottom edge of the board
                    if((int) anchorCoordinates.anchorRow > 14) return false;

                    tempBodyCoordinate = (anchorCoordinates.anchorRow + 1, anchorCoordinates.anchorCol);
                    tempTailCoordinate = (anchorCoordinates.anchorRow + 2, anchorCoordinates.anchorCol);
                    break;
                case FenceOrientation.Horizontal:
                    // On Odd rows, fences MUST be horizontal
                    if((int) anchorCoordinates.anchorCol > 14) return false;
                    tempBodyCoordinate = (anchorCoordinates.anchorRow, anchorCoordinates.anchorCol + 1);
                    tempTailCoordinate = (anchorCoordinates.anchorRow, anchorCoordinates.anchorCol + 2);
                    break;
            }

            if(GetTileStatusAt(anchorCoordinates) == TileStatus.Blocked || GetTileStatusAt(tempBodyCoordinate) == TileStatus.Blocked || GetTileStatusAt(tempTailCoordinate) == TileStatus.Blocked) {
                isFit = false;
            }
            return isFit;
        }

        public bool CheckIsPlayerBlockedByFence((Row row, Column col) playerCoordinate, PlayerMoves moveDirection) {
            bool isBlocked = false;
            
            switch (moveDirection) {
                case PlayerMoves.Up:
                    if((int) playerCoordinate.row == 0) return true;
                    isBlocked = GetTileStatusAt((playerCoordinate.row - 1, playerCoordinate.col)) == TileStatus.Blocked;
                    break;
                case PlayerMoves.Down:
                    if((int) playerCoordinate.row == 16) return true;
                    isBlocked = GetTileStatusAt((playerCoordinate.row + 1, playerCoordinate.col)) == TileStatus.Blocked;
                    break;
                case PlayerMoves.Left:
                    if((int) playerCoordinate.col == 0) return true;
                    isBlocked = GetTileStatusAt((playerCoordinate.row, playerCoordinate.col - 1)) == TileStatus.Blocked;                    
                    break;
                case PlayerMoves.Right:
                    if((int) playerCoordinate.col == 16) return true;
                    isBlocked = GetTileStatusAt((playerCoordinate.row, playerCoordinate.col + 1)) == TileStatus.Blocked;
                    break;
            }
            return isBlocked;
        }

        public void CheckIsAllPathsBlockedFrom((Row row, Column column) playerCurrentCoordinate) {
            if((int) playerCurrentCoordinate.row > 16) return;
            if((int) playerCurrentCoordinate.column > 16) return;
            if((int) playerCurrentCoordinate.row < 0) return;
            if((int) playerCurrentCoordinate.column < 0) return;

            // The first call to start off the recursion version of flood fill algo
            if(!GetTileCheckedStatusAt(playerCurrentCoordinate)) {
                GetTileAt(playerCurrentCoordinate).SetIsTileCheckedTo(true);

                // Check Up
                if(!CheckIsPlayerBlockedByFence(playerCurrentCoordinate, PlayerMoves.Up)) {
                    CheckIsAllPathsBlockedFrom((playerCurrentCoordinate.row - 2, playerCurrentCoordinate.column));
                }
                
                // Check Down
                if(!CheckIsPlayerBlockedByFence(playerCurrentCoordinate, PlayerMoves.Down)) {
                    CheckIsAllPathsBlockedFrom((playerCurrentCoordinate.row + 2, playerCurrentCoordinate.column));
                }
        
                // Check Left
                if(!CheckIsPlayerBlockedByFence(playerCurrentCoordinate, PlayerMoves.Left)) {
                    CheckIsAllPathsBlockedFrom((playerCurrentCoordinate.row, playerCurrentCoordinate.column - 2));
                }
                
                // Check Right
                if(!CheckIsPlayerBlockedByFence(playerCurrentCoordinate, PlayerMoves.Right)) {
                    CheckIsAllPathsBlockedFrom((playerCurrentCoordinate.row, playerCurrentCoordinate.column + 2));
                } 
            } 
        }

        public bool CheckIsPossibleToWinFor(Player player) {
            bool canPlayerWin = false;

            switch(player.PlayerType) {
                case PlayerType.PlayerOne:
                    canPlayerWin = _boardRows[(int) Row.R0].RowTiles.Where(tile => tile.IsTileChecked).ToList().Count() > 0;
                    break;
                case PlayerType.PlayerTwo: case PlayerType.BasicAI:
                    canPlayerWin = _boardRows[(int) Row.R16].RowTiles.Where(tile => tile.IsTileChecked).ToList().Count() > 0;
                    break;
            }

            return canPlayerWin;
        }

        public bool CheckHasPlayerWon(Player player) {
            bool isPlayerWin = false;

            switch(player.PlayerType) {
                case PlayerType.PlayerOne:
                    isPlayerWin = _boardRows[(int) Row.R0].RowTiles.Where(tile => tile.TileCoordinate == player.PlayerCoordinate).ToList().Count() > 0;
                    break;
                case PlayerType.PlayerTwo: case PlayerType.BasicAI:
                    isPlayerWin = _boardRows[(int) Row.R16].RowTiles.Where(tile => tile.TileCoordinate == player.PlayerCoordinate).ToList().Count() > 0;
                    break;
            }

            return isPlayerWin;
        }

        public bool GetTileCheckedStatusAt ((Row row, Column column) tileCoordinate) {
            return _boardRows[(int) tileCoordinate.row].RowTiles[(int) tileCoordinate.column].IsTileChecked;
        }

        public void ResetCheckedStatusAllTiles() {
            foreach(BoardRow row in _boardRows) {
                foreach(BoardTile tile in row.RowTiles) {
                    tile.SetIsTileCheckedTo(false);
                }
            }
        }

        public bool CheckHasPlayerPickedUpItem(Player player) {
            bool hasPickedUp = false;

            List<BuffDebuff> playerPickedBuffDebuff = _gameBuffDebuffs.Where(buffDebuff => buffDebuff.TileCoordinate == player.PlayerCoordinate).ToList();
            if(playerPickedBuffDebuff.Count() > 0) {
                if(playerPickedBuffDebuff[0].BuffDebuffCategory != GameItems.Nothing) {
                    // Found player standing on same tile as buff/debuff
                    hasPickedUp = true;
                    GetTileAt(player.PlayerCoordinate).TileStatus = TileStatus.Player; // Change the tile statuse to Player
                    BuffDebuff pickedUpItem = GetBuffDebuffAt(player.PlayerCoordinate);
                    player.SetAbnormalStatus(pickedUpItem.PowerUpType);
                    player.SetBuffDebuff(pickedUpItem.BuffDebuffCategory);
                    Console.WriteLine("Picked up {1} {0}", pickedUpItem.PowerUpType, pickedUpItem.BuffDebuffCategory);
                    //SplashKit.FreeBitmap(pickedUpItem.BuffDebuffBitmap);
                    _gameBuffDebuffs.Remove(pickedUpItem);
                }  
            }

            return hasPickedUp;
        }

        public void AddBuffDebuffToBoard(BuffDebuff buffDebuff) {
            if(GetTileStatusAt(buffDebuff.TileCoordinate) == TileStatus.Clear) {
                GetTileAt(buffDebuff.TileCoordinate).TileStatus = TileStatus.Item;
                _gameBuffDebuffs.Add(buffDebuff);
            }
        }

        public void RemoveBuffDebuffFromBoardSetTileToClear(BuffDebuff buffDebuff) {
            GetTileAt(buffDebuff.TileCoordinate).TileStatus = TileStatus.Clear;
            SplashKit.ClearBitmap(buffDebuff.BuffDebuffBitmap, Constants.TransparentColor);
            SplashKit.FreeBitmap(buffDebuff.BuffDebuffBitmap);
            _gameBuffDebuffs.Remove(buffDebuff);
        }

        public BuffDebuff GetBuffDebuffAt((Row row, Column column) coordinates) {
            List<BuffDebuff> foundBuffDebuff = _gameBuffDebuffs.Where(item => item.TileCoordinate == coordinates).ToList();
            if(foundBuffDebuff.Count() > 0) return foundBuffDebuff[0];
            return new NormalState(coordinates);
        }

        public List<Fence> GameFences {
            get { return _gameFences; }
            set { _gameFences = value; }
        }

        public List<BoardRow> BoardRows {
            get { return _boardRows; }
        }

        public List<BuffDebuff> GameBuffDebuffs {
            get { return _gameBuffDebuffs; }
            set { _gameBuffDebuffs = value; }
        }
    }
}