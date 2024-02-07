using System;
using SplashKitSDK;

namespace Distinction_Task
{
    public class BoostedStamina : BuffDebuff
    {
        public BoostedStamina ((Row row, Column column) tileCoordinate) : base (tileCoordinate) {
            RandomizePowerType();
            BuffDebuffName = Constants.BoostedStaminaName;
            BuffDebuffDescription = Constants.BoostedStaminaDesc;
            BuffDebuffCategory = GameItems.BoostedStamina;

            string imagePath = (PowerUpType == PowerUpType.Buff) ? Constants.BuffBoostedStaminaImgPath : Constants.DebuffBoostedStaminaImgPath;
            BuffDebuffApplicationDescription = (PowerUpType == PowerUpType.Buff) ? Constants.BuffBoostedStaminaApplyDesc : Constants.DebuffBoostedStaminaApplyDesc;
            BuffDebuffBitmap = new Bitmap("Item" + TileCoordinate.ToString(), imagePath);
        }

        public override void ApplyOnPlayer(Player buffDebuffPicker, Player opponent)
        {
            switch(PowerUpType) {
                case PowerUpType.Buff:
                    // Boost own stamina
                    buffDebuffPicker.SetAbnormalStatus(PowerUpType);
                    buffDebuffPicker.SetBuffDebuff(BuffDebuffCategory);
                    // Reset the number of moves to make sure its 0
                    buffDebuffPicker.GivePlayerMoves(1);
                    break;
                case PowerUpType.Debuff:
                    // Debuff version of Boosted Stamina will boost opponents stamina instead
                    opponent.SetAbnormalStatus(PowerUpType.Buff);
                    opponent.SetBuffDebuff(BuffDebuffCategory);
                    // Reset here to ensure the move count is right. Only for Normal status will moves be reset as program progresses
                    opponent.ResetPlayerMoves();
                    opponent.GivePlayerMoves(1);

                    buffDebuffPicker.SetBuffDebuff(GameItems.Nothing);
                    break;
            }
        }
    }
}