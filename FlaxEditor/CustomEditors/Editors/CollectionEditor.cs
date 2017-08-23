////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using FlaxEditor.CustomEditors.Elements;

namespace FlaxEditor.CustomEditors.Editors
{
    /// <summary>
    /// Default implementation of the inspector used to edit arrays/list.
    /// </summary>
    public abstract class CollectionEditor : CustomEditor
    {
        private IntegerValueElement _size;
        private int _elementsCount;

        /// <summary>
        /// Gets the length of the collection.
        /// </summary>
        public abstract int Count { get; }

        /// <inheritdoc />
        public override void Initialize(LayoutElementsContainer layout)
        {
            // No support for different colelctions for now
            if (HasDiffrentValues || HasDiffrentTypes)
                return;

            var type = Values.Type;
            var size = Count;

            // Size
            _size = layout.IntegerValue("Size");
            _size.IntValue.MinValue = 0;
            _size.IntValue.MaxValue = ushort.MaxValue;
            _size.IntValue.Value = size;
            _size.IntValue.ValueChanged += OnSizeChanged;

            // Elements
            if (size > 0)
            {
                var elementType = type.IsGenericType ? type.GetGenericArguments()[0] : type.GetElementType();
                for (int i = 0; i < size; i++)
                {
                    layout.Object("Element " + i, new ListValueContainer(elementType, i, Values));
                }
            }
            _elementsCount = size;
        }

        private void OnSizeChanged()
        {
            if (IsSetBlocked)
                return;

            Resize(_size.IntValue.Value);
        }

        /// <summary>
        /// Resizes collection to the specified new size.
        /// </summary>
        /// <param name="newSize">The new size.</param>
        protected abstract void Resize(int newSize);
        
        /// <inheritdoc />
        public override void Refresh()
        {
            // Check if collection has been resized (by UI or from external source)
            if (Count != _elementsCount)
            {
                RebuildLayout();
            }
        }
    }
}