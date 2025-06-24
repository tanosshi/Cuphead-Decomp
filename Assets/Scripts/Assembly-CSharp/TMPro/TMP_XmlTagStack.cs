namespace TMPro
{
	public struct TMP_XmlTagStack<T>
	{
		public T[] itemStack;

		public int index;

		public TMP_XmlTagStack(T[] tagStack)
		{
			itemStack = tagStack;
			index = 0;
		}

		public void Clear()
		{
			index = 0;
		}

		public void SetDefault(T item)
		{
			itemStack[0] = item;
			index = 1;
		}

		public void Add(T item)
		{
			if (index < itemStack.Length)
			{
				itemStack[index] = item;
				index++;
			}
		}

		public T Remove()
		{
			index--;
			if (index <= 0)
			{
				index = 0;
				return itemStack[0];
			}
			return itemStack[index - 1];
		}

		public T CurrentItem()
		{
			return itemStack[index - 1];
		}
	}
}
