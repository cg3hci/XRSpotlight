using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EcaRules
{
    /// <summary>
    /// <b>Furniture</b> is an <see cref="EcaEnvironment"/> subclass that represents a piece of furniture.
    /// </summary>
    [EcaRules4All("forniture")]
    [RequireComponent(typeof(EcaEnvironment))]
    [DisallowMultipleComponent]
    public class EcaFurniture : MonoBehaviour
    {
        /// <summary>
        /// <b>Price</b> is the price of the furniture.
        /// </summary>
        [EcaStateVariable("price", EcaRules4AllType.Float)]
        public float price;
        /// <summary>
        /// <b>Color</b> is the color of the furniture.
        /// </summary>
        [EcaStateVariable("color", EcaRules4AllType.Color)]
        public Color color;
        /// <summary>
        /// <b>Dimension</b> is the rough dimension of the furniture.
        /// </summary>
        [EcaStateVariable("dimension", EcaRules4AllType.Float)]
        public float dimension;

    }
}
