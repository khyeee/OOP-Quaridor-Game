using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class BoardTile
    {
        private (Row row, Column column) _tileCoordinate;
        private TileStatus _tileStatus;
        private TileType _tileType;
        private bool _isTileChecked;
        private Bitmap _tileBitmap, _objectBitmap;
        private int _UIx, _UIy, _UIxEnd, _UIyEnd;

        public BoardTile((Row row, Column column) tileCoordinate, TileType tileType) {
            _tileCoordinate = tileCoordinate;
            _tileType = tileType;
            _tileStatus = TileStatus.Clear;
            _isTileChecked = false;

            if(_tileType  == TileType.BoardTile) {
                _tileBitmap = new Bitmap(_tileCoordinate.ToString(), Constants.TileImgPath);
            }

            (_UIx, _UIy) = Constants.ConvertArrayCoordinateToUICoordinateForTiles(_tileCoordinate);
            (_UIxEnd, _UIyEnd) = Constants.ConvertArrayCoordinateToUICoordinateForTiles((_tileCoordinate.row + 1, _tileCoordinate.column + 1));
        }

        public void DrawMouseHover(Window gamewindow, HoverStatus hoverStatus) {
            Color glowColor = Color.White;
            switch(hoverStatus) {
                case HoverStatus.ValidPlayerMove:
                    glowColor = Constants.GreenGlow;
                    break;
                case HoverStatus.ValidFencePlacement:
                    glowColor = Constants.GreyGlow;
                    break;
                case HoverStatus.Invalid:
                    glowColor = Constants.RedGlow;
                    break;
            }
            gamewindow.FillRectangle(glowColor, _UIx, _UIy, Constants.TileWidth, Constants.TileHeight);
            DrawOutline(Color.Black);

        }

        public void DrawOutline(Color lineColor) {
            int topLeftx = _UIx;
            int topLefty = _UIy;
            int bottomLeftx = _UIx + Constants.TileHeight;
            int bottomLefty = _UIy;
            int topRightx = _UIx;
            int topRighty = _UIy + Constants.TileWidth;
            int bottomRightx = _UIx + Constants.TileHeight;
            int bottomRighty = _UIy + Constants.TileWidth;
        
            SplashKit.DrawLine(lineColor, topLeftx, topLefty, topRightx, topRighty);
            SplashKit.DrawLine(lineColor, topLeftx, topLefty, bottomLeftx, bottomLefty);
            SplashKit.DrawLine(lineColor, bottomLeftx, bottomLefty, bottomRightx, bottomRighty);
            SplashKit.DrawLine(lineColor, topRightx, topRighty, bottomRightx, bottomRighty);
        }

        public void SetIsTileCheckedTo(bool checkedStatus) {
            _isTileChecked = checkedStatus;
        }

        public (Row row, Column column) TileCoordinate {
            get { return _tileCoordinate; }
            set { _tileCoordinate = value; }
        }

        public TileStatus TileStatus {
            get { return _tileStatus; }
            set { _tileStatus = value; }
        }

        public TileType TileType {
            get { return _tileType; }
            set { _tileType = value; }
        }

        public bool IsTileChecked {
            get { return _isTileChecked; }
            set { _isTileChecked = value;}
        }

        public Bitmap TileBitmap {
            get { return _tileBitmap; }
            set { _tileBitmap = value; } 
        }

        public Bitmap ObjectBitmap {
            get { return _objectBitmap; }
            set { _objectBitmap = value; }
        }

        public int UIx {
            get { return _UIx; }
        }

        public int UIy {
            get {return _UIy; }
        }
    }
}