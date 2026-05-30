using System.Collections.Generic;

namespace NetworkPractice
{
    public class InputBuffer
    {
        private readonly List<(int Tick, ControllerInput Input)> _buffer;
        private readonly int _delayTicks;

        public InputBuffer(int delayTicks = 3)
        {
            _buffer = new List<(int Tick, ControllerInput Input)>();
            _delayTicks = delayTicks;
        }

        public void AddInput(int currentTick, ControllerInput input)
        {
            _buffer.Add((currentTick, input));
        }

        public ControllerInput[] GetReadyInputs(int currentTick)
        {
            List<ControllerInput> ready = new List<ControllerInput>();

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
