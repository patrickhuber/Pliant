namespace Pliant.Runtime
{
    public interface IParseRunner
    {
        /// <summary>
        /// Reads a single unit from the token stream. For character based parses, this is a single character.
        /// </summary>
        /// <returns></returns>
        bool Read();

        /// <summary>
        /// Gets the current unit position within the parse. For character based parses, this is the character position.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// The Line number of the current position
        /// </summary>
        int Line { get; }

        /// <summary>
        /// The current column is the current position within the current line
        /// </summary>
        int Column { get; }

        /// <summary>
        /// Gets the current <see cref="IParseEngine">parse engine</see> used by the <see cref="IParseRunner">Pase Interface</see>
        /// </summary>
        IParseEngine ParseEngine { get; }

        /// <summary>
        /// Determines if the end of the input stream has been reached.
        /// </summary>
        /// <returns>true if the end of stream has been reached, false otherwise.</returns>
        bool EndOfStream();

        /// <summary>
        /// Runs the parse runner to the end of stream or until an error is encountered.
        /// </summary>
        /// <returns>true if end of stream, false if error is encountered.</returns>
        bool RunToEnd();
    }
}