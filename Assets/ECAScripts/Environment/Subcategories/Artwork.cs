using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECARules4All.RuleEngine
{
    /// <summary>
    /// <b>Artwork</b> is an <see cref="Environment"/> subclass that represents an artwork.
    /// </summary>
    [ECARules4All("artwork")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Artwork : MonoBehaviour
    {
        /// <summary>
        /// <b>Author</b> is the name of the artist who created the artwork.
        /// </summary>
        [StateVariable("author", ECARules4AllType.Text)]
        public string author;
        /// <summary>
        /// <b>Price</b> is the price of the artwork.
        /// </summary>
        [StateVariable("price", ECARules4AllType.Float)]
        public float price;
        /// <summary>
        /// <b>Year</b> is the year the artwork was created.
        /// </summary>
        [StateVariable("year", ECARules4AllType.Integer)]
        public int year;
    }
}
