
namespace SandSimulator.Util
{
	internal class SmoothFramerate
	{
		int _sampleCount;
		int _currentFrame;
		double[] _frametimes;
		double _currentFrametimes;

		public double Framerate
		{
			get
			{
				return (_sampleCount / _currentFrametimes);
			}
		}

		public SmoothFramerate(int sampleCount)
		{
			_sampleCount = sampleCount;
			_currentFrame = 0;
			_frametimes = new double[_sampleCount];
			//Array.Fill(_frametimes, 1.0 / 60.0);
			//_currentFrametimes = sampleCount / 60.0;
		}

		public void Update(double timeSinceLastFrame)
		{
			_currentFrame++;
			if (_currentFrame >= _frametimes.Length) { _currentFrame = 0; }

			_currentFrametimes -= _frametimes[_currentFrame];
			_frametimes[_currentFrame] = timeSinceLastFrame;
			_currentFrametimes += _frametimes[_currentFrame];
		}
	}
}
