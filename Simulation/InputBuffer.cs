using System.Collections.Generic;

namespace NetworkPractice
{
    public class InputBuffer
    {
        private readonly List<(int Tick, PlayerInput Input)> _buffer;
        private readonly int _delayTicks;

        public InputBuffer(int delayTicks = 3)
        {
            _buffer = new List<(int Tick, PlayerInput Input)>();
            _delayTicks = delayTicks;
        }

        public void AddInput(int currentTick, PlayerInput input)
        {
            _buffer.Add((currentTick, input));
        }

        public PlayerInput[] GetReadyInputs(int currentTick)
        {
            List<PlayerInput> ready = new List<PlayerInput>();

            for (int i = _buffer.Count - 1; i >= 0; i--)
            {
                if (currentTick - _buffer[i].Tick >= _delayTicks)
                {
                    ready.Add(_buffer[i].Input);
                    _buffer.RemoveAt(i);
                }
            }

            return ready.ToArray();
        }
    }
}