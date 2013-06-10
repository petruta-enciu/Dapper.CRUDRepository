using System;

namespace CRUDRepository.Repository
{
    /// <summary>
    /// Validation extensions
    /// </summary>
    public static class ArgumentsExtensions
    {
        /// <summary>
        /// Determines whether an argument is null 
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="value">
        /// Instance if type T
        /// </param>
        /// Is checked object
        /// <param name="argument">
        /// When value is null validation will throw an ArgumentNullException
        /// </param>
        /// <param name="message">
        /// Exception message
        /// </param>
        public static void ThrowIfArgumentIsNull<T>(this T value, string message = "") where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Determines whether an string is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">
        /// Instance if type T
        /// </param>
        /// Is checked string
        /// <param name="argument">
        /// When string argument is null or empty validation will throw an ArgumentNullException
        /// </param>
        /// <param name="argument">
        /// When string argument is null or empty validation will throw an ArgumentNullException
        /// </param>
        /// <param name="message">
        /// Exception message
        /// </param>
        public static void ThrowIfStringIsNullOrEmpty<T>(this T value, string argument, string message = "") where T : class
        {
            if (String.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Determines whether an string is null or contains just white spaces
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">
        /// Instance if type T
        /// </param>
        /// <param name="argument">
        /// When string argument is null or contains just white spaces validation will throw an ArgumentNullException
        /// </param>
        /// <param name="message">
        /// Exception message
        /// </param>
        public static void ThrowIfStringIsNullOrWhiteSpace<T>(this T value, string argument, string message = "") where T : class
        {
            if (String.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(message);
            }

        }
    }
}
