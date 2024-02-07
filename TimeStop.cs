using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class TimeStop : BuffDebuff {
        public TimeStop((Row row, Column column) tileCoordinate) : base (tileCoordinate) {
            RandomizePowerType();
            BuffDebuffName = Constants.TimeStopName;
            BuffDebuffDescription = Constants.TimeStopDesc;
            BuffDebuffCategory = GameItems.TimeStop;

            string imagePath = (PowerUpType == PowerUpType.Buff) ? Constants.BuffTimeStopImgPath : Constants.DebuffTimeStopImgPath;
            BuffDebuffApplicationDescription = (PowerUpType == PowerUpType.Buff) ? Constants.BuffTimeStopApplyDesc : Constants.DebufTimeStopApplyDesc;
            BuffDebuffBitmap = new Bitmap("Item" + TileCoordinate.ToString(), imagePath);
        }

        public override void ApplyOnPlayer(Player buffDebuffPicker, Player opponent)
        {
             switch(PowerUpType) {
                case PowerUpType.Buff:
                    // Stops Opponents Time - Skips their turn
                    opponent.SetAbnormalStatus(PowerUpType.Debuff);
                    opponent.SetBuffDebuff(BuffDebuffCategory);
                    opponent.SetPlayerMoves(0);
                    opponent.WasAffectedByTimeStop = true;

                    // Reset status applied to player picking it up
                    buffDebuffPicker.SetAbnormalStatus(PowerUpType.Normal);
                    buffDebuffPicker.SetBuffDebuff(GameItems.Nothing);
                    break;
                case PowerUpType.Debuff:
                    // Stops player's own time
                    buffDebuffPicker.SetAbnormalStatus(PowerUpType);
                    buffDebuffPicker.SetBuffDebuff(BuffDebuffCategory);
                    buffDebuffPicker.SetPlayerMoves(0);
                    buffDebuffPicker.WasAffectedByTimeStop = true;

                    // Reset status applied to player picking it up
                    //opponent.SetAbnormalStatus(PowerUpType.Normal);
                    //opponent.SetBuffDebuff(GameItems.Nothing);
                    break;
            }
        }
    }
}