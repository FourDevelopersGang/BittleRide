using System;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace _src.Scripts.GameEventsSystem
{
	public abstract class BaseGameEvent
	{
		public event Action OnComplete;

		public event Action OnFail;

		public virtual async UniTaskVoid Start(CancellationTokenSource stopCancellationTokenSource)
		{
		}

		protected virtual void RaiseOnComplete()
		{
			OnComplete?.Invoke();
		}

		protected virtual void RaiseOnFail()
		{
			OnFail?.Invoke();
		}
	}
}
