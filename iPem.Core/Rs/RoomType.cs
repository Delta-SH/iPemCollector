using System;

namespace iPem.Core {
    /// <summary>
    /// Represents an room type
    /// </summary>
    [Serializable]
    public partial class RoomType {
        /// <summary>
        ///Gets or sets the identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the comment
        /// </summary>
        public string Comment { get; set; }
    }
}
