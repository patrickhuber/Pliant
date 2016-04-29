namespace Pliant
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
        /// Gets the current <see cref="IParseEngine">parse engine</see> used by the <see cref="IParseRunner">Pase Interface</see>
        /// </summary>
        IParseEngine ParseEngine { get; }

        /// <summary>
        /// Determines if the end of the input stream has been reached.
        /// </summary>
        /// <returns>true if the end of stream has been reached, false otherwise.</returns>
        bool EndOfStream();
    }
}