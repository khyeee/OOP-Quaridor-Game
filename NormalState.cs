using System;

namespace Distinction_Task
{
    public class NormalState : BuffDebuff {
        public NormalState((Row row, Column column) coordinate) : base (coordinate) {
            BuffDebuffName = Constants.NormalStateName;
            BuffDebuffDescription = Constants.NormalStateDesc;
        }

        public override void ApplyOnPlayer(Player buffDebuffPicker, Player opponent)
        {
            return;
        }
    }
}