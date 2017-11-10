////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using FlaxEngine;

namespace FlaxEditor.Content.Settings
{
    /// <summary>
    /// The game building settings container. Allows to edit asset via editor.
    /// </summary>
    public sealed class BuildSettings : SettingsBase
    {
        /// <summary>
        /// The maximum amount of assets to include into a single assets package. Assets will be spli into several packages if need to.
        /// </summary>
        [EditorOrder(10), Limit(32, short.MaxValue), EditorDisplay("General", "Max assets per package"), Tooltip("The maximum amount of assets to include into a single assets package. Assets will be spli into several packages if need to.")]
        public int MaxAssetsPerPackage = 256;

        /// <summary>
        /// The maximum size of the single assets package (in megabytes). Assets will be spli into several packages if need to.
        /// </summary>
        [EditorOrder(20), Limit(16, short.MaxValue), EditorDisplay("General", "Max package size (in MB)"), Tooltip("The maximum size of the single assets package (in megabytes). Assets will be spli into several packages if need to.")]
        public int MaxPackageSizeMB = 256;

        /// <summary>
        /// The game content cooking keycode. Use the same value for a game and DLC packages to support loading them by the builded game. Use 0 to randomize it during building.
        /// </summary>
        [EditorOrder(30), EditorDisplay("General"), Tooltip("The game content cooking keycode. Use the same value for a game and DLC packages to support loading them by the builded game. Use 0 to randomize it during building.")]
        public int ContentKey = 0;

        // TODO: add build presets and custom targets configuration
    }
}
