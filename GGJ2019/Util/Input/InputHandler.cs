using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace GGJ2019.Util.Input
{
    public class InputHandler
    {
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;
        VirtualButton _jumpButton;
        VirtualButton _shootButton;

        Vector2 _previousAxialInput;
        Vector2 _axialInput; //utility vec2 to hold input values without constantly creating/destroying vec2s
        public int gamepadIndex = 0;
        public InputMapping mapping;
        public Vector2 axialInput
        {
            get
            {
                _axialInput.X = _xAxisInput.value;
                _axialInput.Y = _yAxisInput.value;
                return _axialInput;
            }
        }

        public float XInput => _xAxisInput.value;

        public float YInput => _yAxisInput.value;

        public VirtualButton JumpButtonInput
        {
            get
            {
                return _jumpButton;
            }
        }
        public VirtualButton ShootButtonInput
        {
            get
            {
                return _shootButton;
            }
        }

        public bool AnyButtonPressed => JumpButtonInput.isPressed || ShootButtonInput.isPressed;

        public InputHandler(int index)
        {
            this.gamepadIndex = index;
            using (StreamReader reader = new StreamReader("input.json"))
            {
                string json = reader.ReadToEnd();
                mapping = JsonConvert.DeserializeObject<List<InputMapping>>(json).Single(m => m.index == index);
            }
            SetupInput();
        }

        public InputHandler(InputMapping mapping)
        {
            gamepadIndex = mapping.index;
            SetupInput();
        }

        /// <summary>
        /// Needs a better way to bind keys, just hard bind for now
        /// </summary>
        public void SetupInput()
        {
            _axialInput = Vector2.Zero;
            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadDpadLeftRight(gamepadIndex));
            _xAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadLeftStickX(gamepadIndex));
            for (int i = 0; i < mapping.Left.Length; i++)
            {
                _xAxisInput.nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, (Keys)mapping.Left[i], (Keys)mapping.Right[i]));
            }

            // vertical input from dpad, left stick or keyboard up/down
            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadDpadUpDown(gamepadIndex));
            _yAxisInput.nodes.Add(new Nez.VirtualAxis.GamePadLeftStickY(gamepadIndex));
            for (int i = 0; i < mapping.Up.Length; i++)
            {
                _yAxisInput.nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, (Keys)mapping.Up[i], (Keys)mapping.Down[i]));
            }

            //action buttons
            _jumpButton = new VirtualButton();
            foreach (var key in mapping.jumpKey)
            {
                _jumpButton.nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.jumpButton)
            {
                _jumpButton.nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }


            _shootButton = new VirtualButton();
            foreach (var key in mapping.shootKey)
            {
                _shootButton.nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.shootButton)
            {
                _shootButton.nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }
        }
    }
}
