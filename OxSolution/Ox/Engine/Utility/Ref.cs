namespace Ox.Engine.Utility
{
    /// <summary>
    /// Enables a value type to be passed around by reference.
    /// </summary>
    public class Ref<T>
    {
        /// <summary>
        /// Create a Ref.
        /// </summary>
        /// <param name="value">See property Value.</param>
        public Ref(T value)
        {
            this.value_ = value;
        }

        /// <summary>
        /// Create a Ref.
        /// </summary>
        /// <param name="value">See property Value.</param>
        public Ref(ref T value)
        {
            this.value_ = value;
        }

        /// <summary>
        /// Create a Ref.
        /// </summary>
        public Ref() { }

        /// <summary>
        /// The underlying value.
        /// </summary>
        public T Value
        {
            get { return value_; }
            set { value_ = value; }
        }

        /// <summary>
        /// Set the underlying value.
        /// </summary>
        public void Set(ref T value)
        {
            this.value_ = value;
        }

        /// <summary>
        /// Get the underlying value.
        /// </summary>
        public void Get(out T value)
        {
            value = this.value_;
        }

        private T value_;
    }
}
