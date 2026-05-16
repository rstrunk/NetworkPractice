using Microsoft.Xna.Framework;

namespace AudioGameLearning
{
    public class AudioCalculator
    {
        private float _maxHearingDistance;
        public AudioCalculator(float maxHearingDistance)
        {
            _maxHearingDistance = maxHearingDistance;
        }

        public float CalculateVolume(Vector2 listenerPosition, Vector2 sourcePosition)
        {
            float distance = Vector2.Distance(listenerPosition, sourcePosition);
            if (distance >= _maxHearingDistance)
            {
                return 0f;
            }
            return 1f - (distance / _maxHearingDistance);
        }

        public float CalculatePan(Vector2 listenerPosition, Vector2 sourcePosition)
        {
            float horizontalDifference = sourcePosition.X - listenerPosition.X;
            float pan = horizontalDifference / _maxHearingDistance;
            return MathHelper.Clamp(pan, -1f, 1f);
        }

        public float CalculatePitch(Vector2 listenerPosition, Vector2 sourcePosition)
        {
            float distance = Vector2.Distance(listenerPosition, sourcePosition);
            float normalizedDistance = distance / _maxHearingDistance;
            return MathHelper.Clamp(-normalizedDistance * 0.5f, -1f, 0f);
        }

        public float CalculateUrgencyPitch(Vector2 listenerPosition, Vector2 sourcePosition)
        {
            float distance = Vector2.Distance(listenerPosition, sourcePosition);
            float normalizedDistance = distance / _maxHearingDistance;
            return MathHelper.Clamp((1f - normalizedDistance) * 0.5f, 0f, 0.5f);
        }
    }
}