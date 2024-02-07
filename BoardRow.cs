using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class BoardRow
    {
        private List<BoardTile> _rowTiles;
        private Row _rowNumber;
        
        public BoardRow(Row rowNumber) {
            _rowNumber = rowNumber;
            _rowTiles = new List<BoardTile>();

            if((int) _rowNumber % 2 != 0) {
                // Odd Rows are Fence Rows
                InitFenceRows();
            } else {
                InitMixedRows();
            }
        }

        public void DrawFenceRow(List<Fence> gameFences) {
            foreach(BoardTile tile in _rowTiles) {
                switch (tile.TileStatus) {
                    case TileStatus.Blocked:
                        List<Fence> blockingFenceAnchorBased = gameFences.Where(fence => fence.Coordinates == tile.TileCoordinate).ToList();
                        if(blockingFenceAnchorBased.Count != 0) {
                            // Fence Anchor is on current tile
                            blockingFenceAnchorBased[0].DrawFenceBitmaps();
                        } 
                        break;
                } 

                tile.DrawOutline(Color.Black);
            }
        }

        public void DrawMixedRows(List<Fence> gameFences, List<BuffDebuff> gameBuffDebuffs, Player playerOne, Player playerTwo) {
            // Draw Tiles First, everything else goes on top.
            // foreach(BoardTile tile in _rowTiles) {
            //     if((int) tile.TileCoordinate.column % 2 == 0) {
            //         // On even columns, this means theres a tile.
            //         tile.TileBitmap.Draw(tile.UIx, tile.UIy);
            //     }
            // }

            // Draw Players - Clear Previous positions and update to new ones
            // Names are _playerType.ToString()
            //SplashKit.ClearBitmap(playerOne.PlayerBitmap, Constants.TransparentColor);
            //SplashKit.ClearBitmap(playerTwo.PlayerBitmap, Constants.TransparentColor);
            //playerOne.PlayerBitmap.Draw(playerOne.UIx, playerTwo.UIy);
            //playerTwo.PlayerBitmap.Draw(playerTwo.UIx, playerTwo.UIy);

            // Check for Items and Fences to Draw
            foreach(BoardTile tile in _rowTiles) {
                switch (tile.TileStatus) {
                    case TileStatus.Item: 
                        // Draw Item (Buff/Debuff)
                        List<BuffDebuff> currentTileBuffDebuff = gameBuffDebuffs.Where(item => item.TileCoordinate == tile.TileCoordinate).ToList();
                        if(currentTileBuffDebuff.Count > 0) {
                            // Theres a game item on the current tile
                            // Draw item bitmap
                            gameBuffDebuffs[gameBuffDebuffs.IndexOf(currentTileBuffDebuff[0])].DrawItemBitmap();
                            // Old items are cleared in Quaridor class when timer runs out
                        }
                        break;
                    case TileStatus.Blocked:
                        // Draw Fences
                        List<Fence> fenceAnchorAtTile = gameFences.Where(fence => fence.Coordinates == tile.TileCoordinate).ToList();
                        if(fenceAnchorAtTile.Count > 0) {
                            // Theres a fence at the current tile.
                            gameFences[gameFences.IndexOf(fenceAnchorAtTile[0])].DrawFenceBitmaps();
                        }
                        break;
                    case TileStatus.Player:
                        if(tile.TileCoordinate == playerOne.PlayerCoordinate) {
                            playerOne.PlayerBitmap.Draw(playerOne.UIx, playerOne.UIy);
                        } else {
                            playerTwo.PlayerBitmap.Draw(playerTwo.UIx, playerTwo.UIy);
                        }
                        break;
                }

                tile.DrawOutline(Color.Black);
            }            
        }

        private void InitFenceRows() {
            int maxColumnIndex =16;
            int startingIndex = 0;     

            for (int i = startingIndex; i <= maxColumnIndex; i++) {
                (Row row, Column column) tileCoordinate = (_rowNumber, (Column) i);
                BoardTile fenceTile = new BoardTile(tileCoordinate, TileType.FenceTile);
                _rowTiles.Add(fenceTile);
            }
        }

        private void InitMixedRows() {
            bool isBoardTile = true;
            TileType mixedTileType;
            int maxColumnIndex =16;
            int startingIndex = 0;   

            for (int i = startingIndex; i <= maxColumnIndex; i++) {
                (Row row, Column column) tileCoordinate = (_rowNumber, (Column) i);
                mixedTileType = (isBoardTile) ? TileType.BoardTile : TileType.FenceTile;

                BoardTile mixedTile = new BoardTile(tileCoordinate, mixedTileType);
                _rowTiles.Add(mixedTile);

                isBoardTile = !isBoardTile;
            }
        }

        public List<BoardTile> RowTiles {
            get { return _rowTiles; }
            set { _rowTiles = value; }
        }

        public Row RowNumber {
            get { return _rowNumber; }
        }
    }
}