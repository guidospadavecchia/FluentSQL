using System.Data;

namespace FluentSQL.Core
{
    /// <summary>
    /// Represents an output parameter for a Stored Procedure
    /// </summary>
    public sealed class OutputParameter
    {
        /// <summary>
        /// Name of the output parameter.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Data type of the output parameter.
        /// </summary>
        public DbType? Type { get; private set; }
        /// <summary>
        /// Size of the output parameter.
        /// </summary>
        public int? Size { get; private set; }
        /// <summary>
        /// Numeric precision of the output parameter.
        /// </summary>
        public byte? Precision { get; private set; }
        /// <summary>
        /// Numeric scale of the output parameter.
        /// </summary>
        public byte? Scale { get; private set; }

        /// <summary>
        /// Creates a new output parameter.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        public OutputParameter(string name, DbType type)
        {
            Name = name.StartsWith("@") ? name[1..] : name;
            Type = type;
        }

        /// <summary>
        /// Creates a new output parameter.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="size">Size of the output parameter. Must be specified if <paramref name="type"/> is any variant of <see cref="DbType.String"/> or <see cref="DbType.Binary"/>.</param>
        public OutputParameter(string name, DbType type, int size)
        {
            Name = name.StartsWith("@") ? name[1..] : name;
            Type = type;
            Size = size;
        }

        /// <summary>
        /// Creates a new output parameter.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="precision">Numeric precision of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        /// <param name="scale">Numeric scale of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        public OutputParameter(string name, DbType type, byte precision, byte scale)
        {
            Name = name.StartsWith("@") ? name[1..] : name;
            Type = type;
            Precision = precision;
            Scale = scale;
        }

        /// <summary>
        /// Creates a new output parameter.
        /// </summary>
        /// <param name="name">Name of the output parameter.</param>
        /// <param name="type">Data type of the output parameter.</param>
        /// <param name="size">Size of the output parameter. This parameter must be specified if <paramref name="type"/> is any variant of <see cref="DbType.String"/> or <see cref="DbType.Binary"/>.</param>
        /// <param name="precision">Numeric precision of the output parameter. Must be specified when the parameter is any kind of number with decimals.</param>
        /// <param name="scale">Escala numérica del parámetro. Must be specified when the parameter is any kind of number with decimals.</param>
        public OutputParameter(string name, DbType type, int? size = null, byte? precision = null, byte? scale = null)
        {
            Name = name.StartsWith("@") ? name[1..] : name;
            Type = type;
            Size = size;
            Precision = precision;
            Scale = scale;
        }
    }
}
