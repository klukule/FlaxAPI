﻿////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;
using FlaxEngine.Rendering;

namespace FlaxEngine.GUI
{
    /// <summary>
    /// A common control used to present rendered frame in the UI.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.ContainerControl" />
    public class RenderOutputControl : ContainerControl
    {
        /// <summary>
        /// The default back buffer format used by the GUI controls presenting rendered frames.
        /// </summary>
        public const PixelFormat DefaultBackBufferFormat = PixelFormat.R8G8B8A8_UNorm;

        protected SceneRenderTask _task;

        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <value>
        /// The task.
        /// </value>
        public SceneRenderTask Task => _task;

        /// <summary>
        /// Gets a value indicating whether render to that output only if parent window exists, otherwise false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if render only with window attached; otherwise, <c>false</c>.
        /// </value>
        public bool RenderOnlyWithWindow { get; }

        /// <summary>
        /// The output buffer.
        /// </summary>
        public readonly RenderTarget BackBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderOutputControl"/> class.
        /// </summary>
        /// <param name="task">The task. Cannot be null.</param>
        /// <param name="renderOnlyWithWindow">True if render to that output only if parent window exists, otherwise false.</param>
        /// <exception cref="System.ArgumentNullException">Invalid task.</exception>
        public RenderOutputControl(SceneRenderTask task, bool renderOnlyWithWindow = true)
            : base(true)
        {
            if (task == null)
                throw new ArgumentNullException();
            
            RenderOnlyWithWindow = renderOnlyWithWindow;
            BackBuffer = RenderTarget.Create();
            _task = task;
            _task.Output = BackBuffer;
        }

        /// <inheritdoc />
        public override void Draw()
        {
            // Draw backbuffer texture
            Render2D.DrawRenderTarget(BackBuffer, new Rectangle(Vector2.Zero, Size), Color.White);

            base.Draw();
        }

        /// <inheritdoc />
        protected override void SetSizeInternal(Vector2 size)
        {
            base.SetSizeInternal(size);

            SyncBackBufferSize();
        }

        /// <inheritdoc />
        protected override void PerformLayoutSelf()
        {
            base.PerformLayoutSelf();

            // Disable task rendering if control is not used in a window (has issing ParentWindow)
            if (RenderOnlyWithWindow)
            {
                Task.Enabled = ParentWindow != null;
            }

            SyncBackBufferSize();
        }

        /// <summary>
        /// Synchronizes size of the back buffer.
        /// </summary>
        protected void SyncBackBufferSize()
        {
            int width = (int)Width;
            int height = (int)Height;
            if (width >= 1 && height >= 1)
            {
                BackBuffer.Init(DefaultBackBufferFormat, width, height);
            }
            else
            {
                BackBuffer.Dispose();
            }
        }
    }
}
