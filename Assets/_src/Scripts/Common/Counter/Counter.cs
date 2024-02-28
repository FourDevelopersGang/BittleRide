using System;


namespace _src.Scripts.Common.Counter
{
	public class Counter
	{
		private int _currentValue;

		private readonly int _target;

		public event Action<int> CounterValueChanged;

		public event Action ReachedTarget;

		public bool IsCancelled => _currentValue >= _target;

		public Counter(int target)
		{
			_target = target;
			_currentValue = 0;
		}
		
		public void Add()
		{
			if (_currentValue == _target)
				return;
			
			++_currentValue;
			CounterValueChanged?.Invoke(_currentValue);
			
			if(_currentValue == _target)
				ReachedTarget?.Invoke();
		}
		
		public CounterInfoProvider GetInfoProvider() => new(this);

		public class CounterInfoProvider
		{
			private readonly Counter _counter;

			public CounterInfoProvider(Counter counter)
			{
				_counter = counter;
			}

			public int Target => _counter._target;

			public int CurrentValue => _counter._currentValue;
			
			public bool IsCancelled => _counter.IsCancelled;

			public void AddListenerChangedCurrentValue(Action<int> action)
			{
				_counter.CounterValueChanged += action;
			}
			
			public void RemoveListenerChangedCurrentValue(Action<int> action)
			{
				_counter.CounterValueChanged -= action;
			}
			
			public void AddListenerReachedTarget(Action action)
			{
				_counter.ReachedTarget += action;
			}
			
			public void RemoveListenerReachedTarget(Action action)
			{
				_counter.ReachedTarget -= action;
			}
		}
	}
}
