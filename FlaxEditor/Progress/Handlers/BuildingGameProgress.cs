////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;

namespace FlaxEditor.Progress.Handlers
{
    /// <summary>
    /// Game building progress reporting handler.
    /// </summary>
    /// <seealso cref="FlaxEditor.Progress.ProgressHandler" />
    public sealed class BuildingGameProgress : ProgressHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingGameProgress"/> class.
        /// </summary>
        public BuildingGameProgress()
        {
            GameCooker.OnEvent += OnGameCookerEvent;
        }

        private void OnGameCookerEvent(GameCooker.EventType eventType)
        {
            switch (eventType)
            {
                case GameCooker.EventType.BuildStarted:
                    OnStart();
                    OnUpdate(0, "Building gane...");
                    break;
                case GameCooker.EventType.BuildFailed:
                    OnEnd();
                    break;
                case GameCooker.EventType.BuildDone:
                    OnEnd();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }
    }
}