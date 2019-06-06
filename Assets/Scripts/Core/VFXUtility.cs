namespace VFX
{
	public static class VFXUtility
	{
		private static System.Diagnostics.Stopwatch ms_Stopwatch;

		static VFXUtility()
		{
			ms_Stopwatch = new System.Diagnostics.Stopwatch();
			ms_Stopwatch.Start();
		}

		public static long GetElapsedMilliseconds()
		{
			return ms_Stopwatch.ElapsedMilliseconds;
		}
	}
}