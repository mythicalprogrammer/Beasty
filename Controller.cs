using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Beasty.Units;

namespace Beasty
{
    public enum ControllerState
    {
        Normal, Exit
    }

    class Controller
    {
        private bool _paused = false;
        private bool _disconnected = false;
        private bool usingGamePad = false;
        private PlayerIndex player;
        private GamePadState currentState, lastState;

        private BaseUnit unit;

        public bool paused
        {
            get { return _paused; }
        }
        public bool disconnected
        {
            get { return _disconnected; }
        }

        public Controller(PlayerIndex player, BaseUnit unit)
        {
            this.unit = unit;
            this.player = player;
            currentState = GamePad.GetState(this.player);
            if (currentState.IsConnected == true) { usingGamePad = true; }
        }

        internal ControllerState Update(GameTime time)
        {
            lastState = currentState;
            currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected == false) { _disconnected = true; }
            else if (currentState.IsConnected == true) { _disconnected = false; }

            // Allows the game to exit
            if (currentState.Buttons.Back == ButtonState.Pressed)
                return ControllerState.Exit;
            if (currentState.Buttons.Start == ButtonState.Pressed && lastState.Buttons.Start == ButtonState.Released)
                _paused = (_paused) ? false : true;

            if (!_paused)
            {
                Vector2 moveUnit = new Vector2(currentState.ThumbSticks.Left.X,
                    currentState.ThumbSticks.Left.Y);
                if (moveUnit.Length() > 0.1f)
                {
                    Console.Out.WriteLine(moveUnit.ToString());
                    unit.MoveUnit(moveUnit);
                }

                unit.MoveAimer(new Vector2(currentState.ThumbSticks.Right.X,
                    currentState.ThumbSticks.Right.Y));
            }

            if (currentState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                unit.Fire();
            }

            return ControllerState.Normal;
        }
    }
}
