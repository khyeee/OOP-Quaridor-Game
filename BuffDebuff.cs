using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public abstract class BuffDebuff
    {
        private (Row row, Column column) _tileCoordinate;
        private string _buffDebuffName;
        private string _buffDebuffDescription;
        private string _buffDebuffApplicationDescription;
        private int _buffDuration;
        private PowerUpType _powerUpType;
        private GameItems _buffDebuffCategory;
        private Bitmap _buffDebuffBitmap;
        private int _UIx, _UIy;

        public BuffDebuff((Row row, Column column) tileCoordinate) {
            _powerUpType = PowerUpType.Normal;
            _tileCoordinate = tileCoordinate;
            _buffDebuffName = "";
            _buffDebuffDescription = "";

            (UIx, UIy) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_tileCoordinate);
            SetDurationOnBoard();
        }

        public void RandomizePowerType() {
            Random random = new Random();
            _powerUpType = (random.Next(2) == 0) ? PowerUpType.Buff : PowerUpType.Debuff;
        }

        public void SetDurationOnBoard() {
            // How long the buff stays on the board
            Random randomDuration = new Random();
            _buffDuration = randomDuration.Next(5, 8); // Duration ranges from 1 to 3 turns.
        }

        public void CountDownDuration() {
            _buffDuration -= 1;
        }

        public void DrawItemBitmap() {
            _buffDebuffBitmap.Draw(_UIx, _UIy);
        }
        
        public abstract void ApplyOnPlayer(Player buffDebuffPicker, Player opponent);

        public GameItems BuffDebuffCategory {
            get { return _buffDebuffCategory; }
            set { _buffDebuffCategory = value; }
        }

        public void SetPowerUpTypeTo(PowerUpType buffOrDebuff) {
            _powerUpType = buffOrDebuff;
        }

        public (Row row, Column column) TileCoordinate {
            get { return _tileCoordinate; }
            set { _tileCoordinate = value; }
        }

        public string BuffDebuffName {
            get { return _buffDebuffName; }
            set { _buffDebuffName = value; }
        }

        public string BuffDebuffDescription {
            get { return _buffDebuffDescription; }
            set { _buffDebuffDescription = value; }
        }

        public string BuffDebuffApplicationDescription {
            get { return _buffDebuffApplicationDescription; }
            set { _buffDebuffApplicationDescription = value;}
        }

        public PowerUpType PowerUpType {
            get {return _powerUpType; }
            set { _powerUpType = value; }
        }

        public int BuffDuration {
            get { return _buffDuration; }
            set { _buffDuration = value; }
        }

        public Bitmap BuffDebuffBitmap {
            get { return _buffDebuffBitmap; }
            set { _buffDebuffBitmap = value;}
        }

        public int UIx {
            get { return _UIx; }
            set { _UIx = value; }
        }

        public int UIy {
            get { return _UIy; }
            set { _UIy = value; }
        }
    }
}