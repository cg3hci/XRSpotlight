using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EcaRules
{
    /// <summary>
    /// <b>Artwork</b> is an <see cref="EcaEnvironment"/> subclass that represents an artwork.
    /// </summary>
    [EcaRules4All("artwork")]
    [RequireComponent(typeof(EcaEnvironment))]
    [DisallowMultipleComponent]
    public class EcaArtwork : MonoBehaviour
    {
        /// <summary>
        /// <b>Author</b> is the name of the artist who created the artwork.
        /// </summary>
        [EcaStateVariable("author", EcaRules4AllType.Text)]
        public string author;
        /// <summary>
        /// <b>Price</b> is the price of the artwork.
        /// </summary>
        [EcaStateVariable("price", EcaRules4AllType.Float)]
        public float price;
        /// <summary>
        /// <b>Year</b> is the year the artwork was created.
        /// </summary>
        [EcaStateVariable("year", EcaRules4AllType.Integer)]
        public int year;
    }
}
