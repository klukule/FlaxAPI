////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using FlaxEngine;

namespace FlaxEditor.SceneGraph.Actors
{
    /// <summary>
    /// Scene tree node for <see cref="Camera"/> actor type.
    /// </summary>
    /// <seealso cref="ActorNode" />
    public sealed class CameraNode : ActorNode
    {
        /// <inheritdoc />
        public CameraNode(Actor actor)
            : base(actor)
        {
        }
    }
}
