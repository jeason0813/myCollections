
//-----------------------------------------------------------------------
// <copyright file="GreyscaleEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//  Greyscale effect.
// </summary>
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace myCollections.Controls.CustomEffects {

    /// <summary>
    /// Greyscale effect.
    /// </summary>
    public class GreyscaleEffect : ShaderEffect {

        /// <summary>
        /// Dependency property for Input.
        /// </summary>
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(GreyscaleEffect), 0);

        /// <summary>
        /// PixelShader for this effect.
        /// </summary>
        private static readonly PixelShader pixelShader;
        /// <summary>
        /// Static constructor - Create a PixelShader for all GreyscaleEffect instances. 
        /// </summary>
        static GreyscaleEffect() {
            pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("pack://application:,,,/myCollections;component/Controls/CustomEffects/ByteCode/Greyscale.ps");
            pixelShader.Freeze();
        }

        /// <summary>
        /// Constructor - Assign the PixelShader property and set the shader parameters to default values.
        /// </summary>
        public GreyscaleEffect() {
            PixelShader = pixelShader;
            UpdateShaderValue(InputProperty);
        }

        /// <summary>
        /// Gets or sets Input properties. 
        /// </summary>
        public Brush Input {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
}
}