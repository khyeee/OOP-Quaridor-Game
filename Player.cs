using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class Player {
        private (Row row, Column column) _playerCoordinate;
        private PlayerType _playerType;
        private PowerUpType _abnormalStatus;
        private GameItems _buffDebuffOnPlayer;
        private int _numberOfMoves;
        private bool _canPlayerPassThroughWalls;
        private bool _wasAffectedByTimeStop;
        private Bitmap _playerBitmap;
        private int _UIx, _UIy;

        public Player(PlayerType playerType) {
            _playerType = playerType;
            _abnormalStatus = PowerUpType.Normal;
            _buffDebuffOnPlayer = GameItems.Nothing;
            _numberOfMoves = 1;
            _canPlayerPassThroughWalls = false;
            _wasAffectedByTimeStop = false;

            _playerCoordinate = (_playerType == PlayerType.PlayerOne) ? (Row.R16, Column.C8) : (Row.R0, Column.C8);
            _playerBitmap = (_playerType == PlayerType.PlayerOne) ? 
            new Bitmap(_playerType.ToString(), Constants.PlayerOneImgPath) : 
            new Bitmap(_playerType.ToString(), Constants.PlayerTwoImgPath); 

            (_UIx, _UIy) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_playerCoordinate);
        }

        public (List<Fence>, int selectedFenceIndex) UseFence (ref List<Fence> gameFences, (Row row, Column column) placementLocation, FenceOrientation placementOrientation) {
            List<Fence> playersFences = gameFences.Where(fence => fence.Ownership == _playerType).ToList();
            List<Fence> availableFences = playersFences.Where(fence => fence.FenceStatus == FenceStatus.InStorage).ToList();
            int fenceIndexInMainList = -1;

            if(availableFences.Count > 0) {
                // Found Fences in stock
                gameFences[gameFences.IndexOf(availableFences[0])].PlaceFenceAt(placementLocation, placementOrientation);
                fenceIndexInMainList = gameFences.IndexOf(availableFences[0]);
            } else {
                Console.WriteLine("{0} is out of fences", _playerType);
            }

            return (gameFences, fenceIndexInMainList);
        }

        public int GetNumberOfAvailableFences(List<Fence> gameFences) {
            List<Fence> playersFences = gameFences.Where(fence => fence.Ownership == _playerType).ToList();
            List<Fence> availableFences = playersFences.Where(fence => fence.FenceStatus == FenceStatus.InStorage).ToList();
            return availableFences.Count();
        }

        public void MovePlayer(PlayerMoves inDirection) {
            Row newRowCoord = _playerCoordinate.row;
            Column newColumnCoord = _playerCoordinate.column;

            switch(inDirection) {
                case PlayerMoves.Up:
                    newRowCoord = _playerCoordinate.row - 2;
                    newColumnCoord = _playerCoordinate.column;
                    break;
                case PlayerMoves.Down:
                    newRowCoord = _playerCoordinate.row + 2;
                    newColumnCoord = _playerCoordinate.column;
                    break;
                case PlayerMoves.Left:
                    newRowCoord = _playerCoordinate.row;
                    newColumnCoord = _playerCoordinate.column - 2;
                    break;
                case PlayerMoves.Right:
                    newRowCoord = _playerCoordinate.row;
                    newColumnCoord = _playerCoordinate.column + 2;
                    break;
                case PlayerMoves.Stay:
                    break;
            }

            _playerCoordinate = (newRowCoord, newColumnCoord);
        }

        public void ResetAbnormalStatus() {
            _abnormalStatus = PowerUpType.Normal;
            _buffDebuffOnPlayer = GameItems.Nothing;
        }

        public void SetAbnormalStatus(PowerUpType abnormalStatus) {
            _abnormalStatus = abnormalStatus;
        }

        public void SetBuffDebuff(GameItems buffDebuff) {
            _buffDebuffOnPlayer = buffDebuff;
        }

        public BuffDebuff GetPlayerAbnormalStatus() {
            switch (_buffDebuffOnPlayer) {
                case GameItems.BoostedStamina:
                    BoostedStamina newBSItem = new BoostedStamina(_playerCoordinate);
                    newBSItem.SetPowerUpTypeTo(AbonormalStatus);
                    return newBSItem;
                case GameItems.TimeStop:
                    TimeStop newTSItem = new TimeStop(_playerCoordinate);
                    newTSItem.SetPowerUpTypeTo(AbonormalStatus);
                    return newTSItem;
                case GameItems.GhostForm:
                    GhostForm newGFItem = new GhostForm(_playerCoordinate);
                    newGFItem.SetPowerUpTypeTo(AbonormalStatus);
                    return newGFItem;
                default:
                    NormalState newNormal = new NormalState(_playerCoordinate);
                    newNormal.SetPowerUpTypeTo(AbonormalStatus);
                    return new NormalState(_playerCoordinate);
            }
        }

        public bool UseMove() {
            _numberOfMoves -= 1;
            if(_numberOfMoves  < 1) _numberOfMoves = 0;
            // Check if there's moves to be used. Returns false if NumberOfMoves is 0
            return (_numberOfMoves > 0);
        }

        public void GivePlayerMoves(int numberOfExtraMoves) {
            _numberOfMoves += numberOfExtraMoves;
        }

        public void ResetPlayerMoves() {
            _numberOfMoves = 1;
        }

        public void SetPlayerMoves(int numberOfMoves) {
            _numberOfMoves = numberOfMoves;
        }

        public void ResetPlayerGhostState() {
            _canPlayerPassThroughWalls = false;
        }

        public void EnterGhostState() {
            _canPlayerPassThroughWalls = true;
        }

        public bool IsPlayerGhostState() {
            return _canPlayerPassThroughWalls;
        }

        public (string decision, (Row row, Column column) FenceCoordinate, FenceOrientation orientation) DecideNextMove() {
            Random random = new Random();
            int decidingInt = random.Next(11); // Random number from 0 to 3

            switch(decidingInt) {
                case 0: case 1: case 2: case 3: case 4: case 5:
                    return ("MOVE DOWN", (Row.R_Null, Column.C_Null), FenceOrientation.OrientError);
                case 6:
                    return ("MOVE UP", (Row.R_Null, Column.C_Null), FenceOrientation.OrientError);
                case 7:
                    return ("MOVE LEFT", (Row.R_Null, Column.C_Null), FenceOrientation.OrientError);
                case 8:
                    return ("MOVE RIGHT", (Row.R_Null, Column.C_Null), FenceOrientation.OrientError);
                default:
                    // Place fence
                    return ("FENCE", (Row.R_Null, Column.C_Null), FenceOrientation.OrientError);
            }
        }

        public void UpdatePlayerUIxUIyCoordinates() {
            (_UIx, _UIy) = Constants.ConvertArrayCoordinateToUICoordinateForItems(_playerCoordinate);
        }

        public FenceOrientation GenerateRandomFenceOrientation((Row row, Column column) placementCoordinate) {
            Random myRandom = new Random();

            if((int) placementCoordinate.row % 2 == 0) {
                return FenceOrientation.Vertical;
            } else {
                if((int) placementCoordinate.column % 2 != 0) {
                    if(myRandom.Next(7) < 3) {
                        return FenceOrientation.Horizontal;
                    } else {
                        return FenceOrientation.Vertical;
                    }
                }

                return FenceOrientation.Horizontal;
            }
            
        }

        public (Row row, Column column) GenerateRandomFenceCoordinate(Player playerAnchor) {
            Random myRandom = new Random();
            (Row row, Column column) playerAnchorCoordinate = playerAnchor.PlayerCoordinate;
            int rndDisplaceRow = 0;
            int rndDisplaceCol = 0;
            int potentialRow, potentialCol;

            if((int) playerAnchorCoordinate.row > 2) {
                rndDisplaceRow = myRandom.Next(1, 4) * -1; // Displace by 1 to 3 steps
            } else {
                // To handle when player at R2 (last row before winning)
                // At Row 0, game will end automatically, so no need to check
                rndDisplaceRow = myRandom.Next(2) * -1; // 0 or -1
            }

            if((int) playerAnchorCoordinate.column == 16) {
                rndDisplaceCol = myRandom.Next(1, 3) * -1;
            } else if((int) playerAnchorCoordinate.column == 0) {
                rndDisplaceCol = myRandom.Next(1, 3);
            } else {
                int rndSign = (myRandom.Next(2) == 0) ? -1 : 1;
                rndDisplaceCol = myRandom.Next(1, 3) * rndSign;
            }
            
            potentialRow = (int) playerAnchorCoordinate.row + rndDisplaceRow;
            potentialCol = (int) playerAnchorCoordinate.column + rndDisplaceCol;
            
            if(potentialRow % 2 == 0) {
                // Even numbered row. Column must be odd
                if(potentialCol % 2 == 0) {
                    // Column is even. Make it odd
                    potentialCol += (potentialCol == 0) ? 1 : -1; 
                }
            } else {
                // Odd Numbered row, Column must be even
                if(potentialCol % 2 != 0 || potentialCol == 16) {
                    potentialCol += (potentialCol == 16) ? -1 : 1;
                }
            }
            
            return ((Row) potentialRow, (Column) potentialCol);
        }

        public PlayerType PlayerType {
            get { return _playerType; }
            set { _playerType = value;}
        }   

        public (Row row, Column column) PlayerCoordinate {
            get { return _playerCoordinate; }
            set { _playerCoordinate = value; }
        }

        public PowerUpType AbonormalStatus {
            get { return _abnormalStatus; }
            set { _abnormalStatus = value; }
        }

        public GameItems BuffDebuffOnPlayer {
            get { return _buffDebuffOnPlayer; }
            set { _buffDebuffOnPlayer = value; }
        }

        public int NumberOfMoves {
            get { return _numberOfMoves; }
            set { _numberOfMoves = value; }
        }

        public bool CanPlayerPassThroughWalls {
            get { return _canPlayerPassThroughWalls; }
            set { _canPlayerPassThroughWalls = value; }
        }

        public Bitmap PlayerBitmap {
            get { return _playerBitmap; }
        }

        public int UIx {
            get { return _UIx; }
        }

        public int UIy {
            get {return _UIy; }
        }

        public bool WasAffectedByTimeStop {
            get { return _wasAffectedByTimeStop; }
            set { _wasAffectedByTimeStop = value; }
        }
    }
}