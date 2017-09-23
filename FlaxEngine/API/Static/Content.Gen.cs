////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
	/// <summary>
	/// Loads and manages asset objects.
	/// </summary>
	public static partial class Content
	{
		/// <summary>
		/// Loads asset to the Content Pool and holds it until it won't be referenced by any object. Returns null if asset was not created (see log for error info).
		/// </summary>
		/// <param name="id">Asset unique ID.</param>
		/// <typeparam name="T">Type of the asset to load. Includes any asset types derived from the type.</typeparam>
		/// <returns>Asset instance if loaded, null otherwise.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static T LoadAsync<T>(Guid id) where T : Asset
		{
#if UNIT_TEST_COMPILANT
			throw new FlaxTestCompilantNotImplementedException();
#else
			return (T)Internal_LoadAsync1(ref id, typeof(T));
#endif
		}

		/// <summary>
		/// Loads asset to the Content Pool and holds it until it won't be referenced by any object. Returns null if asset was not created (see log for error info).
		/// </summary>
		/// <param name="path">Path to the asset.</param>
		/// <typeparam name="T">Type of the asset to load. Includes any asset types derived from the type.</typeparam>
		/// <returns>Asset instance if loaded, null otherwise.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static T LoadAsync<T>(string path) where T : Asset
		{
#if UNIT_TEST_COMPILANT
			throw new FlaxTestCompilantNotImplementedException();
#else
			return (T)Internal_LoadAsync2(path, typeof(T));
#endif
		}

		/// <summary>
		/// Loads asset to the Content Pool and holds it until it won't be referenced by any object. Returns null if asset was not created (see log for error info).
		/// </summary>
		/// <param name="internalPath">Intenral path to the asset. Relative to the Engine startup folder and without an asset file extension.</param>
		/// <typeparam name="T">Type of the asset to load. Includes any asset types derived from the type.</typeparam>
		/// <returns>Asset instance if loaded, null otherwise.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static T LoadAsyncInternal<T>(string internalPath) where T : Asset
		{
#if UNIT_TEST_COMPILANT
			throw new FlaxTestCompilantNotImplementedException();
#else
			return (T)Internal_LoadAsync3(internalPath, typeof(T));
#endif
		}

		/// <summary>
		/// Renames the asset. Handles situation when asset is being loaded or storage file locked. Available only in editor.
		/// </summary>
		/// <param name="oldPath">The asset path to rename.</param>
		/// <param name="newPath">The new asset path to set.</param>
		/// <returns>True if failed, otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool RenameAsset(string oldPath, string newPath) 
		{
#if UNIT_TEST_COMPILANT
			throw new FlaxTestCompilantNotImplementedException();
#else
			return Internal_RenameAsset(oldPath, newPath);
#endif
		}

		/// <summary>
		/// Removes asset in a safe way. Available only in editor.
		/// </summary>
		/// <param name="path">The asset path.</param>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void DeleteAsset(string path) 
		{
#if UNIT_TEST_COMPILANT
			throw new FlaxTestCompilantNotImplementedException();
#else
			Internal_DeleteAsset(path);
#endif
		}

		/// <summary>
		/// Gets the asset from the Content Pool if it has been loaded.
		/// </summary>
		/// <param name="id">Asset unique ID.</param>
		/// <typeparam name="T">Type of the asset to load. Includes any asset types derived from the type.</typeparam>
		/// <returns>Asset instance if loaded, null otherwise.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static T GetAsset<T>(Guid id) where T : Asset
		{
#if UNIT_TEST_COMPILANT
			throw new FlaxTestCompilantNotImplementedException();
#else
			return (T)Internal_GetAsset1(ref id, typeof(T));
#endif
		}

		/// <summary>
		/// Gets the asset from the Content Pool if it has been loaded.
		/// </summary>
		/// <param name="path">Path to the asset.</param>
		/// <typeparam name="T">Type of the asset to load. Includes any asset types derived from the type.</typeparam>
		/// <returns>Asset instance if loaded, null otherwise.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static T GetAsset<T>(string path) where T : Asset
		{
#if UNIT_TEST_COMPILANT
			throw new FlaxTestCompilantNotImplementedException();
#else
			return (T)Internal_GetAsset2(path, typeof(T));
#endif
		}

		/// <summary>
		/// Gets the amount of created asset objects.
		/// </summary>
		[UnmanagedCall]
		public static int AssetsCount
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetAssetsCount(); }
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Asset Internal_LoadAsync1(ref Guid id, Type type);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Asset Internal_LoadAsync2(string path, Type type);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Asset Internal_LoadAsync3(string internalPath, Type type);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_RenameAsset(string oldPath, string newPath);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_DeleteAsset(string path);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Asset Internal_GetAsset1(ref Guid id, Type type);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Asset Internal_GetAsset2(string path, Type type);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetAssetsCount();
#endif
#endregion
	}
}

