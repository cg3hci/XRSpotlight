using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EcaRules
{
    /// <summary>
    /// <b>Furniture</b> is an <see cref="Environment"/> subclass that represents a piece of furniture.
    /// </summary>
    [ECARules4All("forniture")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Furniture : MonoBehaviour
    {
        /// <summary>
        /// <b>Price</b> is the price of the furniture.
        /// </summary>
        [StateVariable("price", ECARules4AllType.Float)]
        public float price;
        /// <summary>
        /// <b>Color</b> is the color of the furniture.
        /// </summary>
        [StateVariable("color", ECARules4AllType.Color)]
        public Color color;
        /// <summary>
        /// <b>Dimension</b> is the rough dimension of the furniture.
        /// </summary>
        [StateVariable("dimension", ECARules4AllType.Float)]
        public float dimension;

    }
}
