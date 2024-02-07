using System;
using NUnit.Framework;

namespace Distinction_Task {
    /// <summary>
    /// Main used to test the functionality of child classes of the main BuffDebuff Abstract class
    /// </summary>
    [TestFixture()]
    public class BuffDebuffTests {

        /// <summary>
        /// BoostedStamina will increase the player's moves by 1 or the opponent's moves by 1
        /// depending on the PowerUpType of the BoostedStamina item itself. 
        /// Buff Boosted Stamina = Increase moves of player that picked it up
        /// Debuff Boosted Stamina = Increase moves of opponent of the player that picked up BoostedStamina
        /// </summary>
        [Test()]
        public void TestBoostedStaminaChild() {
            Player itemPicker = new Player(PlayerType.PlayerOne);
            Player testPlayerTwo = new Player(PlayerType.PlayerTwo);
            (Row row, Column col) tempCoordinates = (Row.R0, Column.C0);
            BoostedStamina bsChild = new BoostedStamina(tempCoordinates);

            // Before applying Boosted stamina to test players
            Assert.AreEqual(1, itemPicker.NumberOfMoves);
            Assert.AreEqual(1, testPlayerTwo.NumberOfMoves);

            // Testing Buff variation of Boosted Stamina
            bsChild.PowerUpType = PowerUpType.Buff;
            bsChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.AreEqual(2, itemPicker.NumberOfMoves);
            Assert.AreEqual(1, testPlayerTwo.NumberOfMoves);

            itemPicker.ResetPlayerMoves();
            testPlayerTwo.ResetPlayerMoves();

            // Testing Debuff variation of Boosted Stamina
            bsChild.PowerUpType = PowerUpType.Debuff;
            bsChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.AreEqual(2, testPlayerTwo.NumberOfMoves);
            Assert.AreEqual(1, itemPicker.NumberOfMoves);
        }

        /// <summary>
        /// GhostForm allows players to walk through walls by 1 turn. Each player object has an attribute to indicate
        /// if it is able to pass through walls.
        /// GhostForm will activate this attribute, either for the item picker or their opponent, depending on if the 
        /// Ghost Form item is a Buff or Debuff.
        /// Buff Ghost Form will allow the player that picked up the item to walk past walls.
        /// Debuff Ghost Form gives their opponents the ability to walk past walls.
        /// </summary>
        [Test()]
        public void TestGhostFormChild() {
            Player itemPicker = new Player(PlayerType.PlayerOne);
            Player testPlayerTwo = new Player(PlayerType.PlayerTwo);
            (Row row, Column col) tempCoordinates = (Row.R0, Column.C0);
            GhostForm gfChild = new GhostForm(tempCoordinates);

            // Before applying Ghost Form to test players
            Assert.IsFalse(itemPicker.CanPlayerPassThroughWalls);
            Assert.IsFalse(testPlayerTwo.CanPlayerPassThroughWalls);

            // Testing Buff variation of Ghost Form
            gfChild.PowerUpType = PowerUpType.Buff;
            gfChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.IsTrue(itemPicker.CanPlayerPassThroughWalls);
            Assert.IsFalse(testPlayerTwo.CanPlayerPassThroughWalls);

            itemPicker.ResetPlayerGhostState();
            testPlayerTwo.ResetPlayerGhostState();

            // Testing Debuff variation of Ghost Form
            gfChild.PowerUpType = PowerUpType.Debuff;
            gfChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.IsFalse(itemPicker.CanPlayerPassThroughWalls);
            Assert.IsTrue(testPlayerTwo.CanPlayerPassThroughWalls);
        }

        /// <summary>
        /// TimeStop will set a player's move to 0, consequently skipping the affected players turn
        /// Buff TimeStop will set the opponent's moves to 0.
        /// Debuff Timestop will set the item picker's number of moves to 0.
        /// </summary>
        [Test()]
        public void TestTimeStopChild() {
            Player itemPicker = new Player(PlayerType.PlayerOne);
            Player testPlayerTwo = new Player(PlayerType.PlayerTwo);
            (Row row, Column col) tempCoordinates = (Row.R0, Column.C0);
            TimeStop tsChild = new TimeStop(tempCoordinates);

            // Before applying Time Stop to test players
            Assert.AreEqual(1, itemPicker.NumberOfMoves);
            Assert.AreEqual(1, testPlayerTwo.NumberOfMoves);
            Assert.IsFalse(itemPicker.WasAffectedByTimeStop);
            Assert.IsFalse(testPlayerTwo.WasAffectedByTimeStop);

            // Testing Buff variation of Time Stop
            tsChild.PowerUpType = PowerUpType.Buff;
            tsChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.AreEqual(1, itemPicker.NumberOfMoves);
            Assert.AreEqual(0, testPlayerTwo.NumberOfMoves);
            Assert.IsFalse(itemPicker.WasAffectedByTimeStop);
            Assert.IsTrue(testPlayerTwo.WasAffectedByTimeStop);

            itemPicker.ResetPlayerMoves();
            testPlayerTwo.ResetPlayerMoves();
            itemPicker.WasAffectedByTimeStop = false;
            testPlayerTwo.WasAffectedByTimeStop = false;

            // Testing Debuff variation of Time Stop
            tsChild.PowerUpType = PowerUpType.Debuff;
            tsChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.AreEqual(1, testPlayerTwo.NumberOfMoves);
            Assert.AreEqual(0, itemPicker.NumberOfMoves);
            Assert.IsTrue(itemPicker.WasAffectedByTimeStop);
            Assert.IsFalse(testPlayerTwo.WasAffectedByTimeStop);
        }

        /// <summary>
        /// Normal State is a placeholder state to indicate a Null Status for a player's Abnormal Status
        /// Applying it on a player regardless of Buff or Debuff variation will not affect the player.
        /// </summary>
        [Test()]
        public void TestNormalStateChild() {
            Player itemPicker = new Player(PlayerType.PlayerOne);
            Player testPlayerTwo = new Player(PlayerType.PlayerTwo);
            (Row row, Column col) tempCoordinates = (Row.R0, Column.C0);
            NormalState nsChild = new NormalState(tempCoordinates);

            // Before applying Normal State to test players
            Assert.AreEqual(1, itemPicker.NumberOfMoves);
            Assert.AreEqual(1, testPlayerTwo.NumberOfMoves);

            // Testing Buff variation of Normal State
            nsChild.PowerUpType = PowerUpType.Buff;
            nsChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.AreEqual(1, itemPicker.NumberOfMoves);
            Assert.AreEqual(1, testPlayerTwo.NumberOfMoves);

            itemPicker.ResetPlayerMoves();
            testPlayerTwo.ResetPlayerMoves();

            // Testing Debuff variation of Normal State
            nsChild.PowerUpType = PowerUpType.Debuff;
            nsChild.ApplyOnPlayer(itemPicker, testPlayerTwo);
            Assert.AreEqual(1, testPlayerTwo.NumberOfMoves);
            Assert.AreEqual(1, itemPicker.NumberOfMoves);
        }
    }
}