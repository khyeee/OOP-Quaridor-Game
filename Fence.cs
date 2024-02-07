using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class Fence {
        // Coordinates are the anchor for fences:
            // Horizontal fences have 2nd tile to right of anchor
            // Vertical tiles have 2nd tile below its anchor
        private (Row row, Column column) _coordinates;
        private (Row rowBody, Column columnBody) _coordinateBody;
        private (Row rowTail, Column columnTail) _coordinateTail;
        private PlayerType _ownership;
        private string _fenceID;
        private FenceOrientation _orientation;
        private FenceStatus _fenceStatus;
        private Bitmap _anchorBitmap, _bodyBitmap, _tailBitmap;
        private int _UIxAnchor, _UIxBody, _UIxTail, _UIyAnchor, _UIyBody, _UIyTail;

        public Fence(PlayerType ownership, string fenceID) {
            _ownership = ownership;
            _fenceID = fenceID;
            _fenceStatus = FenceStatus.InStorage;
            _coordinates = (Row.R_Null, Column.C_Null);

            if(_ownership == PlayerType.PlayerOne) {
                _anchorBitmap = new Bitmap(_coordinates.ToString(), Constants.FencePOneImgPath);
                _bodyBitmap = new Bitmap(_coordinateBody.ToString(), Constants.FencePOneImgPath);
                _tailBitmap = new Bitmap(_coordinateTail.ToString(), Constants.FencePOneImgPath);
            } else {
                _anchorBitmap = new Bitmap(_coordinates.ToString(), Constants.FencePTwoImgPath);
                _bodyBitmap = new Bitmap(_coordinateBody.ToString(), Constants.FencePTwoImgPath);
                _tailBitmap = new Bitmap(_coordinateTail.ToString(), Constants.FencePTwoImgPath);
            }

            (_UIxAnchor, _UIyAnchor) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_coordinates);
            (_UIxBody, _UIyBody) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_coordinateBody);
            (_UIxTail, _UIyTail) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_coordinateTail);
        }

        public Fence() {
            _fenceID = "ERR";
            _fenceStatus = FenceStatus.InStorage;
            _coordinates = (Row.R_Null, Column.C_Null);
        }

        // Sets the Anchor, Body, and Tail coordinate oriented relative to the placement location
        public void PlaceFenceAt((Row anchorRow, Column anchorColumn) anchorCoordinates, FenceOrientation orientation) {
            _fenceStatus = FenceStatus.InPlay;
            _coordinates = anchorCoordinates;
            _orientation = orientation;

            switch(orientation) {
                case FenceOrientation.Horizontal:
                    _coordinateBody = (anchorCoordinates.anchorRow, anchorCoordinates.anchorColumn + 1);
                    _coordinateTail = (anchorCoordinates.anchorRow, anchorCoordinates.anchorColumn + 2);
                    break;
                case FenceOrientation.Vertical:
                    _coordinateBody = (anchorCoordinates.anchorRow + 1, anchorCoordinates.anchorColumn);
                    _coordinateTail = (anchorCoordinates.anchorRow + 2, anchorCoordinates.anchorColumn);
                    break;
            }

            if(_ownership == PlayerType.PlayerOne) {
                _anchorBitmap = new Bitmap(_coordinates.ToString(), Constants.FencePOneImgPath);
                _bodyBitmap = new Bitmap(_coordinateBody.ToString(), Constants.FencePOneImgPath);
                _tailBitmap = new Bitmap(_coordinateTail.ToString(), Constants.FencePOneImgPath);
            } else {
                _anchorBitmap = new Bitmap(_coordinates.ToString(), Constants.FencePTwoImgPath);
                _bodyBitmap = new Bitmap(_coordinateBody.ToString(), Constants.FencePTwoImgPath);
                _tailBitmap = new Bitmap(_coordinateTail.ToString(), Constants.FencePTwoImgPath);
            }

            UpdateFenceUIxUIyCoordinates();
        }

        public void RemoveFenceFrom((Row anchorRow, Column anchorColumn) anchorCoordinates) {
            _fenceStatus = FenceStatus.InStorage;
            _coordinates = (Row.R_Null, Column.C_Null);
            _coordinateBody = (Row.R_Null, Column.C_Null);
            _coordinateTail = (Row.R_Null, Column.C_Null);
        }

        public void DrawFenceBitmaps() {
            if(_ownership == PlayerType.PlayerOne) {
                _anchorBitmap = new Bitmap(_coordinates.ToString(), Constants.FencePOneImgPath);
                _bodyBitmap = new Bitmap(_coordinateBody.ToString(), Constants.FencePOneImgPath);
                _tailBitmap = new Bitmap(_coordinateTail.ToString(), Constants.FencePOneImgPath);
            } else {
                _anchorBitmap = new Bitmap(_coordinates.ToString(), Constants.FencePTwoImgPath);
                _bodyBitmap = new Bitmap(_coordinateBody.ToString(), Constants.FencePTwoImgPath);
                _tailBitmap = new Bitmap(_coordinateTail.ToString(), Constants.FencePTwoImgPath);
            }

            _anchorBitmap.Draw(_UIxAnchor, _UIyAnchor);
            _bodyBitmap.Draw(_UIxBody, _UIyBody);
            _tailBitmap.Draw(_UIxTail, _UIyTail);
        }

        public void UpdateFenceUIxUIyCoordinates() {
            (_UIxAnchor, _UIyAnchor) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_coordinates);
            (_UIxBody, _UIyBody) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_coordinateBody);
            (_UIxTail, _UIyTail) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_coordinateTail);
        }

        public (Row row, Column column) Coordinates {
            get { return _coordinates; }
            set { _coordinates = value;}
        }

        public (Row rowBody, Column columnBody) CoordinateBody {
            get { return _coordinateBody; }
            set { _coordinateBody = value; }
        }

        public (Row rowTail, Column columnTail) CoordinateTail {
            get { return _coordinateTail; }
            set { _coordinateTail = value; }
        }

        public PlayerType Ownership {
            get { return _ownership; }
        }

        public FenceOrientation Orientation {
            get { return _orientation; }
            set { _orientation = value; }
        }

        public FenceStatus FenceStatus {
            get { return _fenceStatus; }
            set { _fenceStatus = value; }
        }

        public string FenceID {
            get { return _fenceID; }
        }

        public Bitmap AnchorBitmap {
            get { return _anchorBitmap; }
            set { _anchorBitmap = value; }
        }

        public Bitmap BodyBitmap {
            get { return _bodyBitmap; }
            set { _bodyBitmap = value; }
        }

        public Bitmap TailBitmap {
            get { return _tailBitmap; }
            set { _tailBitmap = value; }
        }

        public int UIxAnchor {
            get { return _UIxAnchor; }
            set { _UIxAnchor = value; }
        }

        public int UIyAnchor {
            get { return _UIyAnchor; }
            set { _UIyAnchor = value; }
        }

        public int UIxBody {
            get { return _UIxBody; }
            set { _UIxBody = value; }
        }

        public int UIyBody {
            get { return _UIyBody; }
            set { _UIyBody = value; }
        }

        public int UIxTail {
            get { return _UIxTail; }
            set { _UIxTail = value; }
        }

        public int UIyTail {
            get { return _UIyTail; }
            set { _UIyTail = value; }
        }
    }
}