namespace VFX
{
	public static class StringUtility
	{
		/// <summary>
		/// 例：
		///		E:\qwe\sdf\wts.gwe 返回wts
		/// </summary>
		/// <param name="path">完整路径</param>
		/// <returns>文件名(不包括扩展名)</returns>
		public static string SubFileNameFromPath(string path)
		{
			int index = path.LastIndexOfAny(new char[] { '\\', '/' });
			if (index > 0)
			{
				path = path.Substring(index + 1);
			}
			index = path.IndexOf('.');
			if (index > 0)
			{
				path = path.Substring(0, index);
			}
			return path;
		}
	}
}