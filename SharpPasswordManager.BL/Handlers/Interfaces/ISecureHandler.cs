﻿namespace SharpPasswordManager.BL.Handlers
{
	/// <summary>
	/// Gain access to entered by user password which is used as cryptography key and calculates starting index for data storage.
	/// </summary>
	public interface ISecureHandler
    {
		/// <summary>
		/// Key of config password value.
		/// </summary>
		public string PasswordKey { get; }

		/// <summary>
		/// File name with main data info.
		/// </summary>
		public string DataFileName { get; } 

		/// <summary>
		/// File name with categories info.
		/// </summary>
		public string CategoriesFileName { get; }

		/// <summary>
		/// Current secret app key.
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Returns data cell index, whith given the secure key shift.
		/// </summary>
		/// <param name="i">Needed data index.</param>
		public int GetIndexOf(int i);
	}
}
