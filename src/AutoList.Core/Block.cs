using System;

namespace AutoList.Core
{
        public class Block
        {
            /// <summary>
            ///     Values backend of the Block type
            /// </summary>
            private readonly double[] _values;

            public Block(string id, double frontage = 0, double area = 0)
            {
                Id = id;
                _values = new[] {frontage, area};
            }

            /// <summary>
            ///     The ID/name of the block
            /// </summary>
            public string Id { get; }

            /// <summary>
            ///     The Frontage of the block
            /// </summary>
            public double Frontage => _values[0];

            /// <summary>
            ///     The Area of the block
            /// </summary>
            public double Area => _values[1];
        }
}