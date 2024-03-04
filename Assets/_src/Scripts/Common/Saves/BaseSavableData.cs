using System;
using JetBrains.Annotations;


namespace _src.Scripts.Common.Saves
{
	public class BaseSavableData
	{
		public event Action OnDataChanged;

		public bool WasChanged { get; private set; }

		public void MarkDataAsChanged()
		{
			WasChanged = true;
			OnDataChanged?.Invoke();
		}

		public void UnmarkDataAsChanged()
		{
			WasChanged = false;
		}

		public void ChangeField<T>([NotNull] ref T field, T value)
		{
			if (field == null)
				throw new ArgumentNullException(nameof(field));

			field = value;
			MarkDataAsChanged();
		}

	}
}
