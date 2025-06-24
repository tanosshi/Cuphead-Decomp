namespace RektTransform
{
	public static class Anchors
	{
		public static readonly MinMax TopLeft = new MinMax(0f, 1f, 0f, 1f);

		public static readonly MinMax TopCenter = new MinMax(0.5f, 1f, 0.5f, 1f);

		public static readonly MinMax TopRight = new MinMax(1f, 1f, 1f, 1f);

		public static readonly MinMax TopStretch = new MinMax(0f, 1f, 1f, 1f);

		public static readonly MinMax MiddleLeft = new MinMax(0f, 0.5f, 0f, 0.5f);

		public static readonly MinMax TrueCenter = new MinMax(0.5f, 0.5f, 0.5f, 0.5f);

		public static readonly MinMax MiddleCenter = new MinMax(0.5f, 0.5f, 0.5f, 0.5f);

		public static readonly MinMax MiddleRight = new MinMax(1f, 0.5f, 1f, 0.5f);

		public static readonly MinMax MiddleStretch = new MinMax(0f, 0.5f, 1f, 0.5f);

		public static readonly MinMax BottomLeft = new MinMax(0f, 0f, 0f, 0f);

		public static readonly MinMax BottomCenter = new MinMax(0.5f, 0f, 0.5f, 0f);

		public static readonly MinMax BottomRight = new MinMax(1f, 0f, 1f, 0f);

		public static readonly MinMax BottomStretch = new MinMax(0f, 0f, 1f, 0f);

		public static readonly MinMax StretchLeft = new MinMax(0f, 0f, 0f, 1f);

		public static readonly MinMax StretchCenter = new MinMax(0.5f, 0f, 0.5f, 1f);

		public static readonly MinMax StretchRight = new MinMax(1f, 0f, 1f, 1f);

		public static readonly MinMax TrueStretch = new MinMax(0f, 0f, 1f, 1f);

		public static readonly MinMax StretchStretch = new MinMax(0f, 0f, 1f, 1f);
	}
}
