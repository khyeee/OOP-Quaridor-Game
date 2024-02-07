using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class GhostForm : BuffDebuff
    {
        public GhostForm ((Row row, Column column) tileCoordinate) : base (tileCoordinate) {
            RandomizePowerType();
            BuffDebuffName = Constants.GhostFormName;
            BuffDebuffDescription = Constants.GhostFormDesc;
            BuffDebuffCategory = GameItems.GhostForm;
            BuffDebuffApplicationDescription = Constants.GhostFormApplyDesc;
            
            string imagePath = (PowerUpType == PowerUpType.Buff) ? Constants.BuffGhostFormImgPath : Constants.DebuffGhostFormImgPath;
            BuffDebuffApplicationDescription = (PowerUpType == PowerUpType.Buff) ? Constants.BuffGhostFormApplyDesc : Constants.DebuffGhostFormApplyDesc;
            BuffDebuffBitmap = new Bitmap("Item" + TileCoordinate.ToString(), imagePath);
        }

        public override void ApplyOnPlayer(Player buffDebuffPicker, Player opponent)
        {
            switch(PowerUpType) {
                case PowerUpType.Buff:
                    // Boost own stamina
                    buffDebuffPicker.SetAbnormalStatus(PowerUpType);
                    buffDebuffPicker.SetBuffDebuff(BuffDebuffCategory);
                    //buffDebuffPicker.ResetPlayerMoves();
                    buffDebuffPicker.EnterGhostState();
                    break;
                case PowerUpType.Debuff:
                    // Debuff version of Boosted Stamina will boost opponents stamina instead
                    opponent.SetAbnormalStatus(PowerUpType.Buff);
                    opponent.SetBuffDebuff(BuffDebuffCategory);
                    //opponent.ResetPlayerMoves();
                    opponent.EnterGhostState();
                    
                    // Reset the players status after application of debuff to opponent
                    buffDebuffPicker.SetBuffDebuff(GameItems.Nothing);
                    break;
            }
        }
    }
}